using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class uiMusicBounce : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource; // The music source
    public int spectrumSize = 64;   // Number of samples for the spectrum
    public int spectrumIndex = 1;   // Which frequency band to use for scaling

    [Header("Scaling")]
    public Vector3 baseScale = Vector3.one;   // Normal scale
    public float scaleMultiplier = 2f;        // How much it scales with the music
    public float smoothSpeed = 5f;            // Smooth scaling speed

    private float[] spectrumData;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = FindObjectOfType<AudioSource>();
        }

        spectrumData = new float[spectrumSize];
        transform.localScale = baseScale;
    }

    void Update()
    {
        if (audioSource == null || !audioSource.isPlaying) return;

        // Get spectrum data
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        // Get the amplitude of the chosen frequency band
        float amplitude = spectrumData[Mathf.Clamp(spectrumIndex, 0, spectrumData.Length - 1)] * scaleMultiplier;

        // Compute target scale
        Vector3 targetScale = baseScale + Vector3.one * amplitude;

        // Smoothly scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothSpeed);
    }
}
