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

Throughout the game, the player will be tasked with clearing stages and advancing to a specific room within each scene. Upon reaching this room, they must interact with an object or element before returning to the entry door. The interaction will involve pressing a button, with the specific context varying according to the storyline's requirements. This could involve actions such as stealing an object, burning a document, or other objectives aligned with the narrative. During the time they are walking to or from that room, a player might encounter enemies on its path. Enemies will behave according the described interactions on the _Enemies_ section, but in most cases this will result in a fight between the enemy an the player. As noted in the _Weapon System_ section, there is no reference to a traditional damage system. This is because instant death plays a crucial role in the combat mechanics. Both the player character and enemies will be killed instantly by a single bullet, while melee attacks will require two hits to achieve a kill. If the player character dies, the player will be prompted to restart the scene with the press of a button.

Upon exiting the building and finishing the level, the score screen will appear, recording the player's high score on the save file.Additional information, such as the number of enemies killed, total enemies eliminated in the scene, highest kill combo, and time taken to complete the scene, will also be displayed in this section. After this screen, the player will be presented with some dialog before entering the next stage. This loop will be repeated until the completion of the game.

### Dialog System

In order to deliver its story, the game will feature a dialog system. This dialog system will read conversations from a JSON file and serialize them into Unity. The UI will appear in the bottom of the screen, and will show the player the text from the JSON file. The players may press any button in order to advance in the conversation. During the conversation, the players may be prompted to answer within a predefined set of options. This election will change the next line(s) of the dialog but will never have meaningfull changes on the story. The player will always experience the same linear story regarding their choices on the dialog system, thus being there only to make the player interact and engage with the story directly.

On the dialog UI, there will be a sprite of which character is talking. The dialog system is able to broadcast Unity Messages, performing actions on the game such as changing its UI or enabling a post-processing effect for the sake of engagement and storytelling.

### Weapon System

There are two main kinds of weapons in the game: fire weapons and melee weapons.

- **Fire weapons** will use a raycast system in order to shoot projectiles, inflicting damage on characters (be it an enemy or the player character). All fire weapons will have unlimited ammunition allowing the player to use them throughout the entire scene if they choose to. However, magazines will not be infinite, requiring players to reload once a magazine is depleated. Reloading the gun will put the player into a state in which they cannot shoot nor change weapon, so a good management of the bullets is needed in order to not have to reload mid-combat. Fire weapons (specially those that can be spammed) will have a random recoil pattern that will be unique to each weapon, emphasizing the need for careful planning during combat. The abstract class will then have the current variables that can be modified on each of the weapon variants: fireRate(float), ammoSize(int), ammoType(enum), recoilCurve(animCurve), hasAutoFire(bool). From this class the following weapons are going to be created: a pistol (type: single bullet, no autofire), a shotgun (type: shotgun bullet, no autofire), an SMG (single bullet, autofire, lots of recoil) and an assault riffle (single bullet, autofire, smaller recoil).
- Melee weapons are designed for close-range attacks, wounding enemies and requiring a follow-up action to finish them off. When a character is attacked by a melee weapon, it enters in an stunned state for a given ammount of time, providing the attacker with the opportunity to either deliver a finishing blow or continue attacking, whether with melee or firearms, to kill them. Once that time passes, the affected character will return to their normal state. There will be two different types of melee weapons: a short melee weapon and a long melee weapon, but the only difference between the two is the range of attack.

Both enemies and the player will be able to use any of the given weapon, but some mechanics (like having to reload or being able to hold multiple guns) might be discarded on the enemy side to improve the game feel and player experience.

### Score

Score is calculated during the shooter part of the game which each scene having a unique value (score is not transfered from one scene to the next). Actions like eliminations or completing the level's objective will increase the score, while combos (achieved by killing multiple enemies within a short time frame) will provide additional ones. On the other hand, being killed, reloading too many times or taking too much time to finish the level will substract score. Score will not be visible during gameplay but will be detailed on the Score page at the end of the level. The highest achived score will be saved on the game's save, allowing the player to try to beat their previous score.

## Level Design

The scenes are the main part of the game and they are where the players will spend the mayority of their time in. Each of the nine scenes in the game will be a custom-crafted house in which the player has two objectives: go to the "key room", where they can fulfill the task they've been assigned to, and exit the building alive. This two conditions MUST be made in order for the player to finish the scene and go to the score screen. 

All the scenes will consist of a house with a various number of rooms and corridors. Inside, players will find a variety of objects like TVs, Tables, Chairs, Sofas... that will limit their movement in the rooms. Alongside all this static objects, there will also be two interactable objects: windows and doors. Windows allows the player and the enemies to see through them and will be broken when shooting. Gameplay-wise this means that they will stop the first bullet and then dissapear, allowing fire cross them. Doors will consist of a rigidbody with a joint fixture component that prevent vision and fire. They cannot be broken, however, using Unity physics, they can be rotated pivoting one of their sides when the player pushes them. Doors will open both ways. Bullets will apply a force when colliding with the door, allowing the player to open them by shooting. Enemies will not be programmed to shoot doors to open them. Along the obstacles, the scenes will be populated with enemies. More information on this is included on the _Enemies_ section. Killing all the enemies won't be needed in order to finish the level, allowing the player to just run away from the action if they wish. It is an intended mechanic. However, it will be very difficult or, depending on the scene, even imposible to end a scene without being detected so the game is not intended to be played pacefully or stealthy.

## Art

Project-Hotline's art features a strong pixel art style made by downscaling the vector file of the top-down shooter asset pack provided on class. Some sprites may need to be modified in order to account for things like different weapon types. 2D lighting will be used in the scenes to provide the dark ambience the game will have, making the player feel uneasy as they play the game. As stated as the beginig, all the game design and artistic direction of the game should make the player become addicting to the act of defeating enemies while also reflecting on the morality of killing. In addition to this, a custom post-processing effect mimicking a CRT display will be made using hlsl.

The game's UI will mimick the style of the old operating system MS-DOS. This provides the game with a strong artistic style while also making the UI sprites easy to make as the OS used ASCII characters in order to draw its UI.

## Story

## Inspirations

- Hotline Miami
- Katana Zero
- Silent Hill
- Signalis
- Doki Doki Literature Club