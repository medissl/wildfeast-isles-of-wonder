"""Explicit native pixel drawings for Living Districts. No image generation service."""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont,ImageEnhance
import json,random,math
ROOT=Path(__file__).resolve().parents[1];ART=ROOT/'Game/Assets/Wildfeast/Resources/Art'
FONT=ROOT/'Game/Assets/Wildfeast/Fonts/PixelifySans.ttf'
INK='#354b43';WOOD='#92613e';PAPER='#f8dfa2'
def text(d,xy,s,size,color,stroke=0):
 d.fontmode='1';d.text(xy,s,font=ImageFont.truetype(str(FONT),size),fill=color,stroke_width=stroke,stroke_fill='#76452f')
def save(im,name):im.save(ART/(name+'.png'))
def leaf(d,x,y,color='#62934c'):
 d.polygon([(x,y),(x+2,y-4),(x+8,y-6),(x+10,y-5),(x+8,y),(x+2,y+2)],fill='#385944');d.polygon([(x+2,y),(x+4,y-3),(x+8,y-4),(x+6,y)],fill=color)

# A manually composed daytime mountain / food harbor panorama at 640 x 360.
im=Image.new('RGB',(640,360));d=ImageDraw.Draw(im)
for y in range(360):
 t=min(1,y/245);d.line((0,y,639,y),fill=(round(79+108*t),round(169+60*t),round(215-5*t)))
d.ellipse((482,24,517,59),fill='#fff3b1');d.ellipse((488,30,511,53),fill='#ffe49a')
for n,col,base in [(0,'#9abdc8',198),(1,'#719ca6',227),(2,'#507e7f',247)]:
 points=[(-20,360),(-20,base)]
 for x in range(-20,670,4):points.append((x,base-round(40*math.sin(x*.008+n)**2+25*math.sin(x*.017+n*.7)**4)))
 points.append((660,360));d.polygon(points,fill=col)
# Far mountain snow and lit ridges.
d.polygon([(74,173),(131,108),(211,205)],fill='#7d9fae');d.polygon([(110,134),(131,108),(158,145),(146,142),(137,151),(131,137),(119,147)],fill='#d9e5df')
d.polygon([(423,194),(465,132),(538,212)],fill='#648f99');d.polygon([(450,151),(465,132),(486,161),(474,157),(464,167)],fill='#cfe1d8')
d.rectangle((0,248,639,359),fill='#3d9fac');rng=random.Random(14)
for _ in range(210):
 x=rng.randrange(640);y=rng.randrange(253,360);d.line((x,y,x+rng.randrange(3,12),y),fill=rng.choice(['#65bdbe','#80cecb','#318d9d']))
# Foreground banks, distinct cultivated gardens and a tiny harbor village.
for pts in [[(0,231),(50,224),(102,235),(149,266),(190,327),(191,360),(0,360)],[(640,220),(590,233),(546,263),(503,316),(476,360),(640,360)]]:
 d.polygon(pts,fill='#b99e64');d.polygon([(x,y-5) for x,y in pts],fill='#67934b')
for x,y in [(10,280),(47,299),(84,325),(544,307),(583,279),(620,248)]:
 d.rectangle((x,y,x+5,y+30),fill='#6b5539')
 for cx,cy,r in [(x-6,y+5,12),(x+7,y+5,13),(x+1,y-7,14)]:
  d.ellipse((cx-r,cy-r,cx+r,cy+r),fill='#385f42');d.ellipse((cx-r+2,cy-r+1,cx+r-3,cy+r-5),fill='#56854b')
  for k in range(10):
   px=rng.randint(cx-r+4,cx+r-5);py=rng.randint(cy-r+4,cy+r-6);d.line((px,py,px+3,py),fill='#82ad5d')
# Little gingerbread-roof inn and herb garden, preserving open central sky.
d.rectangle((21,222,65,253),fill='#e8c789');d.rectangle((22,250,69,253),fill='#886947');d.rectangle((40,235,48,251),fill='#65533e')
d.polygon([(14,224),(40,196),(74,224)],fill='#645268');d.polygon([(20,220),(40,201),(65,220)],fill='#957384')
for yy in [212,217,222]:d.line((29,yy,60,yy),fill='#bc9293')
d.rectangle((27,232,34,240),fill='#edaa59');d.rectangle((53,232,60,240),fill='#edaa59')
for x in range(20,94,9):
 d.rectangle((x,320,x+2,347),fill='#795b39');d.line((x,329,x+10,329),fill='#bc9253')
 for yy in [334,342]:leaf(d,x+3,yy,'#96b75e');d.rectangle((x+5,yy-1,x+7,yy+2),fill='#dd9159')
