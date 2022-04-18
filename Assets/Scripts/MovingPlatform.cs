using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    // TODO: Variable that stores the previous parent of the player

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug.Log("Collision detected with " + collision.gameObject.name);

        // TODO: If the collison's tag is "Player", save the collision's current parent to a variable, then set its parent to this platform
            // ALSO, don't forget to give this platform an animation so that it actually moves! You can use Assets/Animations/MovingPlatformTest.anim as a reference
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // TODO: If the collison's tag is "Player", set its parent to savedParent
    }
}