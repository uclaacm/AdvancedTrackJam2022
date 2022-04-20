# ACM Studio at UCLA Advanced Track Jam 2022

## Programming Tasks
These are the "required" coding tasks that the skeleton code will walk you through! These are all located within Assets/Scripts near comments that being with "TODO"
### Controls.inputactions
- Create a Jump action
### PlatformerCharacter2D.cs
- Set the start of a Jump Action to call Jump()
- Set the cancelation of a Jump Action to call JumpCancel()
- To detect if the player should slide down a wall, create a circlecast at the wallcheck position that hits anything designated as ground, then set the appropriate booleans
- If the player is gliding, constrain downward velocity and lower gravity
- When jumping off a wall, add horizontal force (maxSpeed * wallJumpXForce)
- Detect when the player is attempting to glide (pressing Jump button in midair)
- Prevent gliding upwards
### Respawn.cs
- Detect if the collision's tag is "player"
- Start the WaitForDeath Coroutine, which waits for 1 second before reloading the level
  - Wait one second (try looking at the Unity docs for Coroutines)
  - Load the same scene (restart the level)
### LevelGoal.cs
- Detect if the collider's tag is "Player"
- Start the coroutine that loads the next level
  - Set the "currentLevel" PlayerPref to nextLevelNumber (It HAS to be named "currentLevel" for LevelSelect to recognize it)
  - If the next level hasn't been reached before (ie. the "latestLevel" PlayerPref < nextLevelNumber), update the latestLevel int
### MovingPlatform.cs
- Declare a variable that stores the previous parent of the player
- Detect if the collison's tag is "Player"
- Save the collision's current parent to a variable, then set its parent to the script's transform
- When a collision ends, detect if the collison's tag is "Player" and set its parent to the previous parent
## Level Design Tasks
There are three template levels that you can mess with as much as you want!
- Duplicate, move, scale, edit the SpriteShapes, etc. of the Terrain to create level layouts
- Duplicate and reposition spikes to give the player obstacles to avoid
- Edit the collider of the death zone (and perhaps create new death zones) so that the player never falls infinitely!
- Add moving platforms:
  - Make a terrain object (or some other platform you create with a collider)
  - Create an animation for it to move repeatedly (make sure that the animation loops or yo-yos)
  - Add the MovingPlatform script as a Component in the Inspector window
## Optional Tasks!
These are some ideas that you can implement to expand your game!
- Create platforms that fall after an adjustable amount of time (bonus points if you can do it without animations!)
- Add a new control for Skreech, such as dashing, stomping, etc.
- Create an equivalent for Sonic springs
