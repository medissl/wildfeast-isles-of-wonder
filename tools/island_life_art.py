"""Original pixel assets and three synthesized, seamless musical compositions.
Run after make_art, polish_art, creature_art. No reference pixels or music samples.
"""
from pathlib import Path
from PIL import Image,ImageDraw
import random, math, wave
import numpy as np
ROOT=Path(__file__).resolve().parents[1];ART=ROOT/'Game/Assets/Wildfeast/Resources/Art';AUDIO=ART.parent/'Audio'
r=random.Random(812);ink='#40372e';gold='#e6bd73';silver='#c2d3c5'
def save(name,size,fn):
 im=Image.new('RGBA',size);fn(ImageDraw.Draw(im));im.save(ART/(name+'.png'))
def tool(d,kind):
 d.line([(7,35),(24,6)],fill=ink,width=5);d.line([(8,34),(24,6)],fill='#ae7952',width=3);d.line([(10,31),(23,9)],fill=gold)
 if kind=='shovel':d.polygon([(14,7),(23,3),(30,12),(25,20),(20,20)],fill=ink);d.polygon([(17,8),(23,5),(28,12),(24,18),(21,17)],fill=silver);d.line([(21,8),(25,14)],fill='#829c98',width=2)
 if kind=='axe':d.polygon([(14,5),(24,2),(30,8),(29,17),(22,18),(22,10)],fill=ink);d.polygon([(18,6),(25,4),(28,8),(27,15),(24,16),(24,9)],fill=silver)
 if kind=='scythe':d.arc((2,1,30,29),180,340,fill=ink,width=6);d.arc((2,1,30,29),185,335,fill=silver,width=3)
 if kind=='pickaxe':d.arc((7,0,31,24),195,342,fill=ink,width=6);d.arc((7,0,31,24),195,342,fill=silver,width=3)
for k in ['shovel','axe','scythe','pickaxe']:save('tool-'+k,(32,40),lambda d,k=k:tool(d,k))
save('wood',(32,32),lambda d:[d.rounded_rectangle((2,8,29,25),radius=5,fill=ink),d.rectangle((7,10,26,23),fill='#a7764e'),d.ellipse((2,10,13,23),fill='#d6ad71'),d.ellipse((5,13,10,19),outline='#8b5b40'),d.line([(14,13),(26,13)],fill='#c99862'),d.line([(12,20),(25,20)],fill='#694c38')])
save('stone',(32,32),lambda d:[d.polygon([(3,23),(5,10),(14,4),(26,9),(29,23),(20,29)],fill=ink),d.polygon([(5,21),(7,11),(14,6),(25,11),(26,23),(19,26)],fill='#a4c9c3'),d.polygon([(8,11),(14,7),(24,12),(16,16)],fill='#e6e6c7'),d.line([(16,16),(19,25)],fill='#689f9c')])
save('fiber',(32,32),lambda d:[d.line([(8,26),(6,9),(16,5),(25,12),(18,25)],fill=ink,width=8),d.line([(8,26),(6,9),(16,5),(25,12),(18,25)],fill='#c0d68c',width=4),d.rectangle((7,16,21,20),fill='#bc8355')])
def grass(d):
 for x in range(4,32,5):d.line([(x,29),(x-2,14),(x+4,7)],fill=ink,width=3);d.line([(x,28),(x-1,14),(x+4,8)],fill='#83ac69',width=2);d.ellipse((x+1,6,x+5,10),fill='#e3ca85')
save('noodlegrass',(40,32),grass)
def rock(d):
 d.ellipse((2,24,61,42),fill='#233f3f77');d.polygon([(6,31),(11,12),(29,3),(49,11),(59,30),(44,39),(21,37)],fill=ink);d.polygon([(9,29),(14,14),(30,6),(47,13),(54,29),(42,35),(23,33)],fill='#77a9a4');d.polygon([(15,14),(30,7),(45,14),(32,23)],fill='#c5d5b7');d.line([(32,23),(41,34)],fill='#477976',width=2)
 for x,y in [(13,19),(43,24),(30,28)]:d.rectangle((x,y,x+4,y+4),fill='#f3e2ba')
