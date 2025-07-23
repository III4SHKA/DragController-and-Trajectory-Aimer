Drag and Trajectory Visualization for 2D Rigidbody in Unity
This Unity C# script enables a drag-and-launch mechanic for a 2D Rigidbody object with real-time trajectory prediction and visualization.

DragController.cs — lets the player click and drag to set the launch direction and force within a limited range. When released, it applies an impulse force to the Rigidbody2D to launch it.

TrajectoryAimer2D.cs — simulates the projectile’s trajectory under gravity, friction, and bounciness, including collisions with the environment. It visualizes this predicted path using fading dots to help the player aim.

This system is useful for games with slingshot or drag-to-throw mechanics, providing intuitive controls and visual feedback for precise aiming.

