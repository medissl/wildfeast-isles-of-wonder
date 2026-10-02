"""Original pixel sprites for the living-world pass. No reference image is sampled."""
from pathlib import Path
import random, math
from PIL import Image, ImageDraw

ART=Path(__file__).resolve().parents[1]/'Game/Assets/Wildfeast/Resources/Art'
r=random.Random(8491)
INK='#28313e'; WOOD='#805246'; GOLD='#e5b56d'; CREAM='#f5e3bd'
def save(name,size,fn):
    im=Image.new('RGBA',size);d=ImageDraw.Draw(im);fn(d);im.save(ART/(name+'.png'));return im
def foliage(name,colors):
    def draw(d):
        d.ellipse((14,99,85,111),fill='#25393b66')
        d.polygon([(39,100),(44,66),(52,66),(58,99),(67,105),(52,103),(40,108),(27,107)],fill=INK)
        d.polygon([(42,101),(46,58),(51,58),(54,99)],fill=WOOD)
        d.line([(48,95),(47,69),(32,54)],fill='#bb8b62',width=2)
        d.line([(51,78),(66,58)],fill='#b38255',width=2)
        lobes=[(29,23,24),(15,43,15),(68,30,22),(47,14,19),(48,47,28),(77,49,17),(26,60,18),(57,64,18)]
        for x,y,rad in lobes:d.ellipse((x-rad,y-rad,x+rad,y+rad),fill=INK)
        for x,y,rad in lobes:
            d.ellipse((x-rad+2,y-rad+2,x+rad-2,y+rad-2),fill=colors[0])
            for _ in range(rad*12):
                xx=r.randrange(x-rad+2,x+rad-1);yy=r.randrange(y-rad+2,y+rad-1)
                if ((xx-x)/rad)**2+((yy-y)/rad)**2>.77:continue
                weight=(1-(yy-y+rad)/(2*rad))*.8+r.random()*.5
                col=colors[min(3,max(0,int(weight*3)))];d.rectangle((xx,yy,xx+r.randrange(1,4),yy+1),fill=col)
        for x,y in [(24,32),(45,17),(66,38),(42,57)]:d.rectangle((x,y,x+3,y+1),fill=colors[3])
    return save(name,(96,112),draw)
foliage('tree',['#365746','#4d7648','#78a34f','#bad379'])
foliage('tree-purple',['#55345e','#77477c','#ab69a0','#e6a5c2'])
foliage('tree-gold',['#865744','#b57847','#e4a35f','#ffda93'])
foliage('tree-blue',['#2d546b','#3e7a84','#6bb4bb','#a8dace'])

def shrub(name,colors):
    def draw(d):
        d.ellipse((1,21,46,39),fill='#21383970')
        for x,y,rad in [(12,20,11),(24,12,11),(34,21,12)]:
            d.ellipse((x-rad,y-rad,x+rad,y+rad),fill=INK)
            d.ellipse((x-rad+1,y-rad+1,x+rad-1,y+rad-1),fill=colors[0])
            for _ in range(45):
                xx=r.randrange(x-rad+2,x+rad-1);yy=r.randrange(y-rad+2,y+rad-1)
                if ((xx-x)/rad)**2+((yy-y)/rad)**2<.8:d.rectangle((xx,yy,xx+2,yy+1),fill=r.choice(colors))
        for x,y in [(8,19),(17,12),(29,17),(38,24),(22,26)]:
            d.ellipse((x-3,y-3,x+3,y+3),fill='#cc7ca6');d.rectangle((x-1,y-1,x,y),fill='#ffe4ad')
    save(name,(48,40),draw)
shrub('flower-purple',['#3f6750','#6a985a','#b0c67e'])
def fern(d):
    for x,y in [(3,18),(9,5),(18,1),(27,6),(34,19)]:
        d.line([(18,33),(x,y)],fill='#2e544a',width=3)
        for n in range(1,6):
            xx=int(18+(x-18)*n/6);yy=int(33+(y-33)*n/6)
            d.line([(xx-4,yy-2),(xx,yy),(xx+4,yy-4)],fill='#8aba70' if n%2 else '#57875c',width=2)
