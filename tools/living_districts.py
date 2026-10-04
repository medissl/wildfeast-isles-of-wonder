"""Refresh native district terrain materials from the canonical authored catalog.
Layouts live in Archipelago.json; this tool never reconstructs them.
"""
from pathlib import Path
import json,shutil
from PIL import Image,ImageDraw,ImageEnhance
ROOT=Path(__file__).resolve().parents[1];CAT=ROOT/'Game/Assets/Wildfeast/Resources/Archipelago.json';ART=CAT.parent/'Art'
data=json.loads(CAT.read_text(encoding='utf-8'))
assert len(data['islands'])==6 and any(i['id']==7 for i in data['islands']), 'Use the authored Living Districts catalog.'
# Town tiles share Saltleaf materials exactly, rather than inventing a disconnected palette.
for src in ART.glob('tile-saltleaf-*.png'):shutil.copyfile(src,ART/src.name.replace('saltleaf','bramblewick'))
for f in range(3):shutil.copyfile(ART/f'grass-saltleaf-{f}.png',ART/f'grass-bramblewick-{f}.png')
shutil.copyfile(ART/'grass-saltleaf.png',ART/'grass-bramblewick.png')
# Soil borrows the exact existing earth color and clustered texture; ragged turf edges blend contiguous plots.
for isle in data['islands']:
 src=Image.open(ART/f'tile-{isle["key"]}-path-15-0.png').convert('RGBA')
 grass=Image.open(ART/f'tile-{isle["key"]}-grass-0.png').convert('RGBA')
 for mask in range(16):
  im=src.copy();d=ImageDraw.Draw(im)
  for edge,bit in enumerate([1,2,4,8]):
   if mask&bit:continue
   for q in range(32):
    for z in range(1+(q*7%3)):
     x,y=[(q,z),(31-z,q),(q,31-z),(z,q)][edge];im.putpixel((x,y),grass.getpixel((x,y)))
  # A handful of short marks, not contrasting raised brown boxes.
  for x,y in [(6,10),(17,17),(24,25)]:d.line((x,y,x+3,y),fill=tuple(max(0,c-12) for c in src.getpixel((x,y))[:3])+(255,))
  savepath=ART/f'tile-{isle["key"]}-soil-{mask}.png';im.save(savepath);ImageEnhance.Brightness(im).enhance(.86).save(ART/f'tile-{isle["key"]}-soil-{mask}-wet.png')
print('Six connected outdoor districts across three islands; town materials and contiguous biome soil authored.')
