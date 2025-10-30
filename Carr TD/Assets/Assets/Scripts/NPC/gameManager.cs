using UnityEngine;
using TMPro;
using System.Collections;

public class gameManager : MonoBehaviour
{
    public static gameManager Instance;

    [Header("Player Stats")]
    public int money = 1000;

    [Header("UI")]
    public TMP_Text moneyText;

    [Header("Animation Settings")]
    public float baseDurationPerUnit = 0.01f; // Counting speed per money change
    public float scaleUpFactor = 1.2f;        // Pop size
    public AnimationCurve easeCurve;          // Optional, can assign a curve in inspector

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip moneySound;
    public float pitchStep = 0.03f;

    private Coroutine moneyRoutine;
    private float currentPitch = 1f;
    private Vector3 originalScale;
    private Color originalColor;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        originalScale = moneyText.rectTransform.localScale;
        originalColor = moneyText.color;
        UpdateUIImmediate();
    }

    public void AddMoney(int amount)
    {
        StartMoneyChange(money + amount, Color.green);
    }

    public void LoseMoney(int amount)
    {
        StartMoneyChange(Mathf.Max(money - amount, 0), Color.red);
    }

    private void StartMoneyChange(int targetMoney, Color activeColor)
    {
        // Reset before starting new animation
        if (moneyRoutine != null)
            StopCoroutine(moneyRoutine);

        moneyText.rectTransform.localScale = originalScale;
        moneyText.color = originalColor;

        moneyRoutine = StartCoroutine(ChangeMoneyRoutine(targetMoney, activeColor));
    }

    private IEnumerator ChangeMoneyRoutine(int targetMoney, Color activeColor)
    {
        int startMoney = money;
        int delta = Mathf.Abs(targetMoney - startMoney);
        if (delta == 0) yield break;

        // Duration scales with money difference
        float duration = Mathf.Clamp(delta * baseDurationPerUnit, 0.3f, 3f);

        float elapsed = 0f;
        float pitch = currentPitch;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Smooth out the curve (fast → slow)
            float smoothT = easeCurve != null ? easeCurve.Evaluate(t) : Mathf.Sin(t * Mathf.PI * 0.5f);

            // Interpolate money
            money = (int)Mathf.Lerp(startMoney, targetMoney, smoothT);
            UpdateUIImmediate();

            // Play sound
            if (audioSource != null && moneySound != null)
            {
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(moneySound);
                pitch += pitchStep;
            }

            // Pop animation
            float popT = Mathf.Sin(smoothT * Mathf.PI);
            moneyText.rectTransform.localScale = Vector3.Lerp(originalScale, originalScale * scaleUpFactor, popT);

            // Color transition
            moneyText.color = Color.Lerp(activeColor, originalColor, smoothT);

            yield return null;
        }

        // Finalize
        money = targetMoney;
        moneyText.rectTransform.localScale = originalScale;
        moneyText.color = originalColor;
        UpdateUIImmediate();

        currentPitch = 1f; // Reset pitch after finish
        moneyRoutine = null;
    }

    private void UpdateUIImmediate()
    {
        if (moneyText != null)
            moneyText.text = $"Money: {money}";
    }
}
