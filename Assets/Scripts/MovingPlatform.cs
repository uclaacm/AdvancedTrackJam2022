using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Transform savedParent;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug.Log("Collision detected with " + collision.gameObject.name);

        // If the collison's tag is "Player", set savedParent to the collision's current parent, then set its parent to this platform
        if (collision.gameObject.tag == "Player")
        {
            savedParent = collision.transform.parent;
            collision.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // If the collison's tag is "Player", set its parent to savedParent
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.SetParent(savedParent);
        }
    }
}