save('saltstone',(64,48),rock)
save('stump',(48,32),lambda d:[d.ellipse((3,12,45,30),fill='#35423788'),d.rectangle((9,9,38,24),fill=ink),d.rectangle((11,10,36,24),fill='#976b47'),d.ellipse((8,4,39,19),fill='#deb581'),d.ellipse((14,7,34,16),outline='#976b47',width=2),d.ellipse((20,10,27,14),outline='#b18152')])
for wet in [False,True]:
 def soil(d):
  d.rounded_rectangle((0,1,31,30),radius=5,fill='#8c6846' if not wet else '#624b3b')
  for y in [7,15,23]:d.line([(3,y),(28,y)],fill='#5c4032' if not wet else '#3d3531',width=2)
  for n in range(15):x=r.randrange(3,28);y=r.randrange(4,28);d.point((x,y),fill='#b99262' if not wet else '#7e7960')
 save('soil-wet' if wet else 'soil',(32,32),soil)
save('dirt-puff',(24,24),lambda d:[d.ellipse((2,10,15,21),fill='#a88053aa'),d.ellipse((8,3,20,17),fill='#d7b78399')])
save('map-player',(16,16),lambda d:[d.polygon([(8,0),(15,8),(8,15),(0,8)],fill=ink),d.polygon([(8,2),(13,8),(8,13),(2,8)],fill=gold),d.rectangle((7,6,9,10),fill='#fff4d4')])
save('map-landmark',(12,12),lambda d:[d.rectangle((1,1,10,10),fill=ink),d.rectangle((3,3,8,8),fill='#dd7968')])
def dining_table(d):
 d.ellipse((3,43,77,63),fill='#31403b88');d.rectangle((13,34,21,58),fill=ink);d.rectangle((59,34,67,58),fill=ink);d.rectangle((15,34,19,56),fill='#936544');d.rectangle((61,34,65,56),fill='#936544')
 d.ellipse((1,6,79,48),fill=ink);d.ellipse((3,6,77,43),fill='#8e6348');d.ellipse((5,7,75,39),fill='#c3935e')
 for y in [14,22,30]:d.line((11,y,69,y),fill='#ac794e');d.line((14,y+1,64,y+1),fill='#dbad70')
 for x in [22,55]:
  d.ellipse((x-10,17,x+10,30),fill=ink);d.ellipse((x-9,17,x+9,28),fill='#eee2b8');d.ellipse((x-6,19,x+6,26),fill='#8faa75');d.line((x-13,16,x-13,29),fill='#d6dcc9',width=2)
 d.rectangle((36,18,43,30),fill='#89604b');d.ellipse((33,11,46,23),fill='#629563');d.ellipse((35,9,43,15),fill='#cf86a2');d.rectangle((36,29,44,32),fill=gold)
save('table',(80,64),dining_table)
def frame(d):
 d.rectangle((0,0,23,23),fill='#4d2928');d.rectangle((1,1,22,22),fill='#cc9a58');d.rectangle((3,3,20,20),fill='#823b34');d.rectangle((5,5,18,18),fill='#ecd4a6')
 for x,y in [(0,0),(20,0),(0,20),(20,20)]:d.rectangle((x,y,x+3,y+3),fill='#f0cf83')
save('ui-frame',(24,24),frame)
# A different coastline and trail network make Mistwake a distinct authored island.
MIST=[(140,530),(100,420),(160,250),(260,190),(300,85),(510,100),(590,150),(760,115),(940,150),(1090,270),(1150,450),(1070,550),(1000,635),(830,685),(690,660),(560,725),(380,725),(240,645)]
W,H=1280,832
def ocean():
 im=Image.new('RGBA',(W,H),'#245d83');d=ImageDraw.Draw(im)
 for n in range(4500):x=r.randrange(W);y=r.randrange(H);d.line((x,y,x+r.randrange(3,12),y),fill=r.choice(['#2d7092','#357b9d','#28668d']))
 return im
