"""Original five-island food ecology, authored layouts and pixel assets. Run LAST.
Layout JSON is shared by terrain artwork, navigation, maps and tilling rules.
No reference image pixels, downloaded artwork or unlicensed assets are used.
"""
from pathlib import Path
from PIL import Image, ImageDraw
import json, random, math
ROOT=Path(__file__).resolve().parents[1]; RES=ROOT/'Game/Assets/Wildfeast/Resources'; ART=RES/'Art'
def v(x,y): return {'x':x,'y':y}
def route(*pts): return {'points':[v(*p) for p in pts]}
def point(action,label,x,y,item='',source='',art='',kind=''):
 return dict(action=action,label=label,position=v(x,y),item=item,source=source,art=art,kind=kind)
salt=[(90,500),(80,300),(140,185),(330,130),(470,72),(650,78),(780,110),(990,100),(1140,230),(1180,370),(1140,590),(1050,655),(850,706),(590,735),(410,735),(280,690),(175,630)]
mist=[(140,530),(100,420),(160,250),(260,190),(300,85),(510,100),(590,150),(760,115),(940,150),(1090,270),(1150,450),(1070,550),(1000,635),(830,685),(690,660),(560,725),(380,725),(240,645)]
def oldcoast(p): return [v(round((x+(640-x)/12-640)/32,4),round((416-y-(420-y)/12)/32,4)) for x,y in p]
def island(id,key,name,subtitle,ground,path,tree,coast,pond,paths,regions,points):
 return dict(id=id,key=key,name=name,subtitle=subtitle,ground=ground,path=path,tree=tree,coast=coast,pond=v(*pond),pondSize=v(3.35,1.9),roads=paths,regions=regions,points=points,props=[])
def region(x,y,w,h,label):return dict(center=v(x,y),size=v(w,h),label=label)
isles=[]
isles.append(island(0,'saltleaf','Saltleaf Shore','Harbor village / kitchen garden / Steamstone spring','#64885c','#c5ac78','cinnamon',oldcoast(salt),(10.3,5.95),
 [route((-11,-6),(-11,-5.8),(-6,-5.8),(-6,-2.5),(-2,-2),(1,-1),(4,1),(7,3)),route((-2,-2),(1,-4),(4,-6),(7.6,-7.8)),route((-2,-2),(-2,-3.1)),route((-6,-2.5),(-10,-1.8),(-12,2),(-9,4.1)),route((4,1),(3.2,-1.3),(4.8,-3.4))],
 [region(-8,-4,9,3,'Kitchen garden'),region(-10,4,7,7,'Glowgrove'),region(7,3,8,6,'Steamstone habitat')],
 [point('enter','Harbor Table',-6,-1.15),point('service','Open the restaurant',-3.6,-1.2,art='sign-closed'),point('fish','Leafgill fishing bank',8,-8.6,'leafgill'),point('forage','Pepperbell patch',4.5,-1.3,'pepperbell','pepper-east','pepperbell'),point('forage','Pepperbell garden',6,-3.4,'pepperbell','pepper-south','pepperbell'),point('forage','Glowgrove roots',-9,5.3,'lanternroot','root-grove','lanternroot'),point('hunt','Steamstone spring',8,4,'brothback','broth-spring','brothback'),point('boat','Harbor skiff',-11,-7.2,art='boat'),point('upgrades','Harbor workshop',-2,-3.8,art='workshop')] ))
isles.append(island(1,'mistwake','Mistwake Isle','Terraced orchard / custard meadow / tidekeeper cottage','#718b81','#bcb6a0','cloud',oldcoast(mist),(10.3,5.95),
 [route((-11,-6),(-11,-5.8),(-5,-5.8),(-2,-3),(3,0),(3,2)),route((-2,-3),(-7,1),(-9.3,2.6),(-8,4.2),(-3.2,4.2)),route((3,0),(6,0),(8,-4),(7,-6)),route((3,2),(5,2),(6.5,2.7))],
 [region(-7,4,10,6,'Cloudfruit terraces'),region(5,4,8,5,'Custard meadow'),region(2,-3,8,4,'Seedkeeper garden')],
 [point('boat','Mistwake landing',-11,-7.2,art='boat'),point('story',"Iona's cottage",3,2,art='mailbox'),point('fruit','Cloudfruit terraces',-2,4,'cloudfruit','fruit-mist','cloudfruit'),point('fruit','Cloudfruit bough',6,0,'cloudfruit','fruit-east','cloudfruit'),point('forage','Lanternroot beds',-8,3,'lanternroot','root-mist','lanternroot'),point('fish','Bubblecarp bank',7,-6,'bubblecarp'),point('forage','Custardpetal flowers',-4,6,'custardpetal','petal-mist','custardpetal'),point('creature','Custardram meadow',7,4,'ramcream','ram-mist','custardram',kind='ram')] ))
isles.append(island(3,'emberfold','Emberfold Cay','Toasted woodland / spice terraces / warm mineral pools','#96805b','#d6ad72','ember', [v(*p) for p in [(-16,-5),(-15,3),(-10,8),(-4,10),(3,9),(8,10),(15,5),(17,-1),(14,-6),(9,-9),(0,-10),(-9,-9),(-14,-7)]],(9,5),
 [route((-11,-6),(-9,-5),(-4,-3),(0,-1),(4,1),(5.2,2)),route((-4,-3),(-9.3,1),(-7.2,3.8)),route((0,-1),(5,-4),(9,-7.5)),route((4,1),(1,5),(-1,5.8))],
 [region(-7,3,10,7,'Cinnamon terraces'),region(4,3,8,6,'Spiceclaw quarry'),region(3,-4,8,4,'Warm seed beds')],
 [point('boat','Emberfold pier',-11,-7.2,art='boat'),point('forage','Emberbulb terraces',-6,5,'emberbulb','bulb-ember','emberbulb'),point('forage','Smoked Pepperbells',-8,1,'pepperbell','pepper-ember','pepperbell'),point('fish','Lavafin warmwater bank',9,-7.5,'lavafin'),point('creature','Spiceclaw quarry',6,3,'spiceclaw','crab-ember','spicecrab',kind='crab'),point('tap','Cinnamon sap tree',-1,7,'syrup','sap-ember','sap-tap')] ))
