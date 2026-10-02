"""Original characterful ingredient sprites; run after polish_art.py."""
from pathlib import Path
from PIL import Image, ImageDraw
import math
ROOT=Path(__file__).resolve().parents[1]/'Game/Assets/Wildfeast/Resources/Art'
def sprite(name,size,draw):
    im=Image.new('RGBA',size);d=ImageDraw.Draw(im);draw(d);im.save(ROOT/(name+'.png'))
def brothback(d,frame=0):
    ink='#293a43';bob=frame%2
    d.ellipse((13,56,70,69),fill='#233e4160')
    for x,side in [(12,-1),(65,1)]:
        for n in range(3):d.line([(x+side*2,39+n*6),(x+side*11,47+n*5+bob),(x+side*7,51+n*5+bob)],fill=ink,width=5);d.line([(x+side*2,39+n*6),(x+side*11,47+n*5+bob)],fill='#c78566',width=2)
    d.ellipse((11,17+bob,66,58+bob),fill=ink);d.ellipse((14,19+bob,63,54+bob),fill='#71918a');d.arc((17,24+bob,60,52+bob),0,180,fill='#b5c3a5',width=2)
    d.ellipse((19,10+bob,60,44+bob),fill=ink);d.ellipse((22,12+bob,57,41+bob),fill='#ac7854');d.ellipse((27,16+bob,52,35+bob),fill='#d7a06b');d.arc((27,16+bob,52,35+bob),180,360,fill='#efd6a1',width=2)
    d.rectangle((35,4+bob,43,14+bob),fill=ink);d.rectangle((37,5+bob,41,11+bob),fill='#9cafa0');d.rectangle((31,1+bob,47,5+bob),fill=ink);d.rectangle((33,2+bob,45,4+bob),fill='#c9d0b1')
    for x in [22,49]:
        d.line([(x+3,44+bob),(x+3,48+bob)],fill=ink,width=3);d.ellipse((x,47+bob,x+8,55+bob),fill='#e7e0b5');d.rectangle((x+3,50+bob,x+5,54+bob),fill=ink)
    d.line([(32,56+bob),(40,58+bob),(47,55+bob)],fill=ink,width=2)
    d.arc((53,14+bob,70,34+bob),270,90,fill=ink,width=5);d.arc((53,14+bob,68,33+bob),270,90,fill='#b2a17a',width=2)
    for x in [17,62]:d.ellipse((x-6,33+bob,x+6,46+bob),fill=ink);d.ellipse((x-4,34+bob,x+4,42+bob),fill='#d49d73')
for f in range(4):sprite('brothback-'+str(f),(80,72),lambda d,f=f:brothback(d,f))
sprite('brothback',(80,72),brothback)
def fish(d):
    ink='#284747'
    d.polygon([(10,18),(0,8),(2,29),(12,25)],fill=ink);d.polygon([(9,19),(2,12),(4,25)],fill='#6da76e')
    d.ellipse((8,5,44,35),fill=ink);d.ellipse((10,7,42,33),fill='#9ac285')
    for x,y in [(12,8),(19,6),(23,9),(30,10)]:
        d.arc((x,y,x+16,y+21),80,270,fill='#537e5d',width=2);d.arc((x+2,y+2,x+14,y+19),100,245,fill='#d4e1a6')
    d.polygon([(21,31),(27,38),(31,31)],fill='#517b5c');d.ellipse((33,13,41,22),fill='#f0e9c1');d.rectangle((37,15,39,19),fill=ink);d.point((37,15),fill='#ffffff');d.line([(37,27),(42,25)],fill=ink)
