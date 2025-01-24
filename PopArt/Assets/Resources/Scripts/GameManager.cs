using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Artwork
{
    public int id;
    public string title;
    public List<ComponentData> components;
}

[System.Serializable]
public class ComponentData
{
    public string name;
    public string correctColor;
    public bool isRightColor;
}

[System.Serializable]
public class ArtworksData
{
    public List<Artwork> artworks;
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject bubblePrefab;
    public Transform artworkContainer;
    public int bubbleCount = 20;

    private Artwork currentArtwork;
    private List<Color> bubbleColors = new List<Color>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadArtwork(1);
        SpawnBubbles();
    }

    void LoadArtwork(int id)
    {
        // Load JSON
        TextAsset jsonFile = Resources.Load<TextAsset>("artworks");
        ArtworksData data = JsonUtility.FromJson<ArtworksData>(jsonFile.text);

        currentArtwork = data.artworks.Find(a => a.id == id);

        foreach(var component in currentArtwork.components)
        {
            bubbleColors.Add(HexToColor(component.correctColor));

            Transform child = artworkContainer.Find(component.name);
            if (child != null)
            {
                child.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
    }

    void SpawnBubbles()
    {
        for (int i = 0; i < bubbleCount; i++)
        {
            Vector2 randomPosition = new Vector2(Random.Range(-8f, 8f), Random.Range(-4f, 4f));
            GameObject bubble = Instantiate(bubblePrefab, randomPosition, Quaternion.identity);
            int randomIndex = Random.Range(0, currentArtwork.components.Count);
            var component = currentArtwork.components[randomIndex];
            bubble.GetComponent<BubbleManager>().bubbleColor = HexToColor(component.correctColor);
            bubble.GetComponent<BubbleManager>().targetComponent = component.name;
        }
    }

    Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }
        return Color.white;
    }

    public void FillShape(string shapeName, Color color)
    {
        Transform shape = artworkContainer.Find(shapeName);
        if (shape != null)
        {
            SpriteRenderer shapeRenderer = shape.GetComponent<SpriteRenderer>();
            shapeRenderer.color = color;

            var component = currentArtwork.components.Find(c => c.name == shapeName);
            if (component != null)
            {
                if (shapeRenderer.color == HexToColor(component.correctColor))
                {
                    component.isRightColor = true;
                    Debug.Log($"{shapeName} hat die richtige Farbe!");
                }
                else
                {
                    Debug.Log($"{shapeName} hat NICHT die richtige Farbe!");
                }
            }

            CheckWinCondition();
        }
    }

    void CheckWinCondition()
    {
        // check if all components are colored correctly
        if (currentArtwork.components.TrueForAll(c => c.isRightColor))
        {
            Debug.Log("Du hast gewonnen!");
        }
    }
}