isles.append(island(4,'moonfen','Moonfen Hollow','Luminous mushroom grove / pollen glade / moonwater marsh','#516d73','#aaa8a3','moon', [v(*p) for p in [(-16,-6),(-17,0),(-13,5),(-7,9),(-2,10),(3,7),(10,9),(16,4),(16,-3),(10,-7),(6,-10),(-3,-9),(-11,-9)]],(10,4),
 [route((-11,-6),(-9,-4),(-5,-2),(0,0),(5,2),(6,2.3)),route((-5,-2),(-8.3,2),(-5,4.8)),route((0,0),(1,-4),(7,-7)),route((0,0),(-1,4),(1,4.8))],
 [region(-6,4,10,7,'Mooncap grove'),region(3,4,8,6,'Mochimoth glade'),region(-2,-4,7,4,'Marsh garden')],
 [point('boat','Moonfen boardwalk',-11,-7.2,art='boat'),point('forage','Mooncap grove',-5,6,'mooncap','cap-moon','mooncap'),point('forage','Lanternroot marsh',-7,2,'lanternroot','root-moon','lanternroot'),point('fish','Jellyray moonwater bank',7,-7,'jellyray'),point('creature','Mochimoth glade',1,6,'mochipollen','moth-moon','mochimoth',kind='moth'),point('bud','Dewblossom',7,3,'dewnectar','dew-moon','dewblossom')] ))
isles.append(island(5,'pearltide','Pearltide Atoll','Coral orchard / kelp pools / pearlroot beds','#729d87','#d6c49b','coral', [v(*p) for p in [(-16,-5),(-15,2),(-10,7),(-5,9),(0,8),(6,10),(13,7),(17,2),(15,-4),(10,-8),(4,-10),(-4,-10),(-10,-8),(-14,-7)]],(8,4),
 [route((-11,-6),(-9,-5),(-5,-2),(0,-1),(4,1),(3.9,2.8)),route((-5,-2),(-7,2),(-6,3.8)),route((0,-1),(2,-5),(8,-7.5)),route((4,1),(1,4),(-1,6))],
 [region(-6,4,10,6,'Coral orchard'),region(5,2,7,6,'Kelp pools'),region(-1,-4,8,4,'Pearlroot beds')],
 [point('boat','Pearltide jetty',-11,-7.2,art='boat'),point('forage','Pearlsprout beds',-6,5,'pearlsprout','pearl-tide','pearlsprout'),point('fruit','Seafoam Cloudfruit',-1,6,'cloudfruit','fruit-tide','cloudfruit'),point('fish','Jellyray reef bank',8,-7.5,'jellyray'),point('creature','Kelpsnail tidepool',5,2,'kelpjelly','snail-tide','kelpsnail',kind='snail')] ))
def distance(p,a,b):
 x,y=p;ax,ay=a;bx,by=b;dx=bx-ax;dy=by-ay;t=max(0,min(1,((x-ax)*dx+(y-ay)*dy)/(dx*dx+dy*dy))) if dx or dy else 0
 return math.hypot(x-ax-t*dx,y-ay-t*dy)
def onroad(p,i,margin=0):return any(distance(p,(a['x'],a['y']),(b['x'],b['y']))<.65+margin for r in i['roads'] for a,b in zip(r['points'],r['points'][1:]))
def inside(p,poly):
 x,y=p;c=False
 for a,b in zip(poly,poly[1:]+poly[:1]):
  if (a['y']>y)!=(b['y']>y) and x<(b['x']-a['x'])*(y-a['y'])/(b['y']-a['y'])+a['x']:c=not c
 return c
def water(p,i):
 return not inside(p,i['coast']) or any(((p[0]-pool['center']['x'])/pool['size']['x'])**2+((p[1]-pool['center']['y'])/pool['size']['y'])**2<1 for pool in [dict(center=i['pond'],size=i['pondSize'])]+i.get('pools',[]))
def available(p,i):return not water(p,i) and not onroad(p,i,.6) and all(math.hypot(p[0]-q['position']['x'],p[1]-q['position']['y'])>1.8 for q in i['points'])
for i in isles:
 i['pools']=[]
 if i['id']==1:i['pools']=[dict(center=v(-12,0),size=v(1.2,.7))]
 if i['id']==3:i['pools']=[dict(center=v(-2,3),size=v(1.6,.9))]
 if i['id']==4:i['pools']=[dict(center=v(-10,0),size=v(1.5,1)),dict(center=v(8,-3.8),size=v(1.8,1.1))]
 if i['id']==5:i['pools']=[dict(center=v(-1,1),size=v(2.2,1))]
