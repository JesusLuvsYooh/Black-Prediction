# Black Prediction
This is a client side prediction and server reconciliation plugin for Mirror. It is a completely server authoritative movement solution.

Download Unity Package: https://github.com/bluejayboy/Black-Prediction/releases

# Feature
- Client side prediction.
- Server reconciliation.
- Input buffer.
- Speed exploit prevention.
- Supports both default Unity CC and KCC.

# How It Works
- The client collects inputs, stores them in a list, and applies them to the controller to predict movement.
- The client sends the movement input data to the server.
- The server simulates every controller with the first input in a single frame.
- The server sends the movement result data to the client and discards the used input.
- The client applies most recent result from the server to reconciliate movement and then reapply inputs starting after that result's frame to again predict where it should be. All old inputs that come before get discarded as they are not needed anymore.

# Quick Start
- Create a build.
- Host inside the build and join as client inside the editor.
- Move around on the client to try out prediction and reconciliation.
- Modify the client's player prefab and alter the speed for movement and jump.
- Realize that it gets snapped back to the position that it belongs to according to the server.
- Drag the latency simulation script inside of the NetworkManager Transport field to see how it works through latency.

# Tutorial
- Open AuthoritativeCharacterData.cs script.
- Add in desired movement inputs inside of ClientInput struct and results inside of ServerResult struct.
- Do not edit AuthoritativeCharacterMotor.cs and AuthoritativeCharacterSystem.cs scripts unless you know what you are doing.
- Create your player movement script and have it derive from AuthoritativeCharacterMotor.cs script.
- Add all the required function overrides and follow ExampleCharacterController.cs inside the example folder to see how it should be implemented.
- Create your own player prefab. Add Network Identity, NetworkTransform, and your newly created playerMovement script to it. Go to your start scene and drop it in the NetworkManager Player Prefab field.
- Create a new GameObject in your game scene with NetworkIdentity and AuthoritativeCharacterSystem script attached to it.
- Create a build and test how it works.

# To Do
No promises.

- Add first person view and rotation.
- Add Rigidbody/KCC example.
- Add editor helper so the user does not need to edit the scripts directly. 
- Add interpolation for reconciliation.
- Add performance checks to save bandwidth.
- Support non-mirror networking solutions.

uwu
