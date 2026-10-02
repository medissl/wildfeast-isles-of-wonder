"""Original deterministic pixel artwork and audio for Wildfeast. No external assets."""
from pathlib import Path
import random, math, wave, struct, argparse
from PIL import Image, ImageDraw

ROOT = Path(__file__).resolve().parents[1]
ART = ROOT / 'Game/Assets/Wildfeast/Resources/Art'
ART.mkdir(parents=True, exist_ok=True)
rng = random.Random(417)
PAL = dict(ink='#17383d', water='#24566b', wave='#36788a', foam='#b3d7bf', grass='#6d995d', light='#95b768', dark='#477b55', sand='#ddc28c', sanddark='#b79868', leaf='#347050', mint='#b4d884', wood='#8a5741', woodlight='#b98055', cream='#f7e8b3', orange='#e8a15b', pink='#d78485', stone='#72817d')

def sprite(name, size, draw):
    im=Image.new('RGBA',size); d=ImageDraw.Draw(im); draw(d)
    im.save(ART/f'{name}.png'); return im

def rect(d,box,c): d.rectangle(box,fill=PAL.get(c,c))
def ellipse(d,box,c): d.ellipse(box,fill=PAL.get(c,c))
def line(d,pts,c,w=1): d.line(pts,fill=PAL.get(c,c),width=w)

def tree(d):
    rect(d,(27,50,37,89),'ink'); rect(d,(29,51,34,88),'wood'); rect(d,(31,58,33,81),'woodlight')
    for box in [(9,33,54,65),(1,19,40,48),(24,12,61,49),(12,1,49,35)]: ellipse(d,box,'ink')
    for box in [(10,30,52,60),(3,18,39,43),(25,11,59,43),(14,2,47,32)]: ellipse(d,box,'leaf')
    for box in [(10,29,28,40),(9,19,27,31),(27,13,48,28),(22,5,39,18)]: ellipse(d,box,'dark')
    for x,y in [(20,8),(32,15),(11,25),(26,37),(40,29)]: rect(d,(x,y,x+6,y+2),'grass')
    line(d,[(16,47),(25,49),(30,46)],'#285744',2)
