# PROG7312 — Smart-X Part 3 Implementation and Completion Report

**Student:** Frida Banda  
**Student number:** ST10233318  
**Project:** Smart-X Network Topology and Mesh Routing  
**Status:** Draft report accompanying source package; replace verification placeholders after running locally.

## 1. Introduction
Smart-X is an IoT monitoring prototype developed across three phases. Part 1 handles sensor registration and telemetry ingestion. The final application extends the existing API and MVC frontend with a simulated command history and a mesh-network topology page. The Part 3 objective is to model device interconnections, discover gateway-reachable sensors, calculate a minimum-weight spanning structure, and query historical readings by timestamp efficiently.

## 2. Architecture and integration
The solution has three projects: `SmartX.Api` (ASP.NET Core Web API), `SmartX.Client` (ASP.NET Core MVC and browser UI), and `SmartX.Tests` (xUnit tests). The browser calls same-origin MVC proxy routes; the proxy forwards to the configured backend API. In-memory singleton services manage topology and simulated commands. The telemetry processor indexes ingested readings into an AVL tree while retaining the original concurrent reading queue. Sensor registration adds an isolated node to the topology; a link must be explicitly configured to make the node reachable.

## 3. Implemented data structures
### 3.1 Graph adjacency list
`NetworkGraph.cs` uses a case-insensitive dictionary of neighbour dictionaries. Each vertex identifies a gateway or sensor by a device identifier (including a MAC address if supplied). Each undirected edge has a finite, strictly positive weight. `AddConnection` adds both directions and rejects self-loops. This is appropriate for sparse mesh topologies and permits neighbour lookup without scanning an entire edge table.

### 3.2 Breadth-First Search (BFS)
`BreadthFirstSearch(startNode)` maintains a queue and visited set. It enumerates the connected component of the chosen gateway in O(V+E) time. Comparing the returned set with all nodes identifies devices not connected to that gateway. A disconnected device may still be powered on; graph reachability and physical device health are distinct.

### 3.3 Minimum Spanning Tree (Prim's algorithm)
`MinimumSpanningTree(startNode)` begins with the gateway, inserts incident edges into `PriorityQueue<MstEdge,double>`, and repeatedly selects the cheapest edge leading to an unvisited node. The output includes selected edges, total cost, and unreachable nodes. The example graph Gateway–A (2), Gateway–B (5), A–B (1), B–C (3), A–C (7) produces total weight **6** through Gateway–A, A–B and B–C. `GetMstRoute` follows the resulting tree to show a route to an individual node. A minimum spanning tree minimizes total link cost, not individual shortest-path latency. Measured radio power efficiency would require appropriate real-world edge metrics.

### 3.4 AVL tree
`TelemetryAvlTree.cs` stores timestamped `SensorReading` objects in a height-balanced binary search tree. Insertions perform left or right rotations when subtree heights differ by more than one. Equal timestamps share a list, avoiding dropped duplicate readings. The `Query(from,to,deviceId)` method prunes subtrees outside the requested UTC time interval and optionally filters by device identifier. Compared with a full linear scan, an AVL tree provides logarithmic navigation to the requested range and visits matching timestamp nodes. The implementation uses a shared lock for synchronization.

## 4. UI and user workflow
The dashboard links to sensor registration, command history and network topology. The topology page shows a live SVG graph, selected MST edges, connected and disconnected nodes, total tree weight, route tracing, and a historical telemetry table. Users can add a weighted connection through a form. The command page permits queuing predefined actions for registered sensors and viewing a timestamped command history. These are simulated application actions, not real ESP32 control.

## 5. Implementation challenges and solutions
**Existing functionality:** The inherited frontend contained disabled navigation cards. They were replaced with working links while retaining existing dashboard content. **Algorithm placement:** Previous `MinimumSpanningTree` calls in tests failed when the method was missing from the compiled `NetworkGraph` class. The revised graph implementation places the method directly in that class and supplies corresponding tests. **Timestamp indexing:** Existing ingestion used a concurrent queue. The revised processor additionally inserts readings into the synchronized AVL index so that the history API can query them by time. **Cross-origin access:** The MVC client uses same-origin proxy routes, avoiding direct browser cross-origin requests to the API.

## 6. Testing and validation
Automated test files cover BFS reachability, MST total cost, disconnected nodes, invalid weights, gateway validation, single-node MST, AVL range filtering, duplicate timestamps and invalid ranges. Existing Part 1 tests are preserved. **Local test results must be inserted after running `dotnet build SmartX.slnx` and `dotnet test SmartX.slnx` on a machine with .NET 10.** The package creation environment did not provide a .NET SDK, so this document does not assert a successful build or test run.

**Build result:** [Insert actual output]  
**Test result:** [Insert actual output]  
**UI demonstration evidence:** [Insert screenshots and video link]

## 7. Technology recommendations (10 marks)
**GitHub Actions CI/CD:** Run `dotnet restore`, `dotnet build` and `dotnet test` on each pull request to detect regressions. This fits .NET CLI tooling and provides repeatable evidence. **Database persistence:** Adopt EF Core with SQLite for local demonstrations or PostgreSQL for multi-user deployments; persist sensors, commands, telemetry and network edges. **SignalR:** Broadcast sensor updates, route changes and command status to browser clients without manual refresh. **MQTT broker:** Integrate a broker such as Mosquitto for real device telemetry and commands, with acknowledgements and retry logic. **OpenTelemetry:** Instrument API latency, errors and ingestion rates; export to an observability backend. **Home Assistant integration:** For supported home-automation devices, expose or consume entities through its APIs. **LLM-generated daily summaries:** Optionally summarize validated telemetry alerts, with explicit privacy controls and human verification. These are proposed enhancements, **not features claimed as implemented**.

## 8. Project completion and reflection
The project demonstrates how separate data structures solve different operational problems: queues support telemetry ingestion, graphs represent links, BFS discovers reachability, priority queues support Prim's algorithm, and balanced trees support indexed historical searches. Key practical lessons include keeping interfaces consistent between frontend and backend, preserving regression tests, and distinguishing simulated behavior from hardware-backed behavior. A production deployment would require persistent storage, secure device identities, real-time transports, monitoring, and operational testing under failure conditions.

## 9. Remaining actions before submission
Run and verify the solution locally; capture build and test evidence; add screenshots of the integrated UI; incorporate actual lecturer feedback; push the verified source to GitHub; export this report as PDF; and provide the required YouTube demonstration link. Do not submit this draft as a verified report until these steps are completed.
