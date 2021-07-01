# Black Prediction
This is a client side prediction and server reconciliation plugin for Mirror. It is a completely server authoritative movement solution.

Download Unity Package: https://github.com/bluejayboy/Black-Prediction/releases

# Feature
- Client side prediction.
- Server reconciliation.
- Input buffer.
- Speed exploit prevention.
- Supports both default Unity CC and KCC.

# Quick Start
- Create a build.
- Host inside the build and join as client inside the editor.
- Move around on the client to try out prediction and reconciliation.
- Modify the client's player prefab and alter the speed for movement and jump.
- Realize that it gets snapped back to the position that it belongs to according to the server.
- Drag the latency simulation script inside of the NetworkManager Transport field to see how it works through latency.

# Tutorial
- Open MovementData.cs script.
- Add in desired movement inputs inside of ClientInput struct and results inside of ServerResult struct.
- Do NOT edit MovementEntity.cs and MovementSimulation.cs scripts unless you know what you are doing.
- Create your player movement script and have it derive from MovementEntity.cs script.
- Add all the required function overrides and follow ExampleMovement.cs inside the example folder to see how it should be implemented.
- Create your own player prefab. Add Network Identity, NetworkTransform, and your newly created playerMovement script to it. Go to your start scene and drop it in the NetworkManager Player Prefab field.
- Create a new GameObject in your game scene with NetworkIdentity and MovementSimulation.cs script attached to it.
- Create a build and test how it works.

# To Do
- Fix camera rotation reconciliation.
- Add Rigidbody/KCC example.
- Add editor helper so the user does not need to edit the scripts directly. 
- Add interpolation for reconciliation.
- Add performance checks to save bandwidth.
- Support non-mirror networking solutions.
