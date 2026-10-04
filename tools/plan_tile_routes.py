"""Replan explicit landmark trails around authored solid footprints, preserving saves."""
from pathlib import Path
import json, heapq, math
from PIL import Image, ImageDraw
ROOT=Path(__file__).resolve().parents[1];p=ROOT/'Game/Assets/Wildfeast/Resources/Archipelago.json'
data=json.loads(p.read_text(encoding='utf-8-sig'))
for i in data['islands']:
 w,h=int(i['size']['x']),int(i['size']['y']);xmin,ymin=-w//2,-h//2
 def cell(n):return n%w+xmin,n//w+ymin
 def idx(p):return (p[1]-ymin)*w+p[0]-xmin
 land={cell(n) for n in i['gridLand']};obstacles=set()
 dock=i['dock'];arrival=(i['arrival']['x'],i['arrival']['y']);dx=round(dock['x']);dy=math.floor(dock['y']+.5)+1
 deck={(x,y) for x in [dx,dx+(1 if arrival[0]>=dx else -1)] for y in range(dy,arrival[1]+2)}
 land|=deck;i['gridDeck']=sorted(idx(p) for p in deck);i['gridLand']=sorted(idx(p) for p in land)
 for prop in i['props']:
  if prop['role']!='grass':
   px,py=prop['position']['x'],prop['position']['y'];obstacles.add((px,py))
   if prop['role']=='decor':obstacles|={(px-1,py),(px+1,py)}
 for pt in i['points']:
  if pt['action'] in ('home','shop') and pt.get('art'):
   px,py=round(pt['position']['x']),round(pt['position']['y']);obstacles|={(px+dx,py+dy) for dx in range(-2,3) for dy in range(1,4)}
  if pt['action'] in ('upgrades','discovery','tap') or pt.get('kind') in ('ram','crab','snail'):obstacles.add((pt['position']['x'],pt['position']['y']))
 if i['id']==0:
  obstacles|={(x,y) for x in range(-8,-3) for y in range(0,3)}
  obstacles|={(1,1),(1,2),(2,1),(2,2)}
 if i['id']==1:obstacles|={(x,y) for x in range(2,5) for y in range(3,6)}
 safe={p for p in land if p in deck or all((p[0]+x,p[1]+y) in land for x,y in [(0,1),(1,0),(0,-1),(-1,0)])}-obstacles
 def nearest(p):return min(safe,key=lambda a:(a[0]-p[0])**2+(a[1]-p[1])**2)
 anchors=[arrival,nearest((dx,dy))]
 anchors += [nearest((pt['position']['x'],pt['position']['y']-1)) for pt in i['points'] if pt['action'] in ('enter','upgrades','story','discovery','forage','fruit','passage','home','shop','talk')]
 anchors += [nearest((r['center']['x'],r['center']['y'])) for r in i['regions']]
 if i['id']==0:anchors += [nearest((-6,-2)),nearest((1,0))]
 if i['id']==1:anchors += [nearest((3,2))]
 # Keep new doorway/passage cells on dry reachable ground.
 for pt in i['points']:
  if pt['action'] in ('passage','talk'):
   q=nearest((pt['position']['x'],pt['position']['y']));pt['position']={'x':q[0],'y':q[1]};anchors.append(q)
 anchors=list(dict.fromkeys(anchors));roads=set(deck);connected=[arrival];routes=[]
 for goal in anchors[1:]:
  start=min(connected,key=lambda p:abs(p[0]-goal[0])+abs(p[1]-goal[1]));front=[(0,0,start)];cost={start:0};parents={start:None}
  while front:
   _,g,p=heapq.heappop(front)
   if p==goal:break
   if g>cost[p]:continue
   for dx,dy in [(1,0),(0,1),(-1,0),(0,-1)]:
    q=p[0]+dx,p[1]+dy
    if q not in safe:continue
    ng=g+(0.7 if q in roads else 1)+(.18 if parents[p] and (dx,dy)!=(p[0]-parents[p][0],p[1]-parents[p][1]) else 0)
    if ng<cost.get(q,1e9):cost[q]=ng;parents[q]=p;heapq.heappush(front,(ng+.6*(abs(q[0]-goal[0])+abs(q[1]-goal[1])),ng,q))
  if goal not in parents:raise RuntimeError((i['key'],goal))
  path=[];p=goal
  while p is not None:path.append(p);p=parents[p]
  path.reverse();roads.update(path);connected+=path;routes.append({'points':[{'x':x,'y':y} for x,y in path],'width':.45})
 if i['id']==0:roads|={q for x,y in list(roads) if -13<x<4 and -7<y<0 for q in [(x+1,y),(x,y+1)] if q in safe}
 i['roads']=routes;i['gridRoad']=sorted(idx(p) for p in roads)
ROOT.joinpath('Game/Assets/Wildfeast/Resources/Archipelago.json').write_bytes((json.dumps(data,indent=2)+'\n').encode('utf-8'))
for mask in range(16):
 im=Image.new('RGBA',(32,32),'#a87a46');d=ImageDraw.Draw(im)
 for y in range(0,32,8):
  d.line((0,y,31,y),fill='#63482e');d.line((0,y+1,31,y+1),fill='#deb878');d.line((5,y+4,22,y+4),fill='#c29458')
  for x in [3,28]:d.point((x,y+3),fill='#65513b')
 im.save(ROOT/f'Game/Assets/Wildfeast/Resources/Art/tile-deck-{mask}.png')
print('District route networks connect docks and landmarks, avoiding solid footprints.')
