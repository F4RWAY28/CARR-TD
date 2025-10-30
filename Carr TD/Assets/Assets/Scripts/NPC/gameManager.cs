using UnityEngine;
using TMPro;
using System.Collections;

public class gameManager : MonoBehaviour
{
    public static gameManager Instance;

    [Header("Player Stats")]
    public int money = 1000;              // Displayed money
    private int actualMoney;              // True money

    [Header("UI")]
    public TMP_Text moneyText;

    [Header("Animation Settings")]
    public float unitsPerSecond = 200f;   // How fast displayed money moves toward actual money
    public float scaleUpFactor = 1.2f;
    public AnimationCurve easeCurve;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip moneySound;
    public float pitchStep = 0.03f;

    private Vector3 originalScale;
    private Color originalColor;
    private float currentPitch = 1f;

    private void Awake()
    {
        Instance = this;
        actualMoney = money;
    }

    private void Start()
    {
        originalScale = moneyText.rectTransform.localScale;
        originalColor = moneyText.color;
        UpdateUIImmediate();
    }

    public void AddMoney(int amount)
    {
        actualMoney += amount;
        if (!IsAnimating) StartCoroutine(AnimateMoney());
    }

    public void LoseMoney(int amount)
    {
        actualMoney = Mathf.Max(actualMoney - amount, 0);
        if (!IsAnimating) StartCoroutine(AnimateMoney());
    }

    private bool IsAnimating = false;

    private IEnumerator AnimateMoney()
    {
        IsAnimating = true;

        while (money != actualMoney)
        {
            // Move displayed money toward actual money
            int direction = actualMoney > money ? 1 : -1;
            float step = unitsPerSecond * Time.deltaTime;
            int newMoney = money + Mathf.Clamp((int)step * direction, direction, Mathf.Abs(actualMoney - money) * direction);
            int delta = newMoney - money;
            money = newMoney;

            // Update UI
            UpdateUIImmediate();

            // Pop animation
            float popT = Mathf.Sin((Mathf.Abs(delta) / unitsPerSecond) * Mathf.PI);
            moneyText.rectTransform.localScale = Vector3.Lerp(originalScale, originalScale * scaleUpFactor, popT);

            // Color feedback
            Color activeColor = delta > 0 ? Color.green : Color.red;
            moneyText.color = Color.Lerp(activeColor, originalColor, Mathf.Abs(delta) / unitsPerSecond);

            // Sound
            if (audioSource != null && moneySound != null)
            {
                audioSource.pitch = currentPitch;
                audioSource.PlayOneShot(moneySound);
                currentPitch += pitchStep;
            }

            yield return null;
        }

        // Reset
        moneyText.rectTransform.localScale = originalScale;
        moneyText.color = originalColor;
        currentPitch = 1f;
        IsAnimating = false;
    }

    private void UpdateUIImmediate()
    {
        if (moneyText != null)
            moneyText.text = $"Money: {money}";
    }
}