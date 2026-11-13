# Smart-X — PROG7312 Part 3

## Overview
ASP.NET Core solution integrating sensor telemetry (Part 1), simulated command history (Part 2), and mesh-network analysis (Part 3). The topology uses an undirected weighted graph, BFS for reachability, Prim's MST for minimum total edge cost, and an AVL tree for timestamp range queries.

> **Verification:** This source package has not been compiled in the preparation environment. Run the commands below and resolve any environment-specific failures before submission.

## Requirements
- Windows 10/11 or compatible OS
- Visual Studio 2026 with ASP.NET and web development workload, or .NET 10 SDK
- Internet connection for first NuGet package restore

## Setup
1. Extract the ZIP into a new folder.
2. Open `SmartX.slnx` in Visual Studio.
3. In Developer PowerShell, run:

```powershell
dotnet restore SmartX.slnx
dotnet build SmartX.slnx
dotnet test SmartX.slnx
```

4. Run the API in one terminal:

```powershell
dotnet run --project SmartX.Api --launch-profile http
```

5. Run the MVC client in another terminal:

```powershell
dotnet run --project SmartX.Client
```

6. Open the client URL printed in the second terminal. The API HTTP profile listens on `http://localhost:5277/`; the client uses this URL by default in `SmartX.Client/appsettings.json`. If you change the API port, update `SmartXApi:BaseUrl`.

## API endpoints
| Method | Route | Purpose |
|---|---|---|
| GET | `/api/sensors` | List sensors |
| POST | `/api/sensors/register` | Register sensor |
| POST | `/api/telemetry/temperature` | Ingest temperature |
| POST | `/api/telemetry/power` | Ingest power |
| POST | `/api/telemetry/switch` | Ingest switch state |
| GET | `/api/telemetry` | List readings |
| GET | `/api/topology` | Nodes, edges, BFS, MST, disconnected nodes |
| POST | `/api/topology/nodes/{id}` | Add topology node |
| POST | `/api/topology/connections` | Add weighted undirected connection |
| GET | `/api/topology/route/{deviceId}` | Trace gateway-to-device route on MST |
| GET | `/api/history?from=...&to=...&deviceId=...` | AVL timestamp query |
| POST | `/api/commands` | Queue simulated command |
| GET | `/api/commands` | Command history |

## Demonstration walkthrough
1. Open dashboard and register a sensor using the existing registration page.
2. Open Network Topology. Add a connection `Gateway` → sensor ID with weight `2`.
3. Add more nodes and weighted connections, including an alternative path, and refresh topology.
4. Trace the MST route from gateway to the sensor.
5. Post telemetry for the registered sensor through Swagger and query the date range on the topology page.
6. Open Commands, enter a registered sensor ID, queue an action, and inspect command history.
7. Run automated tests and capture the results.

## Algorithm notes
- **Graph:** `Dictionary<string, Dictionary<string,double>>` adjacency list; bidirectional weighted links.
- **BFS:** queue plus visited set, O(V+E) for reachable component.
- **Prim's MST:** priority queue, O(E log E) for the gateway-reachable component; unreachable nodes reported separately. MST minimizes total selected edge weight; it does **not** necessarily minimize each sensor's shortest path or actual radio power usage.
- **AVL tree:** balanced binary search tree keyed by UTC timestamps, with duplicate timestamps grouped in a list. Insert O(log n); range query O(log n + k) on balanced data, excluding any device-filter scan within timestamp matches.

## Current limitations
- In-memory state is lost on restart. No persistent database or physical ESP32 transport is included.
- Command stream is a **simulation**: queued commands do not reach hardware and are not acknowledged.
- Network weights are user-provided illustrative costs, not measured battery consumption or signal quality.
- Device online status is based on existing registration/ingestion flags, not active network probing.
- The topology graph starts with `Gateway`; register devices and add links to populate it.
- For production, add authentication, authorization, input throttling, audit logging, persistent storage, and real device connectivity.

## Docker
The inherited API Dockerfile is retained. Docker is optional. Check its .NET SDK/runtime tags against .NET 10 before building a container. The client is not containerized in this package.

## GitHub
After verifying the project, use GitHub Desktop or git to commit your own changes. Do not claim earlier commits that do not exist. Recommended separate commits: graph, MST, AVL, API endpoints, UI, tests, documentation. Push to your own course repository and confirm the required total commit count.

## Submission checklist
- [ ] Build succeeds on your PC
- [ ] Tests pass on your PC
- [ ] API and client both launch
- [ ] Register sensor and ingest telemetry
- [ ] Topology, MST routing, historical queries, command history demonstrated
- [ ] Add genuine lecturer feedback and actual responses
- [ ] Export `docs/Part3_Report.md` to PDF
- [ ] Record and upload a YouTube demonstration
- [ ] Push source to GitHub and confirm commit count
