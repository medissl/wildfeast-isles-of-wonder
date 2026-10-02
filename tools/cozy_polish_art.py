"""Original room furniture, compact held props and chef action poses. Run last.
All world assets retain 32 PPU; smaller native canvases mean smaller objects.
"""
from pathlib import Path
from PIL import Image, ImageDraw
ART=Path(__file__).resolve().parents[1]/'Game/Assets/Wildfeast/Resources/Art'
INK='#293b40'; WOOD='#9d6b49'; GOLD='#d6ad70'; CREAM='#eee0ae'; SKIN='#e5b48a'
def save(name,size,paint):
    im=Image.new('RGBA',size);paint(ImageDraw.Draw(im));im.save(ART/(name+'.png'))

def kitchen(d):
    # A single connected worktop, with its pot and board seated on the surface.
    d.ellipse((4,48,155,63),fill='#23383c55')
    d.rectangle((8,30,151,56),fill=INK);d.rectangle((10,31,149,54),fill=WOOD)
    for x in [13,47,112,137]:
        d.rectangle((x,36,x+18,50),fill='#79523c');d.rectangle((x+1,37,x+17,49),outline='#bb8657');d.rectangle((x+7,38,x+11,39),fill=GOLD)
    d.rectangle((6,25,153,34),fill=INK);d.rectangle((8,26,151,30),fill='#e2c294');d.line((9,27,150,27),fill='#f4ddaf')
    d.rectangle((71,32,104,54),fill=INK);d.rectangle((74,34,101,52),fill='#76534a');d.rectangle((77,37,98,49),fill='#3c4845')
    d.rectangle((81,40,94,46),fill='#d69656');d.rectangle((83,41,92,44),fill='#f6d780')
    for x in [76,87,98]:d.rectangle((x,29,x+3,31),fill=GOLD)
    # Small soup pot, pan and chopping board, drawn at world scale.
    d.ellipse((67,19,91,28),fill=INK);d.rectangle((69,14,89,23),fill='#6a9990');d.ellipse((69,9,89,19),fill=CREAM);d.ellipse((72,11,86,16),fill='#d4ad70')
    d.rectangle((65,14,69,17),fill=INK);d.rectangle((89,14,93,17),fill=INK)
    d.ellipse((99,19,119,27),fill=INK);d.ellipse((102,20,115,24),fill='#d0a568');d.line((116,23,130,25),fill=INK,width=3)
    d.rectangle((21,19,52,27),fill=INK);d.rectangle((23,20,50,25),fill='#dab279');d.line((29,23,44,23),fill='#ae7f55')
    d.ellipse((32,17,39,23),fill='#93b77a');d.ellipse((42,17,47,22),fill='#dd9360')
    d.line((48,21,57,12),fill='#d6debe',width=2);d.line((54,14,59,9),fill=WOOD,width=3)
    d.rectangle((136,15,146,26),fill='#725047')
    for x in [137,141,145]:d.line((x,18,x-1,7),fill='#b3c49a',width=2)
save('kitchen-worktop',(160,64),kitchen)

def plant(d):
    d.ellipse((2,24,30,31),fill='#23383c55');d.polygon([(7,18),(25,18),(22,29),(10,29)],fill=INK);d.polygon([(9,19),(23,19),(20,27),(12,27)],fill='#b47b55');d.line((11,20,21,20),fill=GOLD)
    d.line((16,21,16,6),fill='#486c52',width=2)
    for box,col in [((5,8,17,17),'#74986a'),((14,3,28,13),'#91b67a'),((10,0,20,9),'#c59aaf')]:d.ellipse(box,fill=INK);d.ellipse(tuple(v+1 if i<2 else v-1 for i,v in enumerate(box)),fill=col)
save('room-planter',(32,32),plant)

