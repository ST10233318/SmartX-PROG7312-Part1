(() => {
 const $ = id => document.getElementById(id);
 const esc = v => String(v ?? '').replace(/[&<>"']/g, c => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
 const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
 const message = txt => $('feedback').textContent = txt;
 async function request(path, data) {
  const res = await fetch('/Realtime/' + path, { method:'POST', headers:{'Content-Type':'application/json','RequestVerificationToken':token}, body: data ? JSON.stringify(data) : null });
  const result = await res.json().catch(() => ({}));
  if (!res.ok) throw Error(result.error || 'Request failed ('+res.status+')');
  return result;
 }
 async function refresh() {
  const query = new URLSearchParams({search:$('search').value,category:$('category').value,alertsOnly:$('alertsOnly').checked});
  try {
   const res = await fetch('/Realtime/State?' + query);
   if (!res.ok) throw Error('API unavailable');
   const data = await res.json();
   $('connection').textContent = 'Connected · auto refresh 3s';
   $('deviceCount').textContent = data.devices.length; $('alertCount').textContent = data.alerts.length;
   $('fifoCount').textContent = data.normalPending; $('priorityCount').textContent = data.urgentPending;
   $('devices').innerHTML = data.devices.map(d => `<tr><td>${esc(d.deviceId)}</td><td>${esc(d.category)}</td><td>${esc(d.state)}</td><td>${d.alert ? '⚠ Alert' : 'Normal'}</td></tr>`).join('') || '<tr><td colspan="4">No matching devices</td></tr>';
   $('suggestions').innerHTML = data.suggestions.map(s=>`<li>${esc(s)}</li>`).join('');
   $('alerts').textContent = data.alerts.join(', ') || 'No active alerts';
   $('commands').innerHTML = data.commands.map(c=>`<div class="history-row"><b>${esc(c.deviceId)}</b> ${esc(c.action)} ${c.isUndo?'(undo)':''}<small>${esc(c.timestamp)}</small></div>`).join('') || 'No commands yet';
   $('timeline').innerHTML = data.timeline.map(t=>`<tr><td>${esc(t.timestamp)}</td><td>${esc(t.deviceId)}</td><td>${esc(t.category)}</td><td>${esc(t.displayValue)}</td><td>${t.isAnomaly?'⚠ Alert':'Normal'}</td></tr>`).join('') || '<tr><td colspan="5">No telemetry yet</td></tr>';
  } catch (e) { $('connection').textContent = 'Disconnected'; message(e.message); }
 }
 $('commandForm').addEventListener('submit',async e=>{e.preventDefault();try {await request('Command',{deviceId:$('deviceId').value,action:$('action').value});message('Command recorded successfully.');refresh();}catch(err){message(err.message)}});
 $('undo').onclick=async()=>{try{await request('Undo');message('Last command reverted.');refresh()}catch(err){message(err.message)}};
 $('simulateForm').addEventListener('submit',async e=>{e.preventDefault();try{await request('Simulate',{deviceId:$('deviceId').value,category:$('simCategory').value,value:Number($('simValue').value),critical:$('critical').checked});message('Telemetry accepted by API.');refresh()}catch(err){message(err.message)}});
 $('process').onclick=async()=>{try{const r=await request('ProcessNext');message('Processed: '+r.queue);refresh()}catch(err){message(err.message)}};
 for(const id of ['search','category','alertsOnly']) $(id).addEventListener(id==='alertsOnly'?'change':'input',refresh);
 refresh();setInterval(refresh,3000);
})();
