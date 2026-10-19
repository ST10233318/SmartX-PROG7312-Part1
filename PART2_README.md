# Smart-X Part 2 — Real-Time Processing and Device Management

This project extends the supplied Part 1 solution, preserving existing API endpoints, MVC pages and tests.

## Requirements
- Visual Studio 2026 or .NET 10 SDK
- Start **SmartX.Api** and **SmartX.Client** simultaneously (Visual Studio: Configure Startup Projects > Multiple startup projects).
- API expected at `http://localhost:5259/` by the MVC client. If the API runs on a different port, update `SmartX.Client/Program.cs`.
- Visit the MVC site and select **Real-Time Command Stream & History**.

## Try the Part 2 features
1. Register a sensor through **Register Sensor** and note its DeviceId.
2. Open the real-time page and enter the registered DeviceId in **Manual Override**.
3. Select **PUMP_ON** and send. Send **PUMP_OFF** next, then **Undo Last Command**; confirm the device state and history change.
4. Under **Simulate Sensor**, set category `moisture`, value `12`, check **Critical alert**, then send. This POSTs to the API, enters the priority queue, and marks the device in the active-alert set.
5. Send a noncritical reading with category `temperature`, value `24`, critical unchecked; this enters the FIFO queue.
6. Press **Process Next Queued Packet**: priority items are processed before normal FIFO items. Repeat to empty the queues.
7. Search by device ID, filter by category, or select **Alerts only**. The timestamp-sorted timeline and command history refresh every 3 seconds.
8. Issue the same command twice or search the same device three times; **Automated Insights** suggests frequently used actions and searches. Critical alerts also generate recommendations.

## Data structures and rubric mapping
- `Queue<SensorReading>`: FIFO telemetry processing.
- `PriorityQueue<SensorReading,int>`: critical packets processed before normal packets.
- `Stack<DeviceCommand>`: undo history; undo restores the previous stored command state (simulation only).
- `Dictionary<string,...>`: state, command and interaction lookup; existing `ConcurrentDictionary<string,Sensor>` stores registered devices.
- `SortedDictionary<DateTime,SensorReading>`: chronological telemetry history, capped at 500 entries.
- `HashSet<string>`: deduplicated active device alerts.
- Rule-based predictive engine: recurring searches/commands and active alerts.

## API endpoints
- `GET /api/realtime/state?category=&alertsOnly=false&search=`
- `POST /api/realtime/simulate` with `{ "deviceId":"YOUR_ID", "category":"moisture", "value":12, "critical":true }`
- `POST /api/realtime/commands` with `{ "deviceId":"YOUR_ID", "action":"PUMP_ON" }`
- `POST /api/realtime/undo`
- `POST /api/realtime/process-next`
- `DELETE /api/realtime/alerts/{deviceId}`

## Limitations
In-memory single-process demonstration: state resets when API restarts. Simulated commands do not control physical devices. This project was packaged in a Linux environment without the .NET SDK; compilation and end-to-end runtime testing must be performed in Visual Studio on Windows before submission. Network topology is a navigation placeholder for the final PoE.

## Submission checklist
Run `dotnet restore`, `dotnet build`, and `dotnet test` on Windows. Commit changes incrementally to GitHub; the assignment expects over 20 commits total. Verify Docker build if required. Record the requested demonstration after successful testing. Do not claim unperformed tests as passed.
