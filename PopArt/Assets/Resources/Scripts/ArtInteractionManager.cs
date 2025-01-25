using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ArtInteractionManager : MonoBehaviour
{
    private SpriteRenderer renderer;
    private bool isOnArt = false;
    private int gameID;
    private bool gameStarted;

    void Start()
    {
        // Get the material of the SpriteRenderer
        renderer = GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the GameObject!");
        }
        gameID = int.Parse(gameObject.name);
        gameStarted = false;
    }

    private void Update()
    {
        if (isOnArt && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)) && gameStarted)
        {
            PerformAction();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && ColorUtility.TryParseHtmlString("#B38F28", out Color color) && gameStarted)
        {
            Debug.Log("Is on art!");
            // Enable the outline by changing the thickness
            renderer.color = color;
            isOnArt = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && gameStarted)
        {
            // Disable the outline by setting thickness to 0
            renderer.color = Color.white;
            isOnArt = false;
        }
    }

    void PerformAction()
    {
        if (gameStarted)
        {
            renderer.color = Color.gray;
            Debug.Log($"Button {name} pressed!");
            SceneManager.LoadScene("MiniGame" + gameID);
        }   
    }

    public void SetGameStarted()
    {
        gameStarted = true;
        Debug.Log("game started: " + gameID);
    }
}
