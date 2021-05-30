# Black Prediction
This is a client side prediction and server reconciliation plugin for Mirror. It is a completely server authoritative movement solution.

Download Unity Package: https://github.com/bluejayboy/Black-Prediction/releases

# Feature
- Client side prediction.
- Server reconciliation.
- Only rolls back states when there is a mismatch between client and server states.
- Once a mismatch occurs, the client applies that state to the controller and replays the inputs starting after that state's frame. All old inputs before that get discarded.
- Simulates all controllers in one frame and a single input per frame.
- Speedhack proof.
- Supports both default Unity CC and KCC.

# Disclaimer
KCC is a paid asset, so I can not include the files for it directly but the example for it might be added soon.

Buy KCC: https://assetstore.unity.com/packages/tools/physics/kinematic-character-controller-99131

# Quick Start
- Make a build.
- Host from the build and join as a client inside the editor.
- Start moving around on the client. Notice how it is very smooth.
- Modify the client's player prefab and change the speed for movement and jump.
- Realize that it gets snapped back to the position that it belongs to according to the server.
- Add the latency simulation script inside of the NetworkManager transport field to see how it works through latency.

# Tutorial
- Open AuthoritativeCharacterData.cs
- Add in desired player inputs inside of the ClientInput struct and desired player physics results inside of the ServerResult struct.
- DO NOT edit AuthoritativeCharacterMotor.cs and AuthoritativeCharacterSystem.cs unless you know what you are doing.
- Create your player movement script and make it derive from the AuthoritativeCharacterMotor.cs script.
- Add all the required function overrides and follow ExampleCharacterController.cs inside the example folder to see how it should be implemented.
- Make your own player prefab for it, add Network Identity, NetworkTransform, and your playerMovement script on it, and then throw it in the NetworkManager script in the start scene.
- Make a new GameObject in your game scene with NetworkIdentity and AuthoritativeCharacterSystem script on it.
- Play and test how it works.

# Known Issue
- Rotation appears to be completely messed up when using the default Unity controller. This is being looked into.

# To Do
No promises.

- Add custom network time and frame management.
- Add interpolation for reconciliation.
- Add first person view.
- Make it rotate properly. The default Unity controller is not great at handling positional and rotational data.
- Add Rigidbody support.
- Add editor helper so the user does not need to edit the scripts directly. 
- Add performance checks to save bandwidth.
- Support non-mirror networking solutions.

uwu
