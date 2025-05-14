using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : MonoBehaviour
{
    public Image fadeImage;
    public float fadeSpeed = 1.5f;
    public CanvasGroup uiCanvasGroup;
    
    private IEnumerator FadeOut()
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

    private IEnumerator FadeIn()
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
        

        yield return StartCoroutine(FadeIn());
    }
}