for i in isles:
 # Deliberately shaped grove bands. Outer canopy encloses habitats; paths stay clear.
 groves={0:[(-13,6,4,3),(-5,8,6,2),(4,8,5,2),(12,-2,3,3),(-14,-1,2,2)],1:[(-13,4,3,3),(-6,8,5,2),(5,8,4,2),(13,0,2,3),(-2,2,2,2)],3:[(-12,5,3,3),(-6,8,5,2),(4,8,4,2),(12,-1,2,2),(-13,-2,2,2)],4:[(-13,5,4,3),(-6,8,3,2),(5,7,5,2),(11,-2,3,3),(-3,3,2,2)],5:[(-12,3,3,3),(-6,8,3,2),(3,8,4,2),(12,0,2,2),(-2,2,2,2)]}
 for zone in groves[i['id']]:
  x,y,cols,rows=zone
  for row in range(rows):
   for col in range(cols):
    p=(round(x+col*1.7+(row%2)*.5+math.sin(col*2.7+row*1.9+i['id'])*.38,3),round(y-row*1.65+math.cos(col*1.8+row+i['id'])*.35,3))
    if available(p,i) and not (i['id']==0 and -9<p[0]<-3 and p[1]<3):i['props'].append(dict(art='tree-'+i['tree'],position=v(*p),role='tree'))
 # Low ground cover clustered on edges of groves, never isolated giant weeds.
 for cx,cy in [(-12,3),(-4,7),(4,6),(11,-3),(-8,-2),(3,-5)]:
  for dx,dy in [(-.7,0),(0,.3),(.6,-.1),(-.3,-.5),(.8,.5)]:
   p=(cx+dx,cy+dy)
   if available(p,i):i['props'].append(dict(art='grass-'+i['key'],position=v(*p),role='grass'))
 quarries={0:[(7,-5),(8.5,-5),(10,-4),(-12,0),(-11,1),(11,1)],1:[(10,-4),(11,-2),(12,0),(-12,2),(-10,1)],3:[(7,6),(5,7),(3,7),(10,-3),(11,-4),(-12,0)],4:[(-12,-2),(-11,2),(11,-4),(10,-3),(4,7)],5:[(-10,0),(-11,1),(12,-2),(10,-4),(11,-3)]}
 for p in quarries[i['id']]:
  if available(p,i):i['props'].append(dict(art='rock-'+i['key'],position=v(*p),role='stone'))
for i in isles:
 i['props'].append(dict(art='landmark-'+i['key'],position=v(-4,3) if i['id']==0 else v(-1,-5.5),role='landmark'))
 for x,y in [(-10,-5.4),(-3,-1.6),(3,1.7)]:
  if not water((x,y),i) and not onroad((x,y),i,.4):i['props'].append(dict(art='trail-post',position=v(x,y),role='landmark'))
RES.joinpath('Archipelago.json').write_text(json.dumps({'islands':isles},indent=2),encoding='utf-8')
W,H=1280,832
def px(p):return (round(640+p['x']*32),round(416-p['y']*32))
def native(name,size,fn):
 im=Image.new('RGBA',size);fn(ImageDraw.Draw(im));im.save(ART/(name+'.png'));return im
