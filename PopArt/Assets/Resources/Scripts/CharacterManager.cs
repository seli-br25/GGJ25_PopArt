using UnityEngine;
using System.Collections;

public class CharacterManager : MonoBehaviour
{
    // Reference to the Animator component
    private Animator animator;

    // Movement speed in the x-direction
    public float moveSpeed = 5f;

    // Rigidbody2D for movement
    private Rigidbody2D rb;

    // Movement direction
    float moveDirection = 0f;
    bool isKeyPressed = false;
    bool gameStarted = false;

    public AudioSource backgroundMusic;
    public AudioSource clickMusic;
    public BoxCollider2D wallLeft;

    public ArtInteractionManager artInteractionManager1;
    public ArtInteractionManager artInteractionManager2;
    public ArtInteractionManager artInteractionManager3;

    void Start()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        AudioClip musicClip = Resources.Load<AudioClip>("Audio/gallery");
        backgroundMusic = gameObject.AddComponent<AudioSource>();
        backgroundMusic.clip = musicClip;
        backgroundMusic.loop = true;
        backgroundMusic.volume = 0.1f;
        backgroundMusic.Play();


        AudioClip clickSound = Resources.Load<AudioClip>("Audio/pop3");
        clickMusic = gameObject.AddComponent<AudioSource>();
        clickMusic.clip = clickSound;
        clickMusic.volume = 2.0f;

        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (gameStarted)
        {
            // Check for specific key inputs
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                isKeyPressed = true;

                // Set the animator parameter to trigger the run animation
                animator.SetBool("isRunning", true);

                // Ensure the character faces right
                transform.rotation = Quaternion.Euler(0, 0, 0);

                // Move in the positive x-direction
                moveDirection = 1f;
            }
            else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                isKeyPressed = true;

                // Set the animator parameter to trigger the run animation
                animator.SetBool("isRunning", true);

                // Ensure the character faces left
                transform.rotation = Quaternion.Euler(0, 180, 0);

                // Move in the negative x-direction
                moveDirection = -1f;
            }
            else
            {
                isKeyPressed = false;
                animator.SetBool("isRunning", false);
                moveDirection = 0f;
            }

            // Apply movement using Rigidbody2D
            rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
        }
    }

    public void ComeIn()
    {
        StartCoroutine(ComeInCoroutine());
    }

    private IEnumerator ComeInCoroutine()
    {
        isKeyPressed = true;

        // Set the animator parameter to trigger the run animation
        animator.SetBool("isRunning", true);

        // Ensure the character faces right
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // Continue moving until the character reaches the target position
        while (transform.position.x < 0)
        {
            if (backgroundMusic.volume < 0.4)
            {
                backgroundMusic.volume += 0.001f;
            }
            // Move in the positive x-direction
            moveDirection = 1f;
            rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);

            yield return null; // Wait for the next frame
        }

        // Stop the running animation
        animator.SetBool("isRunning", false);

        isKeyPressed = false;
        moveDirection = 0f;
        gameStarted = true;
        backgroundMusic.volume = 0.4f;
        wallLeft.enabled = true;
        artInteractionManager1.SetGameStarted();
        artInteractionManager2.SetGameStarted();
        artInteractionManager3.SetGameStarted();
    }

    public void PlayClickSound()
    {
        clickMusic.Play();
    }
}