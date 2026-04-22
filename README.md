# 🧠 AI Simulation - Miners & Pathfinding (Unity)

## 📌 Overview

This project is a simulation developed in **Unity 6** as part of the course **Artificial Intelligence with Unity**.

The goal is to implement core AI systems from scratch, focusing on:

- Pathfinding algorithms
- Finite State Machines (FSM)
- Autonomous agent behavior

The simulation features multiple **miner agents** navigating a map, collecting gold, and interacting with the environment dynamically.

---

## 🎮 Core Features

### 🤖 Autonomous Agents (Miners)
- Navigate the map using custom pathfinding
- Select optimal gold mines (avoiding occupied ones)
- Collect and deposit gold in a base
- Repeat behavior in a continuous loop

### 🧭 Pathfinding System
- Custom implementation 
- Supports multiple algorithms:
  - Depth-First Search (DFS)
  - Breadth-First Search (BFS)
  - Dijkstra
  - A*

- Runtime switching between algorithms

### 🔁 Finite State Machine (FSM)

Each miner uses a custom FSM with states such as:

- `Idle`
- `MoveToMine`
- `Mining`
- `ReturnToBase`
- `Depositing`
- `Flee` (if enemy detected)
- `Dead`

Transitions are event-driven and modular.

---

## ⚔️ Advanced Behaviors

- Progressive mining and depositing using coroutines
- Terrain cost system:
  - Different movement speeds (e.g., sand, water)
- Enemy AI:
  - Independent FSM
  - Can detect, chase, and kill miners
- Miner reactions:
  - Flee behavior
  - Death state

---

## 🗺️ Environment

- Grid-based node system generated via raycasting
- Obstacles that agents must avoid
- Multiple gold mines distributed across the map
- Base location for resource delivery

---

## 🖥️ UI System

- Displays:
  - Total gold collected
  - Individual miner gold capacity
- Updates in real time

---

## 🧱 Project Structure

```
/Scripts
    /AI
        /FSM
        /States
    /Pathfinding
        /Nodes
        /Algorithms
    /Agents
        Miner
        Enemy
    /Managers
        GameManager
        PathfindingManager
/UI
/Prefabs
/Scenes
```

---

## ⚙️ Requirements

- Unity **6000.0.36f1**
- Visual Studio / Rider

---

## ▶️ How to Run

1. Clone the repository:

```bash
git clone https://github.com/your-repo/project-name.git
```

2. Open the project in Unity 6 (6000.0.36f1)

3. Open the main scene:

```
Scenes/SampleScene.unity
```

4. Press **Play**

---


## 📊 Evaluation Context

This project includes:

- Custom pathfinding system
- FSM-based AI behavior
- Multi-agent simulation
- Progressive resource gathering
- Enemy interaction


---

## 🚀 Possible Improvements

- Real-time path visualization
- Behavior tuning (avoid edge-locking issues)
- Smarter enemy AI
- Dynamic map generation

---

## 👨‍💻 Author

Matias Pulido  
Game Developer

---

## 📄 License

This project is for academic purposes only.
