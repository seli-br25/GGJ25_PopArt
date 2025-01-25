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
        Vector3 position = Camera.main.WorldToViewportPoint(transform.position);
        if (position.x < 0 || position.x > 1) movementDirection.x = -movementDirection.x;
        if (position.y < 0 || position.y > 1) movementDirection.y = -movementDirection.y;
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

        Vector2 randomPosition = new Vector2(Random.Range(-8f, 8f), Random.Range(-4f, 4f));
        transform.position = randomPosition;

        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
        reflectionSprite.enabled = true;
    }
}
