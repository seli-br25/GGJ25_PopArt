using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    public bool gameStarted = false;

    public AudioSource backgroundMusic;
    public AudioSource clickMusic;
    public AudioSource paperAudioSource;
    public AudioClip paperUnfoldedSound;
    public BoxCollider2D wallLeft;

    public ArtInteractionManager artInteractionManager1;
    public ArtInteractionManager artInteractionManager2;
    public ArtInteractionManager artInteractionManager3;

    public GameObject background;

    public GameObject letterObject;
    public GameObject letter;
    public GameObject openLetter;
    public GameObject paper; 
    private bool letterRevealed = false;
    private bool letterOpened = false;
    private bool letterRead = false;

    public float floatAmplitude = 0.5f; 
    public float floatSpeed = 1f;

    private Vector3 startPositionLetter;
    public AudioSource paperSource;
    private bool isOpen;

    public RawImage backgroundImage;
    private UIFader backgroundUIFader;

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

        AudioClip paperSound = Resources.Load<AudioClip>("Audio/paper");
        paperSource = gameObject.AddComponent<AudioSource>();
        paperSource.clip = paperSound;
        paperSource.volume = 2.0f;

        startPositionLetter = letterObject.GetComponent<RectTransform>().anchoredPosition;

        backgroundUIFader = backgroundImage.GetComponent<UIFader>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on " + gameObject.name);
        }

        isOpen = false;
    }

    void Update()
    {
        if(letterRevealed)
        {
            if(background != null)
            {
                background.SetActive(true);
                Color currentColor = background.GetComponent<RawImage>().color;
                if (currentColor.a < 0.8f) 
                {
                    currentColor.a += Time.deltaTime * 1.0f; 
                    background.GetComponent<RawImage>().color = currentColor;
                }
            }

            if (letterObject != null)
            {
                float newY = startPositionLetter.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
                letterObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(startPositionLetter.x, newY);
                
            }

            if (letterOpened)
            {
                if (isOpen == false)
                {
                    letter.gameObject.SetActive(false);
                    openLetter.gameObject.SetActive(true);
                    isOpen = true;
                }
                
                Vector3 startPositionPaper = paper.GetComponent<RectTransform>().anchoredPosition;
                float targetYPosition = 27.7f;
                Vector2 targetPosition = new Vector2(startPositionPaper.x, targetYPosition);

                // Move the paper directly to the target position
                paper.GetComponent<RectTransform>().anchoredPosition = targetPosition;
            } else
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                {
                    letterOpened = true;
                    PlayPaperSound();
                }
            }
            if(letterRead)
            {
                openLetter.gameObject.SetActive(false);
                StartGame();
                letterRevealed = false;
            } else if (!letterRead && isOpen)
            {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
                {
                    letterRead = true;
                    backgroundUIFader.FadeOutOnClick(backgroundImage);
                }
            }
        }
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

        // Apply movement using Rigidbody2D
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);

        ManageLetter();
    }

    private void StartGame()
    {
        gameStarted = true;
        backgroundMusic.volume = 0.4f;
        wallLeft.enabled = true;
        artInteractionManager1.SetGameStarted();
        artInteractionManager2.SetGameStarted();
        artInteractionManager3.SetGameStarted();
    }


    private void ManageLetter()
    {
        letterRevealed = true;
        if (letter != null && background != null)
        {
            background.SetActive(true);
            letter.gameObject.SetActive(true);
        }
    }

    public void setLetterState(string action)
    {
        if(action == "open")
        {
            letterOpened = true;
        }
        if(action == "read")
        {
            letterRead = true;
        }
    }

    private IEnumerator MovePaperUp()
    {
        // Start position of the paper (e.g., off-screen or below
        Vector2 startPosition = paper.GetComponent<RectTransform>().anchoredPosition;

        // Final position of the paper (where it should settle)
        Vector2 targetPosition = new Vector2(paper.GetComponent<RectTransform>().anchoredPosition.x, 27.7f); // Adjust this to where you want the paper to end up

        float timeElapsed = 0f;

        // Animate the paper's position over time
        while (timeElapsed < 1.0f)
        {
            paper.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(startPosition, targetPosition, timeElapsed / 1.0f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the paper ends up at the target position
        paper.GetComponent<RectTransform>().anchoredPosition = targetPosition;
        Debug.Log("New Position: " + letterObject.GetComponent<RectTransform>().anchoredPosition);
    }

    public void PlayClickSound()
    {
        clickMusic.Play();
    }

    public void PlayPaperSound()
    {
        paperSource.Play();
    }
}