ink='#303b3d'
palettes=[('#294f48','#4c7951','#87b76a','#c1d18a'),('#344b61','#567a88','#83b4b8','#cfddd0'),('#633f39','#a45d45','#d99c59','#f1ca87'),('#333e56','#655a84','#a78aac','#ded6c0'),('#31574f','#599b83','#99cbb0','#e1e5a4')]
for index,i in enumerate(isles):
 r=random.Random(991+i['id']);im=Image.new('RGBA',(W,H),'#285d7a');d=ImageDraw.Draw(im);poly=[px(p) for p in i['coast']]
 for n in range(3800):x=r.randrange(W);y=r.randrange(H);d.line((x,y,x+r.randrange(4,11),y),fill=r.choice(['#2f6d89','#367891','#306782']))
 for width,col in [(48,'#386f83'),(28,'#6ea4a5'),(14,'#c6c29a')]:d.line(poly+[poly[0]],fill=col,width=width,joint='curve')
 d.polygon(poly,fill=i['ground']);mask=Image.new('1',(W,H));ImageDraw.Draw(mask).polygon(poly,fill=1)
 # Restrained ground texture; negative space remains readable.
 for n in range(8000):
  x=r.randrange(W);y=r.randrange(H)
  if mask.getpixel((x,y)):d.rectangle((x,y,x+2,y+1),fill=palettes[index][1] if n%3 else i['ground'])
 for reg in i['regions']:
  cx,cy=px(reg['center']);sx=reg['size']['x']*16;sy=reg['size']['y']*16
  # Ground-level flowers, moss and roots cluster within named habitats.
  for n in range(220):
   xx=r.uniform(-sx,sx);yy=r.uniform(-sy,sy);pworld=((cx+xx-640)/32,(416-cy-yy)/32)
   if (xx/sx)**2+(yy/sy)**2<1 and not water(pworld,i) and not onroad(pworld,i,.3):
    x=round(cx+xx);y=round(cy+yy);d.line((x,y,x+2,y-2),fill=palettes[index][1]);
    if n%5==0:d.rectangle((x-1,y-3,x+1,y-2),fill=palettes[index][3])
 # Visible, edged untillable paths with individual worn stone/packed sand motifs.
 for road in i['roads']:
  pts=[px(p) for p in road['points']];d.line(pts,fill=palettes[index][1],width=48,joint='curve')
  for x,y in pts:d.ellipse((x-24,y-24,x+24,y+24),fill=palettes[index][1])
  d.line(pts,fill=i['path'],width=42,joint='curve')
  for x,y in pts:d.ellipse((x-21,y-21,x+21,y+21),fill=i['path'])
  for a,b in zip(pts,pts[1:]):
   count=max(1,int(math.dist(a,b)/14))
   for n in range(count):t=n/count;x=int(a[0]+(b[0]-a[0])*t);y=int(a[1]+(b[1]-a[1])*t);d.line((x-7,y,x+6,y),fill='#a79672',width=1)
 # Paint all path interiors last so junctions have no artificial seams.
 for road in i['roads']:
  pts=[px(p) for p in road['points']];d.line(pts,fill=i['path'],width=42,joint='curve')
  for x,y in pts:d.ellipse((x-21,y-21,x+21,y+21),fill=i['path'])
  for a,b in zip(pts,pts[1:]):
   count=max(1,int(math.dist(a,b)/11))
   for n in range(count):
    t=n/count;x=int(a[0]+(b[0]-a[0])*t);y=int(a[1]+(b[1]-a[1])*t)
    d.line((x-5,y,x+5,y),fill='#b3a47e');d.point((x+3,y+2),fill='#dbc79c')
 # Terraced stone edging and biome beds occupy deliberate garden boundaries.
 if i['id']==1:
  for y in [6.7,5.5]:
   cx,cy=px(v(-8,y))
   for x in range(cx-85,cx+85,15):d.rectangle((x,cy,x+13,cy+6),fill='#586c68');d.rectangle((x+1,cy,x+12,cy+3),fill='#b7c3ac')
 if i['id']==3:
  for cx,cy in [px(v(-2,4.5)),px(v(8,7.3))]:
   for n in range(5):x=cx+n*17;d.polygon([(x,cy),(x+13,cy-8),(x+20,cy),(x+12,cy+10)],fill='#7a5b4b');d.line((x+2,cy,x+12,cy-5,x+18,cy),fill='#c19a66',width=2)
 if i['id']==4:
  for x,y in [(-11,-1),(5,-5.7),(11,5.8)]:
   cx,cy=px(v(x,y))
   for n in range(15):xx=cx+r.randrange(-24,25);yy=cy+r.randrange(-10,11);d.line((xx,yy,xx+1,yy-8),fill='#87a9a0');d.point((xx+1,yy-9),fill='#d8c7ba')
 if i['id']==5:
  for x,y in [(-3,1),(1,2),(9,6.1),(11,2.5)]:
   cx,cy=px(v(x,y))
   for n in range(7):xx=cx+r.randrange(-18,19);d.line((xx,cy,xx-3,cy-12,xx+3,cy-20),fill='#cca9a1',width=3);d.line((xx-3,cy-12,xx-7,cy-14),fill='#e3c5b6',width=2)
 # Stable dock connected to its road, no disconnected decorative pier.
 x,y=px(v(-11,-7.2));d.rectangle((x-28,y-18,x+28,y+41),fill=ink);d.rectangle((x-25,y-17,x+25,y+37),fill='#a27d54')
 for yy in range(y-14,y+38,7):d.line((x-25,yy,x+25,yy),fill='#d4af77');d.point((x-21,yy),fill=ink);d.point((x+21,yy),fill=ink)
 cx,cy=px(i['pond']);rx=i['pondSize']['x']*32;ry=i['pondSize']['y']*32
 d.ellipse((cx-rx-10,cy-ry-8,cx+rx+10,cy+ry+8),fill=i['path']);d.ellipse((cx-rx,cy-ry,cx+rx,cy+ry),fill='#3c748a')
 for n in range(100):x=r.randint(int(cx-rx*.7),int(cx+rx*.7));y=r.randint(int(cy-ry*.7),int(cy+ry*.7));d.line((x,y,x+9,y),fill='#90c4c0')
 for pool in i['pools']:
  cx,cy=px(pool['center']);rx=pool['size']['x']*32;ry=pool['size']['y']*32
  d.ellipse((cx-rx-5,cy-ry-4,cx+rx+5,cy+ry+4),fill=palettes[index][1]);d.ellipse((cx-rx,cy-ry,cx+rx,cy+ry),fill='#467f91' if index!=2 else '#739e9c')
  for n in range(20):x=r.randint(int(cx-rx*.7),int(cx+rx*.7));y=r.randint(int(cy-ry*.6),int(cy+ry*.6));d.line((x,y,x+7,y),fill='#b4d7cb')
 # Farm tiles are green soil, explicitly surrounded by path rather than baked crops.
 farm=i['regions'][-1] if index>0 else i['regions'][0];cx,cy=px(farm['center'])
 for n in range(26):x=cx+r.randrange(-65,66);y=cy+r.randrange(-28,29);d.line((x,y,x+3,y),fill=palettes[index][1])
 im.save(ART/(i['key']+'.png'))
 colors=palettes[index]
 for f in range(4):
  def tree(d,f=f,c=colors):
   d.ellipse((12,81,53,95),fill='#253f3b55');d.polygon([(24,93),(27,63),(38,62),(42,93),(34,90)],fill=ink);d.polygon([(28,91),(29,60),(36,62),(38,91)],fill='#916847');d.line((31,65,31,87),fill='#c29462',width=2)
   d.line((30,75,16,55),fill=ink,width=6);d.line((35,68,49,48),fill=ink,width=5)
   for x,y,rx,ry in [(18,40,18,23),(43,36,19,24),(31,18,21,18),(32,51,24,15)]:
    sway=1 if f==1 else -1 if f==3 else 0
    d.ellipse((x-rx+sway,y-ry,x+rx+sway,y+ry),fill=c[0]);d.ellipse((x-rx+3+sway,y-ry+2,x+rx-2+sway,y+ry-5),fill=c[1]);d.arc((x-rx+4+sway,y-ry+3,x+rx-5+sway,y+ry-8),180,300,fill=c[2],width=4)
   # Dense leaf clusters replace smooth bubbles, while branch/roots stay stable.
   tr=random.Random(410+index)
   for n in range(420):
    x=tr.randrange(3,61);y=tr.randrange(4,63)
    insideleaf=any(((x-cx)/rx)**2+((y-cy)/ry)**2<.8 for cx,cy,rx,ry in [(18,40,18,23),(43,36,19,24),(31,18,21,18),(32,51,24,15)])
    if insideleaf:
     shade=c[2] if n%3 else c[3];xx=x+(1 if f==1 and y<27 else -1 if f==3 and y<27 else 0)
     d.line((xx,y,xx+tr.randrange(1,4),y),fill=shade if y<44 or n%4 else c[0]);d.point((xx+1,y+1),fill=c[1])
   for x,y in [(12,34),(25,14),(42,24),(34,48),(51,43),(17,52),(34,9)]:d.line((x,y,x+4,y-2),fill=c[3],width=2)
   for x,y in [(21,35),(46,39),(32,55)]:d.ellipse((x-3,y-4,x+4,y+3),fill='#e8c085');d.point((x,y-3),fill='#fff0b5')
  native('tree-'+i['tree']+'-'+str(f),(64,96),tree)
 Image.open(ART/('tree-'+i['tree']+'-0.png')).save(ART/('tree-'+i['tree']+'.png'))
 for f in range(3):
  def grass(d,f=f,c=colors):
   d.ellipse((2,11,22,15),fill=c[0]+'66')
   for x,h in [(4,7),(8,11),(12,8),(17,12),(20,6)]:
    sway=(1 if f==1 else -1 if f==2 else 0);d.line((x,13,x-2+sway,13-h),fill=c[1],width=2);d.line((x,12,x+3+sway,13-h+3),fill=c[2]);d.point((x-2+sway,13-h),fill=c[3])
  native('grass-'+i['key']+'-'+str(f),(24,16),grass)
 Image.open(ART/('grass-'+i['key']+'-0.png')).save(ART/('grass-'+i['key']+'.png'))
 native('rock-'+i['key'],(48,32),lambda d,c=colors:[d.ellipse((2,21,46,31),fill=c[0]+'88'),d.polygon([(3,23),(9,9),(21,3),(38,8),(45,24),(31,29),(14,27)],fill=c[0]),d.polygon([(6,22),(12,10),(21,6),(36,10),(41,22),(29,26)],fill='#93aca0'),d.polygon([(12,10),(21,6),(35,10),(24,16)],fill='#d2d8bb'),d.line((24,16,29,26),fill='#668179',width=2),d.rectangle((12,18,16,21),fill='#f1dab0')])