# Small boat and layered ripples.
d.polygon([(415,301),(450,301),(442,309),(421,309)],fill='#825e3d');d.line((433,282,433,302),fill='#725f45');d.polygon([(435,282),(435,298),(449,298)],fill='#f3dfb0');d.line((414,311,453,311),fill='#91d7cf')
save(im,'title-horizon')

# Carved wood plaque, decorative corners with restrained ingredient accents.
im=Image.new('RGBA',(360,138));d=ImageDraw.Draw(im)
d.rectangle((12,11,347,125),fill='#694a35');d.rectangle((8,15,351,121),fill='#694a35');d.rectangle((14,13,345,121),fill='#e6bb73');d.rectangle((17,17,342,117),fill='#c89456');d.rectangle((20,20,339,113),fill='#f1ce88')
for y in [27,49,78,104]:
 for x in range(25,328,27):d.line((x,y,x+15,y),fill='#ddb473')
d.line((21,21,338,21),fill='#ffe5a7');d.line((21,111,338,111),fill='#a56d3d')
for x,y in [(22,23),(333,23),(22,109),(333,109)]:d.rectangle((x,y,x+2,y+2),fill='#89633e');d.point((x,y),fill='#fff0b9')
for x,y in [(11,16),(325,119),(325,20),(14,118)]:
 leaf(d,x,y);leaf(d,x+7,y+6,'#88b057');d.ellipse((x+2,y+3,x+6,y+7),fill='#ae5553');d.point((x+3,y+3),fill='#efb28a')
text(d,(42,31),'WILDFEAST',44,'#70482f');text(d,(83,85),'ISLES OF WONDER',17,'#855a3d')
# Spoon and tiny cabbage fish at opposite ends.
d.line((12,81,26,57),fill='#725b3f',width=3);d.ellipse((22,47,34,61),fill='#e8d69c');d.ellipse((25,49,31,57),fill='#fff1ba')
d.ellipse((323,76,343,92),fill='#48785c');d.ellipse((326,77,339,89),fill='#9bbd70');d.polygon([(325,82),(318,77),(318,89)],fill='#7b9d60');d.point((337,81),fill='#283e38')
save(im,'title-plaque')
# Matching warm pixel menu panel, intentionally simpler than the title.
im=Image.new('RGBA',(64,28));d=ImageDraw.Draw(im)
d.rectangle((0,0,63,27),fill='#694a35');d.rectangle((2,2,61,25),fill='#c89456');d.rectangle((4,4,59,23),fill='#f1ce88')
d.line((4,4,59,4),fill='#ffe5a7');d.line((4,23,59,23),fill='#a56d3d')
for x,y in [(3,3),(60,3),(3,24),(60,24)]:d.point((x,y),fill='#fff0b9')
save(im,'menu-button')


# The unused rod has no baked string. Real line rendering lives in Unity.
for name in ['tool-rod','held-rod']:
 im=Image.open(ART/(name+'.png')).convert('RGBA');d=ImageDraw.Draw(im);d.rectangle((0,0,im.width-1,im.height-1),fill=(0,0,0,0))
 # Compact grip and diagonal cane. The tip is explicitly (18, 1) in image pixels.
 d.line((4,29,18,1),fill='#475c4e',width=2);d.line((5,28,18,2),fill='#c9b478');d.line((4,27,6,23),fill='#926446',width=3);d.ellipse((2,24,6,28),fill='#738d88');save(im,name)

