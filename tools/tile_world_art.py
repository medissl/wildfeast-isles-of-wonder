"""Original 32 px terrain tiles and authored connected grid routes. No background raster map."""
from pathlib import Path
import json, random, math, heapq
from PIL import Image, ImageDraw, ImageEnhance

ROOT=Path(__file__).resolve().parents[1]
ART=ROOT/'Game/Assets/Wildfeast/Resources/Art'
CAT=ROOT/'Game/Assets/Wildfeast/Resources/Archipelago.json'
palettes=[('#70af47','#649f40','#8abd57','#579442','#c6ad73'),('#79b86b','#69a65e','#96ca80','#559750','#d5bb86'),('#9eaa43','#909b38','#b8bc58','#7c8c36','#dca971'),('#668b94','#557d88','#7ba5a2','#456d7e','#a99bad'),('#87bd72','#73ab62','#a2d18b','#609950','#e6cb8e')]

def point(v):return v['x'],v['y']
def vec(p):return {'x':p[0],'y':p[1]}
def oldland(p,isle):
    x,y=p; poly=[point(v) for v in isle['coast']];inside=False
    for a,b in zip(poly,poly[-1:]+poly[:-1]):
        if (a[1]>y)!=(b[1]>y) and x<(b[0]-a[0])*(y-a[1])/(b[1]-a[1])+a[0]:inside=not inside
    for pool in [{'center':isle['pond'],'size':isle['pondSize']}]+isle.get('pools',[]):
        cx,cy=point(pool['center']);sx,sy=point(pool['size'])
        if ((x-cx)/sx)**2+((y-cy)/sy)**2<1:return False
    return inside

def tile(name,colors,variant,mask=None,kind='grass'):
    rng=random.Random(variant*37+17); im=Image.new('RGBA',(32,32));d=ImageDraw.Draw(im)
    base,dark,light,shade,path=colors
    d.rectangle((0,0,31,31),fill=base if kind=='grass' else path)
    if kind=='grass':
        # Clustered blade groups; restrained contrast and no tile-boundary outline.
        for _ in range(14):
            x,y=rng.randrange(32),rng.randrange(32);c=rng.choice([dark,dark,light,base])
            d.line((x,y,x+2,y),fill=c);d.line((x+1,y-2,x+1,y),fill=c)
        for _ in range(3):
            x,y=rng.randrange(32),rng.randrange(32);d.line((x,y,x+3,y+1),fill=dark)
    else:
        for _ in range(24):
            x,y=rng.randrange(32),rng.randrange(32);d.line((x,y,x+rng.randrange(1,4),y),fill=rng.choice(['#bea36b','#d8c28c',path]))
        if mask is not None:
            # Tapered ragged grass fringes, never beveled "button" paving.
            for edge,bit in enumerate([1,2,4,8]):
                if mask&bit:continue
                for q in range(32):
                    depth=4+int(2*math.sin(q*.55))+rng.randrange(3)
                    for z in range(depth):
                        px,py=[(q,z),(31-z,q),(q,31-z),(z,q)][edge]
                        im.putpixel((px,py),ImageColor_get(base if z<depth-2 else rng.choice([base,dark,path])))
    im.save(ART/(name+'.png'))

def ImageColor_get(s):
    from PIL.ImageColor import getcolor
    return getcolor(s,'RGBA')

def water(phase,variant,mask):
    im=Image.new('RGBA',(32,32),'#287cb1');d=ImageDraw.Draw(im);rng=random.Random(variant+9)
    # Long sparse wavelets, temporal travel without a checkerboard sparkle carpet.
    for k in range(4):
        x=(rng.randrange(32)+phase*2)%32;y=rng.randrange(32)
        c=['#369ac5','#3fadd1','#2381b4','#5abed5'][k]
        d.line((x,y,min(31,x+5+k),y),fill=c);d.point((min(31,x+2),y-1),fill=c)
    for edge,bit in enumerate([1,2,4,8]):
        if not mask&bit:continue
        for q in range(32):
            # shallow turquoise at dry-bank boundary, animated broken white foam
            depth=4+int(math.sin((q+phase*2)*.4)*1.5)
            for z in range(depth):
                px,py=[(q,z),(31-z,q),(q,31-z),(z,q)][edge]
                im.putpixel((px,py),ImageColor_get('#70d1cf' if z<2 else '#42b5c9'))
            if (q+phase*3)%13<8:
                px,py=[(q,depth),(31-depth,q),(q,31-depth),(depth,q)][edge]
                im.putpixel((px,py),ImageColor_get('#c0f1df'))
    im.save(ART/f'tile-water-{mask}-{variant}-{phase}.png')

