using UnityEngine;
using System.Collections;

public class BubbleManager : MonoBehaviour
{
    public Color bubbleColor;
    public string targetComponent;
    private Vector2 movementDirection;
    public float speed = 5f;

    public AudioClip[] popSounds;
    public GameObject reflection;
    private SpriteRenderer reflectionSprite;
    public Bounds bounds;
    private bool isRespawning = false;


    void Start()
    {
        movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        GetComponent<SpriteRenderer>().color = bubbleColor;
        popSounds = new AudioClip[11];
        for (int i = 0; i < popSounds.Length; i++)
        {
            popSounds[i] = Resources.Load<AudioClip>($"Audio/pop{i + 1}");
        }
        reflectionSprite = reflection.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.Translate(movementDirection * speed * Time.deltaTime);

        if (transform.position.x < bounds.min.x || transform.position.x > bounds.max.x)
        {
            movementDirection.x = -movementDirection.x;
        }

        if (transform.position.y < bounds.min.y || transform.position.y > bounds.max.y)
        {
            movementDirection.y = -movementDirection.y;
        }

        if (!isRespawning &&
        (transform.position.x < bounds.min.x - 0.6f || transform.position.x > bounds.max.x + 0.6f ||
         transform.position.y < bounds.min.y - 0.6f || transform.position.y > bounds.max.y + 0.6f))
        {
            StartCoroutine(RespawnBubble());
        }
    }

    private void OnMouseDown()
    {
        int layerMask = LayerMask.GetMask("Shapes");
        Collider2D hitCollider = Physics2D.OverlapPoint(transform.position, layerMask);

        if (hitCollider != null)
        {
            Debug.Log($"Kollision erkannt mit: {hitCollider.transform.name}");
            GameManager.Instance.FillShape(hitCollider.transform.name, bubbleColor);
            if (popSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, popSounds.Length);
                AudioSource.PlayClipAtPoint(popSounds[randomIndex], transform.position);
            }
            // hide bubble for 2 seconds before spawning it elsewhere
            StartCoroutine(RespawnBubble());
        }
        else
        {
            Debug.Log("Keine Kollision erkannt.");
        }
    }

    private IEnumerator RespawnBubble()
    {
        isRespawning = true;
        GetComponent<SpriteRenderer>().enabled = false; 
        GetComponent<Collider2D>().enabled = false;
        reflectionSprite.enabled = false;

        yield return new WaitForSeconds(2f);

        Vector3 randomPosition = new Vector3(
            Random.Range(bounds.min.x + 0.5f, bounds.max.x - 0.5f), // Abstand von 0.5f von den X-Grenzen
            Random.Range(bounds.min.y + 0.5f, bounds.max.y - 0.5f), // Abstand von 0.5f von den Y-Grenzen
            -2
        );

        transform.position = randomPosition;

        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        reflectionSprite.enabled = true;
         isRespawning = false;
    }
}
