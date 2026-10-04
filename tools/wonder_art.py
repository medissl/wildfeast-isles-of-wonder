"""Original project-native pixel art and deliberate geography. Run after archipelago_art.py.
Stable IDs and source names preserve saves. No downloaded/reference-image pixels.
"""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
import json,math,random
ROOT=Path(__file__).resolve().parents[1];RES=ROOT/'Game/Assets/Wildfeast/Resources';ART=RES/'Art'
INK='#273740';CREAM='#f5e2b5';GOLD='#e5b66a'
def v(x,y):return dict(x=x,y=y)
def route(*p):return dict(points=[v(*q) for q in p],width=.5)
def save(name,size,fn):
 im=Image.new('RGBA',size);fn(ImageDraw.Draw(im));im.save(ART/(name+'.png'));return im
def dist(p,a,b):
 dx=b['x']-a['x'];dy=b['y']-a['y'];t=max(0,min(1,((p[0]-a['x'])*dx+(p[1]-a['y'])*dy)/(dx*dx+dy*dy))) if dx or dy else 0
 return math.hypot(p[0]-a['x']-dx*t,p[1]-a['y']-dy*t)
def road(p,i,margin=0):return any(dist(p,a,b)<=r.get('width',.5)+.1+margin for r in i['roads'] for a,b in zip(r['points'],r['points'][1:]))
def inside(p,poly):
 x,y=p;c=False
 for a,b in zip(poly,poly[1:]+poly[:1]):
  if (a['y']>y)!=(b['y']>y) and x<(b['x']-a['x'])*(y-a['y'])/(b['y']-a['y'])+a['x']:c=not c
 return c
def wet(p,i):return not inside(p,i['coast']) or any(((p[0]-q['center']['x'])/q['size']['x'])**2+((p[1]-q['center']['y'])/q['size']['y'])**2<1 for q in [dict(center=i['pond'],size=i['pondSize'])]+i['pools'])
isles=json.loads((RES/'Archipelago.json').read_text())['islands']
# Real landforms and route topology, not palette variations of one clearing.
layouts={
 1:dict(size=(32,38),coast=[(-14,-9),(-13,-3),(-11,4),(-12,11),(-7,16),(-1,17),(5,14),(10,8),(12,1),(9,-5),(5,-10),(-2,-13),(-9,-12)],pond=(7,10),pondSize=(2.3,1.6),dock=(-11,-7.2),arrival=(-10,-6),roads=[route((-11,-6),(-8,-5),(-4,-3),(0,0),(3,2)),route((-4,-3),(-7,0),(-8,4),(-5,7),(-2,8)),route((-8,4),(-9,8),(-7,12),(-3,13)),route((0,0),(4,0),(7,-3),(7,-5)),route((3,2),(5,2),(6,3))],regions=[(-6,11,9,9,'Cloudfruit terraces'),(5,5,8,7,'Custard meadow'),(-1,-5,6,4,'Seedkeeper garden')],pools=[(-11,1,1.2,.8)]),
 3:dict(size=(52,24),coast=[(-24,-4),(-23,2),(-18,7),(-9,9),(-3,8),(4,9),(12,8),(22,5),(24,1),(21,-3),(14,-5),(8,-9),(0,-10),(-8,-9),(-17,-8)],pond=(17,3),pondSize=(3.8,2.1),dock=(-19,-6.5),arrival=(-18,-5.1),roads=[route((-19,-5.1),(-15,-4),(-10,-3),(-5,-3),(0,-1),(5,0),(10,0),(12,1)),route((-10,-3),(-13,0),(-10,3),(-6,3),(-4,5)),route((5,0),(5,-4),(10,-6),(13,-6)),route((0,-1),(0,4),(0,5.5)),route((10,0),(10,4),(7,5))],regions=[(-11,3,12,8,'Cinnamon terraces'),(7,4,8,6,'Spicepangolin shelf'),(3,-5,9,4,'Warm seed beds')],pools=[(-2,2,1.1,.8)]),
 4:dict(size=(36,44),coast=[(-15,-13),(-16,-5),(-13,1),(-16,8),(-12,15),(-6,19),(0,20),(7,17),(9,10),(15,6),(14,0),(8,-3),(12,-9),(7,-15),(0,-18),(-8,-16)],pond=(8,8),pondSize=(3,2.6),dock=(-10,-14),arrival=(-9,-12.5),roads=[route((-10,-12.5),(-6,-11),(-3,-7),(-3,-3),(0,0),(3,3),(4,5)),route((-3,-3),(-7,0),(-8,4),(-6,7),(-3,8)),route((-6,7),(-9,10),(-8,14),(-4,15)),route((0,0),(5,-2),(6,-5),(7,-7)),route((3,3),(1,6),(1,10),(3,12))],regions=[(-6,13,10,9,'Mooncap enclosure'),(1,10,8,8,'Pollen sanctuary'),(-6,-7,8,7,'Marsh garden')],pools=[(-10,-6,2,2.2),(4,-10,2.5,1.6)]),
 5:dict(size=(56,32),coast=[(-25,-5),(-24,2),(-20,9),(-13,12),(-5,13),(5,12),(15,9),(23,4),(25,-3),(20,-7),(16,-10),(12,-9),(10,-5),(13,-2),(12,3),(6,6),(-2,7),(-9,5),(-13,1),(-13,-4),(-10,-8),(-14,-11),(-20,-10)],pond=(-3,0),pondSize=(6,3.6),dock=(19,-6),arrival=(18,-4.5),roads=[route((18,-4.5),(19,-2),(18,2),(14,5),(9,7),(3,8),(-4,9),(-11,8),(-16,5),(-19,0),(-18,-4),(-16,-6)),route((14,5),(16,6)),route((-11,8),(-13,10)),route((-19,0),(-21,2)),route((-4,9),(-4,10.5))],regions=[(-14,7,10,7,'Coral orchard'),(17,2,7,6,'Kelp pools'),(-19,-3,8,7,'Pearlroot beds')],pools=[(-18,2,1.4,1.3)])
}
layouts[3]['roads'][2]=route((5,0),(5,-4),(10,-5.5),(12,-5.5))
layouts[5]['pools']=[(-21,4,1.1,1)]
positions={1:{'fruit-mist':(-2,8),'fruit-east':(4,0),'root-mist':(-8,5.5),'petal-mist':(-5,12),'ram-mist':(6,4)},3:{'bulb-ember':(-4,6.5),'pepper-ember':(-10,4.5),'crab-ember':(7,5.8),'sap-ember':(0,7)},4:{'cap-moon':(-4,16.5),'root-moon':(-6,8.5),'moth-moon':(3,13.5),'dew-moon':(5,5.5)},5:{'pearl-tide':(-16,-4.5),'fruit-tide':(-13,11),'snail-tide':(16,2)}}
for i in isles:
 i['size']=v(40,26);i['dock']=v(-11,-7.2);i['arrival']=v(-10,-6)
 for r in i['roads']:r['width']=.5
 if i['id'] in layouts:
  cfg=layouts[i['id']]
  for key in ['size','pond','pondSize','dock','arrival']:i[key]=v(*cfg[key])
  i['coast']=[v(*p) for p in cfg['coast']];i['roads']=cfg['roads'];i['regions']=[dict(center=v(x,y),size=v(w,h),label=label) for x,y,w,h,label in cfg['regions']];i['pools']=[dict(center=v(x,y),size=v(w,h)) for x,y,w,h in cfg['pools']]
  for p in i['points']:
   if p['source'] in positions[i['id']]:p['position']=v(*positions[i['id']][p['source']])
   if p['action']=='boat':p['position']=i['dock'].copy()
   if p['action']=='fish':p['position']=v(*{1:(7,-5),3:(12,-5.5),4:(7,-7),5:(18,-4.5)}[i['id']])
 # Saltleaf keeps its starter land use; remove intrusive generic random placements.
 if i['id']==3:
  next(p for p in i['points'] if p['kind']=='crab')['label']='Spicepangolin shelf'
 i['props']=[]
