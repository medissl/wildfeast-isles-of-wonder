"""Original native pixel assets and outer-island regions. Run after wonder_art.py.
No external pixels. The existing terrain painter is reused without executing its redesign.
"""
from pathlib import Path
from PIL import Image,ImageDraw
import ast,json,math,random
ROOT=Path(__file__).resolve().parents[1];RES=ROOT/'Game/Assets/Wildfeast/Resources';ART=RES/'Art'
catalog=json.loads((RES/'Archipelago.json').read_text());isles=catalog['islands']
def v(x,y):return dict(x=x,y=y)
destinations={0:(-2,15),1:(-3,24),3:(29,2),4:(-3,27),5:(-29,0)}
labels=['The forgotten dinner bell','The rain keeper kettle','The caramel quarry','The moon harp','The homeward compass']
items=['leafgill','cloudfruit','spiceclaw','mochipollen','pearlsprout']
if not catalog.get('firstLight'):
 for n,i in enumerate(isles):
  area=i['id'];p=destinations[area]
  if area==0:
   i['size']=v(44,38)
   for q in i['coast']:
    if -9<q['x']<5 and q['y']>8:q['y']+=8
   path=[(0,4),(-3,8),(-5,12),p,(1,13),(0,4)]
  elif area==1:
   i['size']=v(40,58)
   for q in i['coast']:
    if q['y']>=14:q['y']+=11
   path=[(-7,12),(-7,17),(-5,21),p,(0,20),(-3,13)]
  elif area==3:
   i['size']=v(76,30)
   for q in i['coast']:
    if q['x']>=21:q['x']+=11
   path=[(10,0),(15,0),(20,-1),(26,-1),p,(25,4),(21,5)]
  elif area==4:
   i['size']=v(44,68)
   for q in i['coast']:
    if q['y']>=15:q['y']+=11
   path=[(-8,14),(-8,19),(-6,24),p,(1,23),(3,12)]
  else:
   i['size']=v(78,42)
   for q in i['coast']:
    if q['x']<=-20:q['x']-=10
   path=[(-19,0),(-23,0),(-26,-2),p,(-28,5),(-23,7)]
  i['roads'].append(dict(points=[v(*q) for q in path],width=.5))
  i['regions'].append(dict(center=v(*p),size=v(10,9),label=labels[n]))
  source='wonder-discovery-'+str(area)
  i['points'].append(dict(action='discovery',label=labels[n],item=items[n],source=source,art=source,kind='',position=v(*p)))
  i['subtitle']+=' / '+labels[n].replace('The ','')
 catalog['firstLight']=True
 (RES/'Archipelago.json').write_text(json.dumps(catalog,indent=2),encoding='utf-8')
# Landmarks sit beside the path; their footprint leaves the route clear.
for i in isles:
 p=destinations[i['id']];node=next(p for p in i['points'] if p['action']=='discovery');node['position']=v(p[0]-2,p[1]) if i['id']==5 else v(p[0],p[1]+1.4)
# Load only pure helpers and the proven native terrain painter from the previous generator.
tree=ast.parse((ROOT/'tools/wonder_art.py').read_text())
helpers=[node for node in tree.body if isinstance(node,ast.FunctionDef)]
pal=next(node for node in tree.body if isinstance(node,ast.Assign) and any(isinstance(t,ast.Name) and t.id=='palettes' for t in node.targets))
paint=next(node for node in tree.body if isinstance(node,ast.For) and 'W,H=int' in ast.get_source_segment((ROOT/'tools/wonder_art.py').read_text(),node))
scope=dict(Image=Image,ImageDraw=ImageDraw,Path=Path,json=json,math=math,random=random,ART=ART,INK='#273740',CREAM='#f5e2b5',GOLD='#e5b66a',isles=isles)
exec(compile(ast.Module(body=helpers+[pal],type_ignores=[]),'terrain helpers','exec'),scope)
# Fish habitats are actual water. Find the closest wet cell to each old shore marker.
for i in isles:
 fish=next(p for p in i['points'] if p['action']=='fish');origin=(fish['position']['x'],fish['position']['y'])
 if not scope['wet'](origin,i):
  choices=[(origin[0]+x*.15,origin[1]+y*.15) for x in range(-32,33) for y in range(-32,33) if scope['wet']((origin[0]+x*.15,origin[1]+y*.15),i)]
  point=min(choices,key=lambda p:math.dist(p,origin));fish['position']=v(round(point[0],2),round(point[1],2))
# Populate outer groves around paths and discoveries, with real harvestable trees/stones.
for n,i in enumerate(isles):
 p=destinations[i['id']];rng=random.Random(631+i['id'])
 if not any(prop.get('firstLight') for prop in i['props']):
  for k in range(90):
   x=p[0]+rng.uniform(-6,6);y=p[1]+rng.uniform(-5,5)
   if scope['wet']((x,y),i) or scope['road']((x,y),i,1.2) or math.dist((x,y),p)<2.4:continue
   role='tree' if k%5==0 else 'stone' if k%11==0 else 'grass'
   art='tree-'+i['tree'] if role=='tree' else ('rock-' if role=='stone' else 'grass-')+i['key']
   i['props'].append(dict(art=art,role=role,position=v(round(x,2),round(y,2)),firstLight=True))
