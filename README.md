This original owner has unexpectdly deleted the master project, this is a fork.  
Although they did create this for anyone to use and modify, be aware no licence is present in the github.

# Black Prediction
This is a client side prediction and server reconciliation plugin for Mirror. It is a completely server authoritative movement solution.

Download Unity Package: -link temporarily removed-

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

#KCC
- Delete OnEnable and OnDisable from KinematicCharacterMotor.cs
- Open up KinematicCharacterSystem.cs
- Delete "if (Settings.AutoSimulation)" inside "FixedUpdate"
- Change "FixedUpdate" to "public static void Simulate()"


# To Do
- Fix camera rotation reconciliation.
- Add Rigidbody/KCC example.
- Add editor helper so the user does not need to edit the scripts directly. 
- Add interpolation for reconciliation.
- Add performance checks to save bandwidth.
- Support non-mirror networking solutions.

# Video Previews
- Here you can see the client trying to speed cheat (right), and to the other player everything is displayed normally (left).  
The cheater keeps getting pinged back into correct position.  
https://user-images.githubusercontent.com/57072365/143479914-3a7db021-4e72-49c9-86f4-3ef0c813ee6d.mp4


- Same as above, however bad network conditions (latency/ping/lag) is simulated ontop.  
Everything to the viewing player (left) continues as normal  :)  
https://user-images.githubusercontent.com/57072365/143480081-3bf8f723-b101-4987-8a80-54fcaaf1eac3.mp4





