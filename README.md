# Guardian System

> *You are not the operator. You are the system.*
> *A city-management game about making decisions under pressure and dealing with their consequences.*
> Made for the **FIAP Global Solution 2026.2 — CitizenTech**.

## About the Project

**Guardian System** is a 2D strategy and urban-management game set in a city inspired by São Paulo. Instead of playing as a mayor or a dispatcher, **you take the role of the technology itself**: a system responsible for monitoring the city, interpreting incidents, and coordinating public services.

Handle emergencies, manage limited resources, and keep essential services running. Your performance affects public trust and shapes how the city reacts to the system throughout the story.

## Key Mechanics & Features

- **Emergency dispatch:** Analyze incoming calls and assign the appropriate available units, including police officers, firefighters, paramedics, and sanitation teams.
- **Decision-based incidents:** Respond to situations that require choosing an answer rather than dispatching a unit.
- **Limited resources:** Consider team availability and travel times when deciding which incident to prioritize.
- **Water management:** Monitor water levels across different city zones to avoid shortages and overflows.
- **Trash management:** Keep track of trash accumulation and send sanitation teams before conditions worsen.
- **Public trust:** Successful responses and mistakes affect the system's popularity among citizens.
- **Day-based progression:** Experience the consequences of your performance through narrative cutscenes and different endings.

## Controls

- **Mouse** — Interact with the interface, answer incidents, and manage city services.
- **Esc** — Pause or resume the game.

## Development Highlights

During development, I worked with:

- **Model–View–Presenter (MVP):** Separation of gameplay logic, presentation, and interaction.
- **ScriptableObjects:** Data-driven configuration for gameplay content.
- **Shader Graph and DOTween:** Custom interface effects, transitions, and animations.


<details>
<summary>🔒 System Diagnostics — Restricted Access</summary>

### Hidden Commands

The Guardian System contains a few undocumented commands for those curious enough to explore its systems.

While playing, type:

| Command | Effect |
|---|---|
| `GLOBAL` | Sets public trust to 0% |
| `SOLUTION` | Sets public trust to 100% |
| `FIAP` | Restores public trust to 50% |

A water-overflow sound confirms successful command activation.

While in Main Menu, type:

| Command | Effect |
|---|---|
| `BOOT` | Creates a Day 1 checkpoint |
| `ALERT` | Creates a Day 2 checkpoint |
| `CRISIS` | Creates a Day 3 checkpoint |

A game reload confirms successful command activation.
*Warning: System initialization overwrites existing recovery data.*

*Some systems have more control than they let on.*

</details>

## 🛠️ Built With

- [**Unity**](https://unity.com/) — Game engine (URP)
- **C#** — Programming language
- [**DOTween**](https://dotween.demigiant.com/) — Animations and transitions
- **Unity Shader Graph** — Visual effects

---

*Developed by **Blu D** for FIAP's Global Solution 2026.2.*
