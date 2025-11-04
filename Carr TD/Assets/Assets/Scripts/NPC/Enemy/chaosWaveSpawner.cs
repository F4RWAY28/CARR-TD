using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class chaosWaveSpawner : MonoBehaviour
{
    public static chaosWaveSpawner Instance;

    [Header("Spawner Settings")]
    public Transform spawnPoint;
    public Transform pathParent;
    public float timeBetweenEnemies = 0.5f;
    public float timeBetweenWaves = 3f;
    public float firstWaveCountdown = 10f;
    public float nextWaveCountdown = 5f;
    [Tooltip("How much faster waves spawn each time (e.g., 0.9 = 10% faster each wave)")]
    [Range(0.5f, 1f)] public float waveSpeedMultiplier = 0.9f; // ✅ Each wave is 10% faster

    [Header("Waves")]
    public waveData[] waves;

    [Header("UI")]
    public TMP_Text waveText;
    public TMP_Text countdownText;

    [Header("Sounds")]
    public AudioClip countdownTickSound;
    public AudioClip waveStartSound;
    public AudioSource audioSource;

    private Transform[] waypoints;
    private int currentWaveIndex = 0;
    private bool timerStarted = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (pathParent != null)
        {
            int count = pathParent.childCount;
            waypoints = new Transform[count];
            for (int i = 0; i < count; i++)
                waypoints[i] = pathParent.GetChild(i);
        }

        if (waveText != null)
        {
            waveText.color = new Color(waveText.color.r, waveText.color.g, waveText.color.b, 0f);
            waveText.gameObject.SetActive(false);
        }

        if (countdownText != null)
        {
            countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, 0f);
            countdownText.gameObject.SetActive(false);
        }

        if (waves != null && waves.Length > 0)
            StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        if (countdownText != null)
            yield return StartCoroutine(CountdownRoutine(firstWaveCountdown));

        for (int w = 0; w < waves.Length; w++)
        {
            if (gameManager.Instance != null && gameManager.Instance.GetMoney() <= 0)
            {
                if (gameSessionManager.Instance != null)
                    gameSessionManager.Instance.playerWon = false;

                StopTimerIfRunning();
                TriggerGameOver();
                yield break;
            }

            currentWaveIndex = w + 1;

            // Start timer only once
            if (!timerStarted && timer.Instance != null)
            {
                timer.Instance.StartTimer();
                timerStarted = true;
            }

            if (gameSessionManager.Instance != null)
                gameSessionManager.Instance.wavesReached = currentWaveIndex;

            if (waveStartSound != null && audioSource != null)
                audioSource.PlayOneShot(waveStartSound);

            if (waveText != null)
                yield return StartCoroutine(ShowWaveTextAndFlashColor(currentWaveIndex));

            waveData wave = waves[w];
            if (wave.enemies == null || wave.enemies.Count == 0)
            {
                Debug.LogWarning($"Wave {currentWaveIndex} has no enemies assigned!");
                continue;
            }

            // Randomize and shuffle enemies
            List<enemyData> randomizedEnemies = new List<enemyData>();
            for (int i = 0; i < wave.enemiesInWave; i++)
                randomizedEnemies.Add(wave.enemies[Random.Range(0, wave.enemies.Count)]);

            for (int i = 0; i < randomizedEnemies.Count; i++)
            {
                enemyData temp = randomizedEnemies[i];
                int randomIndex = Random.Range(i, randomizedEnemies.Count);
                randomizedEnemies[i] = randomizedEnemies[randomIndex];
                randomizedEnemies[randomIndex] = temp;
            }

            // Spawn enemies
            foreach (enemyData data in randomizedEnemies)
            {
                if (gameManager.Instance != null && gameManager.Instance.GetMoney() <= 0)
                {
                    if (gameSessionManager.Instance != null)
                        gameSessionManager.Instance.playerWon = false;

                    StopTimerIfRunning();
                    TriggerGameOver();
                    yield break;
                }

                if (data == null || data.enemyPrefab == null)
                {
                    Debug.LogWarning($"chaosWaveSpawner: enemyData or prefab is null in wave {currentWaveIndex}");
                    continue;
                }

                GameObject go = Instantiate(data.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                enemyPathing pathing = go.GetComponent<enemyPathing>();
                if (pathing != null)
                {
                    pathing.lives = data.lives;
                    pathing.moneyReward = data.moneyReward;
                    pathing.moneyLoss = data.moneyLoss;
                    pathing.waypoints = waypoints;
                }
                else
                    Debug.LogError("chaosWaveSpawner: prefab missing enemyPathing: " + go.name);

                yield return new WaitForSeconds(timeBetweenEnemies);
            }

            // Countdown before next wave (except last)
            if (w < waves.Length - 1 && countdownText != null)
                yield return StartCoroutine(CountdownRoutine(nextWaveCountdown));

            // ✅ Speed up next wave
            timeBetweenWaves *= waveSpeedMultiplier;
            timeBetweenEnemies *= waveSpeedMultiplier;
            nextWaveCountdown *= waveSpeedMultiplier;
        }

        // Wait for all enemies to die
        while (FindObjectsOfType<enemyPathing>().Length > 0)
            yield return null;

        // Player won
        if (gameSessionManager.Instance != null)
        {
            gameSessionManager.Instance.playerWon = true;
            gameSessionManager.Instance.wavesReached = currentWaveIndex;
        }

        StopTimerIfRunning();
        TriggerGameOver();
    }

    private void StopTimerIfRunning()
    {
        if (timer.Instance != null)
            timer.Instance.StopTimer();
    }

    private void TriggerGameOver()
    {
        sceneFader fader = FindObjectOfType<sceneFader>();
        if (fader != null)
            fader.FadeToScene();
        else
            Debug.LogWarning("No sceneFader found in the scene!");

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
            countdownText.text = "";
        }
    }

    private IEnumerator CountdownRoutine(float duration)
    {
        if (countdownText == null) yield break;

        countdownText.gameObject.SetActive(true);
        countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, 0f);

        float fadeInDuration = 0.75f;
        float tFade = 0f;
        while (tFade < fadeInDuration)
        {
            tFade += Time.deltaTime;
            Color c = countdownText.color;
            c.a = Mathf.Clamp01(tFade / fadeInDuration);
            countdownText.color = c;
            yield return null;
        }

        float remaining = duration;
        float pulseSpeed = 8f;
        float lastSecond = Mathf.CeilToInt(remaining);

        while (remaining > 0f)
        {
            if (gameManager.Instance != null && gameManager.Instance.GetMoney() <= 0)
                break;

            int seconds = Mathf.CeilToInt(remaining);
            countdownText.text = "Next Wave In: " + seconds;

            if (seconds < lastSecond)
            {
                lastSecond = seconds;
                if (countdownTickSound != null && audioSource != null)
                    audioSource.PlayOneShot(countdownTickSound);
            }

            float progress = 1f - (remaining / duration);
            Color c = Color.Lerp(Color.green, Color.red, progress);
            c.a = countdownText.color.a;
            countdownText.color = c;

            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * 0.05f;
            countdownText.rectTransform.localScale = Vector3.one * pulse;

            remaining -= Time.deltaTime;
            yield return null;
        }

        // Fade out
        float fadeOutDuration = 0.5f;
        float elapsedOut = 0f;
        while (elapsedOut < fadeOutDuration)
        {
            elapsedOut += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, elapsedOut / fadeOutDuration);
            Color c = countdownText.color;
            c.a = a;
            countdownText.color = c;
            yield return null;
        }

        countdownText.text = "";
        countdownText.rectTransform.localScale = Vector3.one;
        countdownText.gameObject.SetActive(false);
    }

    private IEnumerator ShowWaveTextAndFlashColor(int waveNumber)
    {
        if (waveText == null) yield break;

        waveText.gameObject.SetActive(true);
        waveText.text = "Wave: " + waveNumber;

        Color originalColor = waveText.color;
        originalColor.a = 0f;
        waveText.color = originalColor;

        float fadeInDur = 0.6f;
        float elapsed = 0f;

        while (elapsed < fadeInDur)
        {
            elapsed += Time.deltaTime;
            float a = Mathf.Clamp01(elapsed / fadeInDur);
            Color c = waveText.color;
            c.a = a;
            waveText.color = c;
            yield return null;
        }

        waveText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        Color blueColor = new Color(0.2f, 0.6f, 1f, 1f);
        float flashDur = 0.2f;

        elapsed = 0f;
        while (elapsed < flashDur)
        {
            elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(elapsed / flashDur);
            waveText.color = Color.Lerp(originalColor, blueColor, p);
            yield return null;
        }

        yield return new WaitForSeconds(0.15f);

        elapsed = 0f;
        while (elapsed < flashDur)
        {
            elapsed += Time.deltaTime;
            float p = Mathf.Clamp01(elapsed / flashDur);
            waveText.color = Color.Lerp(blueColor, originalColor, p);
            yield return null;
        }

        waveText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
    }

    public int GetCurrentWave()
    {
        return currentWaveIndex;
    }
}
