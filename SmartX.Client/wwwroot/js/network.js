const $ = id => document.getElementById(id);
const svgNS = 'http://www.w3.org/2000/svg';
function element(tag, attrs, parent) { const n = document.createElementNS(svgNS,tag); Object.entries(attrs).forEach(([k,v])=>n.setAttribute(k,v)); parent.appendChild(n); return n; }
async function request(url, options) { const r=await fetch(url,options); const t=await r.text(); if(!r.ok) throw new Error(t); return JSON.parse(t); }
async function refresh() {
 try {
  const d=await request('/proxy/topology');
  $('nodesCount').textContent=d.nodes.length; $('onlineCount').textContent=d.connected.length;
  $('mstCost').textContent=d.mst.totalWeight.toFixed(2);
  $('offline').textContent=d.offline.length ? d.offline.join(', ') : 'None';
  const svg=$('networkSvg'); svg.replaceChildren(); const positions={};
  d.nodes.forEach((id,i)=>{const angle=2*Math.PI*i/Math.max(d.nodes.length,1);positions[id]={x:400+285*Math.cos(angle),y:210+145*Math.sin(angle)};});
  const selected=new Set(d.mst.edges.map(e=>[e.from,e.to].sort().join('|')));
  d.edges.forEach(e=>{const a=positions[e.from],b=positions[e.to]; if(!a||!b)return;const mst=selected.has([e.from,e.to].sort().join('|'));element('line',{x1:a.x,y1:a.y,x2:b.x,y2:b.y,stroke:mst?'#44d9a6':'#3e7dba','stroke-width':mst?4:2},svg);const t=element('text',{x:(a.x+b.x)/2,y:(a.y+b.y)/2,fill:'#fff','font-size':13},svg);t.textContent=e.weight;});
  d.nodes.forEach(id=>{const p=positions[id];element('circle',{cx:p.x,cy:p.y,r:22,fill:d.connected.includes(id)?'#38bdf8':'#64748b'},svg);const t=element('text',{x:p.x,y:p.y+39,'text-anchor':'middle',fill:'#fff','font-size':14},svg);t.textContent=id;});
 }catch(e){$('offline').textContent='Unable to load topology: '+e.message;}
}
$('connectForm').addEventListener('submit',async e=>{e.preventDefault();try{await request('/proxy/connections',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({from:$('from').value,to:$('to').value,weight:Number($('weight').value)})});$('connectionMessage').textContent='Connection added.';await refresh();}catch(err){$('connectionMessage').textContent=err.message;}});
$('traceBtn').addEventListener('click',async()=>{try{const d=await request('/proxy/route/'+encodeURIComponent($('destination').value));$('routeResult').textContent=d.path.length?d.path.join(' → '):'No route from gateway.';}catch(e){$('routeResult').textContent=e.message;}});
$('historyForm').addEventListener('submit',async e=>{e.preventDefault();try{const f=new Date($('fromDate').value).toISOString(),t=new Date($('toDate').value).toISOString();const q=new URLSearchParams({from:f,to:t,deviceId:$('device').value});const data=await request('/proxy/history?'+q);$('historyRows').replaceChildren();data.forEach(r=>{const tr=document.createElement('tr');[r.timestamp,r.deviceId,r.displayValue,r.isAnomaly?'Yes':'No'].forEach(v=>{const td=document.createElement('td');td.textContent=v;tr.appendChild(td);});$('historyRows').appendChild(tr);});$('historyMessage').textContent=data.length+' readings found.';}catch(err){$('historyMessage').textContent=err.message;}});
refresh();
