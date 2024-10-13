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

### Game Loop

Throughout the game, the player will be tasked with clearing stages and advancing to a specific room within each scene. Upon reaching this room, they must interact with an object or element before returning to the entry door. The interaction will involve pressing a button, with the specific context varying according to the storyline's requirements. This could involve actions such as stealing an object, burning a document, or other objectives aligned with the narrative. During the time they are walking to or from that room, a player might encounter enemies on its path. Enemies will behave according the described interactions on the _Enemies_ section, but in most cases this will result in a fight between the enemy an the player. As noted in the _Weapon System_ section, there is no reference to a traditional damage system. This is because instant death plays a crucial role in the combat mechanics. Both the player character and enemies will be killed instantly by a single bullet, while melee attacks will require two hits to achieve a kill. If the player character dies, the player will be prompted to restart the scene with the press of a button.

The player will have an stat called the "Psychosis Bar" that will increase upon killing enemies. Increasing this bar will have an effect of how the character sees the world as it represents the psychological toll of killing enemies.This will manifest on modifying how the game is seen with post-processing effects and an increase on the speed and accuracy of the enemies. If the player stops killing enemies, the bar will start to depleate slowly. However, upon reaching a certain ammount considered the "point of no return", the player will face more severe consequences and the bar will stay up until the player is killed or the scene ends.

Upon exiting the building and finishing the level, the score screen will appear, recording the player's high score on the save file.Additional information, such as the number of enemies killed, total enemies eliminated in the scene, highest kill combo, and time taken to complete the scene, will also be displayed in this section. After this screen, the player will be presented with some dialog before entering the next stage. This loop will be repeated until the completion of the game.


### Enemies

The main obstacles on the game will be enemies. By default, all of them will be equipped with a weapon chosen during scene design. These enemies will work based on a state system that will dictate their behabiour:

- **Idle State**: This is the default state for an enemy and is triggered by default. There are two types of idle states: the static, which means the enemy is fixed in place and will not move until it changes a state; and the path, which means the enemy is folowing a route on loop until something forces them to change state.
- **Chase State**: In this state, the enemy moves towards the player. This state is entered by detecting the player character, which can be done in two ways. The first one would be detecting player through vision, entering in a predetermined field of view. The second one would be detecting the player through noise, by calculating the distance from which the player has shot a fire weapon and determiningh if that distance falls within an specific range. This will be managed by a custom Global Message System that will call an event when any weapon is shot, avoiding calling the reference every time on the awake. Once the chase state is entered, the enemy will go to the player using Unity's 2D Pathfinding System until it reaches the target destination. If the enemy is using a melee weapon, it will move to the player's position and then go to the Attack State. If the enemy is using a fire weapon, it will go to safe distance and then go to the Attack State.
- **Attack State**: In this state, the enemy will execute the attack function associated with the weapon they are holding. If they are unable to attack, they will return to the Chase State.
- **Stun State**: If the enemy has been hit by a melee attack, they will enter the stun state for a fixed amount of time. During that time, the enemy won't be able to move or attack the player. If struck again by a melee weapon, finisher, or fire weapon, the enemy will be killed.
- **Dead State**: Enemy turns into a corpse; cannot move or shoot; cannot change state.

### Weapon System

There are two main kinds of weapons in the game: fire weapons and melee weapons.

- **Fire weapons** will use a raycast system in order to shoot projectiles, inflicting damage on characters (be it an enemy or the player character). All fire weapons will have unlimited ammunition allowing the player to use them throughout the entire scene if they choose to. However, magazines will not be infinite, requiring players to reload once a magazine is depleated. Reloading the gun will put the player into a state in which they cannot shoot nor change weapon, so a good management of the bullets is needed in order to not have to reload mid-combat. Fire weapons (specially those that can be spammed) will have a random recoil pattern that will be unique to each weapon, emphasizing the need for careful planning during combat. The abstract class will then have the current variables that can be modified on each of the weapon variants: fireRate(float), ammoSize(int), ammoType(enum), recoilCurve(animCurve), hasAutoFire(bool). From this class the following weapons are going to be created: a pistol (type: single bullet, no autofire), a shotgun (type: shotgun bullet, no autofire), an SMG (single bullet, autofire, lots of recoil) and an assault riffle (single bullet, autofire, smaller recoil).
- Melee weapons are designed for close-range attacks, wounding enemies and requiring a follow-up action to finish them off. When a character is attacked by a melee weapon, it enters in an stunned state for a given ammount of time, providing the attacker with the opportunity to either deliver a finishing blow or continue attacking, whether with melee or firearms, to kill them. Once that time passes, the affected character will return to their normal state. There will be two different types of melee weapons: a short melee weapon and a long melee weapon, but the only difference between the two is the range of attack.