# Native world props; no screen-sized cooking artwork is placed in the world.
native('trail-post',(24,40),lambda d:[d.ellipse((5,32,20,39),fill='#203b3955'),d.rectangle((10,10,14,37),fill=ink),d.rectangle((11,11,13,36),fill='#aa8057'),d.polygon([(1,8),(19,8),(23,14),(19,20),(1,20)],fill=ink),d.polygon([(3,10),(18,10),(20,14),(18,18),(3,18)],fill='#d5b77d'),d.line((7,14,14,14),fill='#5e7358',width=2)])
for index,i in enumerate(isles):
 def landmark(d,k=index,c=palettes[index]):
  d.ellipse((3,40,60,54),fill=c[0]+'88');d.rectangle((9,23,54,44),fill=ink);d.rectangle((11,24,52,41),fill='#aa8055');d.line((12,36,51,36),fill='#e3bc81',width=2)
  d.rectangle((13,10,48,29),fill=c[0]);d.rectangle((15,12,46,26),fill=c[2]);d.rectangle((17,14,44,24),fill=c[1])
  for x in [18,38]:d.rectangle((x,29,x+5,44),fill=ink);d.rectangle((x+1,30,x+4,42),fill='#d5ad77')
  for x in [21,34]:d.ellipse((x-5,3,x+6,14),fill=c[0]);d.ellipse((x-3,4,x+4,12),fill=c[3]);d.line((x,3,x+2,0),fill=c[2],width=2)
  if k==0:d.rectangle((22,17,40,19),fill='#e8d4a9')
  if k==1:d.arc((19,10,40,22),180,360,fill='#e4e8d0',width=2)
  if k==2:d.polygon([(25,25),(31,11),(37,25)],fill='#edac68')
  if k==3:d.ellipse((24,14,37,24),fill='#ded1ee')
  if k==4:d.arc((23,13,40,25),180,360,fill='#d4e7c2',width=3)
 native('landmark-'+i['key'],(64,56),landmark)
# Four original creatures with food anatomy, expressive faces and four native frames.
creatures=[('custardram','#e6ce8b','#b98964'),('spicecrab','#c86c4c','#f0b65f'),('mochimoth','#b6a8c9','#e9d5ad'),('kelpsnail','#78b598','#d4bba3')]
for kind,body,accent in creatures:
 for f in range(4):
  def creature(d,f=f,k=kind,body=body,a=accent):
   bob=f%2;d.ellipse((12,57,70,70),fill='#243e4655')
   if k=='mochimoth':
    for cx in [19,58]:
     d.ellipse((cx-17,12+bob,cx+17,56-bob),fill=ink);d.ellipse((cx-14,15+bob,cx+14,53-bob),fill=body);d.ellipse((cx-8,24,cx+8,44),fill=a);d.arc((cx-12,18,cx+12,49),45,300,fill='#f5e9c5',width=2)
   if k=='spicecrab':
    for x,s in [(16,-1),(62,1)]:
     for n in range(3):d.line([(x,38+n*6),(x+s*10,49+n*4+bob),(x+s*7,59+n*3)],fill=ink,width=5);d.line([(x,38+n*6),(x+s*10,49+n*4+bob)],fill=a,width=2)
   if k=='custardram':
    for x in [24,50]:d.rectangle((x,49,x+9,64+bob),fill=ink);d.rectangle((x+2,50,x+7,61+bob),fill=a)
   d.ellipse((14,20+bob,65,59+bob),fill=ink);d.ellipse((17,22+bob,62,55+bob),fill=body);d.arc((19,26+bob,59,53+bob),15,170,fill=a,width=3)
   if k=='custardram':
    for x in [22,46]:d.arc((x-9,15,x+10,38),30,335,fill=ink,width=7);d.arc((x-7,17,x+8,36),35,325,fill=a,width=3)
    for x,y in [(31,10),(40,7),(49,13),(30,18)]:d.ellipse((x-8,y,x+8,y+15),fill=body);d.arc((x-6,y+2,x+6,y+12),180,320,fill='#fff0bc',width=2)
    d.ellipse((29,30+bob,52,53+bob),fill='#f5e1b1')
   elif k=='kelpsnail':
    d.ellipse((22,8,64,48),fill=ink);d.ellipse((25,11,61,45),fill=a);d.arc((29,15,57,41),20,325,fill='#947768',width=3);d.arc((34,20,52,36),30,340,fill='#f2dab0',width=2)
    for x in [19,32]:d.line((x,46,x-3,29+bob),fill=body,width=4)
    d.line((54,21,59,8,65,4),fill='#4f9268',width=4)
   elif k=='spicecrab':
    d.polygon([(21,28),(30,9),(37,23),(47,5),(55,28)],fill=ink);d.polygon([(25,27),(30,14),(38,27),(47,11),(52,28)],fill=a)
    for x in [5,67]:d.ellipse((x-5,31+bob,x+9,46+bob),fill=ink);d.ellipse((x-3,33+bob,x+7,42+bob),fill=a);d.line((x+3,32,x+3,37),fill=ink,width=2)
   else:
    for x in [32,47]:d.line((x,27,x-4,9+bob),fill=ink,width=2);d.ellipse((x-7,6+bob,x-1,12+bob),fill=a)
   eyes=[(24,42),(45,42)] if k!='kelpsnail' else [(17,27+bob),(30,27+bob)]
   for x,y in eyes:d.ellipse((x,y,x+8,y+9),fill='#faf1d2');d.rectangle((x+4,y+3,x+6,y+7),fill=ink);d.point((x+4,y+3),fill='white')
   d.line((31,53+bob,37,55+bob,43,53+bob),fill=ink,width=2)
  native(kind+'-'+str(f),(80,72),creature)
 Image.open(ART/(kind+'-0.png')).save(ART/(kind+'.png'))
