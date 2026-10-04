"""Preview the project's native cast without regenerating or editing any game assets."""
from pathlib import Path
from PIL import Image,ImageDraw,ImageFont
import sys
repo=Path(__file__).resolve().parents[1];art=repo/'Game/Assets/Wildfeast/Resources/Art'
folder=Path(sys.argv[1]) if len(sys.argv)>1 else repo/'artifacts/wonder-preview';folder.mkdir(parents=True,exist_ok=True)
font=ImageFont.truetype(str(repo/'Game/Assets/Wildfeast/Fonts/PixelifySans.ttf'),22)
sheet=Image.new('RGB',(1060,600),'#243740');d=ImageDraw.Draw(sheet)
for n,(name,label) in enumerate([('brothback','Stock-pot boar'),('custardram','Custard sheep'),('spicecrab','Spicepangolin'),('mochimoth','Dumpling moth'),('kelpsnail','Kelp spiral snail')]):
 im=Image.open(art/(name+'.png'));im=im.resize((im.width*2,im.height*2),Image.Resampling.NEAREST);x=20+n*210;sheet.paste(im,(x,50),im);d.text((x,200),label,font=font,fill='#edcf95')
for n,name in enumerate(['chef-down-0','chef-up-0','chef-left-0','chef-right-0']+[f'guest-{i}' for i in range(5)]):
 im=Image.open(art/(name+'.png')).resize((64,96),Image.Resampling.NEAREST);sheet.paste(im,(20+n*115,270),im)
for n,name in enumerate(['restaurant','sign-open','sign-closed']):
 im=Image.open(art/(name+'.png'));crop=im.crop((50,85,140,109)) if name=='restaurant' else im;crop=crop.resize((crop.width*3,crop.height*3),Image.Resampling.NEAREST);sheet.paste(crop,(20+n*325,390),crop)
d.text((20,560),'Original native pixel cast and measured world signs',font=font,fill='#edcf95');sheet.save(folder/'wildfeast-wonder-cast.png')
print(folder/'wildfeast-wonder-cast.png')
