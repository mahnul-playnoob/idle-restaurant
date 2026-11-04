from pathlib import Path
import numpy as np,wave,json
sr=44100;seconds=15;n=sr*seconds;rng=np.random.default_rng(51);mix=np.zeros((n,2),np.float64);events=[]
def add(t,sound,amp=.1,pan=0,kind=None):
 p=int(t*sr);s=np.asarray(sound)*amp;s=s[:max(0,n-p)]
 if len(s)==0:return
 mix[p:p+len(s),0]+=s*np.sqrt((1-pan)/2);mix[p:p+len(s),1]+=s*np.sqrt((1+pan)/2)
 if kind:events.append({'time_seconds':round(t,3),'sound':kind,'pan':pan})
def ping(f=1000,d=.3):
 t=np.arange(int(sr*d))/sr;return (np.sin(2*np.pi*f*t)*np.exp(-t*13)+.3*np.sin(2*np.pi*f*2.71*t)*np.exp(-t*22))*np.minimum(1,t/.003)
def pluck(f,d=1.3):
 t=np.arange(int(sr*d))/sr;return (np.sin(2*np.pi*f*t)+.22*np.sin(2*np.pi*f*2*t)+.10*np.sin(2*np.pi*f*3*t))*np.exp(-t*4)*np.minimum(1,t/.008)
def chop():
 t=np.arange(int(.11*sr))/sr;noise=rng.normal(0,1,len(t));soft=np.convolve(noise,np.ones(18)/18,'same');return (.45*soft+.45*np.sin(2*np.pi*185*t))*np.exp(-t*48)*np.minimum(1,t/.002)
def foot():
 t=np.arange(int(.13*sr))/sr;noise=np.convolve(rng.normal(0,1,len(t)),np.ones(45)/45,'same');return (.35*np.sin(2*np.pi*95*t)+noise)*np.exp(-t*32)*np.minimum(1,t/.004)
def plate():
 t=np.arange(int(.55*sr))/sr;return sum(a*np.sin(2*np.pi*f*t)*np.exp(-t*d) for f,a,d in [(1250,1,13),(2327,.35,20),(3513,.18,26)])*np.minimum(1,t/.002)
# Original gentle plucked accompaniment. No borrowed melody or recording.
for bar,chord in enumerate([[130.81,329.63,392,493.88],[174.61,349.23,440,523.25],[110,261.63,329.63,392],[196,293.66,392,493.88]]):
 for beat in range(8):
  t=bar*3.75+beat*.46875
  if t<14.5:add(t,pluck(chord[beat%4]),.035,-.18 if beat%2==0 else .18)
# Continuous low grill bed and individual tiny sizzling pops.
t=np.arange(n)/sr;noise=rng.normal(0,1,n);hiss=noise-np.convolve(noise,np.ones(20)/20,'same');hiss=np.convolve(hiss,np.ones(3)/3,'same');env=np.minimum(1,t/.8)*np.minimum(1,(15-t)/.8)
add(0,hiss*env*(.65+.2*np.sin(t*7)),.012,-.4)
for t in [2.02,2.38,2.7,3.04,3.4,4.05,4.42,5.01,5.38,6.05,8.8,9.1,9.42,10.02,10.35,11.08]:add(t,chop(),.095,-.25,'Chop and utensil')
for a,b,step,pan in [(0,1.9,.38,-.55),(3.3,5.8,.36,-.3),(4.5,6.8,.36,-.1),(7.7,9.8,.34,.0),(8.5,10.5,.28,.25),(11.2,12.6,.28,.35),(12.5,14.8,.4,-.3)]:
 for t in np.arange(a,b,step):add(float(t),foot(),.08,pan,'Footstep')
for t in [1.8,4.5]:add(t,ping(1050,.35),.10,-.45,'Order accepted')
for t,pan in [(2.9,-.35),(5.0,.15),(7.15,-.2),(10.8,.25)]:add(t,plate(),.105,pan,'Porcelain plate')
for t in [7.35,10.98,13.0]:
 for k,f in enumerate([1174.66,1567.98,1975.53]):add(t+k*.055,ping(f,.4),.075,.25,'Coin collection')
add(8.5,ping(650,.12),.08,0,'Kitchen tab tap');add(9.2,ping(850,.12),.1,0,'Upgrade tap')
for i,f in enumerate([523.25,659.25,783.99,1046.5]):add(9.25+i*.075,pluck(f,.7),.11,0,'Upgrade flourish')
for i,f in enumerate([659.25,783.99,987.77,1318.51]):add(12.0+i*.085,ping(f,.6),.11,.25,'Recipe reveal')
# Fade the beginning and ending, retain natural silence-free ambience.
mix*=np.minimum(1,np.arange(n)/(sr*.12))[:,None];mix*=np.minimum(1,(n-1-np.arange(n))/(sr*.35))[:,None]
peak=float(np.max(np.abs(mix)));mix*=.84/max(peak,1e-9);peak=float(np.max(np.abs(mix)));rms=float(np.sqrt(np.mean(mix*mix)))
root=Path(__file__).resolve().parent.parent
with wave.open(str(root/'Assets/Restaurant/Resources/RestaurantSoundtrack.wav'),'wb') as w:w.setnchannels(2);w.setsampwidth(2);w.setframerate(sr);w.writeframes((np.clip(mix,-1,1)*32767).astype('<i2').tobytes())
(root/'Media/SoundDesign.json').write_text(json.dumps({'duration_seconds':15,'sample_rate':sr,'channels':2,'peak_dbfs':20*np.log10(peak),'rms_dbfs':20*np.log10(rms),'provenance':'Original procedurally synthesized SFX and musical accompaniment','events':events},indent=2)+'\n')
print('Audio ready:',15,'seconds; peak',round(20*np.log10(peak),1),'dBFS; events',len(events))