foods=[('custardpetal','#edcd84'),('emberbulb','#e29b59'),('mooncap','#b8a3d2'),('pearlsprout','#d7e7af'),('dewnectar','#96cfc4'),('syrup','#dba15e'),('ramcream','#f4dfb2'),('spiceclaw','#e6986e'),('mochipollen','#d5b3db'),('kelpjelly','#8dbfa3')]
for name,color in foods:
 def food(d,name=name,c=color):
  d.ellipse((6,29,38,38),fill='#28454355')
  if name in ['custardpetal','emberbulb','mooncap','pearlsprout']:
   d.line((23,33,23,13),fill='#53724d',width=3);d.ellipse((7,19,23,26),fill='#7d9e66');d.ellipse((23,23,37,29),fill='#a3be7b')
   if name=='mooncap':d.ellipse((5,2,41,21),fill=ink);d.ellipse((7,4,39,17),fill=c);d.rectangle((20,15,27,31),fill='#e3d3b8');d.arc((10,5,35,18),185,330,fill='#eee5c7',width=2)
   else:
    for x,y in [(12,9),(24,8),(19,1)]:d.ellipse((x-6,y,x+9,y+14),fill=ink);d.ellipse((x-4,y+2,x+7,y+12),fill=c);d.line((x,y+3,x+2,y+7),fill='#fff0c6',width=2)
  else:d.rounded_rectangle((10,6,36,33),radius=8,fill=ink);d.rounded_rectangle((12,8,34,30),radius=6,fill=c);d.ellipse((14,10,31,17),fill='#f2e7c4');d.line((15,20,15,26),fill='#ffffff',width=2)
 native(name,(48,40),food)
 # Compact pickup/carry representation is separately rasterized at native resolution.
 native('held-'+name,(20,20),lambda d,c=color:[d.ellipse((2,3,17,16),fill=ink),d.ellipse((4,4,15,14),fill=c),d.line((6,6,10,6),fill='#fff0c6')])
for name,c in [('bubblecarp','#d6c4ed'),('lavafin','#e7a26c'),('jellyray','#aadbd0')]:
 native(name,(48,40),lambda d,c=c:[d.polygon([(11,20),(1,9),(2,31)],fill=ink),d.polygon([(10,20),(3,13),(4,26)],fill=c),d.ellipse((8,7,43,33),fill=ink),d.ellipse((11,9,41,30),fill=c),d.arc((15,11,29,29),80,270,fill='#f9e8bc',width=3),d.ellipse((33,14,40,21),fill='#fff6da'),d.rectangle((37,16,39,19),fill=ink),d.polygon([(18,10),(25,1),(29,10)],fill=c),d.polygon([(20,29),(27,38),(32,29)],fill=c)])
 native('held-'+name,(20,20),lambda d,c=c:[d.polygon([(6,10),(1,5),(1,15)],fill=c),d.ellipse((4,4,18,15),fill=ink),d.ellipse((6,5,17,13),fill=c),d.point((14,8),fill='#fff4d0')])