sprite('leafgill',(48,40),fish)
def pepper(d):
    d.ellipse((8,35,38,45),fill='#263e3f50');d.line([(24,39),(24,9)],fill='#36594c',width=3)
    for x,y in [(7,14),(27,18),(21,0)]:
        d.line([(24,25),(x+7,y+6)],fill='#5a7c53',width=2);d.ellipse((x,y,x+14,y+22),fill='#383d3e');d.ellipse((x+2,y+2,x+12,y+19),fill='#d38f59');d.ellipse((x+4,y+3,x+7,y+11),fill='#f3c986');d.line([(x+5,y),(x+8,y)],fill='#98b06c',width=3)
    d.ellipse((4,2,20,10),fill='#80a56d');d.ellipse((26,6,43,14),fill='#abd086')
sprite('pepperbell',(48,48),pepper)
print('Creature sprites and Brothback movement frames generated')
for i in range(5):
    sprite('table-number-'+str(i),(16,20),lambda d,i=i:[d.rectangle((0,1,15,17),fill='#304c47'),d.rectangle((1,2,14,16),outline='#dfb675'),d.text((5,3),str(i+1),fill='#f0deae')])
sprite('ui-board',(24,24),lambda d:[d.rectangle((0,0,23,23),fill='#71523f'),d.rectangle((2,2,21,21),fill='#bc976b'),d.rectangle((3,3,20,20),outline='#e0bd87'),d.rectangle((5,5,18,18),fill='#ad865c')])
sprite('garnish-herb',(32,32),lambda d:[d.line([(16,28),(16,9)],fill='#3e6450',width=2),d.ellipse((3,6,16,15),fill='#76a466'),d.ellipse((15,1,28,11),fill='#a6c782'),d.ellipse((6,18,18,24),fill='#92b56c')])
sprite('garnish-salt',(32,32),lambda d:[d.rectangle((7,8,24,28),fill='#d4ddbf'),d.ellipse((7,3,24,13),fill='#648279'),d.rectangle((11,13,20,25),fill='#f2eacb'),d.point((12,7),fill='#f6edd1'),d.point((18,7),fill='#f6edd1')])
def pot(d):
    d.ellipse((8,30,87,68),fill='#29383d');d.rectangle((10,23,85,49),fill='#668b87');d.ellipse((10,11,85,42),fill='#e0d5b0');d.ellipse((14,15,81,36),fill='#d2a263')
    for x in [22,41,62]:d.ellipse((x,20,x+9,25),fill='#a5b475');d.line([(x,10),(x-3,4),(x+1,0)],fill='#e8eed399',width=2)
    d.rectangle((1,22,11,32),fill='#668b87');d.rectangle((84,22,95,32),fill='#668b87');d.line([(48,26),(74,0)],fill='#b08054',width=4)
sprite('cook-pot',(96,72),pot)
def whisk(d):
    d.ellipse((5,24,86,68),fill='#465957');d.ellipse((5,13,86,48),fill='#dfddbb');d.ellipse((11,18,80,43),fill='#f5e7be');d.line([(47,37),(72,4)],fill='#8c624d',width=5)
    for x in [39,45,51]:d.arc((x,19,x+21,46),0,360,fill='#a6b7ab',width=2)
