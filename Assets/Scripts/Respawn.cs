using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class Respawn : MonoBehaviour
{
    private GameObject player;
    private Animator am;
    // [SerializeField] private Transform spawnPoint;
    private Rigidbody2D rb;
    private BoxCollider2D cl;
    private LoadingScreen ls;
    private Vector2 hitVelo = new Vector2(0f, 40f);

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        am = player.GetComponent<Animator>();
        rb = player.GetComponent<Rigidbody2D>();
        cl = gameObject.GetComponent<BoxCollider2D>();
        ls = GameObject.FindGameObjectWithTag("MainCamera").transform.GetChild(0).GetComponent<LoadingScreen>();
    }

    // Visualize what the deathzone looks like in the Editor (make sure Gizmos are enabled in the Scene view!)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)GetComponent<BoxCollider2D>().offset, GetComponent<BoxCollider2D>().size * transform.localScale);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: If the collision's tag is "Player", disable its PlatformerCharacter2D component's controls and restart the level
        if (false)
        {
            PlatformerCharacter2D script = player.GetComponent<PlatformerCharacter2D>();
            if (script != null)
            {
                script.controls.Disable();
            }

            // Freeze Player and play paticle effect if hit a spike (tag = "Lethal_Freeze")
            if (gameObject.tag == "Lethal_Freeze")
            {
                cl.isTrigger = false;
                am.SetBool("Dead", true);

                rb.simulated = false;

                player.GetComponent<ParticleSystem>().Play(false);
                player.GetComponent<SpriteRenderer>().enabled = false;
            }

            // Start to fade in the loading screen
            ls.FadeIn();

            // TODO: Start a Coroutine for WaitForDeath, which waits for 1 second before reloading the level
        }
    }

    private IEnumerator WaitForDeath()
    {
        // TODO: Wait one second before continuing (try looking at the Unity docs for Coroutines)
        yield return null;
        
        // TODO: Load the same scene (restart the level)
    }
}
