using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI")]
    public CanvasGroup screenFader;       // Fullscreen black overlay
    public TMP_Text gameOverText;
    public TMP_Text wavesReachedText;

    [Header("Settings")]
    public float fadeDuration = 1f;
    public float waitBeforeShow = 1f;

    private bool gameOverTriggered = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Start invisible
        if (screenFader != null) screenFader.alpha = 0f;
        if (gameOverText != null) gameOverText.alpha = 0f;
        if (wavesReachedText != null) wavesReachedText.alpha = 0f;
    }

    private void Update()
    {
        // Trigger when money is 0 (only once)
        if (!gameOverTriggered && gameManager.Instance != null && gameManager.Instance.GetMoney() <= 0)
        {
            gameOverTriggered = true;
            StartCoroutine(TriggerGameOver());
        }
    }

    private IEnumerator TriggerGameOver()
    {
        // Stop time
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(waitBeforeShow);

        // Fade screen to black
        if (screenFader != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                screenFader.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            screenFader.alpha = 1f;
        }

        // Show Game Over text
        if (gameOverText != null)
        {
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                gameOverText.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            gameOverText.alpha = 1f;
        }

        // Show Waves Reached text
        if (wavesReachedText != null)
        {
            int waves = FindObjectOfType<waveSpawner>() != null ? FindObjectOfType<waveSpawner>().waves.Length : 0;
            wavesReachedText.text = $"Waves Reached: {waves}";

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                wavesReachedText.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                yield return null;
            }
            wavesReachedText.alpha = 1f;
        }
    }
}
