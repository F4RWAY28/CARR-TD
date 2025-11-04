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
    public float fadeDuration = 1f;
    public float visibleDuration = 2f;
    public string nextSceneName;

    private void Start()
    {
        if (gameOverText != null) gameOverText.alpha = 0f;
        if (wavesReachedText != null) wavesReachedText.alpha = 0f;

        ShowResults();
    }

    private void ShowResults()
    {
        bool playerWon = gameSessionManager.Instance != null && gameSessionManager.Instance.playerWon;
        int wavesReached = gameSessionManager.Instance != null ? gameSessionManager.Instance.wavesReached : 0;

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

        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        yield return StartCoroutine(FadeText(gameOverText, 0f, 1f, fadeDuration));
        yield return StartCoroutine(FadeText(wavesReachedText, 0f, 1f, fadeDuration));

        yield return new WaitForSecondsRealtime(visibleDuration);

        yield return StartCoroutine(FadeText(gameOverText, 1f, 0f, fadeDuration));
        yield return StartCoroutine(FadeText(wavesReachedText, 1f, 0f, fadeDuration));

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