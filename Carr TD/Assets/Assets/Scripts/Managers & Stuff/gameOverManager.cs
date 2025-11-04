using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class gameOverManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text gameOverText;
    public TMP_Text wavesReachedText;

    [Header("Game Over Text Settings")]
    public float gameOverFadeInDelay = 0f;
    public float gameOverFadeInDuration = 1f;
    public float gameOverVisibleDuration = 2f;
    public float gameOverFadeOutDuration = 1f;

    [Header("Waves Reached Text Settings")]
    public float wavesFadeInDelay = 0.5f; // delay after gameOverText starts
    public float wavesFadeInDuration = 1.5f;
    public float wavesVisibleDuration = 3f;
    public float wavesFadeOutDuration = 1.5f;

    [Header("Next Scene")]
    public string nextSceneName;

    private void Start()
    {
        // Make sure texts are invisible at start
        SetTextAlpha(gameOverText, 0f);
        SetTextAlpha(wavesReachedText, 0f);

        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        bool playerWon = gameSessionManager.Instance != null && gameSessionManager.Instance.playerWon;

        // Determine waves reached
        int wavesReached = 0;
        if (waveSpawner.Instance != null)
            wavesReached = waveSpawner.Instance.GetCurrentWave();

        if (gameSessionManager.Instance != null)
            wavesReached = Mathf.Max(wavesReached, gameSessionManager.Instance.wavesReached);

        // Set text and colors
        if (gameOverText != null)
        {
            gameOverText.text = playerWon ? "You Win!" : "Game Over";
            gameOverText.color = playerWon ? Color.green : Color.red;
        }

        if (wavesReachedText != null)
        {
            wavesReachedText.text = "Waves Reached: " + wavesReached;
            wavesReachedText.color = playerWon ? Color.green : Color.red;
        }

        // Fade Game Over Text
        if (gameOverText != null)
            StartCoroutine(FadeTextWithDelay(gameOverText, 0f, 1f, gameOverFadeInDelay, gameOverFadeInDuration, gameOverVisibleDuration, gameOverFadeOutDuration));

        // Fade Waves Reached Text
        if (wavesReachedText != null)
            StartCoroutine(FadeTextWithDelay(wavesReachedText, 0f, 1f, wavesFadeInDelay, wavesFadeInDuration, wavesVisibleDuration, wavesFadeOutDuration));

        // Wait for the longest total duration before loading next scene
        float totalGameOverTime = gameOverFadeInDelay + gameOverFadeInDuration + gameOverVisibleDuration + gameOverFadeOutDuration;
        float totalWavesTime = wavesFadeInDelay + wavesFadeInDuration + wavesVisibleDuration + wavesFadeOutDuration;
        yield return new WaitForSecondsRealtime(Mathf.Max(totalGameOverTime, totalWavesTime));

        // Load next scene
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator FadeTextWithDelay(TMP_Text text, float startAlpha, float endAlpha, float delay, float fadeInDuration, float visibleDuration, float fadeOutDuration)
    {
        if (text == null) yield break;

        // Delay before fade-in
        if (delay > 0f)
            yield return new WaitForSecondsRealtime(delay);

        // Fade in
        yield return StartCoroutine(FadeText(text, startAlpha, endAlpha, fadeInDuration));

        // Stay visible
        if (visibleDuration > 0f)
            yield return new WaitForSecondsRealtime(visibleDuration);

        // Fade out
        yield return StartCoroutine(FadeText(text, endAlpha, 0f, fadeOutDuration));
    }

    private IEnumerator FadeText(TMP_Text text, float startAlpha, float endAlpha, float duration)
    {
        if (text == null) yield break;

        Color original = text.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            text.color = new Color(original.r, original.g, original.b, alpha);
            yield return null;
        }

        text.color = new Color(original.r, original.g, original.b, endAlpha);
    }

    private void SetTextAlpha(TMP_Text text, float alpha)
    {
        if (text == null) return;
        Color c = text.color;
        c.a = alpha;
        text.color = c;
    }
}
