# AR Boat Placement and Game Overview

## Project Overview
This project provides a mixed reality experience involving boat placement on a shoreline based on GPS location, image recognition, and plane detection. The interaction and gameplay involve controlling a player boat, an AI-controlled bot boat, and score tracking based on collisions.

## Table of Contents
1. [Game Overview and Workflow](#game-overview-and-workflow)
2. [Detailed Script Analysis](#detailed-script-analysis)
3. [Scripts Description](#scripts-description)

---

## Game Overview and Workflow

### **Game Start**
1. **Initialization**: 
   - The `GameManager` initializes UI elements like the restart button and starts tracking scores.
   - Player and bot boats are positioned and initialized for movement and interaction.

2. **AR Setup**: 
   - Location verification, image recognition, and plane detection are set up in `SequentialARPlacement` to control object placement based on location and conditions.

---

### **Core Functionalities**

#### 1. **Location Verification**
   - The game first verifies if the user’s GPS coordinates match the target location (within 10 meters).
   - Logs the distance to the target location and proceeds if the user is within the defined range.

#### 2. **Image Recognition**
   - If the location verification succeeds, the game activates image recognition. The AR session listens for a predefined reference image in the AR camera view.

#### 3. **Plane Detection**
   - Once the image is recognized, plane detection is activated to find horizontal planes at the image's position.
   - If a plane is detected, the target prefab is placed on it.

#### 4. **Object Placement and Finalization**
   - Once placed, all tracking is disabled (location, image, and plane detection) to enable smooth user interaction without further tracking interference.

---

### **Player and Bot Interactions**

#### **Player Controls**
- The `MyBoat` script manages player control using touch input from a joystick, updating boat speed and direction.

#### **Bot Controls**
- The `BotRandomMovement` and `BotAnimationController` scripts control bot behavior, including random movements, attacks, and maintaining a minimum distance from the player.

#### **Wave Simulation and Compass Adjustment**
- `BoatWaveMovement` simulates wave motion, adding realism with a rocking effect.
- `BoatPositionAdjuster` uses compass data to orient the boat according to real-world direction.

---

### **Collision Detection and Scoring**

#### **Collision Detection**
- `PaddleCollisionDetector` detects paddle collisions between the player and bot, updating scores accordingly.

#### **Score Tracking**
- `ScoreManager` displays scores on the UI and triggers game-over conditions when a score threshold is reached.

---

### **Game Over and Restart Mechanism**

#### **Game Over**
- `GameManager` monitors game-over conditions and triggers animations for the defeated player or bot, freezing the game afterward.

#### **Restart Game**
- `RestartButtonHandler` listens for restart button clicks, resetting game state and boat positions for a fresh game.

---

## Detailed Script Analysis

### **SequentialARPlacement.cs**
Manages the sequential conditions of AR object placement (location, image, and plane detection) and disables tracking after placement.

- **Awake**: Initializes components and requests GPS permission.
- **StartGPS**: Starts GPS tracking and captures the initial location.
- **Update**: Runs location, image, and plane checks sequentially.
- **VerifyLocation**: Compares device location with the target location.
- **AdjustLightSensitivity**: Adjusts ambient light for low-light conditions.
- **OnTrackedImagesChanged**: Checks for the target image and proceeds to plane detection.
- **RaycastForPlane**: Detects horizontal planes, enabling object placement.
- **PlaceObject**: Places the prefab on the detected plane and disables tracking.
- **DisableAllTracking**: Turns off GPS, image, and plane tracking after placement.

---
### **PlaceBoatOnShoreWithCompass.cs**
The `PlaceBoatOnShoreWithCompass` script controls the precise GPS-based placement of a boat on the shore and uses a compass to help orient the placement.

- **Awake**:
   - Initializes the AR raycast manager and checks for location permissions.
   - Enables the device’s compass and starts GPS location tracking with `StartGPS`.
   
- **StartGPS**:
   - Begins GPS tracking, waits for initialization, and checks the device's location availability.
   - Sets the initial latitude and longitude if the GPS status is successful, allowing the script to monitor distance to the target.
   
- **CheckDistanceToTarget**:
   - Continuously checks the distance between the user’s current location and the target GPS point.
   - If within `spawnRadius` (37 meters), it triggers the `PlaceObject` method to instantiate the boat at a specified position.
   - Pauses checks when the user is within `checkRadius` but outside the spawn radius, periodically logging the distance.
   
- **CalculateDistance**:
   - Uses the Haversine formula to calculate the distance between two geographical points.
   
- **PlaceObject**:
   - Instantiates the boat prefab at the desired location when the target distance condition is met.
   
- **OnDestroy**:
   - Disables the compass when the script instance is destroyed to save device resources.

---

### **GameManager.cs**
Handles game state, game-over conditions, and restart functionality.

- **Start**: Sets up the restart button and hides it initially.
- **Update**: Checks score for game-over conditions.
- **GameOver**: Triggers the "Fall" animation, freezes the game, and shows the restart button.
- **RestartGame**: Resets scores, repositions boats, and unfreezes the game for a fresh start.

---

### **ScoreManager.cs**
Tracks and displays scores, and checks for game-over conditions.

- **AddPointToPlayer / AddPointToBot**: Increments scores and checks for game-over.
- **UpdateScoreUI**: Refreshes score display on the UI.
- **CheckGameOver**: Determines if the score threshold is reached and triggers game over.

---

## Scripts Description

### **MyBoat.cs**
Controls the player’s boat movement using joystick input.

- **OnEnable / OnDisable**: Enables and disables touch controls.
- **OnTouchInput**: Processes joystick input to calculate movement direction.
- **Update**: Updates boat position and rotation smoothly.

### **BotRandomMovement.cs / BotAnimationController.cs**
Handles bot movement, animations, and periodic attacks.

- **BotRandomMovement.FollowPlayerBoat**: Calculates position and movement relative to the player.
- **BotAnimationController**: Triggers attack animations at intervals.

### **BoatWaveMovement.cs / BoatPositionAdjuster.cs**
Simulates wave motion and adjusts the boat’s orientation based on compass data.

- **BoatWaveMovement.Update**: Adds rocking motion.
- **BoatPositionAdjuster.ShiftPositionSouthWestWithCompass**: Aligns the boat’s direction using the compass.

### **PaddleCollisionDetector.cs**
Detects collisions between paddles and awards points.

- **OnTriggerEnter**: Checks for paddle collisions and updates the score.

### **DebugLogDisplay.cs**
Displays debug messages on-screen.

- **HandleLog**: Adds messages to the on-screen log for real-time debugging.

### **RestartButtonHandler.cs**
Controls the restart button visibility and functionality.

- **ShowButton / HideButton**: Displays or hides the restart button.
- **RestartGame**: Resets game states and scores.

### **FixedButton.cs / FixedButtonWithLogs.cs**
Tracks button presses and logs interactions for debugging.

- **Update**: Monitors button press state.
- **HandleLog**: In `FixedButtonWithLogs`, logs messages for debugging button presses.

---

### **SyncColliderWithPaddle.cs**
Syncs the collider with the paddle’s position for accurate collision detection.

- **Update**: Constantly aligns the collider with the paddle.

---

This overview provides a comprehensive explanation of each script’s purpose, workflow, and how the project integrates AR functionalities with the game mechanics. 

### Testing and Debugging
- **Location Testing**: Set a test or desired location to verify functionality in different environments.
- **Debug Log**: Enable `DebugLogDisplay` for real-time feedback on the screen to identify issues as they arise.



