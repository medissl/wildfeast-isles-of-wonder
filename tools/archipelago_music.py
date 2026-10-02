"""Three original seamless island themes, synthesized without audio samples."""
from pathlib import Path
import wave
import numpy as np
OUT=Path(__file__).resolve().parents[1]/'Game/Assets/Wildfeast/Resources/Audio';RATE=22050
for key,root,bpm,melody in [('emberfold',50,108,[0,3,7,10,7,3,5,0]),('moonfen',57,78,[12,7,3,0,5,7,10,7]),('pearltide',60,94,[0,4,7,9,12,9,7,4])]:
 beat=60/bpm;length=64*beat;count=int(RATE*length);song=np.zeros((count,2))
 def note(midi,start,duration,amp,timbre,pan=.5):
  t=np.arange(int(duration*RATE))/RATE;hz=440*2**((midi-69)/12);envelope=np.minimum(1,t/.02)*np.minimum(1,(duration-t)/.09)
  tone=np.sin(2*np.pi*hz*t)+.18*np.sin(2*np.pi*2*hz*t)+.07*np.sin(2*np.pi*3*hz*t)
  if timbre!='pad':tone*=np.exp(-t*(2.8 if timbre=='bell' else 4))
  tone*=envelope*amp;idx=(int(start*RATE)+np.arange(len(t)))%count;song[idx,0]+=tone*(1-pan);song[idx,1]+=tone*pan
 for bar in range(16):
  chord=[0,5,7,0,3,5,7,0][bar%8];start=bar*4*beat
  for degree in [0,3 if key!='pearltide' else 4,7]:note(root+chord+degree,start,3.95*beat,.028,'pad')
  for b in range(4):
   note(root-12+chord,start+b*beat,.95*beat,.075,'pluck');note(root+12+melody[(bar*4+b)%len(melody)],start+b*beat,.9*beat,.065,'bell',.3 if b%2 else .7)
  for b in range(8):note(root+chord+[0,7,12,7][b%4],start+b*beat/2,.45*beat,.024,'pluck',.7 if b%2 else .3)
 pcm=(np.tanh(song*1.3)*32767).astype('<i2')
 with wave.open(str(OUT/('music-'+key+'.wav')),'wb') as out:out.setnchannels(2);out.setsampwidth(2);out.setframerate(RATE);out.writeframes(pcm.tobytes())
 print(key,round(length,1),'seconds of original loop')
