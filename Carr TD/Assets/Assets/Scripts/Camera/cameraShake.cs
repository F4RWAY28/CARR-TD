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
    private Vector3 shakeOffset = Vector3.zero;
    private Vector3 basePosition;

    private void Update()
    {
        // Store the base position *before* applying shake
        basePosition = transform.position;

        // Apply current shake offset
        transform.position = basePosition + shakeOffset;

        // Start shake if triggered
        if (start)
        {
            start = false;
            StartCoroutine(Shaking());
        }
    }

    public IEnumerator Shaking()
    {
        if (isShaking) yield break;
        isShaking = true;

        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            elapsedTime += Time.deltaTime;

            float strength = animCurve.Evaluate(elapsedTime / shakeDuration) * shakeStrength;
            shakeOffset = Random.insideUnitSphere * strength;

            yield return null;
        }

        // Reset shake
        shakeOffset = Vector3.zero;
        isShaking = false;
    }
}

