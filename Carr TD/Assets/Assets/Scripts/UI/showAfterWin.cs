using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class showAfterWin : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // Try to get or add a CanvasGroup to handle fading
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Start hidden and transparent
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    private void Start()
    {
        // If the player has won before, fade the button in
        if (PlayerPrefs.GetInt("PlayerHasWon", 0) == 1)
        {
            gameObject.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    // ✅ Call this when the player wins
    public static void MarkAsWon()
    {
        PlayerPrefs.SetInt("PlayerHasWon", 1);
        PlayerPrefs.Save();
    }

    // ✅ Optional: call this to reset win status (e.g., from a reset button)
    public static void ResetWin()
    {
        PlayerPrefs.DeleteKey("PlayerHasWon");
        PlayerPrefs.Save();
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}