def generate():
    data=json.loads(CAT.read_text(encoding='utf-8-sig'))
    for index,isle in enumerate(data['islands']):
        if not isle.get("tileWorld"):
            w,h=map(int,point(isle['size']));xmin,ymin=-w//2,-h//2
            land={(x,y) for x in range(xmin,xmin+w) for y in range(ymin,ymin+h) if oldland((x,y),isle)}
            # Building and fixed interaction footprints stay open to a one-cell approach.
            obstacles=set()
            if isle['id']==0:
                obstacles|={(x,y) for x in range(-8,-3) for y in range(0,4)}
                obstacles|={(1,2),(2,2)}
            if isle['id']==1:obstacles|={(x,y) for x in range(2,5) for y in range(4,6)}
            for p in isle['points']:
                if p['action'] in ('upgrades','discovery','tap'):
                    x,y=point(p['position']);obstacles.add((round(x),round(y)+1))
            safe={p for p in land if all((p[0]+dx,p[1]+dy) in land for dx,dy in [(0,1),(1,0),(0,-1),(-1,0)])}-obstacles
            def nearest(p):return min(safe,key=lambda a:(a[0]-p[0])**2+(a[1]-p[1])**2)
            arrival=nearest(point(isle['arrival']));isle['arrival']=vec(arrival)
            anchors=[arrival]
            for p in isle['points']:
                if p['action'] in ('enter','upgrades','story','discovery','forage','fruit'):
                    x,y=point(p['position']);anchors.append(nearest((x,y-1)))
            # A main route reaches each inhabited grove; minor gathering happens off-trail.
            anchors += [nearest(point(r['center'])) for r in isle['regions']]
            if isle['id']==0:anchors += [nearest((-6,-2)),nearest((-9,-5)),nearest((1,0))]
            if isle['id']==1:anchors += [nearest((3,2))]
            anchors=list(dict.fromkeys(anchors));roads=set();routes=[];connected=[anchors[0]]
            for goal in anchors[1:]:
                start=min(connected,key=lambda p:abs(p[0]-goal[0])+abs(p[1]-goal[1]))
                frontier=[(0,0,start)];cost={start:0};parent={start:None}
                while frontier:
                    _,g,p=heapq.heappop(frontier)
                    if p==goal:break
                    if g>cost[p]:continue
                    for dx,dy in [(1,0),(0,1),(-1,0),(0,-1)]:
                        q=p[0]+dx,p[1]+dy
                        if q not in safe:continue
                        # prefer shared corridors and few bends; forests retain ample wild ground
                        ng=g+(0.65 if q in roads else 1)+(.15 if parent[p] and (dx,dy)!=(p[0]-parent[p][0],p[1]-parent[p][1]) else 0)
                        if ng<cost.get(q,1e9):
                            cost[q]=ng;parent[q]=p;heapq.heappush(frontier,(ng+abs(q[0]-goal[0])*.6+abs(q[1]-goal[1])*.6,ng,q))
                if goal not in parent:raise RuntimeError(f'Unreachable {isle["key"]} landmark {goal}')
                path=[];p=goal
                while p is not None:path.append(p);p=parent[p]
                path.reverse();roads.update(path);connected+=path
                routes.append({'points':[vec(p) for p in path],'width':.45})
            # Widen village approaches only; small forest trails remain intimate.
            if isle['id']==0:
                roads |= {q for p in list(roads) if -13<p[0]<4 and -7<p[1]<0 for q in [(p[0]+1,p[1]),(p[0],p[1]+1)] if q in safe}
            isle['roads']=routes
            isle['gridLand']=[(y-ymin)*w+x-xmin for x,y in sorted(land)]
            isle['gridRoad']=[(y-ymin)*w+x-xmin for x,y in sorted(roads)]
            occupied=set(obstacles)
            for p in isle['points']:
                if p['action'] not in ('fish','boat'):
                    q=nearest(point(p['position']));p['position']=vec(q);occupied.add(q)
            # Keep old resource identities even when feet now snap to a grid cell.
            props=[]
            for prop in isle['props']:
                x,y=point(prop['position']);prop.setdefault('source',f'archipelago-{isle["id"]}-{x:.2f}-{y:.2f}')
                q=(round(x),round(y));role=prop['role']
                if role=='landmark':
                    options=[p for p in safe if p not in roads and p not in occupied]
                else:
                    options=[p for p in safe if p not in roads and p not in occupied and all(abs(p[0]-a[0])>1 or abs(p[1]-a[1])>1 for a in occupied)]
                if not options:continue
                q=min(options,key=lambda p:(p[0]-x)**2+(p[1]-y)**2)
                if (q[0]-x)**2+(q[1]-y)**2>9:continue
                prop['position']=vec(q);occupied.add(q);props.append(prop)
            isle['props']=props
            isle["tileWorld"]=True
        for v in range(6):tile(f'tile-{isle["key"]}-grass-{v}',palettes[index%5],v)
        for mask in range(16):
            for v in range(3):tile(f'tile-{isle["key"]}-path-{mask}-{v}',palettes[index%5],v,mask,'path')
        for mask in range(16):
            # Bank cells: earthy ledge along exposed sides, turf in the center.
            im=Image.open(ART/f'tile-{isle["key"]}-grass-{mask%6}.png').convert('RGBA');d=ImageDraw.Draw(im)
            for edge,bit in enumerate([1,2,4,8]):
                if mask&bit:continue
                for q in range(32):
                    depth=3+(q*7%3)
                    for z in range(depth):
                        px,py=[(q,z),(31-z,q),(q,31-z),(z,q)][edge]
                        im.putpixel((px,py),ImageColor_get(['#967852','#b89560','#d4b57b'][min(z,2)]))
            im.save(ART/f'tile-{isle["key"]}-bank-{mask}.png')
    for mask in range(16):
        for v in range(2):
            for phase in range(6):water(phase,v,mask)
    for wet in [False,True]:
        im=Image.new('RGBA',(32,32),'#805238' if wet else '#ad7545');d=ImageDraw.Draw(im)
        for y in [5,12,19,26]:d.line((3,y,28,y),fill='#614733' if wet else '#8a5e3a');d.line((4,y-1,27,y-1),fill='#916544' if wet else '#c68b54')
        im.save(ART/('tile-soil-wet.png' if wet else 'tile-soil.png'))
    CAT.write_bytes((json.dumps(data,indent=2)+'\n').encode('utf-8'))
    # Menu horizon: original native pixel landscape, layered cloud animation lives in Unity.
    im=Image.new('RGB',(640,360),'#69cce4');d=ImageDraw.Draw(im)
    for y in range(360):
        t=y/360;d.line((0,y,639,y),fill=(int(65+92*t),int(172+50*t),int(225-31*t)))
    d.ellipse((500,38,552,90),fill='#fff0aa')
    for depth,color,base in [(0,'#68acbd',218),(1,'#43989a',250),(2,'#347d70',290)]:
        ridge=[(0,360),(0,base)]+[(x,base-int(30*math.sin(x*.017+depth)**2)-int(22*math.sin(x*.041+depth)**2)) for x in range(0,641,8)]+[(640,360)]
        d.polygon(ridge,fill=color)
    d.rectangle((0,280,639,359),fill='#258bb5')
    rng=random.Random(15)
    for _ in range(130):
        x,y=rng.randrange(640),rng.randrange(283,360);d.line((x,y,x+rng.randrange(3,12),y),fill=rng.choice(['#39a4c5','#50b7cc','#247fac']))
    # Foreground island silhouettes and original fruit forests support the sign's food world.
    for x,y in [(-20,310),(600,306)]:
        d.ellipse((x-70,y-30,x+92,y+75),fill='#bdb274');d.ellipse((x-65,y-38,x+90,y+62),fill='#69ae52')
        for j in range(4):
            tx=x+j*24;ty=y-35-j%2*15
            d.rectangle((tx-3,ty-7,tx+4,ty+19),fill='#966343')
            d.ellipse((tx-24,ty-40,tx+26,ty+5),fill='#34763d');d.ellipse((tx-22,ty-40,tx+17,ty-8),fill='#78b64b')
            for k in range(3):d.ellipse((tx-13+k*9,ty-25+k%2*9,tx-6+k*9,ty-18+k%2*9),fill='#e5af4d')
    im.save(ART/'title-horizon-study.png')
    print('Native terrain generated; five connected grid plans saved.')

if __name__=='__main__':generate()
