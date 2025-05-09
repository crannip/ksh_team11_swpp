using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance;
    public Image fadeImage;
    public float fadeSpeed = 1.5f;
    public CanvasGroup uiCanvasGroup; // Reference to the CanvasGroup

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public IEnumerator FadeOut()
    {
        float alpha = 0;
        fadeImage.color = new Color(0, 0, 0, 0);
        uiCanvasGroup.alpha = 1; // Ensure UI is fully visible at start

        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            uiCanvasGroup.alpha = 1 - alpha; // Fade out UI
            yield return null;
        }

        uiCanvasGroup.alpha = 0; // Ensure UI is fully transparent at end
    }

    public IEnumerator FadeIn()
    {
        float alpha = 1;
        fadeImage.color = new Color(0, 0, 0, 1);
        uiCanvasGroup.alpha = 0; // Ensure UI is hidden at start

        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            uiCanvasGroup.alpha = 1 - alpha; // Fade in UI
            yield return null;
        }

        uiCanvasGroup.alpha = 1; // Ensure UI is fully visible at end
    }

    public IEnumerator FadeAndLoadScene(string sceneName)
    {
        yield return StartCoroutine(FadeOut());

        // Load the new scene
        SceneManager.LoadScene(sceneName);
        yield return null; // wait one frame for scene to load

        // Reassign fadeImage and uiCanvasGroup from the new scene
        fadeImage = GameObject.Find("FadeImage")?.GetComponent<Image>();
        uiCanvasGroup = GameObject.Find("Canvas")?.GetComponent<CanvasGroup>();

        yield return StartCoroutine(FadeIn());
    }

}
