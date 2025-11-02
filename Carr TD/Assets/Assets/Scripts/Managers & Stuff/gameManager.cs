using UnityEngine;
using TMPro;
using System.Collections;

public class gameManager : MonoBehaviour
{
    public static gameManager Instance;

    [Header("Player Stats")]
    public int money = 1000;
    private int actualMoney;

    [Header("UI")]
    public TMP_Text moneyText;

    [Header("Money Animation Settings")]
    public float unitsPerSecond = 200f;
    public float scaleUpFactor = 1.2f;
    public AnimationCurve easeCurve;

    [Header("Sound")]
    public AudioClip moneySound;
    public AudioClip errorSound;
    public float pitchStep = 0.03f;
    public float soundVolume = 0.7f;

    private Vector3 originalScale;
    private Color originalColor;
    private float currentPitch = 1f;
    private bool isAnimating = false;
    private bool isFlashingRed = false;
    private bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        actualMoney = money;

        if (moneyText != null)
        {
            originalScale = moneyText.rectTransform.localScale;
            originalColor = moneyText.color;
            UpdateUIImmediate();
        }
    }

    #region Money Handling

    public void AddMoney(int amount)
    {
        if (isGameOver) return; // stop adding money after game over
        actualMoney += amount;
        if (!isAnimating) StartCoroutine(AnimateMoney());
    }

    public bool TrySpendMoney(int amount)
    {
        if (isGameOver) return false;

        if (actualMoney >= amount)
        {
            actualMoney -= amount;
            if (!isAnimating) StartCoroutine(AnimateMoney());
            CheckGameOver();
            return true;
        }
        else
        {
            if (!isFlashingRed) StartCoroutine(FlashRedForInsufficientFunds());
            return false;
        }
    }

    public void ForceSpendMoney(int amount)
    {
        if (isGameOver) return;

        actualMoney -= amount;
        if (actualMoney < 0) actualMoney = 0;

        if (!isAnimating) StartCoroutine(AnimateMoney());
        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (actualMoney <= 0 && !isGameOver)
        {
            isGameOver = true;

            // Save waves reached
            if (waveSpawner.Instance != null)
            {
                gameSessionData.Instance.wavesReached = waveSpawner.Instance.GetCurrentWave();
            }

            // Stop gameplay
            Time.timeScale = 0f;

            // Trigger the scene fade and game over sequence
            if (sceneFadeManager.Instance != null)
            {
                sceneFadeManager.Instance.TriggerGameOver(
                    gameSessionData.Instance.wavesReached,
                    sceneFadeManager.Instance.gameOverSceneName
                );
            }
            else
            {
                Debug.LogWarning("sceneFadeManager instance not found!");
            }
        }
    }

    #endregion

    #region Money Animation

    private IEnumerator AnimateMoney()
    {
        isAnimating = true;

        while (money != actualMoney)
        {
            int oldMoney = money;
            int deltaAmount = actualMoney - money;

            float step = Mathf.Max(1f, Mathf.Abs(deltaAmount)) * unitsPerSecond * Time.deltaTime / 100f;
            money = Mathf.RoundToInt(Mathf.MoveTowards(money, actualMoney, step));
            int delta = money - oldMoney;

            UpdateUIImmediate();

            float remaining = Mathf.Abs(actualMoney - money);
            float scaleT = Mathf.Clamp01(remaining / 100f);
            moneyText.rectTransform.localScale = Vector3.Lerp(originalScale, originalScale * scaleUpFactor, easeCurve.Evaluate(scaleT));

            Color targetColor = delta > 0 ? Color.green : Color.red;
            float colorT = Mathf.Clamp01(remaining / 100f);
            moneyText.color = Color.Lerp(originalColor, targetColor, colorT);

            if (moneySound != null && delta != 0)
            {
                float direction = delta > 0 ? 1f : -1f;
                currentPitch = Mathf.Clamp(currentPitch + direction * pitchStep, 0.6f, 1.4f);
                PlaySound(moneySound, currentPitch);
            }

            yield return null;
        }

        moneyText.rectTransform.localScale = originalScale;
        moneyText.color = originalColor;
        currentPitch = 1f;
        isAnimating = false;
    }

    private IEnumerator FlashRedForInsufficientFunds()
    {
        isFlashingRed = true;
        float elapsed = 0f;
        float flashDuration = 0.5f;

        if (errorSound != null)
            PlaySound(errorSound, 1f);

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.PingPong(elapsed * 4f, 1f);
            moneyText.color = Color.Lerp(originalColor, Color.red, t);
            yield return null;
        }

        moneyText.color = originalColor;
        isFlashingRed = false;
    }

    #endregion

    #region Sound

    private void PlaySound(AudioClip clip, float pitch)
    {
        GameObject soundObj = new GameObject("TempSound");
        AudioSource source = soundObj.AddComponent<AudioSource>();
        source.clip = clip;
        source.pitch = pitch;
        source.volume = soundVolume;
        source.spatialBlend = 0f;
        source.Play();
        Destroy(soundObj, clip.length / Mathf.Max(0.1f, pitch));
    }

    #endregion

    private void UpdateUIImmediate()
    {
        if (moneyText != null)
            moneyText.text = $"Money: {money}";
    }

    public int GetMoney() => money;
}
