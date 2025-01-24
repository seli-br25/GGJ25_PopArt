using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

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

    public float countdownTime = 30f;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI startCountdownText;

    private bool gameStarted = false;
    private bool gameEnded = false;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(StartCountdown());
        UpdateCountdownText();
        gameStarted = false;
        LoadArtwork(1);
        SpawnBubbles();
    }

    private void Update()
    {
        if (gameEnded) return;

        if (gameStarted)
        {
            if (countdownTime > 0)
            {
                countdownTime -= Time.deltaTime;
                UpdateCountdownText();
            }
            else
            {
                countdownTime = 0;
                UpdateCountdownText();
                EndGame();
            }
        }
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
        if (gameStarted)
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
                        component.isRightColor = false;
                        Debug.Log($"{shapeName} hat NICHT die richtige Farbe!");
                    }
                }

                CheckWinCondition();
            }
        }
    }

    void UpdateCountdownText()
    {
        int seconds = Mathf.FloorToInt(countdownTime % 60);
        int milliseconds = Mathf.FloorToInt((countdownTime * 1000) % 1000 / 10);
        countdownText.text = $"{seconds:00}:{milliseconds:00}";
    }

    IEnumerator StartCountdown()
    {
        // "3, 2, 1, Go!"-Countdown
        startCountdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            startCountdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        startCountdownText.text = "Go!";
        yield return new WaitForSeconds(1f);

        startCountdownText.gameObject.SetActive(false);
        gameStarted = true;
        UpdateCountdownText();
    }


    void CheckWinCondition()
    {
        // check if all components are colored correctly
        if (currentArtwork.components.TrueForAll(c => c.isRightColor))
        {
            Debug.Log("Du hast gewonnen!");
            startCountdownText.text = "YOU WON!";
            startCountdownText.gameObject.SetActive(true);
            gameEnded = true;
            gameStarted = false;
            // TODO: WIN SCENE
        }
    }

    void EndGame()
    {
        gameEnded = true;
        gameStarted = false;
        Debug.Log("Zeit abgelaufen!");
        startCountdownText.text = "GAME OVER!";
        startCountdownText.gameObject.SetActive(true);
        // TODO: GAME OVER SCENE
    }
}
