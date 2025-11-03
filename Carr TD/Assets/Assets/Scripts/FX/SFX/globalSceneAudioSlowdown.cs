using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GlobalSceneAudioSlowdown : MonoBehaviour
{
    [Header("Slowdown Settings")]
    public float slowdownDuration = 2f; // Duration for audio to stop
    public bool stopAudioAtEnd = true;  // Completely stop audio after slowdown

    private static GlobalSceneAudioSlowdown instance;

    private void Awake()
    {
        // Singleton pattern to persist across scenes
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Listen for any scene changes
        SceneManager.activeSceneChanged += OnSceneChange;
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        // Start coroutine to slow down all audio in the old scene
        StartCoroutine(SlowdownAllAudio());
    }

    private IEnumerator SlowdownAllAudio()
    {
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        float[] originalPitches = new float[allAudioSources.Length];
        float[] originalVolumes = new float[allAudioSources.Length];

        for (int i = 0; i < allAudioSources.Length; i++)
        {
            originalPitches[i] = allAudioSources[i].pitch;
            originalVolumes[i] = allAudioSources[i].volume;
        }

        float elapsed = 0f;

        while (elapsed < slowdownDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / slowdownDuration);
            float pitchMultiplier = Mathf.Lerp(1f, 0f, t);
            float volumeMultiplier = Mathf.Lerp(1f, 0f, t);

            for (int i = 0; i < allAudioSources.Length; i++)
            {
                if (allAudioSources[i] != null)
                {
                    allAudioSources[i].pitch = originalPitches[i] * pitchMultiplier;
                    allAudioSources[i].volume = originalVolumes[i] * volumeMultiplier;
                }
            }

            yield return null;
        }

        // Optionally stop all audio completely
        if (stopAudioAtEnd)
        {
            foreach (AudioSource source in allAudioSources)
            {
                if (source != null)
                {
                    source.pitch = 1f;   // Reset pitch
                    source.volume = 1f;  // Reset volume
                    source.Stop();
                }
            }
        }
    }
}
