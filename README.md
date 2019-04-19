# UnityDragAndSwipe2DCamera

Proof of Concept for a 2d camera that can differentiate between drags and swipes. Good starting point, but not a complete solution.

## Potential Improvements
* Create a 'zone' or collider that limits where to limit drag and swipe inputs
* Multi-Input support


## Hierarchy

Just need a Camera with a Rigidbody2D Component


## CameraDragAndSwipe
- Set X and Y sensitivity to relatively control how much these axes are affected by the force applied
- Tracks input position on the screen over time
- *Trajectory* as referred to here is the angle as user's mouse or finger was moving when a swipe ended
- To identify the trajectory of a swipe, we need two points A and B, right?
  - A would be the last point when the swipe was released
  - B would be the point being touched by the user C seconds ago
  - C in this case is the editor-modifiable variable "Trajectory Lookback Seconds" (0.1f default)
- Swipe Force * Trajectory is applied to the camera rigidbody on swipe
- Swipe Threshold identifies the difference between a drag that has ended and a user swiping
  - Measured by the pixel distance between Trajectory points A and B (and therefore the speed at which the input was moving)


## License
This is just a Proof on concept project, sharing if it helps anyone. See the [license file](../LICENSE.md) for license rights and limitations (MIT).