for i in isles:
 i['props']=[prop for prop in i['props'] if not scope['road']((prop['position']['x'],prop['position']['y']),i,1.2)]
exec(compile(ast.Module(body=[paint],type_ignores=[]),'terrain painter','exec'),scope)
(RES/'Archipelago.json').write_text(json.dumps(catalog,indent=2),encoding='utf-8')
def sprite(name,w,h,draw):
 im=Image.new('RGBA',(w,h));draw(ImageDraw.Draw(im));im.save(ART/(name+'.png'))
sprite('fish-shadow',20,9,lambda d:(d.ellipse((4,1,17,7),fill='#163e5aa0'),d.polygon([(5,4),(0,0),(0,8)],fill='#163e5aa0'),d.point((15,3),fill='#94d2c3b0')))
sprite('title-cloud',72,32,lambda d:(d.ellipse((0,12,37,30),fill='#f8e6c8'),d.ellipse((15,2,55,29),fill='#fff1d1'),d.ellipse((39,9,70,30),fill='#f8e6c8'),d.line((9,29,62,29),fill='#c7d4ce',width=2)))
for n,i in enumerate(isles):
 for awake in (False,True):
  def landmark(d,n=n,awake=awake):
   ink='#273740';cream='#f5e2b5';gold='#e5b66a';shine='#dcf4b5' if awake else '#89a495'
   d.ellipse((8,53,57,62),fill='#243f4355');d.polygon([(9,52),(18,44),(48,44),(58,52),(48,58),(17,58)],fill='#596954');d.line((15,52,49,52),fill='#afb887',width=2)
   if n==0:
    d.rectangle((15,13,19,51),fill=ink);d.rectangle((45,13,49,51),fill=ink);d.rectangle((15,11,49,16),fill='#ae754f');d.line((17,12,47,12),fill=gold,width=2)
    d.polygon([(27,20),(36,20),(41,38),(45,41),(19,41),(23,38)],fill='#8d673c');d.polygon([(29,22),(34,22),(38,36),(24,36)],fill=gold);d.ellipse((29,40,34,45),fill=shine)
   elif n==1:
    d.ellipse((15,27,47,51),fill='#718f8d');d.ellipse((17,25,45,46),fill='#b3d0a5');d.polygon([(43,33),(58,24),(55,38),(43,43)],fill='#b3d0a5');d.arc((2,29,24,48),70,295,fill=gold,width=4);d.ellipse((23,20,40,27),fill=gold);d.line((30,20,30,12),fill=ink,width=2);d.ellipse((20,9,31,16),fill=shine);d.ellipse((29,4,42,14),fill=shine)
   elif n==2:
    d.polygon([(10,47),(18,27),(27,22),(37,26),(45,18),(55,44),(43,53),(20,53)],fill='#795247');d.polygon([(18,44),(23,31),(29,39),(40,32),(48,44)],fill='#e99e55' if awake else '#b77546');d.line((26,39,31,29,38,39),fill=shine,width=3)
   elif n==3:
    d.arc((12,12,54,54),170,365,fill='#8899ad',width=5);d.line((16,17,48,42),fill=gold,width=3)
    for k in range(5):d.line((20+k*5,20+k*3,20+k*5,47),fill=shine)
    if not awake:
     for x in [18,25,43]:d.line((x,53,x-4,25),fill='#617979',width=3);d.line((x,40,x+5,29),fill='#95afa0',width=2)
   else:
    d.ellipse((13,25,51,52),fill='#619e91');d.ellipse((17,27,47,46),fill='#e8d19d');d.polygon([(31,26),(39,36),(32,46),(24,36)],fill='#9b6382');d.line((32,29,32,43),fill=shine,width=3)
    for x,y in [(14,24),(45,24),(32,15)]:d.ellipse((x-4,y-4,x+4,y+4),fill=shine)
   if awake:
    for x,y in [(8,16),(52,9),(44,2)]:d.line((x-2,y,x+2,y),fill=cream);d.line((x,y-2,x,y+2),fill=cream)
  sprite('wonder-discovery-'+str(i['id'])+('-awake' if awake else ''),64,64,landmark)
# Illustrated original title vista, with existing original creatures inhabiting the foreground.
im=Image.new('RGBA',(640,360));d=ImageDraw.Draw(im)
for y in range(360):
 t=y/360;d.line((0,y,640,y),fill=(round(33+80*t),round(57+103*t),round(92+64*t),255))
rng=random.Random(882)
for k in range(70):
 x=rng.randrange(640);y=rng.randrange(180);d.point((x,y),fill='#f3d9a9')
 if k%7==0:d.line((x-1,y,x+1,y),fill='#f3d9a9');d.line((x,y-1,x,y+1),fill='#f3d9a9')