sprite('cook-whisk',(96,72),whisk)
def guest(d,index,frame=0):
    bob=frame%2;step=-1 if frame==1 else 1 if frame==3 else 0;ink='#293b40'
    skin=['#dbab83','#bd865f','#e6ba94','#ab785e','#e0b58a'][index];shirt=['#548b99','#be865d','#9974a0','#b66e7e','#769762'][index]
    d.ellipse((5,39,27,45),fill='#203b4055')
    for x,s in [(10,step),(20,-step)]:d.rectangle((x,31+bob,x+5,40+s),fill=ink);d.rectangle((x,39+s,x+5,41+s),fill='#9d8061')
    d.rounded_rectangle((7,21+bob,26,33+bob),radius=3,fill=ink);d.rectangle((9,22+bob,24,31+bob),fill=shirt);d.rectangle((14,22+bob,18,28+bob),fill='#ddd2ab')
    d.rectangle((5,23+bob,8,30+bob),fill=skin);d.rectangle((25,23+bob,28,30+bob),fill=skin)
    d.rounded_rectangle((8,8+bob,25,23+bob),radius=4,fill=ink);d.rounded_rectangle((10,10+bob,24,21+bob),radius=3,fill=skin)
    d.rounded_rectangle((8,6+bob,25,14+bob),radius=4,fill='#65534a' if index%2 else '#aa7657');d.rectangle((8,11+bob,10,19+bob),fill='#65534a')
    for x in [12,21]:d.rectangle((x,15+bob,x+1,17+bob),fill=ink)
    d.line([(15,19+bob),(19,19+bob)],fill='#a87364')
    if index==1:d.rectangle((5,9+bob,28,12+bob),fill='#d5b07b');d.rectangle((10,2+bob,24,10+bob),fill='#b08e64')
    if index==2:d.ellipse((22,6+bob,30,14+bob),fill='#d591aa');d.ellipse((24,8+bob,27,11+bob),fill='#f8dd9b')
    if index==4:
        for x in [10,19]:d.rectangle((x,14+bob,x+5,18+bob),outline=ink);d.rectangle((x+1,15+bob,x+4,17+bob),fill='#83a6a2')
for n in range(5):
    sprite('guest-'+str(n),(32,48),lambda d,n=n:guest(d,n))
    for f in range(4):sprite('visitor-'+str(n)+'-'+str(f),(32,48),lambda d,n=n,f=f:guest(d,n,f))
sprite('nori',(32,48),lambda d:guest(d,2))
def meal(d,kind):
    d.ellipse((1,12,46,37),fill='#293b43');d.ellipse((3,12,44,34),fill='#ddd8ae');d.ellipse((7,15,40,30),fill='#f5e6b9');d.arc((5,14,42,33),0,180,fill='#a9baaa',width=1)
    if kind=='fish':
        for x,y in [(11,14),(20,10),(29,14)]:
            d.rounded_rectangle((x,y,x+10,y+15),radius=4,fill='#b57549');d.rounded_rectangle((x+1,y+1,x+8,y+12),radius=3,fill='#e5ba72');d.line([(x+2,y+3),(x+7,y+7)],fill='#a8784e',width=2);d.line([(x+2,y+2),(x+5,y+2)],fill='#f6d88e')
        d.ellipse((7,25,18,30),fill='#78945d');d.ellipse((32,25,40,29),fill='#8eab69')
    elif kind=='wrap':
        for x,y in [(10,11),(24,14)]:d.rounded_rectangle((x,y,x+13,y+15),radius=4,fill='#456e51');d.line([(x+3,y+3),(x+10,y+10)],fill='#b7cf85',width=3);d.ellipse((x+3,y+4,x+9,y+9),fill='#dcaa6c')
    elif kind=='cloud':
        d.rectangle((10,14,37,30),fill='#816450');d.rectangle((12,15,35,27),fill='#e5cd9d');d.ellipse((8,6,39,23),fill='#fff0c5');d.arc((11,8,36,20),0,180,fill='#d7b27d',width=2);d.ellipse((20,7,26,11),fill='#c18198')
    else:
        d.ellipse((4,9,43,32),fill='#52746e');d.ellipse((5,7,42,25),fill='#f3deae');d.ellipse((8,10,39,22),fill='#a3b68a' if kind=='lantern' else '#d5ac71')
        for x,y in [(13,13),(25,14),(32,11)]:d.rectangle((x,y,x+3,y+2),fill='#ecd5a0');d.line([(x,y),(x+2,y-2)],fill='#769467')
    for x,y in [(15,4),(33,2)]:d.line([(x,y+4),(x-1,y+1),(x+1,y-2)],fill='#ecead38a')
for kind in ['fish','wrap','cloud','porridge','broth','lantern']:sprite('dish-'+kind,(48,40),lambda d,kind=kind:meal(d,kind))