im=ocean();im.save(ART/'ocean.png');d=ImageDraw.Draw(im)
for div,col in [(100,'#74b7b7'),(35,'#94cbc1'),(12,'#d4c6a0'),(5,'#5e8470')]:points=[(x+(640-x)//div,y+(420-y)//div) for x,y in MIST];d.polygon(points,fill=col)
mask=Image.new('1',(W,H));ImageDraw.Draw(mask).polygon(points,fill=1)
for n in range(13000):
 x=r.randrange(W);y=r.randrange(H)
 if mask.getpixel((x,y)):d.rectangle((x,y,x+2,y+1),fill=r.choice(['#698b73','#73997e','#4f776b','#8da78b']))
# Terraced orchard beds with sweet crystal stalks and winding orchard trails.
routes=[[(290,650),(365,560),(550,550),(700,450),(730,345)],[(550,550),(510,365),(400,280)],[(700,450),(855,415),(950,520)],[(510,365),(585,235),(780,250)]]
for pts in routes:d.line(pts,fill='#6b7569',width=38);d.line(pts,fill='#b8baa1',width=28)
for cx,cy in [(510,220),(760,240),(840,350),(385,360)]:
 d.ellipse((cx-65,cy-33,cx+65,cy+33),fill='#577567');d.ellipse((cx-62,cy-30,cx+62,cy+26),outline='#a5b598',width=3)
 for x in range(cx-48,cx+49,16):
  for y in range(cy-16,cy+20,14):d.line((x,y,x,y-8),fill='#bca7bf',width=2);d.ellipse((x-3,y-13,x+5,y-5),fill='#dde0ba')
# The hot spring is fed by a pale mineral shelf, not the home island's sand pond.
d.ellipse((850,155,1090,300),fill='#acbcb0');d.ellipse((862,165,1078,288),fill='#557c98')
for n in range(170):x=r.randrange(885,1060);y=r.randrange(182,270);d.line((x,y,x+7,y),fill='#91c9d0')
d.rectangle((249,595,387,666),fill=ink);d.rectangle((252,597,384,661),fill='#8d8e75')
for y in range(599,661,7):d.line((254,y,383,y),fill='#c0bd92')
im.save(ART/'mistwake.png')
# Original 32-bar pentatonic pieces; additive instruments synthesized from oscillators.
AUDIO.mkdir(parents=True,exist_ok=True)
RATE=22050;BPM=96;beat=60/BPM;length=64*beat;N=int(RATE*length)
def track(name,root,melody):
 stereo=np.zeros((N,2),dtype=np.float64)
 def note(midi,start,duration,amp,kind,pan):
  count=int(duration*RATE);t=np.arange(count)/RATE;f=440*2**((midi-69)/12)
  attack=np.minimum(1,t/.02);release=np.minimum(1,(duration-t)/.1);env=attack*release
  if kind=='bell':v=(np.sin(2*np.pi*f*t)+.25*np.sin(2*np.pi*f*2*t)+.08*np.sin(2*np.pi*f*3*t))*np.exp(-t*3)
  elif kind=='plucked':v=(np.sin(2*np.pi*f*t)+.22*np.sin(2*np.pi*f*2*t))*np.exp(-t*2)
  else:v=np.sin(2*np.pi*f*t)+.1*np.sin(2*np.pi*f*2*t)
  v*=env*amp;at=int(start*RATE);idx=(at+np.arange(count))%N;stereo[idx,0]+=v*(1-pan);stereo[idx,1]+=v*pan
 for bar in range(16):
  chord=[0,5,7,0,9,5,7,0][bar%8];start=bar*4*beat
  for degree in [0,4,7]:note(root+chord+degree,start,3.9*beat,.035,'pad',.5)
  for b in range(4):note(root-12+chord,start+b*beat,.9*beat,.05,'plucked',.5)
  for b in range(4):
   degree=melody[(bar*4+b)%len(melody)];note(root+12+degree,start+b*beat,.95*beat,.07,'bell',.35 if b%2 else .65)
  for b in range(8):note(root+chord+[0,7,12,7][b%4],start+b*beat/2,.47*beat,.025,'plucked',.25 if b%2 else .75)
 # Soft circular echo, preserving the loop boundary.
 stereo+=np.roll(stereo,int(.3*RATE),axis=0)*.15
 pcm=np.clip(stereo,-.8,.8);pcm=(pcm*32767).astype('<i2')
 with wave.open(str(AUDIO/('music-'+name+'.wav')),'wb') as out:out.setnchannels(2);out.setsampwidth(2);out.setframerate(RATE);out.writeframes(pcm.tobytes())
track('saltleaf',60,[0,4,7,9,7,4,2,4,0,2,4,7,12,9,7,4])
track('mistwake',62,[12,9,7,4,7,9,14,12,9,7,4,2,4,7,9,7])
track('restaurant',65,[0,7,4,9,7,12,9,7,4,7,2,4,0,4,7,9])
print('Island-life art and three original 40-second music loops generated')