def scythe(d,compact=False):
    if compact:
        d.line((7,23,13,6),fill=INK,width=4);d.line((7,22,13,6),fill=WOOD,width=2)
        d.polygon([(12,4),(18,2),(23,4),(24,8),(23,12),(20,15),(21,9),(19,6),(12,7)],fill=INK)
        d.line([(13,5),(18,4),(21,6),(22,10)],fill='#b8d3c4',width=2)
    else:
        d.line((8,37,18,9),fill=INK,width=5);d.line((8,36,18,10),fill=WOOD,width=3)
        d.polygon([(16,7),(22,3),(28,4),(31,9),(31,16),(27,22),(24,25),(27,16),(26,10),(22,8),(17,11)],fill=INK)
        d.line([(18,8),(23,6),(27,8),(29,12),(28,18)],fill='#b8d3c4',width=2)
        d.line((8,31,11,24),fill=GOLD,width=3)
def pick(d,compact=False):
    if compact:
        d.line((9,23,13,7),fill=INK,width=4);d.line((9,22,13,7),fill=WOOD,width=2)
        d.polygon([(2,8),(6,3),(13,1),(21,4),(24,9),(17,6),(13,5),(8,6)],fill=INK)
        d.line([(4,7),(8,4),(13,3),(19,5),(22,7)],fill='#91b7b6',width=2)
    else:
        d.line((12,37,17,12),fill=INK,width=5);d.line((12,36,17,12),fill=WOOD,width=3)
        d.polygon([(1,15),(5,8),(13,5),(20,5),(28,9),(31,15),(23,12),(17,10),(9,11)],fill=INK)
        d.line([(4,13),(7,10),(14,7),(20,7),(27,11)],fill='#94bdbc',width=3)
        d.line((12,30,13,24),fill=GOLD,width=3)
save('icon-scythe',(32,40),scythe);save('icon-pickaxe',(32,40),pick)
save('held-scythe',(26,26),lambda d:scythe(d,True));save('held-pickaxe',(26,26),lambda d:pick(d,True))
save('held-axe',(22,26),lambda d:[d.line((7,24,13,6),fill=INK,width=4),d.line((7,23,13,6),fill=WOOD,width=2),d.polygon([(10,3),(19,2),(21,6),(19,12),(12,10)],fill=INK),d.polygon([(13,4),(18,4),(19,6),(18,10),(13,8)],fill='#bed0b6')])
save('held-shovel',(20,26),lambda d:[d.line((9,5,9,18),fill=INK,width=4),d.line((9,5,9,18),fill=WOOD,width=2),d.rectangle((6,1,12,5),outline=INK,width=2),d.polygon([(4,15),(14,15),(14,22),(9,25),(4,22)],fill=INK),d.polygon([(6,17),(12,17),(12,21),(9,23),(6,21)],fill='#9cbbb0')])
save('held-can',(22,20),lambda d:[d.rectangle((3,7,15,18),fill=INK),d.rectangle((4,8,14,17),fill='#79afac'),d.line([(14,10),(19,7),(21,8)],fill='#bed5bb',width=3),d.arc((0,6,7,15),90,270,fill=GOLD,width=2),d.rectangle((6,4,12,7),fill='#a9cdc1'),d.line((5,10,5,15),fill='#b9d9ca')])
save('held-knife',(18,24),lambda d:[d.line((3,22,8,15),fill=INK,width=4),d.line((3,21,8,15),fill=WOOD,width=2),d.polygon([(7,14),(16,1),(16,8),(10,16)],fill=INK),d.polygon([(9,13),(15,3),(14,8),(10,14)],fill='#cdd8b9')])
save('held-rod',(24,32),lambda d:[d.line((4,30,19,1),fill=INK,width=3),d.line((5,29,19,1),fill=GOLD),d.line((20,2,23,24),fill=CREAM),d.ellipse((2,21,7,26),fill='#85aca9')])
for kind,col in [('pepper','#d3955d'),('root','#a5baa3')]:save('held-seed-'+kind,(16,18),lambda d,col=col:[d.rectangle((2,2,13,17),fill=INK),d.rectangle((3,3,12,16),fill='#deb782'),d.rectangle((3,4,12,5),fill=CREAM),d.ellipse((5,8,10,13),fill=col)])
for kind in ['fish','wrap','cloud','porridge','broth','lantern']:
    def plate(d,kind=kind):
        d.ellipse((1,9,22,19),fill=INK);d.ellipse((2,9,21,17),fill=CREAM)
        if kind=='fish':
            for x in [5,10,15]:d.rectangle((x,6,x+3,13),fill='#d7a969');d.line((x,8,x+3,10),fill=WOOD)
        elif kind=='cloud':d.rectangle((7,7,17,13),fill='#c8a175');d.ellipse((5,2,19,11),fill='#fff0ca');d.point((13,4),fill='#be7796')
        elif kind=='wrap':d.ellipse((4,5,12,13),fill='#648f60');d.ellipse((12,7,19,14),fill='#96b577')
        else:d.ellipse((4,6,20,15),fill='#72a198');d.ellipse((6,7,18,12),fill='#d4b57e');d.rectangle((9,8,11,10),fill='#96b577')
    save('held-dish-'+kind,(24,20),plate)

