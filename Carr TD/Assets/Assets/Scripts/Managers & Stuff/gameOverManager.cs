using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class gameOverManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text gameOverText;
    public TMP_Text wavesReachedText;

    [Header("Settings")]
    public float fadeDuration = 1f;      // Time for text to fade in/out
    public float visibleDuration = 2f;   // How long text stays fully visible
    public string nextSceneName;         // Scene to load after fade out

    private void Start()
    {
        // Start invisible
        if (gameOverText != null) gameOverText.alpha = 0f;
        if (wavesReachedText != null) wavesReachedText.alpha = 0f;

        // Start the sequence
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // Fade in texts
        yield return StartCoroutine(FadeText(gameOverText, 0f, 1f, fadeDuration));
        yield return StartCoroutine(FadeText(wavesReachedText, 0f, 1f, fadeDuration));

        // Wait while fully visible
        yield return new WaitForSecondsRealtime(visibleDuration);

        // Fade out texts
        yield return StartCoroutine(FadeText(gameOverText, 1f, 0f, fadeDuration));
        yield return StartCoroutine(FadeText(wavesReachedText, 1f, 0f, fadeDuration));

        // Load next scene
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FadeText(TMP_Text text, float startAlpha, float endAlpha, float duration)
    {
        if (text == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            text.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }
        text.alpha = endAlpha;
    }
}