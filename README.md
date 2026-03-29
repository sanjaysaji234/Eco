## Overview

<img width="1895" height="950" alt="Image" src="https://github.com/user-attachments/assets/d63444df-0fcb-4637-a3bf-ab58f06efb9c" />

This project is an **open-source ecosystem simulation** built on a **procedurally generated hexagonal world**. It explores how small environmental or population changes can influence an ecosystem and lead to larger, sometimes unexpected outcomes.

The simulation models a simplified ecological system composed of interacting entities such as **plants, trees, humans, herbivores, and carnivores**. Each species follows a set of behavioral rules that determine how it consumes resources, interacts with other entities, and survives within the environment.

By adjusting ecosystem variables such as **species population, food availability, or environmental conditions**, users can observe how these changes affect the balance of the ecosystem.

The project is designed both as a **learning tool and an experimental sandbox** for understanding ecological interactions and system dynamics.

---

## Features

- Procedurally generated **hexagonal world**
- Terrain generation with **water, land, and vegetation**
- **Clustered forest generation** using Perlin noise
- Multiple tree types
- Modular and extensible **entity architecture**
- Open-source project designed for experimentation

---
## Prerequisites
Unity Version: 2022.3 LTS (Recommended)

Git: Installed on your local machine

Installation & Setup
Clone the repository:

Bash
git clone https://github.com/snj4y/ecosystem-simulation.git
Open in Unity Hub:

Open Unity Hub and click Add > Add project from disk.

Navigate to the cloned folder and select it.

Launch Project:

Select the correct Editor version and wait for Unity to import assets and resolve packages.

Open the main scene located in Assets/Scenes/.

Final executable game file is Eco\Executable\Eco

Project Structure
/Scripts/Grid: Contains logic for hexagonal math and procedural generation.

/Scripts/Entities: AI behavior and state machines for species.

/Prefabs: Pre-configured 3D models for terrain and environment.

/Materials: URP/Standard shaders for the low-poly/cinematic aesthetic.

## Planned Systems

### Herbivore Behavior
- Movement
- Grazing
- Reproduction

### Carnivore Behavior
- Predator–prey interactions
- Hunting strategies
- Reproduction


### Plant Systems
- Growth cycles
- Resource regeneration

### Ecosystem Dynamics
- Population feedback loops
- Resource competition

### User Interaction
- Adjustable ecosystem variables
- Simulation control and observation tools

---

## Project Goal

The primary objective of this project is to provide game developers with a robust, procedurally generated hexagonal framework featuring a fully functional ecosystem.

As an open-source tool, it is designed for:

Scalability: The modular architecture allows developers to modify and scale the ecosystem complexity to fit their specific project needs.

Developer Sandbox: A ready-to-use foundation for games requiring complex environmental logic or resource management.

System Dynamics: To demonstrate how "butterfly effects"—such as minor changes in food availability or predator population—can cascade through the system, potentially leading to the extinction of entire species.

This project aims to bridge the gap between complex ecological theory and practical game world implementation.
---

## Tech Stack

- **Unity**
- **C#**
- **Blender**
- **Procedural generation using Perlin noise**
- **Hexagonal grid world simulation**

---

## Project Status

**Ready to be Launched**
Done adding most of the mechanics and the project is ready to be explored

---

## Contributing

Contributions, ideas, and discussions are welcome.

1. Fork the repository  
2. Create a feature branch  
3. Submit a pull request with a clear description of the changes

---

## License

This project is open-source and available under the **MIT License**.
