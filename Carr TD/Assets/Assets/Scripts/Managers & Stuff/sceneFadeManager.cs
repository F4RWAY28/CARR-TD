using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class sceneFadeManager : MonoBehaviour
{
    public static sceneFadeManager Instance;

    [Header("UI")]
    public Image blackScreen;             // The black Image that will fade
    public TMP_Text gameOverText;         // "Game Over" TMP
    public TMP_Text wavesText;            // Waves reached TMP

    [Header("Settings")]
    public float fadeDuration = 1f;       // Fade time
    public float gameOverFadeDuration = 1f; // Fade in/out of game over texts
    public float displayDuration = 2f;    // How long the texts stay visible
    public string gameOverSceneName;      // <<<< Set this in the inspector (Game Over scene name)
    public string mainMenuSceneName;      // <<<< Optional, main menu scene name

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        // Ensure the black screen starts transparent
        if (blackScreen != null)
            blackScreen.color = new Color(0, 0, 0, 0);
    }

    // Call this from gameManager when money reaches 0
    public void TriggerGameOver(int wavesReached, string sceneToLoad)
    {
        StartCoroutine(GameOverSequence(wavesReached, sceneToLoad));
    }

    private IEnumerator GameOverSequence(int wavesReached, string sceneToLoad)
    {
        // Fade black screen in
        if (blackScreen != null)
        {
            float elapsed = 0f;
            Color c = blackScreen.color;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                blackScreen.color = c;
                yield return null;
            }
            c.a = 1f;
            blackScreen.color = c;
        }

        // Show "Game Over" and waves reached
        if (gameOverText != null)
        {
            gameOverText.alpha = 0;
            gameOverText.gameObject.SetActive(true);
        }

        if (wavesText != null)
        {
            wavesText.text = "Waves Reached: " + wavesReached;
            wavesText.alpha = 0;
            wavesText.gameObject.SetActive(true);
        }

        // Fade texts in
        float textElapsed = 0f;
        while (textElapsed < gameOverFadeDuration)
        {
            textElapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(textElapsed / gameOverFadeDuration);
            if (gameOverText != null) gameOverText.alpha = t;
            if (wavesText != null) wavesText.alpha = t;
            yield return null;
        }

        // Wait for display duration
        yield return new WaitForSecondsRealtime(displayDuration);

        // Fade texts out
        textElapsed = 0f;
        while (textElapsed < gameOverFadeDuration)
        {
            textElapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(textElapsed / gameOverFadeDuration);
            if (gameOverText != null) gameOverText.alpha = 1 - t;
            if (wavesText != null) wavesText.alpha = 1 - t;
            yield return null;
        }

        // Load the Game Over scene
        if (!string.IsNullOrEmpty(sceneToLoad))
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
}