Both enemies and the player will be able to use any of the given weapon, but some mechanics (like having to reload or being able to hold multiple guns) might be discarded on the enemy side to improve the game feel and player experience.

### Dialog System

In order to deliver its story, the game will feature a dialog system. This dialog system will read conversations from a JSON file and serialize them into Unity. The UI will appear in the bottom of the screen, and will show the player the text from the JSON file. The players may press any button in order to advance in the conversation. During the conversation, the players may be prompted to answer within a predefined set of options. This election will change the next line(s) of the dialog but will never have meaningfull changes on the story. The player will always experience the same linear story regarding their choices on the dialog system, thus being there only to make the player interact and engage with the story directly.

On the dialog UI, there will be a sprite of which character is talking. The dialog system is able to broadcast Unity Messages, performing actions on the game such as changing its UI or enabling a post-processing effect for the sake of engagement and storytelling.

### Score

Score is calculated during the shooter part of the game which each scene having a unique value (score is not transfered from one scene to the next). Actions like eliminations or completing the level's objective will increase the score, while combos (achieved by killing multiple enemies within a short time frame) will provide additional ones. On the other hand, being killed, reloading too many times or taking too much time to finish the level will substract score. Score will not be visible during gameplay but will be detailed on the Score page at the end of the level. The highest achived score will be saved on the game's save, allowing the player to try to beat their previous score.

### Saving System

Upon finishing a scene, the Score and Statistics for that scene will be saved using a custom serializer. Players can replay scenes at any time or see detailed performance stats via the game menu. The menu will show both the highest and lowest achieved score on each scene.

## Level Design

The scenes are the main part of the game and they are where the players will spend the mayority of their time in. Each of the nine scenes is set in a custom-designed house in which the player has two objectives: reaching the "key room", where they can fulfill the task they've been assigned to, and exiting the building alive. Both conditions MUST be met by the player in order to finish the scene and proceed to the score screen. 

All the scenes will consist of a house with a various number of rooms and corridors. Inside, players will find a variety of objects like TVs, Tables, Chairs, Sofas... that will limit their movement in the rooms. In addition to the static objects, there will be two types of interactable objects: windows and doors. Windows allows the player and the enemies to see through them and will be broken when shooting. Gameplay-wise, this means that they will block the first bullet but then dissapear, allowing subsequent fire to pass through them. Doors will consist of a rigidbody with a joint fixture component that prevent vision and fire. They cannot be broken; however, using Unity physics, they can be rotated pivoting one of their sides when the player pushes them. Doors will open both ways. Bullets will apply a force when colliding with the door, allowing the player to open them by shooting. Enemies will not be programmed to shoot doors to open them. Along the obstacles, the scenes will be populated with enemies. More information on this is included on the _Enemies_ section. Killing all the enemies won't be needed in order to finish the level, allowing the player to just run away from the action if they wish. It is an intended mechanic. However, it will be very difficult or, in some scenes the scene, even imposible complete a scene without being detected. As a result, the game is not intended to be played in a peaceful or stealthy manner.

## Artistic Direction

Project-Hotline's art features a strong pixel art style made by downscaling the vector file of the top-down shooter asset pack provided on class. Some sprites may need to be modified in order to account for things like different weapon types. 2D lighting will be used in the scenes to provide the dark ambience the game will have, making the player feel uneasy as they play the game. As stated as the beginig, all the game design and artistic direction of the game should make the player become addicting to the act of defeating enemies while also reflecting on the morality of killing. In addition to this, a custom post-processing effect mimicking a CRT display will be made using hlsl.

The game's UI will mimick the style of the old operating system MS-DOS. This provides the game with a strong artistic style while also making the UI sprites easy to make as the OS used ASCII characters in order to draw its UI.