d.ellipse((500,26,550,76),fill='#f5d79e');d.ellipse((489,19,532,62),fill='#344c6b')
for x,y,w in [(35,188,130),(225,211,85),(390,185,100),(521,204,115)]:
 d.polygon([(x,y),(x+w,y-7),(x+w-19,y+21),(x+26,y+26)],fill='#344d5a');d.ellipse((x,y-17,x+w,y+9),fill='#589480');d.line((x+8,y+4,x+w-8,y+4),fill='#d3cb96',width=3)
 for j in range(4):
  a=x+18+j*23;d.rectangle((a,y-36,a+4,y-2),fill='#645745');d.ellipse((a-10,y-49,a+18,y-22),fill=['#759d83','#9c7993','#83ae82','#718c9f'][j])
for y in range(239,331,9):
 for x in range(rng.randrange(15),640,45):d.line((x,y,x+rng.randrange(10,28),y),fill='#7db2ae')
d.polygon([(0,272),(75,263),(140,278),(180,310),(220,340),(640,331),(640,360),(0,360)],fill='#344e43')
d.polygon([(0,279),(68,271),(130,285),(176,315),(181,326),(116,316),(0,323)],fill='#839865')
for x,y in [(14,274),(130,285),(598,305),(563,335)]:
 d.line((x,y,x+4,y-41),fill='#6ba19a',width=5);d.ellipse((x-11,y-55,x+21,y-26),fill='#b47b9c');d.ellipse((x-7,y-51,x+14,y-34),fill='#dba9b1');d.line((x+4,y-30,x+12,y-21),fill='#d7bf77',width=3)
for name,pos in [('brothback-0',(23,281)),('mochimoth-0',(541,192)),('spicecrab-0',(551,305))]:
 asset=Image.open(ART/(name+'.png'));im.alpha_composite(asset,pos)
im.save(ART/'title-horizon.png')
content=json.loads((RES/'Content.json').read_text(encoding='utf-8-sig'))
next(r for r in content['recipes'] if r['id']=='cloud')['name']='Cloudfruit Souffle'
if not any(i['id']=='pearlfin' for i in content['ingredients']):
 content['ingredients'].append(dict(id='pearlfin',name='Pearlfin',habitat='Pearltide coral shallows',description='A little fish with a flowering sail and pearl-soft scales. It grazes on salt blossoms around the coral islands.',icon='pearlfin'))
 content['recipes'].append(dict(id='pearlchowder',name='Pearlfin Chowder',description='Pearl-soft fish simmered with harbor grain and a drop of moon dew.',flavor='gentle',icon='dish-pearlchowder',price=96,ingredients=[dict(id='pearlfin',count=1),dict(id='grain',count=1),dict(id='dewnectar',count=1)]))
fish=next(p for i in isles if i['id']==5 for p in i['points'] if p['action']=='fish');fish['item']='pearlfin';fish['label']='Pearlfin shallows'
(RES/'Archipelago.json').write_text(json.dumps(catalog,indent=2),encoding='utf-8')
def pearlfin(d):
 d.polygon([(6,17),(0,9),(1,24),(8,20)],fill='#619e91');d.ellipse((5,10,29,24),fill='#273740');d.ellipse((7,11,28,22),fill='#8bd0ac');d.line((9,20,25,20),fill='#e8d19d',width=2)
 d.polygon([(12,11),(10,4),(16,7),(18,1),(22,7),(28,3),(25,12)],fill='#dca7bc');d.line((18,4,19,11),fill='#f5e2b5');d.rectangle((24,14,26,16),fill='#273740');d.point((24,14),fill='#f5e2b5')
 for x in [10,15,20]:d.ellipse((x,15,x+2,17),fill='#f5e2b5')
sprite('pearlfin',32,32,pearlfin);sprite('held-pearlfin',32,32,pearlfin)
def chowder(d):
 d.ellipse((4,25,44,43),fill='#273740');d.ellipse((5,17,43,36),fill='#8d6388');d.ellipse((8,16,40,31),fill='#f5e2b5');d.ellipse((10,18,38,28),fill='#afd0ad');d.line((12,20,35,20),fill='#e8d19d',width=2)
 for x,y in [(17,21),(28,23),(22,25)]:d.ellipse((x-2,y-2,x+2,y+1),fill='#e8d19d');d.point((x,y-1),fill='#619e91')
sprite('dish-pearlchowder',48,48,chowder)
held=Image.open(ART/'dish-pearlchowder.png');held.resize((24,24),Image.Resampling.NEAREST).save(ART/'held-dish-pearlchowder.png')
(RES/'Content.json').write_text(json.dumps(content,ensure_ascii=False,indent=2),encoding='utf-8')
print('5 enlarged islands, 5 responsive landmarks, native title vista and fish schools created')
