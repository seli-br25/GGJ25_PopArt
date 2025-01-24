using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    public Color bubbleColor;
    private Vector2 movementDirection;
    public float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        movementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        GetComponent<SpriteRenderer>().color = bubbleColor;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(movementDirection * speed * Time.deltaTime);

        Vector3 position = Camera.main.WorldToViewportPoint(transform.position);
        if (position.x < 0 || position.x > 1) movementDirection.x = -movementDirection.x;
        if (position.y < 0 || position.y > 1) movementDirection.y = -movementDirection.y;
    }

    private void OnMouseDown()
    {
        GameManager.Instance.FillShape(bubbleColor, transform.position);
        Destroy(gameObject);
    }
}
