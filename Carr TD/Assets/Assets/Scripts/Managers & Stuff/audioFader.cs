using System.Collections;
using UnityEngine;

public class audioFader : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Reference to the game manager to check money.")]
    public gameManager gameManager;

    [Tooltip("Duration of the global audio fade (seconds).")]
    public float fadeDuration = 1f;

    [Tooltip("Optional: fade out music only, if assigned.")]
    public AudioSource musicSource;

    private bool hasFaded = false;

    private void Update()
    {
        if (hasFaded) return;

        if (gameManager != null && gameManager.GetMoney() <= 0)
        {
            hasFaded = true;
            StartCoroutine(FadeOutAllAudio());
        }
    }

    private IEnumerator FadeOutAllAudio()
    {
        // Find all AudioSources in the scene
        AudioSource[] sources = FindObjectsOfType<AudioSource>();
        float[] startVolumes = new float[sources.Length];

        for (int i = 0; i < sources.Length; i++)
            startVolumes[i] = sources[i].volume;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            for (int i = 0; i < sources.Length; i++)
                sources[i].volume = Mathf.Lerp(startVolumes[i], 0f, t);

            yield return null;
        }

        // Ensure all volumes are zero
        foreach (var s in sources)
            s.volume = 0f;
    }
}