# Inventory food: jars/portions instead of the complete animal or plant in a slot.
content=json.loads((ART.parent/'Content.json').read_text())
for n,item in enumerate(content['ingredients']):
 name=item['id'];original=Image.open(ART/(('held-'+name if (ART/('held-'+name+'.png')).exists() else item['icon'])+'.png')).convert('RGBA')
 if name=='brothback':
  im=Image.new('RGBA',(32,32));d=ImageDraw.Draw(im);d.ellipse((5,26,27,30),fill=(52,69,53,60));d.rectangle((9,6,23,26),fill='#42675d');d.rectangle((11,8,21,24),fill='#a4c9ac');d.rectangle((8,4,24,8),fill='#b9965d');d.rectangle((13,11,20,20),fill='#f5dda1');d.line((15,14,18,14),fill='#99723f');d.point((12,9),fill='#edf5d1');save(im,'held-brothback')
 else:
  bbox=original.getbbox();original=original.crop(bbox) if bbox else original;original.thumbnail((25,26),Image.Resampling.NEAREST);im=Image.new('RGBA',(32,32));im.paste(original,((32-original.width)//2,(30-original.height)//2),original)
 save(im,'item-'+name)
for recipe in content['recipes']:
 original=Image.open(ART/(recipe['icon']+'.png')).convert('RGBA');original=original.crop(original.getbbox());original.thumbnail((30,26),Image.Resampling.NEAREST);im=Image.new('RGBA',(32,32));im.paste(original,((32-original.width)//2,(32-original.height)//2),original);save(im,'item-'+recipe['icon'])

# Five native busts with authored expressions, all 64px and matching outfit palettes.
people=[('nori','#815447','#ddad80','#588f7f'),('iona','#ede0bd','#d6aa84','#8a74a2'),('saff','#463c38','#a7775d','#b77b49'),('luma','#707e8e','#edc5a0','#7392a4'),('pico','#a65947','#ba8964','#71954c')]
for n,(name,hair,skin,shirt) in enumerate(people):
 for expression in ['neutral','smile','mad','love','laugh']:
  im=Image.new('RGBA',(64,64));d=ImageDraw.Draw(im)
  d.ellipse((4,57,59,66),fill=(65,49,36,60));d.polygon([(8,63),(11,47),(21,41),(43,41),(53,48),(57,63)],fill='#354b43');d.polygon([(13,63),(15,48),(25,44),(39,44),(49,49),(52,63)],fill=shirt)
  d.rectangle((27,34,38,47),fill='#9b7057');d.rectangle((29,35,36,46),fill=skin);d.ellipse((15,4,49,41),fill='#354b43');d.ellipse((17,6,47,39),fill=hair)
  d.rectangle((20,17,44,34),fill=skin);d.polygon([(20,33),(23,39),(29,42),(36,42),(42,38),(44,33)],fill=skin)
  d.rectangle((17,11,46,19),fill=hair);d.rectangle((18,17,21,29),fill=hair);d.rectangle((42,17,46,28),fill=hair);d.polygon([(20,18),(24,10),(39,10),(43,18),(37,17),(34,21),(30,16),(27,20)],fill=hair)
  d.line((24,8,36,7),fill='#f0d59a');d.line((20,21,20,27),fill='#f0d0a3')
  if expression=='laugh':
   for x in [25,38]:d.line((x-2,27,x,25,x+2,27),fill=INK,width=1)
   d.rectangle((28,33,36,37),fill='#6d4140');d.line((29,33,35,33),fill='#fff1ce')
  else:
   for x in [25,38]:d.rectangle((x,25,x+1,28),fill=INK);d.point((x,25),fill='#f9e7bb')
   if expression=='mad':d.line((23,22,27,24),fill=INK);d.line((37,24,41,22),fill=INK);d.line((30,35,35,34),fill='#76514a')
   elif expression in ['smile','love']:d.line((28,33,30,35,34,35,36,33),fill='#895648')
   else:d.line((30,34,35,34),fill='#895648')
  if expression=='love':d.rectangle((21,30,25,31),fill='#cc8a77');d.rectangle((39,30,43,31),fill='#cc8a77');d.polygon([(49,8),(51,5),(53,6),(55,5),(57,7),(53,12)],fill='#d3797d')
  if n==0:d.polygon([(16,11),(21,0),(43,0),(50,12)],fill='#d9b575');d.line((15,12,51,12),fill='#855e3c',width=2)
  if n==1:leaf(d,42,11,'#99b568');d.ellipse((43,7,47,11),fill='#dea482')
  if n==2:d.rectangle((26,51,38,63),fill='#665343');d.line((22,48,27,57,38,57,42,47),fill='#bda87e',width=2)
  if n==3:d.ellipse((15,21,49,38),outline='#cec4a4',width=1) if False else None;d.line((21,29,43,29),fill='#806478')
  if n==4:d.rectangle((20,8,45,13),fill='#e5c786');d.rectangle((22,5,44,9),fill='#617c58')
  save(im,'portrait-'+name+'-'+expression)
 # Distinct little world actors on the established 32x48 anatomy.
 for direction in ['down','up','left','right']:
  for f in range(4):
   im=Image.open(ART/f'chef-{direction}-{f}.png').convert('RGBA');pixels=im.load()
   replacements={'dfad84':skin,'584344':hair,'568f88':shirt}
   from PIL.ImageColor import getcolor
   colors={getcolor('#'+k,'RGBA'):getcolor(v,'RGBA') for k,v in replacements.items()}
   for y in range(im.height):
    for x in range(im.width):
     if pixels[x,y] in colors:pixels[x,y]=colors[pixels[x,y]]
   # Replace the chef hat with the resident's silhouette, preserving direction.
   d=ImageDraw.Draw(im);d.rectangle((5,0,27,13),fill=(0,0,0,0));d.ellipse((8,4,24,17),fill=hair);d.line((12,6,18,6),fill='#d8bb82')
   if direction!='up':d.rectangle((10,12,22,14),fill=skin)
   if n in [0,4]:d.rectangle((7,7,25,11),fill='#d9b575');d.rectangle((10,3,23,8),fill='#c7a565')
   save(im,f'resident-{name}-{direction}-{f}')

# Placeable food architecture (same world PPU), not full-screen backgrounds.
for kind,roof in [('seedshop','#588d74'),('teahouse','#957b9f'),('cottage','#ad7b55'),('smith','#b16d51')]:
 im=Image.new('RGBA',(160,144));d=ImageDraw.Draw(im);d.ellipse((7,129,153,142),fill=(42,57,40,65));d.rectangle((18,55,140,130),fill='#655540');d.rectangle((22,59,136,126),fill='#e3c695')
 for x in [23,133]:d.rectangle((x,61,x+4,128),fill='#86643f')
 for y in range(66,122,11):d.line((28,y,130,y),fill='#d3b780')
 d.polygon([(8,63),(35,17),(123,17),(151,63)],fill='#4d4d43');d.polygon([(15,59),(38,21),(120,21),(143,59)],fill=roof)
 for y in range(28,60,7):
  for x in range(28,133,16):d.line((x,y,x+11,y),fill='#ccb795');d.line((x+12,y,x+12,y+3),fill='#655640')
 d.rectangle((66,87,92,130),fill='#685343');d.rectangle((70,92,88,127),fill='#926f47');d.rectangle((73,95,85,105),fill='#edca81');d.point((86,115),fill='#ffdc8c')
 for x in [31,105]:d.rectangle((x,81,x+24,105),fill='#776345');d.rectangle((x+3,84,x+21,102),fill='#9ac2bb');d.line((x+12,84,x+12,102),fill='#e6cb95');d.rectangle((x-2,104,x+26,111),fill='#946642');leaf(d,x,106)
 d.rectangle((49,61,109,79),fill='#684d39');d.rectangle((52,63,106,77),fill='#e6bc75');text(d,(56,63),{'seedshop':'SEEDS','teahouse':'MOON TEA','smith':'SPICE','cottage':'HOME'}[kind],10,'#644b37')
 save(im,'building-'+kind)
for name in ['fence','flowerbed','bench','barrel']:
 im=Image.new('RGBA',(64,32));d=ImageDraw.Draw(im)
 if name=='fence':
  for x in [3,31,58]:d.rectangle((x,8,x+3,31),fill='#775938');d.line((x+1,10,x+1,28),fill='#d8b478')
  for y in [13,23]:d.rectangle((4,y,61,y+3),fill='#b78d55');d.line((4,y,61,y),fill='#e4be7b')
 elif name=='flowerbed':
  d.rectangle((3,21,61,29),fill='#b89663');d.line((3,21,61,21),fill='#f0c996')
  for x in range(9,60,9):leaf(d,x-4,23);d.rectangle((x,12,x+1,22),fill='#547c48');d.ellipse((x-4,8,x+5,16),fill='#dfa387');d.rectangle((x-1,10,x+2,13),fill='#ffe5a7')
 elif name=='bench':
  for y in [8,13,21]:d.rectangle((10,y,54,y+4),fill='#b38b58');d.line((10,y,54,y),fill='#e2be83')
  for x in [13,48]:d.rectangle((x,24,x+3,31),fill='#765738')
 else:
  d.ellipse((19,3,45,31),fill='#74583e');d.rectangle((21,7,43,26),fill='#b48d56')
  for y in [8,23]:d.rectangle((20,y,44,y+2),fill='#6b7365')
  for x in [26,32,38]:d.line((x,7,x,26),fill='#d2af76')
 save(im,'village-'+name)
print('Daytime menu, 25 expression portraits, five directional residents, compact food icons and village props authored on native pixel grids.')

jar=Image.open(ART/'held-brothback.png').convert('RGBA')
if jar.size!=(24,24):
 box=jar.getbbox();jar=jar.crop(box);jar.thumbnail((22,22),Image.Resampling.NEAREST)
 held=Image.new('RGBA',(24,24));held.alpha_composite(jar,((24-jar.width)//2,(24-jar.height)//2));save(held,'held-brothback')
