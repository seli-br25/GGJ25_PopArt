using UnityEngine;
using System.Collections;

public class BubbleManager : MonoBehaviour
{
    public Color bubbleColor;
    public string targetComponent;
    private Vector2 movementDirection;
    public float speed = 5f;

    private AudioClip popSound;
    public GameObject reflection;
    private SpriteRenderer reflectionSprite;

    void Start()
    {
        movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        GetComponent<SpriteRenderer>().color = bubbleColor;
        popSound = Resources.Load<AudioClip>("Audio/pop");
        reflectionSprite = reflection.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.Translate(movementDirection * speed * Time.deltaTime);
        Vector3 position = transform.position;

        // Begrenzung in X-Richtung zwischen -2 und 6
        if (position.x < -2 || position.x > 6)
        {
            movementDirection.x = -movementDirection.x;
        }

        if (position.y < Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y ||
            position.y > Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y)
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
            AudioSource.PlayClipAtPoint(popSound, transform.position);
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

        Vector2 randomPosition = new Vector2(Random.Range(-2f, 6f), Random.Range(-4f, 4f));
        transform.position = randomPosition;

        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        reflectionSprite.enabled = true;
    }
}