bands={0:[(-14,7,4,3),(-5,8,5,2),(3,8,4,2),(13,-2,2,3),(-14,0,2,2)],1:[(-10,15,5,2),(-12,8,2,3),(3,13,3,2),(9,1,2,3),(-3,3,2,2)],3:[(-21,3,5,2),(-15,7,7,2),(2,7,3,2),(19,-1,3,2),(-18,-3,2,2)],4:[(-12,16,4,3),(-6,18,5,2),(9,4,3,2),(-12,3,3,2),(1,-13,3,2)],5:[(-21,6,4,2),(-15,11,4,2),(-6,11,5,1),(7,9,4,1),(20,2,2,2)]}
palettes=[('#2c5149','#527d50','#87b565','#cee18c'),('#344e59','#557e74','#95c3a1','#eef0b8'),('#513f42','#865747','#c68850','#efc880'),('#2c384d','#515575','#888cba','#d7d9e5'),('#285d5a','#479783','#8bd0ac','#e4e8b0')]
for n,i in enumerate(isles):
 rng=random.Random(194+i['id'])
 def free(p,margin=0):
  return not wet(p,i) and not road(p,i,1+margin) and all(math.dist(p,(q['position']['x'],q['position']['y']))>2.2 for q in i['points']) and not(i['id']==0 and -9<p[0]<-3 and p[1]<3) and not(i['id']==1 and 1<p[0]<5 and 2<p[1]<6)
 for x,y,cols,rows in bands[i['id']]:
  for row in range(rows):
   for col in range(cols):
    p=(round(x+col*1.5+row*.2+rng.uniform(-.25,.25),2),round(y-row*1.6+rng.uniform(-.18,.18),2))
    if free(p):i['props'].append(dict(art='tree-'+i['tree'],position=v(*p),role='tree'))
 for reg in i['regions']:
  cx,cy=reg['center'].values();sx,sy=reg['size'].values()
  for k in range(32):
   p=(round(cx+rng.uniform(-sx*.48,sx*.48),2),round(cy+rng.uniform(-sy*.48,sy*.48),2))
   if free(p) and k%4!=0:i['props'].append(dict(art='grass-'+i['key'],position=v(*p),role='grass'))
 for x,y in {0:[(7,-5),(9,-4),(-12,0)],1:[(8,-3),(-10,10),(2,12)],3:[(8,7),(11,5),(-18,1),(18,-2),(7,-5)],4:[(-12,12),(9,4),(-8,-13)],5:[(-22,4),(-17,-7),(20,1)]}[i['id']]:
  if free((x,y)):i['props'].append(dict(art='rock-'+i['key'],position=v(x,y),role='stone'))
 # Story props are clustered beside routes and have real footprints.
 for x,y,name in {0:[(-4,3,'market-crates'),(-9,-3.4,'garden-fence'),(1.5,-4,'harbor-lantern')],1:[(-2,11,'orchard-cart'),(5,7,'apiary')],3:[(-16,-1,'quarry-cart'),(2,4,'spice-kiln')],4:[(-5,11,'mushroom-shrine'),(-1,-8,'marsh-lantern')],5:[(-10,10,'shell-shrine'),(16,-1,'drying-rack')]}[i['id']]:
  if free((x,y),.1):i['props'].append(dict(art=name,position=v(x,y),role='landmark'))
(RES/'Archipelago.json').write_text(json.dumps(dict(islands=isles),indent=2),encoding='utf-8')