def chef(d,direction,kind,frame):
    # Contact frame bends the entire torso, rather than only rotating an accessory.
    bends={'swing':[0,0,2,1,0],'pour':[0,1,2,2,0],'plant':[0,2,5,3,0],'pull':[0,3,5,1,0],'cast':[0,0,1,1,0],'stir':[0,1,2,1,0]}
    bend=bends[kind][frame];lean=0 if direction in ['up','down'] else (-1 if direction=='left' else 1)*(1 if frame==1 else 2 if frame==2 else 0)
    d.ellipse((6,40,26,46),fill='#21333466')
    for x in [9,21]:d.rectangle((x,32,x+5,40),fill=INK);d.rectangle((x,39,x+5,42),fill='#b08a63')
    # Keep feet planted while head and apron move with the action.
    d.rectangle((7+lean,21+bend,26+lean,34),fill=INK);d.rectangle((9+lean,22+bend,24+lean,32),fill='#668f85');d.rectangle((12+lean,23+bend,22+lean,34),fill=CREAM)
    head=Image.new('RGBA',(32,26));h=ImageDraw.Draw(head)
    h.rounded_rectangle((8,8,25,23),radius=4,fill=INK);h.rounded_rectangle((10,10,24,21),radius=3,fill=SKIN);h.rectangle((9,10,23,14),fill='#57433c')
    if direction=='up':h.rectangle((9,13,24,22),fill='#765547');h.line((11,19,21,19),fill='#a07756')
    else:
        for x in ([11] if direction=='left' else [21] if direction=='right' else [12,21]):h.rectangle((x,15,x+1,17),fill=INK)
        h.line((15,19,19,19),fill='#b57866')
    h.rectangle((6,7,27,11),fill=INK);h.rectangle((7,7,26,9),fill=CREAM)
    for box in [(9,3,15,8),(14,0,22,8),(21,3,25,8)]:h.ellipse(box,fill=CREAM)
    h.line((16,3,21,3),fill='#fff4d5')
    d._image.alpha_composite(head,(lean,bend))
    # Raised shoulder, extending arm, then follow-through and recovery.
    if kind in ['swing','cast']:
        arm=[[(26,25),(27,29)],[(25,24),(29,14)],[(25,26),(30,30)],[(25,26),(28,34)],[(26,25),(27,29)]][frame]
    elif kind in ['plant','pull']:
        arm=[[(25,25),(27,30)],[(24,26),(28,34)],[(23,29),(28,38)],[(24,25),(28,26)],[(25,25),(27,30)]][frame]
    else:arm=[(24,25+bend),(29,26+bend+(frame%2)*2)]
    if direction=='left':arm=[(31-x,y) for x,y in arm]
    d.line(arm,fill=INK,width=5);d.line(arm,fill=SKIN,width=3)
    if direction=='down' and kind in ['plant','pull']:d.line([(8,27+bend),(12,35+bend)],fill=SKIN,width=3)
for direction in ['down','up','left','right']:
    for kind in ['swing','pour','plant','pull','cast','stir']:
        for frame in range(5):save(f'action-{kind}-{direction}-{frame}',(32,48),lambda d,direction=direction,kind=kind,frame=frame:chef(d,direction,kind,frame))
