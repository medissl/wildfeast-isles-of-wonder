"""Native rooted foliage, clustered leaf shading and quiet one-pixel canopy motion."""
from pathlib import Path
import json,random
from PIL import Image,ImageDraw
ROOT=Path(__file__).resolve().parents[1];ART=ROOT/'Game/Assets/Wildfeast/Resources/Art'
isles=json.loads((ART.parent/'Archipelago.json').read_text())['islands']
palettes=[['#284f36','#367a3d','#559744','#80b650','#b0cf67'],['#2f5841','#478c50','#6faf60','#96cc7b','#c3de93'],['#3c5031','#587435','#7c953d','#a9b451','#d0ce72'],['#3b426b','#665684','#92789d','#b5a2bb','#d5c5ca'],['#265854','#38887a','#5db296','#8ed0ac','#bde7c6']]
# Canonical bases are authored assets. Derive motion without redesigning them.
for n,isle in enumerate(isles):
 pal=palettes[n%5]
 base=Image.open(ART/f'tree-{isle["tree"]}.png').convert('RGBA')
 split=base.height*2//3
 for f,shift in enumerate([0,1,0,-1]):
  im=Image.new('RGBA',base.size)
  im.paste(base.crop((0,split,base.width,base.height)),(0,split))
  im.paste(base.crop((0,0,base.width,split)),(shift,0))
  im.save(ART/f'tree-{isle["tree"]}-{f}.png')
 for f in range(3):
  im=Image.new('RGBA',(28,16));d=ImageDraw.Draw(im);rng=random.Random(n+11)
  for x in [3,7,12,17,22,25]:
   y=rng.randint(11,14);s=[0,1,0][f]
   d.line((x,y,x-2+s,y-5),fill=pal[1],width=2);d.line((x,y,x+2+s,y-4),fill=pal[2]);d.point((x-1+s,y-5),fill=pal[3]);d.line((x-2,y,x+2,y),fill=pal[1])
  im.save(ART/f'grass-{isle["key"]}-{f}.png')
  if f==0:im.save(ART/f'grass-{isle["key"]}.png')
print('Canonical tree silhouettes preserved; rooted canopy and native grass frames updated.')
