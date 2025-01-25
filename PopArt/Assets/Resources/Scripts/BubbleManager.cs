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
        Vector3 position = transform.position;

        // Begrenzung in X-Richtung zwischen -1 und 5
        if (position.x < -1 || position.x > 5)
        {
            movementDirection.x = -movementDirection.x;
        }

        if (position.y < Camera.main.ViewportToWorldPoint(new Vector3(0, 0.1f, 0)).y ||
            position.y > Camera.main.ViewportToWorldPoint(new Vector3(0, 0.9f, 0)).y)
        {
            movementDirection.y = -movementDirection.y; 
        }
        if (position.x < -2.3 || position.x > 6.3 || position.y < Camera.main.ViewportToWorldPoint(new Vector3(0, -0.3f, 0)).y ||
            position.y > Camera.main.ViewportToWorldPoint(new Vector3(0, 1.3f, 0)).y)
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
        GetComponent<SpriteRenderer>().enabled = false; 
        GetComponent<Collider2D>().enabled = false;
        reflectionSprite.enabled = false;

        yield return new WaitForSeconds(2f);

        Vector2 randomPosition = new Vector2(Random.Range(-1f, 5f), Random.Range(-4f, 4f));
        transform.position = randomPosition;

        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        reflectionSprite.enabled = true;
    }
}