native('sap-tap',(48,72),lambda d:[d.polygon([(14,69),(18,5),(32,1),(36,69)],fill=ink),d.polygon([(18,67),(21,6),(29,5),(32,67)],fill='#9c704a'),d.line((24,10,24,59),fill='#d5a578',width=2),d.rectangle((27,35,43,39),fill='#a5bbb0'),d.rectangle((29,43,44,62),fill=ink),d.rectangle((31,44,42,59),fill='#d3a768'),d.ellipse((31,42,42,48),fill='#e8c584')])
Image.open(ART/'custardpetal.png').save(ART/'dewblossom.png')
newrecipes=[('custard','Custardpetal Tart','sweet',31,['custardpetal','grain']),('carp','Bubblecarp Dumplings','fresh',32,['bubblecarp','grain']),('cream','Cloud Cream Pavlova','sweet',54,['ramcream','cloudfruit']),('bulb','Emberbulb Roast','spiced',34,['emberbulb']),('lava','Lavafin Skillet','spiced',39,['lavafin','pepperbell']),('claw','Spiceclaw Bisque','warm',55,['spiceclaw','emberbulb']),('sap','Cinnamon Sap Pancakes','sweet',32,['syrup','grain']),('cap','Mooncap Risotto','warm',38,['mooncap','grain']),('pollen','Mochi Mooncakes','sweet',49,['mochipollen','grain']),('dew','Dewblossom Custard','sweet',44,['dewnectar','custardpetal']),('ray','Moonwater Jellyray','fresh',36,['jellyray']),('pearl','Pearlsprout Tempura','fresh',38,['pearlsprout']),('kelp','Kelp Jelly Parfait','sweet',52,['kelpjelly','cloudfruit']),('feast','Archipelago Hotpot','warm',70,['spiceclaw','mooncap','pearlsprout'])]
content=json.loads(RES.joinpath('Content.json').read_text(encoding='utf-8'));content['ingredients']=content['ingredients'][:6];content['recipes']=content['recipes'][:6]
notes={
 'custardpetal':('Mistwake meadow','A flower full of golden custard. Pull gently from its rooted meadow patch.'),
 'emberbulb':('Emberfold terraces','A warm bulb that grows beside mineral shelves; roasting releases its smoky sweetness.'),
 'mooncap':('Moonfen grove','A soft luminous mushroom. Gather from the glade without disturbing its tree roots.'),
 'pearlsprout':('Pearltide orchard','A crunchy vegetable with pearl-like kernels nestled in pale sea leaves.'),
 'bubblecarp':('Mistwake shallows','A dumpling-shaped fish with bubbly fins. Use your rod and give slack when it pulls.'),
 'lavafin':('Emberfold warm banks','A toasted fish with a glowing spice sail. It lives in warm water, never in lava.'),
 'jellyray':('Moonfen / Pearltide coast','A gelatinous ray with mint-colored wings. Fish from moonwater or reef banks.'),
 'ramcream':('Mistwake custard meadow','Custardrams startle at footsteps. Stand still nearby for two seconds, then collect a dollop of cream while the ram is calm.'),
 'spiceclaw':('Emberfold quarry','A Spicecrab retreats from your approach. Three careful pickaxe contacts crack its spice shell; collect the shed spice without harming the crab.'),
 'mochipollen':('Moonfen pollen glade','Mochimoths follow Lanternroot light. Hold a Lanternroot ingredient in your hotbar to lure one close, then gently collect its pollen.'),
 'kelpjelly':('Pearltide pools','A Kelpsnail curls up when dry. Water it once with your can, then collect the jelly it sheds. This uses one unit of water.'),
 'dewnectar':('Moonfen dewblossom','Water a closed Dewblossom with your can to open its petals, then harvest the dew nectar. One bloom per day.'),
 'syrup':('Emberfold sap stand','Use your field knife on the Cinnamon sap tap. Each tree gives two portions each morning.')}
names={'ramcream':'Custardram cream','spiceclaw':'Spiceclaw spice','mochipollen':'Mochimoth pollen','kelpjelly':'Kelp jelly','dewnectar':'Dew nectar','syrup':'Cinnamon sap'}
for name,(habitat,desc) in notes.items():content['ingredients'].append(dict(id=name,name=names.get(name,name.capitalize()),icon=name,habitat=habitat,description=desc))
for index,(id,name,flavor,price,items) in enumerate(newrecipes):
 icon='dish-'+id;content['recipes'].append(dict(id=id,name=name,description='A taste of the wild islands.',flavor=flavor,price=price,icon=icon,ingredients=[dict(id=s,count=1) for s in items]));c=(foods+[('fish','#89c9b0')])[index%11][1]
 native(icon,(48,40),lambda d,c=c,j=index:[d.ellipse((1,12,46,36),fill=ink),d.ellipse((3,13,44,32),fill='#e9dfba'),d.ellipse((9,14,38,28),fill=c),d.arc((11,14,35,27),0,180,fill='#946f57',width=2),d.ellipse((13,13,19,19),fill='#66895a'),d.ellipse((27,18,34,24),fill='#f6d687'),d.line((23,15,25,23),fill='#fff0ce',width=2)])
 native('held-'+icon,(24,20),lambda d,c=c:[d.ellipse((1,7,22,17),fill=ink),d.ellipse((2,7,21,15),fill='#ede3c1'),d.ellipse((6,7,18,12),fill=c)])
RES.joinpath('Content.json').write_text(json.dumps(content,ensure_ascii=False,indent=2),encoding='utf-8')
print('Authored five maps, native vegetation frames, four creatures, 13 ingredients and 14 dishes.')

for name,color in foods[:4]:
 native('seed-'+name,(24,28),lambda d,c=color:[d.rectangle((2,1,21,26),fill=ink),d.rectangle((4,3,19,24),fill='#d1b17e'),d.rectangle((5,6,18,18),fill='#eee1b5'),d.ellipse((8,8,15,15),fill=c),d.line((6,21,17,21),fill='#826141')])
 native('held-seed-'+name,(16,20),lambda d,c=color:[d.rectangle((2,2,13,18),fill=ink),d.rectangle((3,3,12,17),fill='#d1b17e'),d.ellipse((5,5,10,10),fill=c)])