# Terrain composition uses layered soil/moss clusters; paths have stone joints/timber.
for index,i in enumerate(isles):
 W,H=int(i['size']['x']*32),int(i['size']['y']*32);rng=random.Random(401+i['id']);pal=palettes[index]
 def px(x,y):return (round(W/2+x*32),round(H/2-y*32))
 def pp(q):return px(q['x'],q['y'])
 im=Image.new('RGBA',(W,H),'#244f68');d=ImageDraw.Draw(im);poly=[pp(q) for q in i['coast']]
 for k in range(W*H//500):
  x=rng.randrange(W);y=rng.randrange(H);d.line((x,y,x+rng.randrange(5,15),y),fill=rng.choice(['#2b6079','#326c83','#2b5d78']))
 for width,color in [(46,'#347884'),(28,'#70b4b0'),(14,'#d3c99e')]:d.line(poly+[poly[0]],fill=color,width=width,joint='curve')
 d.polygon(poly,fill=i['ground'])
 # Interlocking patches of leaf litter/moss give habitats a floor, rather than floating props.
 def blend(a,b,t):return '#'+''.join(f'{round(int(a[k:k+2],16)*(1-t)+int(b[k:k+2],16)*t):02x}' for k in (1,3,5))
 for reg in i['regions']:
  cx,cy=reg['center'].values();sx,sy=reg['size'].values()
  for k in range(12):
   x=cx+rng.uniform(-sx*.42,sx*.42);y=cy+rng.uniform(-sy*.42,sy*.42)
   if wet((x,y),i) or road((x,y),i,1.1):continue
   a,b=px(x,y);r=rng.randrange(9,22);col=blend(i['ground'],pal[1],.22)
   d.polygon([(a-r,b),(a-r+3,b-r//2),(a-1,b-r//2-2),(a+r,b-2),(a+r-3,b+r//2),(a-6,b+r//2+1)],fill=col)
   for j in range(8):
    xx=a+rng.randrange(-r+3,r-2);yy=b+rng.randrange(-r//3,r//3+1);d.line((xx,yy,xx+3,yy),fill=blend(i['ground'],pal[2],.28))
 # Broad subtle patches, local texture rather than regular visual noise.
 for reg in i['regions']:
  cx,cy=reg['center'].values();sx,sy=reg['size'].values()
  for k in range(180):
   x=cx+rng.uniform(-sx/2,sx/2);y=cy+rng.uniform(-sy/2,sy/2)
   if wet((x,y),i) or road((x,y),i,.8):continue
   a,b=px(x,y);col=pal[1] if k%3 else pal[2]
   d.line((a,b,a+3,b-1,a+5,b),fill=col)
   if k%7==0:d.rectangle((a+2,b-4,a+3,b-3),fill=pal[3]);d.point((a+2,b-2),fill=pal[1])
 # Mineral terraces and orchard retaining contours are outside navigable road lanes.
 if index in (1,2):
  for reg in i['regions'][:1]:
   cx,cy=reg['center'].values()
   for j in range(3):
    for k in range(20):
     x=cx-4+k*.42;y=cy+2-j*2
     if wet((x,y),i) or road((x,y),i,1):continue
     a,b=px(x,y);d.polygon([(a,b),(a+12,b-2),(a+13,b+5),(a,b+7)],fill=pal[0]);d.line((a+1,b,a+11,b-1),fill=pal[3]);d.line((a+1,b+2,a+11,b+1),fill=pal[2])
 for q in [dict(center=i['pond'],size=i['pondSize'])]+i['pools']:
  if not inside((q['center']['x'],q['center']['y']),i['coast']):continue
  x,y=pp(q['center']);rx,ry=q['size']['x']*32,q['size']['y']*32
  d.ellipse((x-rx-7,y-ry-6,x+rx+7,y+ry+6),fill=pal[0]);d.ellipse((x-rx-3,y-ry-2,x+rx+3,y+ry+2),fill=pal[2]);d.ellipse((x-rx,y-ry,x+rx,y+ry),fill='#365e80' if index==3 else '#398395')
  for k in range(60):
   xx=rng.uniform(-rx*.9,rx*.9);yy=rng.uniform(-ry*.9,ry*.9)
   if (xx/rx)**2+(yy/ry)**2<.75:d.line((x+xx,y+yy,x+xx+7,y+yy),fill='#79b9be')
  # Reeds and shallow pebbles define the banks, away from walkable paths.
  for k in range(26):
   t=k*math.tau/26;a=x+math.cos(t)*(rx+9);b=y+math.sin(t)*(ry+9);world=((a-W/2)/32,(H/2-b)/32)
   if road(world,i,.65):continue
   d.line((a,b,a-1,b-6),fill=pal[1]);d.line((a+2,b,a+3,b-4),fill=pal[2]);d.point((a,b-7),fill=pal[3])
 # Dirt trail, cobbled village road, carved mineral path, boardwalk, shell paving.
 for r in i['roads']:
  pts=[pp(p) for p in r['points']];half=round(r['width']*32)
  for rad,color in [(half+2,blend(pal[0],i['ground'],.35)),(half,i['path'])]:
   d.line(pts,fill=color,width=rad*2,joint='curve')
   if index!=3:
    for x,y in pts:d.ellipse((x-rad,y-rad,x+rad,y+rad),fill=color)
 # Texture each paved pixel by analytical road mask so joints never paint on grass.
 for y in range(0,H,5 if index==3 else 9):
  for x in range(0,W,11 if index==3 else 13):
   p=((x-W/2)/32,(H/2-y)/32)
   if road(p,i,-.17):
    if index==3:
     d.rectangle((x-4,y-1,x+4,y+2),fill='#ad946f');d.line((x-4,y+2,x+4,y+2),fill='#6e6159');d.point((x-3,y),fill=pal[0])
    else:
     d.line((x-4,y,x+4,y),fill='#a89170');d.line((x+4,y,x+4,y+3),fill='#a89170');d.point((x-3,y-2),fill='#e4d4a3')
 # Main harbor plaza and garden borders don't masquerade as random forage.
 if index==0:
  for x in range(-11,-5):
   a,b=px(x,-4.8);d.rectangle((a,b,a+22,b+4),fill='#876f55');d.line((a,b,a+22,b),fill='#d4bf90')
 # Pier based on each actual landing point.
 x,y=pp(i['dock']);d.rectangle((x-26,y-18,x+26,y+42),fill=INK);d.rectangle((x-23,y-17,x+23,y+38),fill='#a27d54')
 for b in range(y-14,y+39,7):d.line((x-23,b,x+23,b),fill='#dfb67b');d.point((x-20,b),fill=INK);d.point((x+20,b),fill=INK)
 im.save(ART/(i['key']+'.png'))

# Five different canopy structures: clustered deciduous, fruit bough, pine, mushroom, coral.
for n,i in enumerate(isles):
 pal=palettes[n]
 for frame in range(4):
  def tree(d,n=n,f=frame,p=pal):
   shift=[0,1,0,-1][f];d.ellipse((12,86,54,95),fill='#263f4260')
   d.polygon([(25,88),(29,49),(36,49),(39,88),(44,92),(20,92)],fill=INK);d.polygon([(29,87),(31,48),(34,48),(36,88)],fill='#94674d');d.line((31,55,31,87),fill='#c49765',width=2)
   if n==2:
    for y,w in [(3,14),(18,23),(34,29)]:
     d.polygon([(32+shift,y),(32-w,y+30),(32+w,y+30)],fill=p[0]);d.polygon([(32+shift,y+3),(35-w,y+26),(29+w,y+26)],fill=p[1]);d.line((32+shift,y+6,34-w,y+24),fill=p[2],width=3)
    for x,y in [(21,35),(44,52),(31,19)]:d.rectangle((x,y,x+2,y+5),fill=p[3])
   elif n==3:
    d.rectangle((25,39,38,73),fill='#bca9bb');d.line((28,42,28,66),fill='#e1d4c3',width=2)
    d.polygon([(3,43),(7,22),(19,11),(33,5),(46,12),(58,25),(62,43),(49,51),(15,51)],fill=INK)
    d.polygon([(6,40),(11,24),(23,14),(34,9),(44,15),(55,27),(58,41)],fill=p[1]);d.line((13,23,25,15,35,13),fill=p[2],width=3);d.ellipse((14+shift,23,24+shift,29),fill=p[3]);d.ellipse((40,30,48,36),fill=p[2]);d.line((9,44,54,44),fill='#c0b2ce',width=2)
   elif n==4:
    for x,y in [(14,20),(25,8),(40,14),(47,29)]:
     d.line((32,70,x+shift,y),fill=p[0],width=9);d.line((32,67,x+shift,y),fill=p[1],width=5);d.line((x,y+12,x-8,y+6),fill=p[2],width=4);d.ellipse((x-5+shift,y-7,x+7+shift,y+5),fill=p[1]);d.arc((x-3+shift,y-6,x+5+shift,y+2),180,310,fill=p[3],width=2)
   else:
    for x,y,rx,ry in [(20,42,18,16),(45,40,16,17),(31,26,22,22),(33,13,14,12)]:
     x+=shift if y<30 else 0;d.ellipse((x-rx,y-ry,x+rx,y+ry),fill=p[0]);d.ellipse((x-rx+2,y-ry+2,x+rx-3,y+ry-4),fill=p[1]);d.arc((x-rx+4,y-ry+3,x+rx-4,y+ry-5),190,300,fill=p[2],width=4)
    for x,y in [(14,37),(29,18),(43,26),(25,44),(41,46)]:
     d.line((x,y,x+4,y-2,x+8,y),fill=p[2],width=2);d.point((x+4,y-2),fill=p[3])
    if n==1:
     for x,y in [(13,42),(45,37),(33,24)]:d.ellipse((x-3,y-2,x+4,y+5),fill='#c78e6e');d.ellipse((x-2,y-2,x+2,y+2),fill='#f1cd8c')
  save('tree-'+i['tree']+'-'+str(frame),(64,96),tree)
 save('tree-'+i['tree'],(64,96),lambda d:tree(d,f=0))
 for f in range(3):
  def grass(d,f=f,p=pal,n=n):
   d.ellipse((1,11,26,15),fill=p[0]+'50')
   for x,y in [(4,11),(10,14),(15,10),(22,13)]:
    s=[0,1,0][f];d.line((x,y,x-2+s,y-5),fill=p[1]);d.line((x,y,x+2+s,y-4),fill=p[2]);d.point((x-2+s,y-6),fill=p[2])
    if n in (1,3) and x==15:d.point((x,y-5),fill=p[3])
  save('grass-'+i['key']+'-'+str(f),(28,16),grass)
 save('grass-'+i['key'],(28,16),lambda d:grass(d,f=0))
 def rock(d,p=pal):
  d.ellipse((3,24,42,31),fill='#293c4260');d.polygon([(4,24),(9,9),(22,3),(36,7),(43,24),(31,28),(12,28)],fill=p[0]);d.polygon([(8,21),(13,10),(22,6),(33,10),(38,21)],fill=p[1]);d.polygon([(13,10),(22,6),(31,10),(25,18)],fill=p[2]);d.line((22,7,22,15,15,21),fill=p[3]);d.line((27,17,34,23),fill=p[0],width=2)
 save('rock-'+i['key'],(48,32),rock)

# Props use grounded outlines, native material clusters and stable depth footprints.
for name in ['market-crates','garden-fence','harbor-lantern','orchard-cart','apiary','quarry-cart','spice-kiln','mushroom-shrine','marsh-lantern','shell-shrine','drying-rack']:
 def prop(d,name=name):
  d.ellipse((5,48,57,59),fill='#283c4260')
  if 'lantern' in name:
   d.rectangle((29,13,33,55),fill=INK);d.line((31,14,20,14),fill='#ad835f',width=3);d.polygon([(16,11),(20,7),(25,11),(25,25),(17,25)],fill=INK);d.rectangle((18,12,23,23),fill=GOLD);d.rectangle((19,13,21,21),fill=CREAM)
  elif 'shrine' in name or name=='spice-kiln':
   d.polygon([(10,54),(13,32),(22,17),(32,10),(45,25),(52,54)],fill=INK);d.polygon([(14,51),(17,32),(30,15),(41,25),(48,51)],fill='#697d82' if name!='spice-kiln' else '#986850');d.rectangle((24,31,38,49),fill=INK);d.ellipse((26,34,36,43),fill='#b5bdcf' if 'mushroom' in name else GOLD);d.line((19,30,31,19,39,27),fill='#d0c2a1',width=2)
  elif name=='drying-rack':
   d.rectangle((7,15,10,55),fill=INK);d.rectangle((51,15,54,55),fill=INK);d.line((8,20,53,20),fill='#b69668',width=3)
   for x in range(15,49,10):d.line((x,20,x,29),fill=CREAM);d.ellipse((x-3,27,x+4,45),fill='#68a998');d.line((x,29,x,43),fill='#c6d0a0')
  elif name=='garden-fence':
   for x in range(4,61,14):d.polygon([(x,52),(x,27),(x+3,23),(x+7,27),(x+7,52)],fill='#967757');d.line((4,36,60,36),fill='#c4a16f',width=4);d.line((4,45,60,45),fill='#c4a16f',width=4)
  else:
   d.rectangle((9,29,53,51),fill=INK);d.rectangle((11,31,51,48),fill='#9d744e')
   for y in [33,40,47]:d.line((12,y,50,y),fill='#d2a96c');d.line((15,32,15,46),fill='#594638');d.line((46,32,46,46),fill='#594638')
   if 'cart' in name:
    for x in [16,46]:d.ellipse((x-5,46,x+5,57),fill=INK);d.ellipse((x-2,49,x+2,54),fill='#a79474')
   for x in [19,30,42]:d.ellipse((x-5,20,x+5,33),fill='#b69167' if 'quarry' in name else '#85a56d');d.line((x-2,22,x+2,22),fill=CREAM)
 save(name,(64,64),prop)

# Every creature is independently drawn, with different anatomy and idle motion.
def boar(d,f):
 b=[0,1,0,-1][f];d.ellipse((9,57,74,69),fill='#233b435a')
 for x in [18,32,55,66]:d.polygon([(x,43),(x+7,43),(x+6,61),(x+9,64),(x,64)],fill=INK);d.rectangle((x+1,45,x+5,59),fill='#a87653')
 d.ellipse((9,25+b,72,56+b),fill=INK);d.ellipse((12,27+b,69,53+b),fill='#9c714f');d.arc((16,27+b,64,52+b),175,270,fill='#cc9b62',width=4)
 d.ellipse((4,29+b,31,52+b),fill=INK);d.ellipse((6,31+b,28,49+b),fill='#b88a60');d.ellipse((1,41+b,18,52+b),fill='#dfb585');d.rectangle((4,45+b,5,47+b),fill=INK);d.rectangle((10,45+b,11,47+b),fill=INK)
 d.polygon([(12,43+b),(10,35+b),(7,39+b),(7,47+b)],fill=CREAM);d.polygon([(20,32+b),(16,17+b),(10,20+b),(14,34+b)],fill=INK);d.polygon([(17,28+b),(14,21+b),(12,23+b)],fill='#ce957b');d.rectangle((21,35+b,23,38+b),fill=INK);d.point((22,35+b),fill=CREAM)
 d.line((68,38,78,32,77,24,73,23),fill=INK,width=3)
 d.rounded_rectangle((30,9+b,63,38+b),radius=7,fill=INK);d.rounded_rectangle((33,11+b,60,35+b),radius=6,fill='#739d92');d.ellipse((33,8+b,60,19+b),fill='#bdd2b0');d.ellipse((36,10+b,57,17+b),fill='#bc8755');d.line((38,23+b,56,23+b),fill='#aaceb7',width=2);d.rectangle((47,4+b,51,9+b),fill=INK);d.rectangle((49,1,50,3),fill='#eee6c7')
def ram(d,f):
 b=f%2;d.ellipse((5,49,68,60),fill='#283c435a')
 for x in [15,29,48,58]:d.rectangle((x,35,x+6,52),fill='#9c7356');d.rectangle((x-1,49,x+6,54),fill=INK)
 d.ellipse((11,14+b,65,45+b),fill=INK)
 for x,y,r in [(19,29,13),(29,20,13),(44,22,14),(55,32,13),(34,35,14)]:d.ellipse((x-r,y-r+b,x+r,y+r+b),fill='#ceae7d');d.ellipse((x-r+1,y-r+1+b,x+r-2,y+r-4+b),fill='#f1db9d');d.arc((x-r+3,y-r+2+b,x+r-4,y+r-4+b),180,300,fill='#fff2c4',width=2)
 d.ellipse((2,16+b,24,39+b),fill=INK);d.ellipse((5,18+b,22,36+b),fill='#bd865d');d.ellipse((0,30+b,19,41+b),fill='#edd4a1');d.rectangle((9,26+b,11,29+b),fill=INK);d.line((6,35+b,13,36+b),fill='#8f5d4b')
 for x in [8,22]:d.ellipse((x-8,5+b,x+7,20+b),fill=INK);d.ellipse((x-6,6+b,x+5,18+b),fill='#c69b59');d.arc((x-4,8+b,x+3,15+b),0,290,fill='#755945',width=2)
 d.polygon([(59,25),(73,20),(72,27),(63,30)],fill=CREAM)
def pangolin(d,f,ready=False):
 b=f%2;d.ellipse((7,43,83,57),fill='#28373f55')
 d.polygon([(17,41),(9,49),(12,53),(25,47),(30,43)],fill=INK);d.polygon([(51,40),(48,51),(55,52),(63,43)],fill=INK)
 d.polygon([(35,35),(61,28),(83,36),(90,45),(82,51),(72,48),(61,41),(40,46)],fill=INK);d.polygon([(58,31),(80,38),(86,45),(81,48),(62,38)],fill='#9f694b')
 d.ellipse((16,10+b,68,45+b),fill=INK);d.ellipse((18,12+b,65,42+b),fill='#9b6248')
 for row in range(3):
  for col in range(5-row):
   x=25+col*8+row*3;y=14+row*8+b
   d.polygon([(x,y),(x+7,y-3),(x+9,y+4),(x+4,y+9),(x-2,y+5)],fill='#513e3d');d.polygon([(x+1,y),(x+6,y-1),(x+7,y+4),(x+4,y+6)],fill='#d29155' if not ready else '#e6bd70');d.line((x+1,y,x+5,y-1),fill='#f4c987')
 d.polygon([(22,30+b),(11,27+b),(1,36+b),(3,42+b),(23,40+b)],fill=INK);d.polygon([(20,32+b),(11,30+b),(4,36+b),(5,39+b),(22,37+b)],fill='#c79663');d.rectangle((13,32+b,15,34+b),fill=INK);d.point((14,32+b),fill=CREAM)
 if ready:d.ellipse((37,15,46,23),fill=GOLD);d.line((40,14,40,10),fill=CREAM)
def moth(d,f):
 b=[0,2,0,-1][f];fold=[0,4,8,4][f];d.ellipse((16,52,60,58),fill='#2a3a4350')
 for side in [-1,1]:
  def flip(points):return [(38+side*x,y+b) for x,y in points]
  d.polygon(flip([(2,24),(11,4),(31-fold,10),(32-fold,30),(15,36),(4,31)]),fill=INK);d.polygon(flip([(4,25),(13,7),(28-fold,12),(29-fold,27),(14,32)]),fill='#bba0d1');d.polygon(flip([(4,31),(25-fold,29),(30-fold,43),(17,49),(5,39)]),fill='#806caa')
  d.line(flip([(7,25),(16,15),(23-fold,18)]),fill='#e5c9dd',width=3);cx=38+side*(20-fold/2);d.ellipse((cx-4,17+b,cx+4,25+b),fill=GOLD);d.ellipse((cx-2,19+b,cx+2,23+b),fill=INK)
 d.ellipse((31,21+b,45,46+b),fill=INK);d.ellipse((33,23+b,43,43+b),fill='#ead8b8');d.line((34,30+b,42,30+b),fill='#b492ae',width=2);d.ellipse((30,17+b,46,29+b),fill=CREAM)
 for x in [33,43]:d.line((x,19+b,x-2 if x<38 else x+2,10+b),fill=INK,width=2);d.ellipse((x-4 if x<38 else x,7+b,x if x<38 else x+4,11+b),fill=GOLD)
 d.point((34,23+b),fill=INK);d.point((42,23+b),fill=INK)
def snail(d,f):
 b=f%2;d.ellipse((3,47,78,58),fill='#243f4350');d.ellipse((4,32+b,76,53+b),fill=INK);d.ellipse((7,35+b,73,50+b),fill='#79b49b');d.line((12,48,63,48),fill='#b6d8ab',width=2)
 d.ellipse((25,6+b,68,44+b),fill=INK);d.ellipse((28,8+b,65,41+b),fill='#4e897d')
 pts=[]
 for k in range(70):t=k/69*math.tau*2.3;r=17*(1-k/80);pts.append((47+math.cos(t)*r,25+b+math.sin(t)*r*.88))
 d.line(pts,fill='#bfd5a2',width=3)
 for x in [10,23]:d.line((x,38+b,x-3,24+b),fill=INK,width=4);d.line((x,36+b,x-3,24+b),fill='#91c6a8',width=2);d.ellipse((x-7,19+b,x+1,27+b),fill=CREAM);d.ellipse((x-5,21+b,x-2,25+b),fill=INK)
 d.line((12,41+b,20,42+b),fill='#436e66')
 for x,y in [(33,13),(54,12),(61,30)]:d.polygon([(x,y),(x-6,y-6),(x-1,y-7),(x+3,y-1)],fill='#9ecb94')
for name,size,fn in [('brothback',(80,72),boar),('custardram',(80,64),ram),('spicecrab',(96,64),pangolin),('mochimoth',(80,64),moth),('kelpsnail',(80,64),snail)]:
 for f in range(4):save(name+'-'+str(f),size,lambda d,f=f,fn=fn:fn(d,f))
 save(name,size,lambda d,fn=fn:fn(d,0))
save('spicecrab-cracked',(96,64),lambda d:pangolin(d,0,True))

# Characters: consistent native 32×48 anatomy, face/hair, layered garments and poses.
def person(d,direction='down',f=0,kind=None,index=-1):
 bob=0 if kind else [0,1,0,1][f%4];step=0 if kind else [0,1,0,-1][f%4]
 bend=0 if not kind else {'swing':[0,0,2,1,0],'pour':[0,1,2,2,0],'plant':[0,2,4,2,0],'pull':[0,3,4,1,0],'cast':[0,0,1,1,0],'stir':[0,1,2,1,0]}[kind][f]
 skin=['#dfad84','#b78261','#eac098','#c8906c','#dab38a','#e9bd92'][index+1];hair=['#584344','#424653','#87674b','#713c45','#393f48','#a58153'][index+1];coat=['#568f88','#ae8158','#8e789f','#5d879b','#b57660','#729259'][index+1]
 d.ellipse((7,42,25,47),fill='#23383e55')
 for x,dy in [(10,step),(20,-step)]:
  d.rectangle((x,32+dy,x+4,41+dy),fill=INK);d.rectangle((x+1,33+dy,x+3,39+dy),fill='#566173');d.rectangle((x-1,40+dy,x+5,43+dy),fill='#74543e');d.line((x,40+dy,x+3,40+dy),fill='#b28a61')
 y=22+bob+bend
 d.polygon([(9,y),(23,y),(26,y+6),(23,35),(9,35),(6,y+6)],fill=INK);d.polygon([(10,y+1),(22,y+1),(24,y+7),(22,33),(10,33),(8,y+7)],fill=coat)
 if direction!='up':
  d.polygon([(11,y+5),(21,y+5),(23,35),(9,35)],fill='#d7c596' if index>=0 else CREAM);d.rectangle((12,y+9,20,y+11),fill='#b19c79');d.line((15,y+9,15,y+12),fill='#eedead');d.polygon([(11,y+1),(16,y+4),(21,y+1),(18,y+7),(14,y+7)],fill='#b96454' if index<0 else GOLD)
 else:d.line((10,y+3,22,y+3),fill='#97b6a1');d.rectangle((10,31,22,32),fill='#baab87')
 if index in [0,1,3,4] and direction!='up':
  # Guests wear field clothes, not five identical chef aprons.
  if index==0:
   d.rectangle((10,y+6,21,33),fill='#936946');d.line((16,y+5,16,33),fill='#d6b277');d.rectangle((11,y+10,14,y+12),fill='#b38e59');d.rectangle((18,y+10,20,y+12),fill='#b38e59')
  elif index==1:
   d.polygon([(10,y+3),(21,y+3),(25,35),(7,35)],fill='#8c729c');d.line((11,y+5,17,y+10,21,y+5),fill='#d9bba8',width=2);d.rectangle((12,31,21,33),fill='#c5a679')
  elif index==3:
   d.rectangle((10,y+5,22,33),fill='#bd7d61');d.line((11,y+3,20,32),fill='#5f4c46',width=2);d.rectangle((18,y+10,23,y+16),fill='#83654e')
  else:
   d.polygon([(10,y+4),(22,y+4),(24,34),(8,34)],fill='#718958');d.rectangle((10,31,22,33),fill='#c4a075');d.line((12,y+6,20,y+6),fill='#d4ce9c')
 hy=8+bob+bend
 d.rounded_rectangle((8,hy,24,hy+16),radius=5,fill=INK);d.rounded_rectangle((9,hy+1,23,hy+14),radius=4,fill=skin);d.rectangle((8,hy+1,24,hy+5),fill=hair);d.polygon([(9,hy+3),(14,hy+3),(11,hy+8),(9,hy+9)],fill=hair)
 if direction=='up':d.rounded_rectangle((9,hy+2,23,hy+13),radius=3,fill=hair);d.line((11,hy+4,16,hy+3,21,hy+4),fill='#957157')
 else:
  for x in ([11] if direction=='left' else [21] if direction=='right' else [12,20]):d.rectangle((x,hy+8,x+1,hy+10),fill=INK);d.point((x,hy+8),fill=CREAM)
  d.line((15,hy+13,18,hy+13),fill='#ac6f5b');d.point((11,hy+12),fill='#d89278');d.point((22,hy+12),fill='#d89278')
 if index==-1:
  d.rectangle((8,hy-1,24,hy+2),fill='#b9c6b4');d.rectangle((8,hy-2,24,hy),fill=CREAM)
  for x,y,r in [(11,hy-4,4),(17,hy-6,5),(22,hy-4,3)]:d.ellipse((x-r,y-r,x+r,y+r),fill=INK);d.ellipse((x-r+1,y-r+1,x+r-1,y+r-1),fill=CREAM)
  d.line((13,hy-8,18,hy-8),fill='#fff4d3')
 elif index==0:
  d.polygon([(5,hy+1),(10,hy-6),(22,hy-6),(27,hy+1)],fill=INK);d.polygon([(8,hy),(12,hy-5),(21,hy-5),(24,hy)],fill='#b28d58');d.line((5,hy+2,27,hy+2),fill=GOLD,width=2)
 elif index==1:d.polygon([(8,hy+2),(10,hy-4),(16,hy-7),(23,hy-3),(24,hy+2)],fill='#9e739b');d.line((12,hy-4,20,hy-3),fill='#ceb3c2',width=2)
 elif index==2:d.rectangle((8,hy-1,24,hy+2),fill='#bfa078');d.polygon([(9,hy-2),(12,hy-6),(23,hy-5),(24,hy-1)],fill='#4d7b86')
 elif index==3:d.ellipse((7,hy-6,24,hy+3),fill='#884851');d.line((9,hy-2,21,hy-3),fill='#c77a72',width=2)
 else:d.polygon([(7,hy+1),(9,hy-5),(19,hy-7),(24,hy-2),(25,hy+2)],fill='#839569');d.point((21,hy-4),fill=CREAM)
 arm=[(23,y+3),(26,y+9)]
 if kind:
  if kind in ['swing','cast']:arm=[[(23,y+3),(26,y+9)],[(23,y+2),(28,y-8)],[(23,y+3),(29,y+8)],[(23,y+4),(26,y+12)],[(23,y+3),(26,y+9)]][f]
  elif kind in ['plant','pull']:arm=[(23,y+3),(27,y+7+f*2 if f<3 else y+3)]
  else:arm=[(23,y+3),(28,y+4+(f%2)*2)]
 if direction=='left':arm=[(31-x,y) for x,y in arm]
 d.line(arm,fill=INK,width=4);d.line(arm,fill=skin,width=2);d.line((8,y+3,6,y+8),fill=INK,width=4);d.line((8,y+3,6,y+8),fill=skin,width=2)
for direction in ['down','up','left','right']:
 for f in range(4):save(f'chef-{direction}-{f}',(32,48),lambda d,direction=direction,f=f:person(d,direction,f))
 for kind in ['swing','pour','plant','pull','cast','stir']:
  for f in range(5):save(f'action-{kind}-{direction}-{f}',(32,48),lambda d,direction=direction,f=f,kind=kind:person(d,direction,f,kind))
for f in range(4):save('player-'+str(f),(32,48),lambda d,f=f:person(d,'down',f))
for index in range(5):
 save('guest-'+str(index),(32,48),lambda d,index=index:person(d,index=index))
 for f in range(4):save(f'visitor-{index}-{f}',(32,48),lambda d,index=index,f=f:person(d,f=f,index=index))
save('nori',(32,48),lambda d:person(d,index=2))

# Rooted forage species: edible biology determines the silhouette and leaf structure.
for species in ['pepperbell','lanternroot','custardpetal','emberbulb','mooncap','pearlsprout']:
 def plant(d,s=species):
  d.ellipse((3,29,34,37),fill='#233e3d50')
  if s=='mooncap':
   for x,y,r in [(10,18,7),(25,13,9),(19,27,6)]:
    d.rectangle((x-2,y,x+2,33),fill='#e0c8b4');d.ellipse((x-r,y-r//2,x+r,y+4),fill='#354056');d.ellipse((x-r+1,y-r//2+1,x+r-1,y+2),fill='#a197c5');d.point((x-2,y-1),fill='#e7dfca');d.point((x+3,y),fill='#d5c7e1')
  elif s=='pearlsprout':
   for x,y in [(10,23),(25,22),(18,17)]:d.polygon([(18,33),(x-8,y),(x,y-7),(x+8,y),(22,33)],fill='#467964');d.line((18,32,x,y-3),fill='#9fbf8a',width=2)
   for x,y in [(14,19),(22,20),(17,25),(25,26)]:d.ellipse((x-3,y-3,x+3,y+3),fill='#d4d7a8');d.point((x-1,y-1),fill=CREAM)
  elif s in ['lanternroot','emberbulb']:
   color='#dfb666' if s=='lanternroot' else '#bd7151'
   d.polygon([(10,23),(17,18),(24,22),(25,30),(18,36),(10,30)],fill=INK);d.polygon([(12,24),(17,21),(22,24),(22,30),(18,33),(13,29)],fill=color);d.line((14,25,14,29),fill='#f6d58a');d.line((18,34,17,38),fill='#b88d57')
   for pts in [[(18,23),(5,17),(2,10),(13,12)],[(18,23),(25,10),(35,8),(32,19)],[(18,23),(14,9),(20,3),(24,12)]]:
    d.polygon(pts,fill='#456e51');d.line((pts[0],pts[-1]),fill='#83a967',width=2)
   if s=='lanternroot':d.ellipse((15,21,21,29),fill='#f7d991')
  elif s=='custardpetal':
   for x,y in [(10,19),(26,15),(20,27)]:
    d.line((x,y,18,34),fill='#547b56',width=2)
    for a in range(5):
     xx=x+math.cos(a*math.tau/5)*5;yy=y+math.sin(a*math.tau/5)*4;d.ellipse((xx-3,yy-3,xx+3,yy+3),fill='#e4be7a');d.point((xx,yy-1),fill=CREAM)
    d.ellipse((x-2,y-2,x+2,y+2),fill='#b6764f')
  else:
   d.line((18,33,18,9,27,9,29,15),fill='#385c47',width=2);d.line((18,21,9,16,9,20),fill='#537b4c',width=2)
   for x,y in [(9,24),(28,19)]:d.polygon([(x-5,y-7),(x+4,y-7),(x+6,y+2),(x+3,y+5),(x-4,y+5),(x-6,y+1)],fill=INK);d.polygon([(x-3,y-5),(x+2,y-5),(x+4,y+1),(x+2,y+3),(x-3,y+3)],fill='#bd6d52');d.line((x-2,y-3,x-2,y+1),fill='#ecc186')
   d.polygon([(18,12),(9,7),(7,4),(15,5)],fill='#8dae61');d.polygon([(18,15),(22,8),(30,6),(28,13)],fill='#729957')
 save(species,(40,40),plant)

# Measured, intentionally tiny pixel alphabet for world signs; no variable default font.
glyphs={'A':['010','101','111','101','101'],'B':['110','101','110','101','110'],'C':['011','100','100','100','011'],'D':['110','101','101','101','110'],'E':['111','100','110','100','111'],'F':['111','100','110','100','100'],'G':['011','100','101','101','011'],'H':['101','101','111','101','101'],'I':['111','010','010','010','111'],'K':['101','101','110','101','101'],'L':['100','100','100','100','111'],'N':['101','111','111','111','101'],'O':['010','101','101','101','010'],'P':['110','101','110','100','100'],'R':['110','101','110','101','101'],'S':['011','100','010','001','110'],'T':['111','010','010','010','010'],'U':['101','101','101','101','111']}
def lettering(d,text,box,color=CREAM):
 x0,y0,x1,y1=box;width=len(text)*4-1;x=x0+(x1-x0+1-width)//2;y=y0+(y1-y0+1-5)//2
 assert width<=x1-x0+1 and 5<=y1-y0+1,(text,box)
 for ch in text:
  for row,bits in enumerate(glyphs.get(ch,['000']*5)):
   for col,bit in enumerate(bits):
    if bit=='1':d.point((x+col,y+row),fill=color)
  x+=4
for word in ['OPEN','CLOSED']:
 def board(d,word=word):
  d.ellipse((4,38,45,46),fill='#263b4255');d.polygon([(7,42),(12,2),(38,2),(43,42)],fill=INK);d.polygon([(10,37),(14,5),(35,5),(39,37)],fill='#b48757');d.rectangle((11,7,39,24),fill=INK);d.rectangle((13,9,37,22),fill='#39645b');lettering(d,word,(13,10,37,21));d.line((15,29,34,29),fill=GOLD);d.line((14,34,35,34),fill='#7b5d45')
 save('sign-'+word.lower(),(48,48),board)
# Existing repo-native building authoring is retained; replace only its flawed sign strip.
for name,text in [('restaurant','HARBOR TABLE'),('iona-house','TIDEKEEPER')]:
 im=Image.open(ART/(name+'.png')).convert('RGBA');d=ImageDraw.Draw(im);d.rectangle((56,89,133,106),fill=INK);d.rectangle((59,92,130,103),fill='#795941');lettering(d,text,(61,93,128,102));im.save(ART/(name+'.png'))
content=json.loads((RES/'Content.json').read_text())
spice=next(q for q in content['ingredients'] if q['id']=='spiceclaw');spice['name']='Pangolin spice';spice['habitat']='Emberfold mineral shelf';spice['description']='A cinnamon-plated pangolin. Three careful pickaxe contacts loosen shed spice plates. Collect the visible spice with E or right-click its body; the animal stays unharmed.'
(RES/'Content.json').write_text(json.dumps(content,ensure_ascii=False,indent=2),encoding='utf-8')
print('Wonder art: five distinct landforms, native cast, separate creature anatomy, measured signs.')
