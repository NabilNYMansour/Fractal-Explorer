# FractaL Explorer
<img src="https://user-images.githubusercontent.com/56453977/232167755-9b00778a-4731-411a-bbc3-21f4d2fd0a75.png" alt="Screenshot of the game" height="300">

- Fractal Explorer is a VR experience where the player explores an endless fractal world made with the ray marching algorithm.
- [The following is a video showcasing game.](https://www.youtube.com/watch?v=-RkAV7bAaSk)
- Finally, [the following is the link to the website of the game.](https://nabilmansour.com/FractalExplorer/)

## Running the game
### Requirements
To run the game, you will require the following tools:
1. Unity game engine
2. StreamVR
### Installing the game
To get the game running, pull the code from the GitHub repository and import it into unity. Afterwards, you can just click the **play** button at the top to start the game.
### Best Experience
To get the best experience of the game, we recommend that you remain seated and read the tutorial presented in the game.
### Known Issue
Note that if there are any issues with the height of the player, simply increase/decrease the `y` value in the `CameraRig` gameObject which is found under the `PlayerParent` gameObject.

## Overview of the shaders
The project utilizes the implementation of a ***Ray Marching*** algorithm where the marcher renders a modified ***Menger Sponge*** that creates the complexity shown in the game. The shaders that do so are as follows:
1. [RayMarcher.shader](https://github.com/NabilNYMansour/Fractal-Explorer/blob/main/Assets/Ray%20Marching/RayMarcher.shader): The main code that imports all the other shaders with the `#include` statement and handles all the depth texture logic so that both polygonal and ray-marched gemoetry can be drawn on the screen.
2. [mainMarcher.hlsl](https://github.com/NabilNYMansour/Fractal-Explorer/blob/main/Assets/Ray%20Marching/RayMarchHelpers/mainMarcher.hlsl): The shader that imports the distance equation and that holds the actual logic for ray marching and getting the normals of ray-marched geometry.
3. [DE.hlsl](https://github.com/NabilNYMansour/Fractal-Explorer/blob/main/Assets/Ray%20Marching/RayMarchHelpers/DE.hlsl): This shader holds the information of what distance estimator will be used. It gets the code for the various signed distance functions from the import of `sdf.hlsl`
4. [sdf.hlsl](https://github.com/NabilNYMansour/Fractal-Explorer/blob/main/Assets/Ray%20Marching/RayMarchHelpers/sdf.hlsl): The shader that holds the logic for the various signed distance functions where the focus is building the function `MengerSpongeFolded` that is the modified distance estimator that is used in the game.
The other shaders are unused in the game and were used when experimenting other methods of rendering.

Finally, there is also [PortalShader.shader](https://github.com/NabilNYMansour/Fractal-Explorer/blob/main/Assets/Portal/PortalShader.shader) that holds the shader for the portal seen at the end of the game.

## Overview of the scripts

### Ray marching scripts
The following scripts relate to the ray marching feature of the project:
1. [RayMarcherUniformSetter.cs](https://github.com/NabilNYMansour/Fractal-Explorer/blob/main/Assets/Ray%20Marching/Scripts/RayMarcherUniformSetter.cs): A script that simply sets the uniform values that need to be passed from the CPU to the GPU like light position.
2. [RMColliderPoints.cs](https://github.com/NabilNYMansour/Fractal-Explorer/blob/main/Assets/Ray%20Marching/Scripts/RMColliderPoints.cs): The main collider script used in the game for collision detection with the ray-marched geometry. The way the collision works is by having mutliple equidistributed points along a sphere around the player. The points are then tested on the CPU if they are negative when applied to the same distance estimator used in the shaders (that is, the points are used in a function that is exactly similar to `MengerSpongeFolded` function in the shader but written in `C#` instead). We then identify the points that are inside the ray-marched geometry and simply apply an **impulse force** to remove the player from the geometry.
### Game Logic scripts
The following scripts relate to the game logic of the project:
1. [AstroidController.cs](https://github.com/NabilNYMansour/Fractal-Explorer/blob/main/Assets/Scripts/AstroidController.cs): The asteroid controller handles the manipulation of coordinate positions for each asteroid in the scene. To mimic the effect of space, all asteroids start in random locations and rotate around the origin of the scene. The closer an object is to the origin the slower it rotates, vs further the faster. Additionally to add more randomization, we rotate objects randomly by either their Vector3.up direction or Vector3.forward. Additionally, each asteroid checks for a collision with the player. If this collision takes place then we reduce the players health and emit a sound effect to provide feedback to the player. (This feedback is the roblox oof sound for comedic purposes..)
2. [CoinManager.cs](https://github.com/NabilNYMansour/Fractal-Explorer/blob/main/Assets/Scripts/CoinManager.cs): The coin manager handles the rotation and collection of coins. Every frame the coins rotation will update to spin around along its vertical axes. Additionally, each coin has a collision trigger event that updates the players score. If the coin is collected, the coin is then destroyed.
3. [LiftstickDriver.cs](https://github.com/NabilNYMansour/Fractal-Explorer/blob/main/Assets/Scripts/LiftstickDriver.cs): Liftstick driver is the input mechanism located on the left side of the ship, responsible for rotating the ship upward and downward. It listens for a trigger with the hand of the player and if the player is within its range and the player is holding down the trigger button, attach the hand to the input. We then convert the hands position from world coordinate space to local space to then calculate the angle different from the base of the input to the top. Once we calculate this angle, we rotate both the ship and the lever based on the same angle. The rotation of the lever is done through its localRotation, however, for the ship we are using its rigidbody’s AddTorque function which will add angular velocity in a vector direction of our choosing. If the player releases their hand from the lever, we snap the hand to its original position and stop any form of angular velocity. 
4. [JoystickDriver.cs](https://github.com/NabilNYMansour/Fractal-Explorer/blob/main/Assets/Scripts/JoystickDriver.cs): Essentially the same as liftstick driver, the reason for them being in different scripts is due to Unity’s trigger system matching the wrong hand if two of the same scripts exist on both hands. However, for this script in addition to the rotation but for left and right navigation, this also applies a constant velocity to the movement.
5. [GameSingleton.cs](https://github.com/NabilNYMansour/Fractal-Explorer/blob/main/Assets/Scripts/GameSingleton.cs): As the name implies, this is our game singleton which manages the timeline and game events. Due to the small nature of the game we went with a singleton that will control the sequences of events. The game singleton for starters, manages the canvas elements and the text they need to show. At the beginning of the game the singleton will display controls and on start will remove those controls and change the necessary UI. The singleton will begin playing audio on start and subscribe to all the necessary actions that other scripts will invoke to communicate to it. The ship's arrow is controlled by the GameSingleton and points to the closest coin to the player's location. This is done by searching for all objects that are tagged as collectible and taking the shortest distance between the difference of the coins position and player position. Once we find this coin we use Quaternion.Slerp to lerp the quaternions rotation to look at the coins position. This gives it a smooth animation between changing targets. Lastly, the singleton handles the loading of scenes. If a player were to die the singleton will restart the level, or when a player finishes the game it will execute the end game sequence and then restart the game.
6. [ExitManager.cs](https://github.com/NabilNYMansour/Fractal-Explorer/blob/main/Assets/Scripts/ExitManager.cs): If the opened portal interacts with the player, execute the end game sequence handled by the singleton.

The rest of the scripts were used for experimentation and debug purposes.

## Credits
The project is made by [Nabil Mansour](https://github.com/NabilNYMansour) and [Samee Chowdhury](https://github.com/oceansam) as the final project for CPS643 course at Toronto Metropolitan University (formerly Ryerson University).

We thank you for your time :)
