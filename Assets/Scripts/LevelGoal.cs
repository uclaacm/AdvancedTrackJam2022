using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelGoal : MonoBehaviour
{
    [Tooltip("The build index of the next level's scene. These can be found/edited in Build Settings")]
    [SerializeField] private int nextLevelNumber;

    // Visualize what the goalzone looks like in the Editor (make sure Gizmos are enabled in the Scene view!)
    void OnDrawGizmos()
    {
        GetComponent<BoxCollider2D>().offset = Vector2.zero;

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider2D>().size);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If the collider's tag is "Player", start the coroutine that loads the next level
        if(other.tag == "Player")
        {
            StartCoroutine(NextLevel());
            FindObjectOfType<Camera>().transform.GetChild(0).GetComponent<LoadingScreen>().FadeIn();
        }
    }

    private IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(1f);

        // Set the "currentLevel" PlayerPref to nextLevelNumber
        PlayerPrefs.SetInt("currentLevel", nextLevelNumber);
        
        // If the next level hasn't been reached before (ie. the "latestLevel" PlayerPref < nextLevelNumber), update the latestLevel int
        if (nextLevelNumber > PlayerPrefs.GetInt("latestLevel", 0))
        {
            PlayerPrefs.SetInt("latestLevel", nextLevelNumber);
        }

        // Load the next level using nextLevelNumber + 3 (to offset the title screen, main menu, and level select scenes)
        SceneManager.LoadScene(nextLevelNumber + 3, LoadSceneMode.Single);
    }
}