sprite('tree',(64,96),tree)
def pine(d):
    rect(d,(29,47,34,92),'wood')
    for y,w in [(37,28),(22,22),(7,16)]:
        d.polygon([(32,y),(32-w,y+37),(32+w,y+37)],fill=PAL['ink'])
        d.polygon([(32,y+2),(34-w,y+32),(30+w,y+32)],fill='#386d66')
        line(d,[(32,y+5),(25-w//2,y+25),(32,y+21)],'#57897a',3)
sprite('pine',(64,96),pine)
def player(d,frame=0,customer=False):
    bob=frame%2; shift=(-1 if frame==1 else 1 if frame==3 else 0)
    ellipse(d,(6,38,26,44),'#17383d66')
    rect(d,(9,30+bob,14,39+shift),'ink');rect(d,(19,30+bob,24,39-shift),'ink')
    rect(d,(8,21+bob,24,32+bob),'ink');rect(d,(9,22+bob,23,31+bob),'#718f80' if customer else '#d9c99a')
    rect(d,(12,24+bob,22,31+bob),'#f1e3b4');rect(d,(14,23+bob,20,29+bob),'#b98360')
    rect(d,(5,23+bob,8,31+bob),'ink');rect(d,(6,24+bob,8,29+bob),'orange')
    rect(d,(24,23+bob,27,31+bob),'ink');rect(d,(24,24+bob,26,29+bob),'orange')
    rect(d,(8,9+bob,24,22+bob),'ink');rect(d,(10,11+bob,23,21+bob),'#e2ad7b')
    rect(d,(10,9+bob,22,14+bob),'#6b453b');rect(d,(11,14+bob,13,15+bob),'ink');rect(d,(20,14+bob,22,15+bob),'ink')
    rect(d,(15,18+bob,18,18+bob),'#a3614f')
    rect(d,(7,6+bob,25,10+bob),'ink');rect(d,(8,6+bob,24,9+bob),'cream');rect(d,(11,2+bob,22,7+bob),'cream')
    rect(d,(12,3+bob,20,4+bob),'#fff5d5')
for i in range(4): sprite(f'player-{i}',(32,48),lambda d,i=i:player(d,i))
sprite('guest',(32,48),lambda d:player(d,0,True))
sprite('nori',(32,48),lambda d:player(d,2,True))
def guest(d,index):
    player(d,0,True)
    rect(d,(5,0,26,10),(0,0,0,0))
    colors=['#477f8b','#a26a4a','#836490','#ad665d','#688a5b']
    rect(d,(9,22,23,31),colors[index]);rect(d,(14,22,17,30),'cream')
    rect(d,(9,7,24,12),'#473d38' if index%2 else '#805a40')
    rect(d,(8,11,10,18),'#473d38' if index%2 else '#805a40')
    if index==1:rect(d,(7,6,26,10),'sand');rect(d,(11,2,23,7),'sanddark')
    if index==2:rect(d,(10,7,23,10),'pink')
    if index==4:rect(d,(10,14,15,17),'ink');rect(d,(18,14,23,17),'ink');rect(d,(11,15,14,16),'wave');rect(d,(19,15,22,16),'wave')
for i in range(5):sprite('guest-'+str(i),(32,48),lambda d,i=i:guest(d,i))
def fish(d):
    d.polygon([(7,16),(0,8),(1,23),(9,21)], fill=PAL['ink'])
    ellipse(d,(6,4,29,27),'ink');ellipse(d,(7,5,28,25),'mint')
    for x in (10,15,20): d.arc((x,6,x+10,24),90,270,fill=PAL['leaf'],width=2)
    ellipse(d,(24,11,27,14),'ink');rect(d,(25,11,25,11),'cream')
    line(d,[(10,9),(15,7),(21,8)],'cream')
sprite('leafgill',(32,32),fish)
def pepper(d):
    line(d,[(16,27),(16,6)],'ink',3);line(d,[(16,18),(7,10)],'leaf',2);line(d,[(16,12),(26,6)],'leaf',2)
    for x,y in [(4,8),(12,13),(22,5)]:
        ellipse(d,(x,y,x+8,y+12),'ink');ellipse(d,(x+1,y+1,x+7,y+10),'orange');rect(d,(x+2,y+2,x+3,y+5),'cream')
    line(d,[(14,8),(8,3),(4,4)],'mint',2)
sprite('pepperbell',(32,32),pepper)
def crab(d):
    for x in (8,42):
        line(d,[(x,28),(x-5,37),(x+5,40)],'ink',4);line(d,[(x,26),(x-4,32)],'orange',2)
    ellipse(d,(7,9,45,35),'ink');ellipse(d,(9,10,43,32),'stone');ellipse(d,(13,8,39,27),'woodlight')
    ellipse(d,(17,10,35,24),'orange');rect(d,(23,6,29,10),'ink');rect(d,(24,4,28,7),'stone')
    rect(d,(14,29,18,32),'cream');rect(d,(16,29,17,31),'ink');rect(d,(34,29,38,32),'cream');rect(d,(35,29,36,31),'ink')
    line(d,[(15,20),(22,23),(32,22)],'cream',2)
sprite('brothback',(56,48),crab)
def root(d):
    ellipse(d,(8,12,26,29),'ink');ellipse(d,(9,13,24,27),'#e1c78d');line(d,[(18,27),(16,31)],'sanddark',2)
    for end in [(6,3),(16,0),(29,5)]: line(d,[(17,16),end],'leaf',3)
    for box in [(3,3,13,9),(12,0,20,8),(21,3,30,11)]: ellipse(d,box,'#72b8a0')
    rect(d,(13,16,17,18),'cream')
sprite('lanternroot',(32,32),root)
def cloud(d):
    line(d,[(17,26),(17,30)],'leaf',2)
    for box in [(2,11,18,26),(10,4,28,26),(19,12,31,25)]:ellipse(d,box,'ink')
    for box in [(3,11,18,24),(11,5,27,24),(20,12,30,23)]:ellipse(d,box,'#dfdcb2')
    rect(d,(13,8,18,10),'#fff3d1');line(d,[(14,24),(22,24)],'#a4b49d',2)
sprite('cloudfruit',(32,32),cloud)
sprite('grain',(32,32),lambda d:[line(d,[(x,29),(x,5)],'orange',2) or [line(d,[(x,y),(x+s*5,y-4)],'cream',2) for y in (9,15,21) for s in (-1,1)] for x in (10,17,24)])
def dish(d,kind):
    ellipse(d,(1,10,31,29),'ink');ellipse(d,(2,10,30,26),'cream');ellipse(d,(5,12,27,24),'sanddark')
    if kind=='fish': fish(d)
    elif kind=='wrap':
        ellipse(d,(8,8,25,21),'leaf');line(d,[(9,18),(23,10)],'mint',3);rect(d,(14,11,17,14),'orange')
    elif kind=='cloud':
        rect(d,(7,11,25,25),'wood');rect(d,(9,12,23,23),'sand');ellipse(d,(7,4,25,17),'cream');rect(d,(12,7,16,9),'pink')
    else:
        ellipse(d,(4,8,28,25),'wood');ellipse(d,(5,7,27,19),'cream');ellipse(d,(8,9,24,16),'#86b99b' if kind=='lantern' else '#d8a367')
        for x in (11,17,21):rect(d,(x,10,x+2,12),'mint')
    line(d,[(10,6),(9,2)],'#e9efd688');line(d,[(22,4),(23,0)],'#e9efd688')
for k in ['fish','wrap','porridge','broth','lantern','cloud']:sprite('dish-'+k,(32,32),lambda d,k=k:dish(d,k))
def rock(d):
    d.polygon([(4,28),(0,21),(5,8),(16,3),(29,9),(33,23),(25,30)],fill=PAL['ink'])
    d.polygon([(3,22),(6,9),(16,5),(27,10),(30,22),(23,27)],fill=PAL['stone'])
    line(d,[(6,10),(16,7),(25,11)],'#a2a99a',2);line(d,[(9,20),(19,18),(24,24)],'#5c716b',2)
sprite('rock',(36,32),rock)
def bush(d):
    for box in [(0,13,20,31),(11,5,29,30),(20,13,39,32)]:ellipse(d,box,'ink')
    for box in [(1,12,19,27),(12,5,28,27),(21,13,38,28)]:ellipse(d,box,'leaf')
    for x,y in [(5,15),(15,9),(26,17)]:rect(d,(x,y,x+6,y+2),'grass')
sprite('bush',(40,36),bush)
def building(d):
    rect(d,(10,80,148,141),'ink');rect(d,(12,81,146,132),'sand');rect(d,(15,85,143,130),'cream')
    for x in (16,48,80,112,143):rect(d,(x,84,x+3,130),'wood')
    rect(d,(66,98,94,140),'wood');rect(d,(69,101,90,137),'ink');rect(d,(74,104,90,137),'woodlight')
    for x in (22,105):
        rect(d,(x,99,x+28,119),'ink');rect(d,(x+2,101,x+26,117),'water');rect(d,(x+12,101,x+14,117),'cream');line(d,[(x+3,113),(x+10,106)],'wave',2)
    d.polygon([(0,81),(25,24),(132,24),(159,81)],fill=PAL['ink'])
    d.polygon([(3,77),(27,27),(130,27),(155,77)],fill='#a65e48')
    for y in range(32,76,8):line(d,[(26-(y-27)//3,y),(131+(y-27)//3,y)],'#cf8660',2)
    for x in range(32,132,16):line(d,[(x,29),(x-10,75)],'#75483d')
    rect(d,(112,12,128,40),'ink');rect(d,(115,14,126,38),'stone')
    rect(d,(55,65,105,87),'ink');rect(d,(57,67,103,84),'woodlight')
    # A gold fish emblem on the restaurant sign.
    ellipse(d,(72,71,87,80),'cream');d.polygon([(88,75),(95,70),(95,81)],fill=PAL['cream'])
    rect(d,(5,132,154,140),'wood');line(d,[(8,133),(151,133)],'woodlight',2)
sprite('restaurant',(160,144),building)
def table(d):
    rect(d,(7,19,12,38),'ink');rect(d,(37,19,42,38),'ink');ellipse(d,(2,2,47,30),'ink');ellipse(d,(3,2,46,25),'woodlight')
    line(d,[(5,13),(44,13)],'wood');line(d,[(8,6),(40,6)],'sanddark');ellipse(d,(18,8,31,19),'cream')
sprite('table',(48,40),table)
def stove(d):
    rect(d,(2,9,61,43),'ink');rect(d,(4,12,59,39),'stone');rect(d,(5,32,58,40),'wood');rect(d,(10,33,23,38),'orange')
    for x in (12,40):ellipse(d,(x,3,x+15,18),'ink');ellipse(d,(x+2,5,x+13,15),'wood');line(d,[(x+5,2),(x+4,0)],'cream')
sprite('stove',(64,48),stove)
sprite('crate',(32,32),lambda d:[rect(d,(2,4,29,29),'ink'),rect(d,(3,5,28,27),'wood'),rect(d,(5,7,26,9),'woodlight'),rect(d,(5,21,26,24),'woodlight'),line(d,[(6,10),(25,21)],'woodlight',3)])
def boat(d):
    d.polygon([(3,23),(14,6),(48,6),(61,23),(49,45),(15,45)],fill=PAL['ink']);d.polygon([(6,23),(16,9),(46,9),(58,23),(47,41),(17,41)],fill=PAL['woodlight'])
    d.polygon([(13,23),(20,13),(43,13),(50,23),(42,35),(21,35)],fill=PAL['wood'])
    rect(d,(18,21,46,25),'sand');line(d,[(7,4),(51,42)],'cream',3)
sprite('boat',(64,48),boat)
sprite('plot',(64,32),lambda d:[ellipse(d,(0,1,63,30),'ink'),ellipse(d,(2,2,61,27),'wood'),*[line(d,[(7,y),(55,y)],'woodlight',2) for y in (8,16,23)]])
sprite('flower',(16,16),lambda d:[line(d,[(8,8),(8,15)],'leaf',2),ellipse(d,(3,1,12,10),'pink'),rect(d,(6,4,9,7),'cream')])
sprite('reed',(24,32),lambda d:[line(d,[(12,31),(x,y)],'dark',2) for x,y in [(3,11),(8,0),(17,5),(22,13)]])
sprite('spark',(8,8),lambda d:[rect(d,(3,0,4,7),'cream'),rect(d,(0,3,7,4),'cream')])

def world(name,second=False):
    im=Image.new('RGBA',(1280,832),PAL['water']);d=ImageDraw.Draw(im)
    for _ in range(2500):
        x=rng.randrange(1280);y=rng.randrange(832);line(d,[(x,y),(x+rng.randrange(3,15),y)],rng.choice(['wave','#285f73','#2b687c']))
    poly=[(90,500),(80,300),(140,185),(330,130),(470,72),(650,78),(780,110),(990,100),(1140,230),(1180,370),(1140,590),(1050,655),(850,706),(590,735),(410,735),(280,690),(175,630)]
    d.polygon(poly,fill=PAL['foam'])
    poly2=[(x+(640-x)//35,y+(420-y)//35) for x,y in poly];d.polygon(poly2,fill=PAL['sanddark'])
    poly3=[(x+(640-x)//12,y+(420-y)//12) for x,y in poly];d.polygon(poly3,fill=PAL['sand'])
    land=[(x+(640-x)//5,y+(420-y)//5) for x,y in poly];d.polygon(land,fill='#638c76' if second else PAL['grass'])
    # Grain and tiny clustered grass mark the land, while the broad path stays readable.
    mask=Image.new('1',im.size);md=ImageDraw.Draw(mask);md.polygon(land,fill=1)
    for _ in range(5500):
        x=rng.randrange(1280);y=rng.randrange(832)
        if mask.getpixel((x,y)):rect(d,(x,y,x+2,y),rng.choice(['#6e967e','#729c80'] if second else ['light','dark','grass']))
    paths=[[(440,555),(450,440),(670,410),(850,350),(950,250)],[(450,440),(340,330),(350,220)],[(670,410),(790,540),(945,590)]]
    for pts in paths:line(d,pts,'sanddark',28);line(d,pts,'sand',22)
    # Spring / freshwater pond, framed by a double shoreline.
    ellipse(d,(841,146,1100,310),'sanddark');ellipse(d,(850,155,1090,300),'sand');ellipse(d,(862,165,1078,288),'#3b7378' if second else 'wave')
    for _ in range(100):
        x=rng.randrange(874,1060);y=rng.randrange(179,274)
        if ((x-970)/95)**2+((y-226)/49)**2<1:line(d,[(x,y),(x+8,y)],'foam')
    # Harbor boardwalk.
    rect(d,(224,620,430,654),'ink');rect(d,(227,621,427,650),'woodlight')
    for x in range(229,427,10):line(d,[(x,622),(x,649)],'wood')
    for x in (231,413):rect(d,(x,613,x+5,655),'wood');rect(d,(x,613,x+5,616),'sand')
    # Garden fencing and orderly floor under the home area.
    if not second:
        rect(d,(338,408,467,438),'sanddark')
        for x in range(327,488,20):rect(d,(x,364,x+3,387),'wood');rect(d,(x,367,x+20,370),'woodlight')
    im.save(ART/f'{name}.png')
world('saltleaf');world('mistwake',True)

def interior():
    im=Image.new('RGBA',(640,360),'#182f34');d=ImageDraw.Draw(im)
    rect(d,(3,48,636,349),'wood')
    for y in range(64,345,16):
        rect(d,(8,y,631,y+14),'#aa7952' if y%32 else '#b28358')
        for x in range(8+(16 if y%32 else 0),631,64):line(d,[(x,y),(x,y+13)],'wood')
        line(d,[(9,y+14),(630,y+14)],'#78513c')
    rect(d,(3,3,636,64),'sand');rect(d,(10,10,630,53),'cream');rect(d,(3,55,636,64),'wood')
    for x in (14,195,410,620):rect(d,(x,5,x+7,58),'wood')
    for x in (56,472):
        rect(d,(x,14,x+82,47),'ink');rect(d,(x+3,17,x+79,44),'water');line(d,[(x+4,35),(x+77,35)],'wave',2);rect(d,(x+39,17,x+42,44),'cream')
    rect(d,(236,22,404,48),'wood');rect(d,(239,24,401,44),'woodlight')
    for x in range(249,391,24):ellipse(d,(x,27,x+13,39),'cream')
    rect(d,(43,162,596,246),'ink');rect(d,(45,164,594,244),'#835849')
    for x in range(50,590,20):rect(d,(x,168,x+10,240),'#9b6552')
    line(d,[(48,172),(590,172)],'sanddark',2);line(d,[(48,236),(590,236)],'sanddark',2)
    for x in (18,588):
        rect(d,(x,281,x+30,311),'ink');rect(d,(x+2,283,x+28,308),'woodlight')
        ellipse(d,(x+8,270,x+24,287),'leaf');ellipse(d,(x+11,267,x+21,280),'mint')
    # Entry mat.
    rect(d,(275,337,367,351),'sand');line(d,[(280,342),(362,342)],'wood',2)
    im.save(ART/'interior.png')
interior()

# Original low-volume ambient and short interaction cues; reproducible PCM WAV.
AUDIO=ROOT/'Game/Assets/Wildfeast/Resources/Audio';AUDIO.mkdir(parents=True,exist_ok=True)
for name,duration in [('harbor',12),('chime',0.5),('splash',0.35),('cook',0.4)]:
    sr=22050;samples=[]
    for n in range(int(sr*duration)):
        t=n/sr
        if name=='harbor':
            env=math.sin(math.pi*t/duration)**2
            value=env*(0.07*math.sin(2*math.pi*130.81*t)+0.035*math.sin(2*math.pi*196*t)+0.02*math.sin(2*math.pi*261.63*t)+0.01*rng.uniform(-1,1))
        elif name=='chime':value=0.22*math.exp(-t*9)*(math.sin(2*math.pi*523.25*t)+0.4*math.sin(2*math.pi*783.99*t))
        elif name=='splash':value=0.18*math.exp(-t*12)*rng.uniform(-1,1)
        else:value=0.09*math.exp(-t*8)*rng.uniform(-1,1)
        samples.append(struct.pack('<h',int(max(-1,min(1,value))*32767)))
    with wave.open(str(AUDIO/f'{name}.wav'),'wb') as w:w.setnchannels(1);w.setsampwidth(2);w.setframerate(sr);w.writeframes(b''.join(samples))

sheet=Image.new('RGB',(640,620),'#1c373b');sd=ImageDraw.Draw(sheet)
sd.text((18,16),'WILDFEAST / ORIGINAL ART STUDY',fill=PAL['cream'])
names=['player-0','tree','restaurant','leafgill','pepperbell','brothback','lanternroot','cloudfruit','dish-wrap','boat','table','stove']
x,y=18,45
for name in names:
    im=Image.open(ART/f'{name}.png');scale=2 if im.width<=64 else 1
    im=im.resize((im.width*scale,im.height*scale),Image.Resampling.NEAREST)
    if x+im.width>620:x=18;y+=145
    sheet.paste(im,(x,y),im);sd.text((x,y+im.height+4),name,fill=PAL['cream']);x+=im.width+20
parser=argparse.ArgumentParser();parser.add_argument('--study-output',type=Path,default=ROOT/'artifacts');options=parser.parse_args()
out=options.study_output;out.mkdir(parents=True,exist_ok=True)
sheet.resize((1280,1240),Image.Resampling.NEAREST).save(out/'wildfeast-art-study.png')
print(f'Created {len(list(ART.glob("*.png")))} original PNGs and 4 audio clips.')