## Story

Project-Hotline is a story about addiction, regret, and the haunting consequences of a life spiraling out of control. Our protagonist wakes up in a grimy bathroom with no memory of who they are. Shortly after, their phone buzzes, connecting them to the last character of the story: a mysterious man who speaks with them throughout the game. Through conversation, our protagonist will piece together the events that led to this moment. From here, you’ll get the raw story. It won’t be that overly detailed or presented in full in the actual game.

Blake Davis (gender left to the player's interpretation) is an American ex-rockstar, around 30 years old. After their fame faded, money ran dry, and soon they couldn’t sustain the habits they picked up in the music industry. Desperate to feed their growing needs, they made a deal with a mobster to start doing his dirty work. This man is called Delta, though that isn’t his real name. We won’t learn much about him except that he’s the one talking with Blake throughout the game, and the one who ordered all their missions. Blake’s new life as a hitman begins with a simple task: retrieve stolen goods from a rival gang. But for Blake, the real fear is the thought of taking a life for the first time. This was a point of no return. Sitting in their rundown car, they hesitate, digging through a pile of old junk, hoping to find something that could spark a fleeting sense of relief. Nothing. Their breathing becomes shallow, panic setting in. They need to feel that rush—badly. There’s only one way to get back to that feeling again—and this is it. Determined, they arrive at the house, they load the gun and open the door. A massacre. And then again, and again, and again. Every month, once, twice, some months even one per week. So many times that they start embodying the frenzy, laughing instead of wanting to look away. So many times they start to lose their grip on reality. But the high doesn’t last forever. Every morning, turning on the TV, the news rolls in. A murder of four on Fifth Avenue. A murder of seventeen on Dolly St. Photos of the families, sometimes kids. At first, they turn off the TV as fast as they can, telling themselves they’ll grow out of the guilt, that they’ll get used to it. But it doesn’t happen. It never happens. The psychological toll on Blake grows heavier, and they start to see faces in places they shouldn’t. They start to hear voices—voices telling them to keep going, to do it again, to make them suffer longer and longer. They lose their humanity, embracing the frenzy of killing everyone. Then morning arrives. They’re on their couch at home, with cold pizza from yesterday, the TV displaying a no-signal error, filling the room with a white noise that drowns out the sound of their own sobs. Some broken bottles and an old credit card lay on the coffee table, junk all over the floor. Their descent into this state of being is nearly complete—it’s almost as though they’ve become two entirely different people. Then comes the next mission, the one that will be their last. Just a simple task: kill. everyone. It’s 12 a.m. Blake takes a deep breath before stepping out of the car. The silence of the night is only interrupted by the click of their 8mm. Inside, a family stands helpless against their rifle. They beg for mercy, but the voices tell Blake to pull the trigger. To watch the bodies drop to the floor. To see the light leave their eyes. The next morning is the worst yet. The voices are deafening, drowning out every other thought. There’s only one thing to do. Blake stumbles into the bathroom, cold laughter escaping as they watch crimson running down their arms. The color spreads, filling the tub. Everything fades to black. They collapse.

Blake wakes up without remembering anything. A phone buzzes in the sink, casting a faint light across the bathroom. They pick it up. A man named Delta answers. They start talking, memories creeping back in. Faces. Blood. Silence. Delta keeps trying to reel Blake back in, but with every second, Blake pulls further away from their boss. For the first time, they see themselves for what they are: someone who killed hundreds to satisfy a void inside themselves. A broken person who can never undo what they’ve done. Blake tells Delta to never call again. A heated argument erupts. Delta taunts Blake, saying they were never strong enough. He kicks open the bathroom door. The door slams against the wall, casting harsh light over the floor. Holding a gun aimed at Blake, Delta says, “You weren’t even strong enough to end it all yourself.” The screen fades to black, a gunshot echoes. A blue screen with the game’s title, "No Signal" appears as the credits roll. Along with the credits, two stats appear at the end: number of people killed and number of innocent people killed.

## Inspirations

- Hotline Miami: Main gameplay loop
- Katana Zero: Narrative, art style
- Silent Hill: Ambience, storytelling
- Signalis: Ambience, sounds, storytelling