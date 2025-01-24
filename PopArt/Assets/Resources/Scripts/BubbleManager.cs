using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    public Color bubbleColor;
    public string targetComponent;
    private Vector2 movementDirection;
    public float speed = 5f;

    void Start()
    {
        movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        GetComponent<SpriteRenderer>().color = bubbleColor;
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
            Destroy(gameObject); 
        }
        else
        {
            Debug.Log("Keine Kollision erkannt.");
        }
    }

}