save('ui-slot',(16,16),lambda d:[d.rectangle((0,0,15,15),fill='#563731'),d.rectangle((1,1,14,14),fill='#ac774e'),d.rectangle((2,2,13,13),fill='#ebcca0'),d.line((2,2,13,2),fill='#fff0c6'),d.line((2,13,13,13),fill='#b98c5e')])
save('ui-button',(16,16),lambda d:[d.rectangle((0,0,15,15),fill='#502e2a'),d.rectangle((1,1,14,14),fill='#c38c55'),d.rectangle((2,2,13,13),fill='#873e36'),d.line((3,3,12,3),fill='#b56449'),d.line((3,12,12,12),fill='#632d2b')])
print('Cozy furniture, held props and 120 chef action frames created at native pixel size.')
save('held-leafgill',(24,20),lambda d:[d.polygon([(7,9),(1,5),(1,16),(8,13)],fill=INK),d.ellipse((6,3,23,17),fill=INK),d.ellipse((7,4,22,16),fill='#97bc80'),d.arc((9,5,19,16),70,250,fill='#567d59',width=2),d.rectangle((18,7,20,10),fill=CREAM),d.point((20,8),fill=INK)])
save('held-pepperbell',(18,24),lambda d:[d.line((9,6,10,2),fill='#7d9e68',width=2),d.ellipse((3,5,15,22),fill=INK),d.ellipse((4,6,14,21),fill='#d69960'),d.ellipse((6,7,9,14),fill='#f1cb88')])
save('held-lanternroot',(20,22),lambda d:[d.line((10,5,10,1),fill='#73956d',width=2),d.ellipse((3,4,17,18),fill=INK),d.ellipse((4,5,16,17),fill='#b4cdb1'),d.line((10,17,9,21),fill=GOLD,width=2),d.ellipse((7,8,13,14),fill='#f0de92')])
save('held-cloudfruit',(24,20),lambda d:[d.ellipse((2,5,21,17),fill=INK),d.ellipse((3,6,20,16),fill='#eddbab'),d.ellipse((5,1,14,12),fill=CREAM),d.ellipse((12,3,20,12),fill=CREAM),d.line((13,5,17,1),fill='#7b9d6c',width=2)])
save('held-brothback',(20,24),lambda d:[d.rectangle((6,2,13,6),fill=INK),d.rectangle((7,2,12,5),fill=GOLD),d.rounded_rectangle((3,5,16,22),radius=3,fill=INK),d.rectangle((5,7,14,20),fill='#93b5a5'),d.rectangle((5,13,14,20),fill='#b98758'),d.rectangle((7,9,12,13),fill=CREAM)])
save('held-grain',(18,22),lambda d:[d.ellipse((2,7,16,21),fill=INK),d.ellipse((3,8,15,20),fill='#ccaa72'),d.rectangle((6,5,13,8),fill=GOLD),d.line((6,9,13,16),fill='#edcd8d',width=2)])
save('held-wood',(22,16),lambda d:[d.rectangle((4,4,18,14),fill=INK),d.rectangle((5,5,17,12),fill=WOOD),d.ellipse((0,3,8,14),fill=GOLD),d.ellipse((2,5,6,12),outline=WOOD),d.line((9,7,17,7),fill=GOLD)])
save('held-stone',(18,16),lambda d:[d.polygon([(1,11),(5,3),(11,1),(17,9),(14,15),(5,15)],fill=INK),d.polygon([(3,10),(6,4),(11,3),(15,9),(13,13),(5,13)],fill='#94bdb5'),d.line((6,4,12,7),fill=CREAM)])
save('held-fiber',(16,20),lambda d:[d.line([(5,18),(3,7),(8,2),(14,7),(10,18)],fill=INK,width=5),d.line([(5,18),(3,7),(8,2),(14,7),(10,18)],fill='#bbd193',width=3),d.rectangle((4,11,12,14),fill=WOOD)])
