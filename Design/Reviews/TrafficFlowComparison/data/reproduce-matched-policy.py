import json,math,pathlib
root=pathlib.Path(__file__).resolve().parent.parent;project=root.parents[2];D=json.load(open(root/'data/traffic-reference-model.json'));B=json.load(open(project/'Design/FamilyBalance.json'))
def fam_income(i,l):
 n=8+2*(i%8);v=12*1.65**i*(1+.06*l[1]);p=v*sum(s*(1+.12*l[c]) for s,c in zip([.25,.25,.5],[2,5,8]));o=max(.55,1.6/(1+.05*l[0]));c=max(.65,2.4/(1+.06*l[4]));m=max(1.6,3.8/(1+.04*l[6]));w=1.8*(1+.018*l[3]);cycle=n*max(o+1.35/w,c/2,m/6)+36/w+m+1;return p*n*1.15*60/cycle
def family_cost(i,j,l):return max(1,round(B['balance']['upgradeCoins'][j]*1.65**i*1.075**l*(1+.12*l)))
def tf_income(i,ls):
 r=D['routes'][i];b=D['bodies'][r['body']];p=r['premium_seats'];e=r['economy_seats'];n=p+e;v=[.25*(1+.045*ls[0]),1-1/(1+.0222*ls[1]),1+.2*ls[2],9/(1+ls[3]/24),3.5/(1+.15*ls[4]),1+.2*ls[5],3/(1+.05*ls[6]),1-1/(1+.0222*ls[7]),1+.2*ls[8]];check=n*(1-v[1])/v[0]*b['check_mul'];stow=(.28*n+n*v[6]*(1-v[7]))/b['segments'];door=n*v[4]*b['door_mul'];cycle=(v[3]*b['walk_mul']+max(check,stow,door)+4+.5*(min(5,max(1,r['economy_rows']))+(1 if p else 0)-1))*b['prediction_mul'];pay=r['fare']*(60*p+10*e)*(.3*v[2]+.3*v[5]+.4*v[8])*1.1;return pay*60/cycle
def tf_cost(i,j,l,v):
 r=D['routes'][i];base=[45,45,90,40,50,90,55,55,120];dub=[50,50,100,40,50,80,40,40,100];stage=[r['stage_gate'],r['stage_transit'],r['stage_seating']];return round((dub[j]*(.5 if v=='New' else 1) if i==0 else base[j]*r['cost_scale_new' if v=='New' else 'cost_scale_current']*stage[j//3])*(1+.4*l)*1.1**l)
def sim(family,variant='Current'):
 cash=0;mins=0;ips=[];events=[];purchases=0
 for i,r in enumerate(D['routes']):
  total=sum(ips)
  if not family and i and i%8==0:
   cert=next(c for c in D['research'] if c['vehicle_tag'] and c['vehicle_tag'].lower()==r['mode'])
   if total<cert['income_need']:return {'blocked':r['key'],'needed':cert['income_need'],'income':total}
   dt=max(0,(cert['base_cost']-cash)/total);mins+=dt;cash+=dt*total-cert['base_cost'];dt=cert['duration_parameter_minutes'];mins+=dt;cash+=dt*total
  price=0 if i==0 else round(200*1.9**(i-1)) if family else r['unlock_cost']
  if i:dt=max(0,(price-cash)/sum(ips));mins+=dt;cash+=dt*sum(ips)-price
  events.append({'outlet':i+1,'event':'open','minutes':mins});ls=[0]*9;ips.append(fam_income(i,ls) if family else tf_income(i,ls))
  for _ in range(270):
   opts=[]
   for j in range(9):
    if ls[j]>=30:continue
    c=family_cost(i,j,ls[j]) if family else tf_cost(i,j,ls[j],variant);new=ls.copy();new[j]+=1;ni=fam_income(i,new) if family else tf_income(i,new);gain=ni-ips[-1];opts.append((c/max(gain,1e-30),c,j,ni))
   _,c,j,ni=min(opts);dt=max(0,(c-cash)/sum(ips));mins+=dt;cash+=dt*sum(ips)-c;ls[j]+=1;ips[-1]=ni;purchases+=1
  events.append({'outlet':i+1,'event':'max_all_local','minutes':mins})
 return dict(hours=mins/60,purchases=purchases,events=events)
result={'scope':'Matched model policy: zero initial cash, normal service sizes, max all 270 local upgrades before advancing, choose cost/incremental-network-income, aggregate forecast income, no managers/general research/stars/offline/ads/grants; Traffic Flow certificate gates/timers retained. These are model calculations, not observed playtime. Original TF throughput proxy omits some seating/premium/body effects. Family forecast uses steady post-first-service size (8 guests at outlet 0), omits tips as its own HUD estimator does. Onboarding gift/timing excluded for matching.','scenarios':{'Traffic Flow Current':sim(False),'Traffic Flow New':sim(False,'New'),'Restaurant Family':sim(True)},'family_formula_validation':[]}
for i in [0,7,8,15,16,23,24,31,32,39]:
 for l in [0,30]:
  expected=B['outlets'][i]['baseEstimatedCoinsPerMinute' if l==0 else 'maxEstimatedCoinsPerMinute'];actual=fam_income(i,[l]*9);assert abs(actual-expected)/max(1,expected)<1e-6;result['family_formula_validation'].append({'outlet':i+1,'level':l,'relative_difference':actual/expected-1})
for variant in ['Current','New']:
 exp=D['simulations'][variant+' x1'][-1]['hours'];assert abs(result['scenarios']['Traffic Flow '+variant]['hours']-exp)<1e-7
(root/'data/matched-policy-model.json').write_text(json.dumps(result,indent=2));print(result['scope']);print([(k,v['hours'],v['purchases']) for k,v in result['scenarios'].items()])
