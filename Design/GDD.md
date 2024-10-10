# GDD Document

_Project Hotline_ is an intense top down shooter that puts players in high-stakes situations while telling a story about addiction, regret and the hounting consequences of a life driven by desperation. At its core, this game explores the themes of redemption, self-destruction, and the cost of lost potential as the character confronts the memories of his past actions and struggles to reclaim his humanity.

> _Project Hotline_ seeks to make the act of killing both highly satisfying and addictive, while simultaneously encouraging players to reflect on the consequences of their actions. Upon completing the game, players will be confronted with a thought-provoking question: is victory achieved by attaining the highest score or by pursuing the lowest?

## Mechanics

### Player

The players will have control over a character throughout the game, providing them with the following abilities:

- **Movement**: Players can move with an 8-direction movement across any of the scenes. The movement system incorporates acceleration and deceleration curves to enhance the fluidity of motion. Obstacles such as walls or furniture will restrict movement, encouraging players to strategize their movements carefully to avoid becoming trapped. Further details about objects and their impact on movement will be covered in the _Level Design_ section.
- **Aiming**: Players can rotate their character to aim in various directions. The rotation will be smoothed through linear interpolation to enhance the overall responsiveness of movement.
- **Attack**: With the press of a button, Players can perform an attack, this will be explained with more detail on the _Weapon System_ part.
- **Change weapons**: Players are able to hold up to two weapons at the same time and change between them during gameplay. However, switching weapons takes time, making it unwise to do so during combat. Players may also **throw a weapon** to free up a slot and pick up a different weapon from the ground. Throwing a weapon at an enemy will briefly stun them, rendering them unable to move or attack for a short period.

### Enemies

- Idle path
- Detect player
- Go to player

### Game Loop

### Dialog System

- Dialog System
- CSV Parser
- Answers?

### Weapon System

There are two main kinds of weapons in the game: fire weapons and melee weapons.

- **Fire weapons** will use a raycast system in order to shoot projectiles, inflicting damage on characters (be it an enemy or the player character). All fire weapons will have unlimited ammunition allowing the player to use them throughout the entire scene if they choose to. However, magazines will not be infinite, requiring players to reload once a magazine is depleated. Reloading the gun will put the player into a state in which they cannot shoot nor change weapon, so a good management of the bullets is needed in order to not have to reload mid-combat. Fire weapons (specially those that can be spammed) will have a random recoil pattern that will be unique to each weapon, emphasizing the need for careful planning during combat. The abstract class will then have the current variables that can be modified on each of the weapon variants: fireRate(float), ammoSize(int), ammoType(enum), recoilCurve(animCurve), hasAutoFire(bool). From this class the following weapons are going to be created: a pistol (type: single bullet, no autofire), a shotgun (type: shotgun bullet, no autofire), an SMG (single bullet, autofire, lots of recoil) and an assault riffle (single bullet, autofire, smaller recoil).
- Melee weapons are designed for close-range attacks, wounding enemies and requiring a follow-up action to finish them off. When a character is attacked by a melee weapon, it enters in an stunned state for a given ammount of time, providing the attacker with the opportunity to either deliver a finishing blow or continue attacking, whether with melee or firearms, to kill them. Once that time passes, the affected character will return to their normal state. There will be two different types of melee weapons: a short melee weapon and a long melee weapon, but the only difference between the two is the range of attack.

Both enemies and the player will be able to use any of the given weapon, but some mechanics (like having to reload or being able to hold multiple guns) might be discarded on the enemy side to improve the game feel and player experience.

### Score

## Level Design

- Houses
- 9 Levels (structured in 3 Acts)
- Breakable windows
- Dario wants to really give a try to doors

## Art

- Pixel-art style
- Game is on a CRT display
- MS-Dos like menus
- Uneasy feeling

## Story

## Inspirations

- Hotline Miami
- Katana Zero
- Silent Hill
- Signalis
- Doki Doki Literature Club