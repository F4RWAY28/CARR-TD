using System.Collections;
using UnityEngine;

public class cameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeDuration = 1f;
    public float shakeStrength = 0.2f;
    public AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Trigger")]
    public bool start = false;

    private bool isShaking = false;

    private void Update()
    {
        // Manual trigger from Inspector
        if (start)
        {
            start = false;
            StartCoroutine(Shaking());
        }
    }

    public IEnumerator Shaking()
    {
        if (isShaking)
            yield break;

        isShaking = true;
        float elapsedTime = 0f;

        // Capture the current position as the "base" right before shaking
        Vector3 baseLocalPosition = transform.localPosition;

        while (elapsedTime < shakeDuration)
        {
            elapsedTime += Time.deltaTime;

            float strength = animCurve.Evaluate(elapsedTime / shakeDuration) * shakeStrength;
            Vector3 shakeOffset = Random.insideUnitSphere * strength;

            // Always shake relative to current base position
            transform.localPosition = baseLocalPosition + shakeOffset;

            yield return null;
        }

        // Smoothly reset to wherever the camera was when shaking started
        transform.localPosition = baseLocalPosition;

        isShaking = false;
    }
}

