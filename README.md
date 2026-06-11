# Voxaid 🧟‍♂️

*Note: This repository contains only the source code (scripts) of the game to demonstrate coding architecture, mechanics, and clean code principles. 3D models, animations, and sound assets are excluded to respect licensing and file size limits.*

**Play the Game:**
* 🎮 [Google Play Store](https://play.google.com/store/apps/details?id=com.Kaan.Voxaid&pcampaignid=web_share)
* 🍎 [Apple App Store](https://apps.apple.com/tr/app/voxaid/id6760352026)

## 📝 About The Project

**Voxaid** is an action-packed 3D mobile survivor game. The core gameplay loop revolves around intense zombie combat, resource management, and continuous progression. 

Players are thrust into zombie-infested environments where they must eliminate hordes to earn in-game currency. This currency is then utilized to upgrade the character's stats and abilities, allowing them to dive back into the fray stronger and progress further through increasingly difficult waves.

### 🛠️ Built With
* Unity 3D
* C#

## 💻 Technical Highlights & Architecture

In this repository, you will find the scripts that drive the core mechanics of Voxaid. Key systems I developed include:

* **Progression & Economy System:** Implemented a robust currency and upgrade manager that handles the math behind stat scaling, saving player progress, and applying upgrades seamlessly.
* **Enemy AI & Wave Spawner:** Developed dynamic spawner scripts to manage enemy density and scaling difficulty, along with zombie AI controllers for pathfinding and attack states.
* **Combat & Damage Mechanics:** Created modular health and damage interfaces that handle hit detection, combat calculations, and visual/audio feedback triggers.
* **Mobile Controller System:** Optimized input handling specifically for mobile devices, ensuring smooth 3D character movement and responsive combat actions.

## 📂 Repository Structure

The scripts are organized to reflect a clean and modular architecture:
* `/Core` - Game Managers, Game State, and Economy Systems.
* `/Player` - Character movement, combat handling, and stat upgrades.
* `/Enemy` - Zombie AI, state machines, and wave spawning logic.
* `/UI` - Upgrade menus, in-game HUD, and mobile touch controls.

## 📫 Contact

Feel free to explore the code! If you are interested in my work or want to discuss the architecture of Voxaid, let's connect.

* **Email:** [kaankovar@hotmail.com](mailto:kaankovar@hotmail.com)
