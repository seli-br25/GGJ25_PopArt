using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 
    public GameObject bubblePrefab; 
    public Transform[] shapes;
    public int bubbleCount = 20;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        for (int i = 0; i < bubbleCount; i++)
        {
            SpawnBubble();
        }
    }

    void SpawnBubble()
    {
        Vector2 randomPosition = new Vector2(Random.Range(-8f, 8f), Random.Range(-4f, 4f));
        GameObject bubble = Instantiate(bubblePrefab, randomPosition, Quaternion.identity);
        bubble.GetComponent <BubbleManager>().bubbleColor = GetRandomColor();
    }

    Color GetRandomColor()
    {
        Color[] colors = { Color.red, Color.green, Color.blue };
        return colors[Random.Range(0, colors.Length)];
    }

    public void FillShape(Color color, Vector2 position)
    {
        Transform nearestShape = null;
        float minDistance = float.MaxValue;

        foreach (Transform shape in shapes)
        {
            float distance = Vector2.Distance(position, shape.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestShape = shape;
            }
        }

        if (nearestShape != null)
        {
            nearestShape.GetComponent<SpriteRenderer>().color = color;
        }
    }
}
