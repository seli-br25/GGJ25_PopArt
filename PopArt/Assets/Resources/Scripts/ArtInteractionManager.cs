using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ArtInteractionManager : MonoBehaviour
{
    private Material material;
    private bool isOnArt = false;
    private int artID = -1;

    void Start()
    {
        // Get the material of the SpriteRenderer
        material = GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        if (isOnArt && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space)))
        {
            PerformAction();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Is on art!");
            // Enable the outline by changing the thickness
            material.SetFloat("_OutlineThickness", 10.0f);
            isOnArt = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Disable the outline by setting thickness to 0
            material.SetFloat("_OutlineThickness", 0f);
            isOnArt = false;
        }
    }

    void PerformAction()
    {
        material.SetFloat("_OutlineThickness", 0f);
        Debug.Log($"Button {name} pressed!");
        SceneManager.LoadScene("MiniGame");
    }
}