save('fern',(40,36),fern)
def mushrooms(d):
    for x,y,s in [(8,11,8),(23,5,10),(30,20,6)]:
        d.rectangle((x-2,y+5,x+2,y+14),fill=CREAM);d.ellipse((x-s,y-s//2,x+s,y+7),fill=INK);d.ellipse((x-s+1,y-s//2+1,x+s-1,y+4),fill='#b45774')
        d.rectangle((x-2,y-1,x,y+1),fill='#f7caac')
save('mushrooms',(40,40),mushrooms)
def crystal(d):
    for x,y in [(8,12),(20,2),(31,15)]:
        d.polygon([(x,y),(x-5,y+12),(x,y+27),(x+5,y+12)],fill=INK);d.polygon([(x,y+2),(x-3,y+12),(x,y+24)],fill='#91cdd2');d.polygon([(x,y+2),(x+3,y+12),(x,y+24)],fill='#637fab')
save('crystal-plant',(40,40),crystal)

def building(name,roof='#976587',wall='#e3c695'):
    def draw(d):
        d.ellipse((15,174,175,190),fill='#253b3970')
        d.rectangle((24,83,166,174),fill=INK);d.rectangle((27,87,163,169),fill=wall)
        for y in range(95,165,7):
            for x in range(30+(4 if y%2 else 0),158,15):d.line([(x,y),(x+7,y)],fill='#c3aa82')
        for x in [28,65,124,161]:d.rectangle((x,87,x+3,168),fill=WOOD)
        for x in [36,131]:
            d.rounded_rectangle((x,108,x+24,142),radius=11,fill=INK);d.rounded_rectangle((x+3,111,x+21,139),radius=9,fill='#efbe67');d.rectangle((x+5,115,x+10,135),fill='#ffe5a1');d.line([(x+12,111),(x+12,140)],fill=WOOD,width=2);d.rectangle((x-2,142,x+28,148),fill=WOOD)
            for xx in range(x,x+25,5):d.ellipse((xx,140,xx+5,146),fill=r.choice(['#b8678a','#96aa63','#e8ba74']))
        d.rectangle((79,125,113,175),fill=INK);d.rectangle((82,128,110,171),fill='#674e47');d.rectangle((85,133,107,151),fill='#bd9964');d.rectangle((87,136,105,149),fill='#efc678');d.ellipse((104,157,108,160),fill=GOLD)
        d.polygon([(8,96),(32,32),(64,20),(128,20),(160,32),(182,96)],fill=INK)
        d.polygon([(12,91),(35,35),(65,25),(127,25),(157,35),(178,91)],fill=roof)
        for y in range(30,88,7):
            xmin=max(17,37-(y-35)//3);xmax=min(175,156+(y-35)//3)
            for x in range(xmin,xmax,12):
                d.line([(x,y),(x+9,y)],fill='#c187a2' if name=='restaurant' else '#83b3aa',width=2)
                d.line([(x+9,y+1),(x+9,y+4)],fill='#64435f' if name=='restaurant' else '#456c77')
        d.rectangle((139,5,156,43),fill=INK);d.rectangle((142,8,153,40),fill='#9b9b8d');d.rectangle((136,4,159,11),fill='#ced1b0')
        d.polygon([(67,69),(76,40),(95,31),(115,42),(125,69)],fill=INK);d.polygon([(72,65),(79,44),(95,36),(111,45),(119,65)],fill='#db9759')
        d.ellipse((82,44,109,71),fill=WOOD);d.ellipse((85,47,106,68),fill='#edc36e');d.line([(95,47),(95,67)],fill=WOOD,width=2);d.line([(85,57),(106,57)],fill=WOOD,width=2)
        d.rectangle((56,89,133,106),fill=INK);d.rectangle((59,92,130,103),fill=WOOD)
        d.text((67,93),'HARBOR TABLE' if name=='restaurant' else 'TIDEKEEPER',fill=CREAM)
        for x in [22,166]:
            for y in range(75,151,5):
                d.line([(x,y),(x+(3 if y%2 else -2),y+6)],fill='#4f7352',width=2);d.ellipse((x-4,y,x+4,y+4),fill='#90b568')
        d.rectangle((73,173,119,179),fill='#bc9a69');d.rectangle((69,180,123,184),fill='#dac18c')
    return save(name,(192,192),draw)
building('restaurant');building('iona-house','#497983','#c5d2ba')
def mushroom_house(d):
    d.ellipse((3,103,122,120),fill='#29453660');d.rectangle((25,51,103,105),fill=INK);d.rectangle((28,53,100,102),fill='#f1ce94')
    for y in range(62,99,6):d.line([(32,y),(97,y)],fill='#d4ae77')
    d.rounded_rectangle((48,68,76,107),radius=12,fill=WOOD);d.rounded_rectangle((53,73,72,101),radius=8,fill='#94b5a1')
    d.pieslice((2,0,124,108),180,360,fill=INK);d.pieslice((6,3,120,103),180,360,fill='#ab6674')
    for x,y in [(27,25),(70,10),(91,35),(48,40)]:d.ellipse((x,y,x+14,y+7),fill='#f1c38f')
    d.rectangle((4,50,123,55),fill='#5d4255')
save('mushroom-house',(128,120),mushroom_house)
def arch(d):
    d.arc((5,0,91,93),180,360,fill=INK,width=9);d.arc((8,3,88,90),180,360,fill=WOOD,width=4)
    for x in [6,86]:d.rectangle((x,43,x+5,92),fill=WOOD)
    for n in range(35):
        t=n/34*math.pi;x=int(48+41*math.cos(t));y=int(45-41*math.sin(t));d.ellipse((x-5,y-4,x+5,y+4),fill='#60815a');d.ellipse((x-2,y-2,x+2,y+2),fill=r.choice(['#e4a6be','#baa0d1','#ddca8c']))
save('arch',(96,96),arch)
def fountain(d):
    for box,c in [((2,30,77,60),INK),((5,31,74,54),'#a7b2a4'),((11,34,68,50),'#5aa8ae'),((25,15,53,43),'#849795'),((18,9,61,29),INK),((20,9,59,24),'#c1cbb0'),((25,13,54,21),'#6cb2b0')]:d.ellipse(box,fill=c)
    d.rectangle((36,0,43,14),fill='#b9d9cf');d.rectangle((34,0,45,3),fill='#d8eee0')
save('fountain',(80,64),fountain)
def lantern(d):
    d.rectangle((15,14,18,53),fill=WOOD);d.rectangle((6,3,27,21),fill=INK);d.rectangle((9,5,24,18),fill='#efa958');d.rectangle((12,6,21,17),fill='#ffedac');d.rectangle((14,5,17,19),fill=WOOD);d.polygon([(4,4),(16,0),(29,4)],fill='#b5815b');d.ellipse((10,49,23,54),fill='#334b3960')
save('lantern',(32,56),lantern)
def board(d,open_sign=False):
    d.rectangle((7,22,10,47),fill=WOOD);d.rectangle((38,22,41,47),fill=WOOD);d.rectangle((2,0,45,31),fill=INK);d.rectangle((4,2,43,28),fill='#a67651');d.rectangle((7,5,40,25),fill='#304f49')
    if open_sign:d.text((11,10),'OPEN',fill='#f7dba0')
    else:
        d.line([(12,9),(33,9)],fill=CREAM,width=2);d.line([(12,14),(30,14)],fill='#cad0aa');d.line([(12,19),(35,19)],fill='#cad0aa')
save('menu-board',(48,48),board)
save('sign-open',(48,48),lambda d:board(d,True))
def closed(d):board(d);d.rectangle((8,7,40,24),fill='#304f49');d.text((8,12),'CLOSED',fill='#e6bf91')
save('sign-closed',(48,48),closed)
def bed(d):
    d.rectangle((1,8,62,55),fill=INK);d.rectangle((4,11,59,48),fill=WOOD);d.rectangle((6,15,57,43),fill='#ad6480');d.rectangle((9,13,54,24),fill=CREAM);d.line([(6,29),(57,29)],fill='#d990a3',width=2);d.rectangle((4,47,9,59),fill=WOOD);d.rectangle((53,47,59,59),fill=WOOD)
save('bed',(64,64),bed)
def cupboard(d):
    d.rectangle((3,2,60,64),fill=INK);d.rectangle((6,5,57,59),fill=WOOD)
    for y in [17,37,57]:
        for x in [10,24,40]:d.rectangle((x,y-11,x+9,y-2),fill=r.choice(['#a9ae77','#c4945b','#9cbaaa']));d.rectangle((x,y-12,x+9,y-10),fill=GOLD)
        d.rectangle((7,y,56,y+2),fill='#c39665')
save('pantry',(64,68),cupboard)
def mailbox(d):
    d.rectangle((18,23,23,55),fill=WOOD);d.rounded_rectangle((3,4,38,30),radius=5,fill=INK);d.rounded_rectangle((6,7,35,27),radius=4,fill='#7597a0');d.rectangle((10,12,30,24),fill=CREAM);d.line([(10,12),(20,18),(30,12)],fill='#b3997d');d.rectangle((36,4,40,17),fill='#b46470')
save('mailbox',(44,56),mailbox)
def bench(d):
    for y in [7,16,25]:d.rectangle((3,y,62,y+5),fill=WOOD);d.line([(4,y),(60,y)],fill='#bb9062')
    for x in [8,55]:d.rectangle((x,22,x+4,40),fill=INK)
save('bench',(68,44),bench)
def workshop(d):
    d.rectangle((4,21,74,58),fill=INK);d.rectangle((7,24,71,54),fill=WOOD);d.rectangle((8,22,70,30),fill='#d2ad79')
    d.polygon([(0,20),(12,0),(66,0),(79,20)],fill='#b1b47c')
    for x in range(5,74,14):d.rectangle((x,13,x+7,20),fill='#ebe2b1')
    d.line([(19,48),(24,34)],fill='#d2ccaa',width=3);d.rectangle((44,33,62,47),fill='#76968a');d.rectangle((49,28,57,33),fill=GOLD)
save('workshop',(80,64),workshop)
def ship(d):
    d.ellipse((4,45,122,82),fill='#1f405c88');d.polygon([(3,42),(19,23),(106,23),(125,44),(102,72),(27,72)],fill=INK);d.polygon([(7,43),(22,27),(104,27),(120,44),(100,66),(29,66)],fill='#a77352')
    for y in range(30,63,5):d.line([(27,y),(100,y)],fill='#d0a070')
    d.rectangle((58,0,62,51),fill=WOOD);d.polygon([(64,1),(64,35),(108,35)],fill='#e8d9ac');d.line([(65,7),(94,32)],fill='#c8b585',width=2);d.rectangle((19,33,42,52),fill='#8aa499');d.rectangle((21,35,39,46),fill=CREAM)
save('boat',(128,88),ship)

def chef(d,direction,frame):
    bob=frame%2;step=-1 if frame==1 else 1 if frame==3 else 0
    d.ellipse((6,39,26,46),fill='#21333466')
    for x,delta in [(10,step),(20,-step)]:d.rectangle((x,32+bob,x+5,40+delta),fill=INK);d.rectangle((x,39+delta,x+5,41+delta),fill='#b08a63')
    d.rectangle((7,21+bob,26,33+bob),fill=INK);d.rectangle((9,22+bob,24,31+bob),fill='#668f85');d.rectangle((12,23+bob,22,33+bob),fill='#eee0ae');d.rectangle((12,30+bob,22,32+bob),fill='#c2b484')
    d.rectangle((5,23+bob,8,30+bob),fill='#dda980');d.rectangle((25,23+bob,28,30+bob),fill='#dda980')
    d.rounded_rectangle((8,8+bob,25,23+bob),radius=4,fill=INK);d.rounded_rectangle((10,10+bob,24,21+bob),radius=3,fill='#e5b48a');d.rectangle((9,10+bob,23,14+bob),fill='#57433c')
    if direction=='up':d.rectangle((9,13+bob,24,22+bob),fill='#765547');d.line([(11,19+bob),(21,19+bob)],fill='#a07756')
    elif direction in ['left','right']:
        x=11 if direction=='left' else 21;d.rectangle((x,15+bob,x+1,17+bob),fill=INK);d.rectangle((x,19+bob,x+2,19+bob),fill='#ad725d')
    else:
        for x in [12,21]:d.rectangle((x,15+bob,x+1,17+bob),fill=INK)
        d.line([(15,19+bob),(19,19+bob)],fill='#b57866')
    d.rectangle((6,7+bob,27,11+bob),fill=INK);d.rectangle((7,7+bob,26,9+bob),fill=CREAM)
    for box in [(9,3+bob,15,8+bob),(14,0+bob,22,8+bob),(21,3+bob,25,8+bob)]:d.ellipse(box,fill=CREAM)
    d.line([(10,5+bob),(12,5+bob)],fill='#fff4d5');d.line([(16,3+bob),(21,3+bob)],fill='#fff4d5')
for direction in ['down','up','left','right']:
    for f in range(4):save(f'chef-{direction}-{f}',(32,48),lambda d,direction=direction,f=f:chef(d,direction,f))

def tool(d,kind):
    if kind=='rod':
        d.line([(8,34),(25,3)],fill=INK,width=3);d.line([(9,33),(25,3)],fill='#d3a676');d.line([(26,3),(30,26)],fill='#f0e2b6');d.ellipse((6,24,12,30),fill='#759da6')
    elif kind=='can':
        d.ellipse((20,9,31,27),outline=GOLD,width=3);d.rectangle((7,13,23,31),fill=INK);d.rectangle((8,14,22,29),fill='#65a3a2');d.polygon([(8,18),(0,13),(2,23),(8,25)],fill='#90c6b6');d.ellipse((7,9,24,17),fill='#a8d3ba');d.rectangle((11,12,20,14),fill='#367783')
    elif kind=='knife':
        d.polygon([(6,29),(12,22),(28,3),(30,13),(17,27)],fill=INK);d.polygon([(12,22),(28,5),(27,15),(16,25)],fill='#ced9cf');d.line([(6,31),(13,24)],fill=WOOD,width=5);d.line([(7,30),(12,25)],fill=GOLD)
    elif kind=='hand':
        d.rounded_rectangle((7,14,25,33),radius=5,fill=INK);d.rounded_rectangle((9,15,24,31),radius=4,fill='#e4b081')
        for x,h in [(9,9),(14,5),(19,7),(24,11)]:d.rounded_rectangle((x,h,x+4,22),radius=2,fill='#e4b081')
    elif kind=='journal':
        d.rectangle((4,4,29,34),fill=INK);d.rectangle((6,5,27,32),fill='#a37851');d.rectangle((8,7,24,30),fill='#457363');d.rectangle((23,8,25,29),fill=CREAM);d.ellipse((12,13,21,22),fill=GOLD)
    else:
        d.rounded_rectangle((4,10,29,34),radius=5,fill=INK);d.rounded_rectangle((6,12,27,31),radius=4,fill='#a6754e');d.rectangle((8,13,25,22),fill='#c79c66');d.rectangle((15,20,19,26),fill=GOLD);d.arc((10,1,24,19),180,360,fill=WOOD,width=3)
for kind in ['rod','can','knife','hand','journal','bag']:save('tool-'+kind,(32,40),lambda d,kind=kind:tool(d,kind))
def seed(d,color):
    d.rectangle((6,6,27,34),fill=INK);d.rectangle((8,7,25,32),fill='#e5cb98');d.line([(8,12),(25,12)],fill='#b9986c');d.ellipse((12,16,22,25),fill=color);d.line([(17,18),(17,14)],fill='#568361',width=2)
save('seed-pepper',(32,40),lambda d:seed(d,'#cf8456'));save('seed-root',(32,40),lambda d:seed(d,'#90bfa2'))
save('planted-seed',(32,24),lambda d:[d.ellipse((7,15,25,21),fill='#593d35'),d.ellipse((14,14,20,18),fill='#e6c17f')])
save('sprout',(32,32),lambda d:[d.line([(17,30),(17,14)],fill='#567a54',width=2),d.ellipse((5,8,17,16),fill='#90bf78'),d.ellipse((17,4,29,13),fill='#b4d292')])
for wet in [False,True]:
    def plot(d):
        d.rectangle((0,3,63,30),fill=INK);d.rectangle((2,5,61,28),fill='#614a3d' if wet else '#977052')
        for y in [8,16,24]:d.line([(5,y),(58,y)],fill='#3e3935' if wet else '#674b3b',width=2);d.line([(7,y+2),(57,y+2)],fill='#7a6650' if wet else '#b48960')
        for _ in range(24):x=r.randrange(4,58);y=r.randrange(6,28);d.point((x,y),fill='#a6956b' if wet else '#d1a274')
    save('plot-wet' if wet else 'plot',(64,32),plot)
def ui(d):
    d.rectangle((0,0,23,23),fill='#9b7654');d.rectangle((2,2,21,21),fill='#203c38');d.rectangle((3,3,20,20),outline='#d4b07a');d.rectangle((5,5,18,18),fill='#203c38')
    for x,y in [(0,0),(20,0),(0,20),(20,20)]:d.rectangle((x,y,x+3,y+3),fill='#f0d5a0')
save('ui-frame',(24,24),ui)
def prep(d):
    d.rounded_rectangle((4,4,92,66),radius=6,fill=INK);d.rounded_rectangle((6,6,90,63),radius=4,fill='#ba8f5d')
    for y in range(12,60,8):d.line([(10,y),(86,y)],fill='#d6ad75')
    for n in range(4):d.ellipse((26+n*9,21,37+n*9,47),fill='#9cc87c');d.arc((27+n*9,22,35+n*9,45),90,270,fill='#548358',width=2)
    d.polygon([(60,13),(77,3),(86,9),(68,23)],fill='#c7d6c8');d.line([(55,28),(65,19)],fill=WOOD,width=6)
save('prep-board',(96,72),prep)
def pan(d):
    d.ellipse((2,9,73,67),fill=INK);d.ellipse((6,10,69,60),fill='#67776f');d.ellipse((11,16,64,53),fill='#313e41');d.line([(64,40),(92,51)],fill=WOOD,width=8);d.ellipse((24,24,54,48),fill='#d7af66');d.line([(30,26),(46,40)],fill='#bf784b',width=3)
    for x in [18,34,52]:d.line([(x,15),(x-2,6),(x+1,0)],fill='#eee9caaa',width=2)
save('cook-pan',(96,72),pan)
save('bobber',(16,20),lambda d:[d.ellipse((3,6,12,18),fill='#dae7d7'),d.rectangle((4,5,11,10),fill='#d96c6a'),d.line([(8,0),(8,6)],fill=GOLD)])
save('fish-shadow',(32,16),lambda d:[d.ellipse((7,3,28,12),fill='#183d656a'),d.polygon([(9,8),(1,2),(1,13)],fill='#183d656a')])
for f in range(4):save('ripple-'+str(f),(48,24),lambda d,f=f:d.ellipse((12-f*3,9-f*2,35+f*3,15+f*2),outline='#a8d9d066',width=1))
save('steam',(24,32),lambda d:[d.ellipse((5,14,19,26),fill='#e2ebdb70'),d.ellipse((0,8,14,22),fill='#e2ebdb60'),d.ellipse((7,0,23,16),fill='#e2ebdb50')])
save('splash',(24,24),lambda d:[d.arc((1,8,22,21),0,180,fill='#b0e3d1',width=2),d.line([(5,12),(3,4)],fill='#cfeade',width=2),d.line([(17,12),(21,2)],fill='#cfeade',width=2)])
save('butterfly',(16,16),lambda d:[d.ellipse((0,2,7,11),fill='#edb997'),d.ellipse((8,2,15,11),fill='#d783aa'),d.line([(7,4),(8,14)],fill=INK,width=2)])
save('hanging-herbs',(80,48),lambda d:[d.line([(0,3),(79,3)],fill=WOOD,width=2),*[d.line([(x,3),(x,18)],fill=GOLD) or d.ellipse((x-8,16,x+7,43),fill='#6e9a62') or d.line([(x-3,20),(x+2,35)],fill='#a2b679',width=2) for x in [12,37,65]]])

# Terrain retains the same shoreline coordinates used by collision and casting.
def terrain(second=False):
    W,H=1280,832;im=Image.new('RGBA',(W,H),'#235783');d=ImageDraw.Draw(im)
    for y in range(0,H,5):
        for x in range(0,W,13):
            if r.random()<.22:d.line([(x,y),(x+r.randrange(3,9),y)],fill=r.choice(['#316f98','#347ca3','#285e8b']))
    poly=[(90,500),(80,300),(140,185),(330,130),(470,72),(650,78),(780,110),(990,100),(1140,230),(1180,370),(1140,590),(1050,655),(850,706),(590,735),(410,735),(280,690),(175,630)]
    for div,col in [(100,'#99d8ce'),(35,'#b2b38d'),(12,'#dac592'),(5,'#668859' if second else '#7b975a')]:
        points=[(x+(640-x)//div,y+(420-y)//div) for x,y in poly];d.polygon(points,fill=col)
    land=points;mask=Image.new('1',(W,H));ImageDraw.Draw(mask).polygon(land,fill=1)
    for _ in range(14500):
        x=r.randrange(W);y=r.randrange(H)
        if mask.getpixel((x,y)):
            d.rectangle((x,y,x+r.randrange(1,4),y+1),fill=r.choice(['#7b995e','#8da367','#637f52','#9bb471'] if not second else ['#77996f','#8cad85','#5f8369','#a0baa0']))
    # Wide paths connect each recognizable landmark and the garden, with stone insets.
    routes=[[(448,497),(448,555),(325,555),(310,631)],[(448,520),(645,510),(782,459),(843,399)],[(645,510),(850,581),(917,655)],[(448,497),(390,385),(349,267)],[(645,510),(733,414),(881,354)]]
    for pts in routes:d.line(pts,fill='#a79572',width=38);d.line(pts,fill='#d7c192',width=31)
    for pts in routes:
        for a,b in zip(pts,pts[1:]):
            dist=math.dist(a,b)
            for step in range(int(dist/11)):
                f=step/(dist/11);x=int(a[0]+(b[0]-a[0])*f);y=int(a[1]+(b[1]-a[1])*f);d.rectangle((x-4,y-2,x+4,y+1),fill='#c1ad83')
    for box,c in [((841,146,1100,310),'#a7b293'),((850,155,1090,300),'#dac592'),((862,165,1078,288),'#397d98')]:d.ellipse(box,fill=c)
    for _ in range(150):
        x=r.randrange(874,1060);y=r.randrange(179,274)
        if ((x-970)/95)**2+((y-226)/49)**2<1:d.line([(x,y),(x+6,y)],fill='#83c7c7')
    for x,y in [(940,210),(997,250),(901,248)]:d.ellipse((x,y,x+14,y+6),fill='#709969');d.ellipse((x+4,y-1,x+7,y+2),fill='#dfb6bf')
    d.rectangle((249,595,387,666),fill=INK);d.rectangle((252,597,384,661),fill='#ac805b')
    for y in range(599,661,7):d.line([(254,y),(383,y)],fill='#d2aa79');d.line([(254,y+2),(383,y+2)],fill='#7b5b48')
    for x in [255,377]:d.rectangle((x,593,x+5,665),fill=WOOD);d.rectangle((x,593,x+5,597),fill=CREAM)
    for x in range(240,465,17):d.rectangle((x,426,x+3,449),fill=WOOD);d.rectangle((x,431,x+18,434),fill='#c7a278')
    for _ in range(350):
        x=r.randrange(260,460);y=r.randrange(320,400)
        if r.random()<.5:d.rectangle((x,y,x+2,y+2),fill=r.choice(['#cf92b7','#a86fc0','#e7c184','#dce6ba']))
    im.save(ART/('mistwake.png' if second else 'saltleaf.png'))
terrain();terrain(True)

# Warm, textured interior; subtle rug, kitchen tile, window glows, and wall shelves.
im=Image.open(ART/'interior.png');d=ImageDraw.Draw(im)
for _ in range(1500):
    x=r.randrange(12,628);y=r.randrange(75,336)
    if not (162<y<246):d.line([(x,y),(x+r.randrange(2,6),y)],fill=r.choice(['#b99063','#976a4b','#bf9568']))
for x in range(225,440,20):
    for y in range(68,114,16):d.rectangle((x,y,x+18,y+14),fill='#b5b5a1' if (x+y)%3 else '#819d94')
for x in [21,595]:d.rectangle((x,84,x+22,130),fill=WOOD);d.rectangle((x+2,86,x+20,113),fill='#657e70')
im.save(ART/'interior.png')
print('Living-world art generated:',len(list(ART.glob('*.png'))),'original pixel sprites')
