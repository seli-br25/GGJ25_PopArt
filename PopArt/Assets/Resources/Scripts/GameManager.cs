using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

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

    public float countdownTime = 60f;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI startCountdownText;

    private bool gameStarted = false;
    private bool gameEnded = false;

    private AudioSource backgroundMusic;
    public GameObject gameOverUI;
    private UIFader UIFader;

    public GameObject restartButton;
    public GameObject exitButton;

    public AudioClip jailSound;
    public AudioClip countdownSound;
    public AudioClip winSound;
    public AudioClip moneySound;

    private int sceneID;
    public GameObject borderObject;
    private bool pause;

    public GameObject originalPainting;
    public GameObject background;

    public GameObject pauseButton;
    public GameObject unpauseButton;

    private void Awake()
    {
        pause = false;
        Instance = this;
        AudioClip musicClip = Resources.Load<AudioClip>("Audio/minigameBackground"); 
        backgroundMusic = gameObject.AddComponent<AudioSource>();
        backgroundMusic.clip = musicClip;
    }

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Match match = Regex.Match(sceneName, @"\d+");
        if (match.Success)
        {
            sceneID = int.Parse(match.Value); 
            Debug.Log($"Szene: {sceneName}, ID: {sceneID}");
        }
        else
        {
            Debug.LogError($"No ID found in scene name: {sceneName}");
        }

        countdownSound = Resources.Load<AudioClip>("Audio/countdown");
        StartCoroutine(StartCountdown());
        UpdateCountdownText();
        gameStarted = false;
        LoadArtwork(sceneID);
        SpawnBubbles();

        UIFader = gameOverUI.GetComponent<UIFader>();
        jailSound = Resources.Load<AudioClip>("Audio/jail");
        winSound = Resources.Load<AudioClip>("Audio/win");
        moneySound = Resources.Load<AudioClip>("Audio/chaching");
    }

    private void Update()
    {
        if (gameEnded) return;

        if (gameStarted && !pause)
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
        List<Color> colorsToSpawn = new List<Color>();

        foreach (var component in currentArtwork.components)
        {
            Color color = HexToColor(component.correctColor);
            // add each color twice
            colorsToSpawn.Add(color);
            colorsToSpawn.Add(color);
        }

        while (colorsToSpawn.Count < bubbleCount)
        {
            int randomIndex = Random.Range(0, currentArtwork.components.Count);
            Color randomColor = HexToColor(currentArtwork.components[randomIndex].correctColor);
            colorsToSpawn.Add(randomColor);
        }

        Shuffle(colorsToSpawn);
        Bounds bubbleBounds = borderObject.GetComponent<Renderer>().bounds;


        for (int i = 0; i < bubbleCount; i++)
        {
            Vector3 randomPosition = new Vector3(
            Random.Range(bubbleBounds.min.x, bubbleBounds.max.x),
            Random.Range(bubbleBounds.min.y, bubbleBounds.max.y),
            -2);
            GameObject bubble = Instantiate(bubblePrefab, randomPosition, Quaternion.identity);

            bubble.GetComponent<BubbleManager>().bubbleColor = colorsToSpawn[i];
            bubble.GetComponent<BubbleManager>().bounds = bubbleBounds;
            var component = currentArtwork.components.Find(c => HexToColor(c.correctColor) == colorsToSpawn[i]);
            bubble.GetComponent<BubbleManager>().targetComponent = component.name;
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
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
        if (gameStarted && !pause)
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
        int minute = Mathf.FloorToInt(countdownTime / 60);
        int seconds = Mathf.FloorToInt(countdownTime % 60);
        int milliseconds = Mathf.FloorToInt((countdownTime * 1000) % 1000 / 10);
        if (minute < 1)
        {
            countdownText.text = $"{seconds:00}:{milliseconds:00}";
        } else
        {
            countdownText.text = $"{minute:00}:{seconds:00}:{milliseconds:00}";
        }
        
    }

    IEnumerator StartCountdown()
    {
        // "3, 2, 1, Go!"-Countdown
        startCountdownText.gameObject.SetActive(true);
        AudioSource.PlayClipAtPoint(countdownSound, transform.position);
        for (int i = 3; i > 0; i--)
        {
            startCountdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        startCountdownText.text = "FAKE IT!";
        backgroundMusic.loop = true;
        backgroundMusic.volume = 0.2f;
        backgroundMusic.Play();
        yield return new WaitForSeconds(1f);
        originalPainting.SetActive(false);
        background.SetActive(false);
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
            backgroundMusic.Stop();
            PlayerPrefs.SetInt($"Minigame_{sceneID}_Won", 1);
            PlayerPrefs.Save();
            exitButton.SetActive(true);
            AudioSource.PlayClipAtPoint(winSound, transform.position);
            AudioSource.PlayClipAtPoint(moneySound, transform.position);
        }
    }

    void EndGame()
    {
        gameEnded = true;
        gameStarted = false;
        Debug.Log("Zeit abgelaufen!");
        startCountdownText.text = "YOU GOT CAUGHT!";
        startCountdownText.gameObject.SetActive(true);
        backgroundMusic.Stop();
        UIFader.ActivateOnClick(gameOverUI);
        AudioSource.PlayClipAtPoint(jailSound, transform.position);
        restartButton.SetActive(true);
        exitButton.SetActive(true);
        PlayerPrefs.SetInt($"Minigame_{sceneID}_Won", 0);
        PlayerPrefs.Save();
        background.SetActive(true);
    }

    public void TogglePause()
    {
        pause = !pause;
        if (pause)
        {
            backgroundMusic.Pause();
            pauseButton.SetActive(false);
            unpauseButton.SetActive(true);
        } else
        {
            backgroundMusic.UnPause();
            pauseButton.SetActive(true);
            unpauseButton.SetActive(false);
        }
    }
}