# Cuisine silhouettes communicate actual recipes, rather than recolored servings.
def plated(d,id,color):
 d.ellipse((2,15,46,37),fill=ink);d.ellipse((4,16,44,33),fill='#eadfbc');d.arc((7,18,41,31),0,180,fill='#bab898',width=2)
 if id in ['claw','cap','feast']:
  d.ellipse((4,8,43,28),fill=ink);d.rectangle((6,17,41,27),fill='#997657');d.ellipse((7,10,40,23),fill='#d0ae71');d.ellipse((10,12,37,21),fill=color)
  for x,y in [(13,14),(24,13),(31,17)]:d.ellipse((x-3,y-2,x+4,y+3),fill='#a6b479');d.line((x,y-1,x+2,y+1),fill='#f1dab2')
  if id=='claw':d.polygon([(16,16),(10,5),(17,8),(21,3),(23,17)],fill='#e58e67');d.line((15,7,18,14),fill=ink,width=2)
  if id=='cap':
   for x,y in [(15,13),(29,15)]:d.rectangle((x,y,x+3,y+5),fill='#e0d0bc');d.ellipse((x-4,y-4,x+7,y+1),fill='#baa3ca')
  if id=='feast':d.polygon([(12,19),(17,8),(22,19)],fill='#e09066');d.ellipse((28,10,35,18),fill='#c6b8da');d.ellipse((25,17,32,20),fill='#e8e6ad')
 elif id in ['custard','dew']:
  d.polygon([(10,26),(15,11),(32,11),(39,26)],fill='#946949');d.ellipse((13,8,35,17),fill='#f0d8a0');d.ellipse((11,21,38,29),fill='#cfa76d');d.rectangle((16,15,33,23),fill='#e4c281')
  for x,y in [(19,11),(27,10),(24,17)]:d.ellipse((x-3,y-2,x+3,y+2),fill='#d8989c' if id=='custard' else '#8ccdc0')
 elif id in ['carp','pollen']:
  for x,y in [(11,22),(25,23),(18,13)]:
   d.ellipse((x-6,y-5,x+7,y+6),fill=ink);d.ellipse((x-4,y-4,x+5,y+4),fill='#e5cfac' if id=='carp' else '#d4b9d6');d.line((x-2,y-3,x,y+1,x+3,y-2),fill='#a18d73' if id=='carp' else '#a082ba')
 elif id=='cream':
  for x,y in [(10,23),(24,23),(18,14),(29,13),(24,7)]:d.ellipse((x-5,y-5,x+8,y+4),fill='#b6a08b');d.ellipse((x-4,y-5,x+6,y+2),fill='#f5e6c0');d.arc((x-2,y-4,x+5,y+1),180,320,fill='#fff8dc')
  d.ellipse((19,1,28,8),fill='#9ebea0')
 elif id=='sap':
  for y in [25,20,15]:d.ellipse((9,y-5,39,y+6),fill='#7c5b40');d.ellipse((10,y-5,38,y+3),fill='#d6ae70');d.ellipse((15,y-3,32,y+1),fill='#e9c888')
  d.line((26,13,31,16,28,24),fill='#975e36',width=3);d.rectangle((19,11,26,15),fill='#f4df9c')
 elif id=='lava':
  d.polygon([(12,22),(4,13),(5,29)],fill='#9c604d');d.ellipse((10,12,39,29),fill='#a96946');d.ellipse((13,13,37,25),fill='#e0ab65');d.point((34,18),fill=ink);d.line((17,16,18,23),fill='#f3d399',width=2);d.line((24,15,25,24),fill='#f3d399',width=2)
 elif id=='ray':d.polygon([(8,22),(22,8),(38,22),(23,29)],fill=ink);d.polygon([(11,21),(22,10),(35,21),(23,26)],fill='#97cec1');d.line((22,11,23,25),fill='#e0efe0',width=2);d.line((23,25,27,33),fill='#6faba0',width=2)
 elif id=='kelp':
  d.polygon([(13,3),(35,3),(31,29),(18,29)],fill=ink);d.polygon([(15,5),(33,5),(29,27),(20,27)],fill='#abd1b5');d.rectangle((18,12,30,18),fill='#d3ddbd');d.ellipse((15,2,33,10),fill='#edf0cf');d.ellipse((20,0,28,7),fill='#98bfce');d.rectangle((23,29,26,34),fill='#b9d0bb')
 elif id in ['bulb','pearl']:
  for x,y in [(12,22),(25,24),(19,14),(33,16)]:d.ellipse((x-5,y-5,x+6,y+5),fill='#907650');d.ellipse((x-3,y-4,x+4,y+3),fill='#dbae68' if id=='bulb' else '#e4deb1');d.line((x-1,y-3,x+2,y+1),fill='#f0d9a4');d.line((x,y-4,x+3,y-8),fill='#7a9859',width=2)
for id,name,flavor,price,items in newrecipes:
 native('dish-'+id,(48,40),lambda d,id=id:plated(d,id,dict(claw='#d59764',cap='#cab58d',feast='#bf8b59').get(id,'#e3c595')))

def dew(d,opened):
 d.ellipse((8,30,39,39),fill='#25464655');d.line((24,34,24,17),fill='#527d64',width=3)
 for x in [12,27]:d.ellipse((x,24,x+12,31),fill='#82ad88')
 if opened:
  for x,y in [(13,12),(25,12),(19,5),(19,20)]:d.ellipse((x-5,y-3,x+10,y+10),fill='#4a7178');d.ellipse((x-3,y-2,x+8,y+8),fill='#a3d7ca');d.arc((x-2,y-1,x+5,y+6),180,320,fill='#e7efc4')
  d.ellipse((20,14,29,22),fill='#f0dc9a');d.ellipse((22,16,27,20),fill='#b0dbc9')
 else:d.ellipse((17,10,31,26),fill='#345d54');d.ellipse((19,12,29,24),fill='#81b399');d.line((24,13,24,22),fill='#c3d6ac')
native('dewblossom-closed',(48,40),lambda d:dew(d,False));native('dewblossom-open',(48,40),lambda d:dew(d,True));native('dewblossom',(48,40),lambda d:dew(d,False))
native('spicecrab-cracked',(80,72),lambda d:[creature(d,0,k='spicecrab',body='#c86c4c',a='#f0b65f'),d.line((35,18,41,27,36,33,45,39),fill=ink,width=3),d.line((36,18,42,27,37,33,46,39),fill='#ffe3a3')])
