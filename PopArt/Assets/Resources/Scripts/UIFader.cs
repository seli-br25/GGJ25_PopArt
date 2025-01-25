using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFader : MonoBehaviour
{
    public float fadeDuration = 1.0f;
    public float slideDuration = 1.0f;

    public RectTransform myImageRectTransform;
    public Image myImage; 

    [SerializeField]
    Vector3 endPosition;

    [SerializeField]
    bool xSlide = true;

    [SerializeField]
    bool startScreen = true;

    Vector2 startPosition;

    [SerializeField]
    bool noSlide;

    [SerializeField]
    bool down;

    [SerializeField]
    bool left;

    [SerializeField]
    int customDistance = -1;

    public void Start()
    {
        if (!noSlide)
        {
            if (startScreen)
            {
                // Startet links außerhalb des Bildschirms

                if (xSlide)
                {
                    if (left)
                    {
                        startPosition = new Vector2(Screen.width - 500, myImageRectTransform.anchoredPosition.y);
                    } else
                    {
                        startPosition = new Vector2(-Screen.width + 500, myImageRectTransform.anchoredPosition.y);
                    }      
                }
                else
                {
                    if (customDistance == -1)
                    {
                        if (down)
                        {
                            startPosition = new Vector2(myImageRectTransform.anchoredPosition.x, Screen.height - 1000);
                        }
                        else
                        {
                            startPosition = new Vector2(myImageRectTransform.anchoredPosition.x, -Screen.height + 1000);
                        }
                    } else if (customDistance == 2)
                    {
                        this.gameObject.GetComponent<RawImage>().enabled = true;
                        Button buttonComponent = this.gameObject.GetComponent<Button>();
                        if (buttonComponent != null)
                        {
                            buttonComponent.enabled = true; // Button aktivieren, wenn vorhanden
                        }
                        startPosition = new Vector2(myImageRectTransform.anchoredPosition.x, -Screen.height + endPosition.y);
                        SlideInUIElement(myImageRectTransform, startPosition, endPosition);
                    }
                    else
                    {
                        if (down)
                        {
                            startPosition = new Vector2(myImageRectTransform.anchoredPosition.x, Screen.height - endPosition.y);
                        }
                        else
                        {
                            startPosition = new Vector2(myImageRectTransform.anchoredPosition.x, -Screen.height + endPosition.y);
                        }
                    }
                    
                }
                SlideInUIElement(myImageRectTransform, startPosition, endPosition);
                if (customDistance != 2)
                {
                    this.noSlide = true;
                }
            }
            else
            {
                this.gameObject.GetComponent<RawImage>().enabled = false;
                Button buttonComponent = this.gameObject.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.enabled = false; 
                }
            }
          
        }
        
    }

    private void Update()
    {
    }

    // Startet den Fade-Out-Prozess für ein UI-Element
    public void FadeOutUIElement(Graphic uiElement)
    {
        StartCoroutine(FadeOutCoroutine(uiElement));
    }

    // Coroutine, die das UI-Element langsam ausfadet
    private IEnumerator FadeOutCoroutine(Graphic uiElement)
    {
        Color originalColor = uiElement.color;
        float startAlpha = originalColor.a; // Berücksichtige die aktuelle Transparenz
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alphaValue = Mathf.Lerp(startAlpha, 0, elapsedTime / fadeDuration); // Von startAlpha zu 0
            uiElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, alphaValue);
            yield return null; // Warten auf den nächsten Frame
        }

        uiElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        Button buttonComponent = uiElement.gameObject.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.enabled = false;
        }

        // Deaktiviere das GameObject
        uiElement.gameObject.SetActive(false);
    }


    public void FadeInUIElement(Graphic uiElement)
    {
        uiElement.gameObject.SetActive(true);
        Color color = uiElement.color;
        color.a = 1;
        uiElement.color = color;
        StartCoroutine(FadeInCoroutine(uiElement));
    }

    // Coroutine, die das UI-Element langsam einfadet
    private IEnumerator FadeInCoroutine(Graphic uiElement)
    {
        Color originalColor = uiElement.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alphaValue = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            uiElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, alphaValue);
            yield return null; // Warten auf den nächsten Frame
        }

        // Am Ende des Fade-Ins vollständig sichtbar setzen
        uiElement.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
        Button buttonComponent = this.gameObject.GetComponent<Button>();
        if (buttonComponent != null)
        {
            buttonComponent.enabled = true; // Button aktivieren, wenn vorhanden
        }
    }


    // Methode zum Hereinsliden des UI-Elements von einer bestimmten Richtung
    public void SlideInUIElement(RectTransform uiElement, Vector2 fromPosition, Vector2 toPosition)
    {
        StartCoroutine(SlideInCoroutine(uiElement, fromPosition, toPosition));
    }

    // Coroutine, die das UI-Element von einer Off-Screen-Position in die Zielposition gleiten lässt
    private IEnumerator SlideInCoroutine(RectTransform uiElement, Vector2 fromPosition, Vector2 toPosition)
    {
        float elapsedTime = 0f;
        uiElement.anchoredPosition = fromPosition;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            uiElement.anchoredPosition = Vector2.Lerp(fromPosition, toPosition, elapsedTime / slideDuration);
            yield return null;
        }

        // Am Ende der Animation die Zielposition setzen
        uiElement.anchoredPosition = toPosition;
    }

    // Methode zum Heraus-Sliden des UI-Elements aus der Szene
    public void SlideOutUIElement(RectTransform uiElement, Vector2 fromPosition, Vector2 outPosition)
    {
        StartCoroutine(SlideOutCoroutine(uiElement, fromPosition, outPosition));
    }

    // Coroutine, die das UI-Element von der aktuellen Position nach draußen gleiten lässt
    private IEnumerator SlideOutCoroutine(RectTransform uiElement, Vector2 fromPosition, Vector2 outPosition)
    {
        float elapsedTime = 0f;
        uiElement.anchoredPosition = fromPosition;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            uiElement.anchoredPosition = Vector2.Lerp(fromPosition, outPosition, elapsedTime / slideDuration);
            yield return null;
        }

        // Am Ende der Animation die Zielposition setzen (Out-Position)
        uiElement.anchoredPosition = outPosition;
    }

    // Diese Methode wird durch den Button-Click aufgerufen
    public void FadeOutOnClick(Graphic uiElement)
    {
        FadeOutUIElement(uiElement); 
    }

    public void FadeInOnClick(Graphic uiElement)
    {
        this.gameObject.GetComponent<RawImage>().enabled = true;
        FadeInUIElement(uiElement);
    }

    public void ActivateOnClick(GameObject otherGameObject)
    {
        this.gameObject.GetComponent<RawImage>().enabled = true;
        otherGameObject.SetActive(true);
        if (!noSlide)
        {
            if (down)
            {
                startPosition = new Vector2(myImageRectTransform.anchoredPosition.x, Screen.height - endPosition.y);
            }
            else
            {
                startPosition = new Vector2(myImageRectTransform.anchoredPosition.x, -Screen.height + endPosition.y);
            }
            SlideInUIElement(myImageRectTransform, startPosition, endPosition);
        }
    }

    public void DeactivateOnClick(GameObject gameObject)
    {
        RectTransform uiElement = myImageRectTransform; // Das UI-Element, das du herausschieben willst
        if (!noSlide)
        {
            Vector2 currentPos = uiElement.anchoredPosition; // Aktuelle Position
            Vector2 outPos;
            if (down)
            {
                outPos = new Vector2(currentPos.x, Screen.height - endPosition.y);
            }
            else
            {
                outPos = new Vector2(currentPos.x, -Screen.height + endPosition.y);
            }

            SlideOutUIElement(uiElement, currentPos, outPos);
        } else {
            this.gameObject.GetComponent<RawImage>().enabled = false;
        } 
    }
}
