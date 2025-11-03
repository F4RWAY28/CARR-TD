using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class sceneFader : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("The Image component covering the screen (should be black).")]
    public Image fadeImage;

    [Tooltip("Duration of fade in (black -> scene).")]
    public float fadeInDuration = 1f;

    [Tooltip("Duration of fade out (scene -> black).")]
    public float fadeOutDuration = 1f;

    [Tooltip("Time to hold fully black before fading in or switching scene.")]
    public float holdBlackTime = 0.5f;

    [Header("Scene Settings")]
    [Tooltip("Name of the scene to load when calling FadeToScene().")]
    public string targetSceneName;

    private void Awake()
    {
        if (fadeImage != null)
        {
            // Start fully black
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(true);

            // Fade in scene
            StartCoroutine(FadeIn());
        }
    }

    public void FadeToScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
            StartCoroutine(FadeOutAndLoad(targetSceneName));
        else
            Debug.LogWarning("SceneFader: No target scene name set in inspector!");
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        // Ensure image is visible
        fadeImage.gameObject.SetActive(true);

        // Hold black if needed
        if (holdBlackTime > 0f)
            yield return new WaitForSecondsRealtime(holdBlackTime);

        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / fadeInDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(0f);

        // Disable image after fade-in
        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        if (fadeImage == null) yield break;

        // Enable image at start
        fadeImage.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeOutDuration);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(1f);

        // Hold black if needed
        if (holdBlackTime > 0f)
            yield return new WaitForSecondsRealtime(holdBlackTime);

        SceneManager.LoadScene(sceneName);
    }

    private void SetAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = alpha;
            fadeImage.color = c;
        }
    }
}
