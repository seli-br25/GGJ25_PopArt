using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    // Reference to the Animator component
    private Animator animator;

    // Movement speed in the x-direction
    public float moveSpeed = 5f;

    // Rigidbody2D for movement
    private Rigidbody2D rb;

    void Start()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

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
        // Movement direction
        float moveDirection = 0f;
        bool isKeyPressed = false;

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

        // Apply movement using Rigidbody2D
        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);

        // Immediately stop the running animation if no key is pressed
        if (!isKeyPressed)
        {
            animator.SetBool("isRunning", false);
        }
    }
}