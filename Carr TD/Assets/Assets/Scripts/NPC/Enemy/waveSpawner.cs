using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class waveSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public Transform spawnPoint;
    public Transform pathParent;
    public float timeBetweenEnemies = 0.5f;
    public float timeBetweenWaves = 3f;
    public float firstWaveCountdown = 10f; // Countdown before first wave
    public float nextWaveCountdown = 5f;   // Countdown before subsequent waves

    [Header("Waves")]
    public waveData[] waves;

    [Header("UI")]
    public TMP_Text waveText;
    public TMP_Text countdownText; // Countdown display

    private Transform[] waypoints;

    private void Start()
    {
        // Build waypoint array from pathParent
        if (pathParent != null)
        {
            int count = pathParent.childCount;
            waypoints = new Transform[count];
            for (int i = 0; i < count; i++)
                waypoints[i] = pathParent.GetChild(i);
        }

        if (waves != null && waves.Length > 0)
            StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        // Initial countdown before the first wave
        if (countdownText != null)
            yield return StartCoroutine(CountdownRoutine(firstWaveCountdown));

        for (int w = 0; w < waves.Length; w++)
        {
            if (waveText != null)
                waveText.text = "Wave: " + (w + 1);

            waveData wave = waves[w];
            if (wave.enemies == null || wave.enemies.Count == 0)
            {
                Debug.LogWarning($"Wave {w + 1} has no enemies assigned!");
                continue;
            }

            // 🔀 Randomize enemies for this wave
            List<enemyData> randomizedEnemies = new List<enemyData>();
            for (int i = 0; i < wave.enemiesInWave; i++)
                randomizedEnemies.Add(wave.enemies[Random.Range(0, wave.enemies.Count)]);

            // Shuffle order
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
                if (data == null || data.enemyPrefab == null)
                {
                    Debug.LogWarning($"waveSpawner: enemyData or prefab is null in wave {w + 1}");
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
                {
                    Debug.LogError("waveSpawner: prefab does not have enemyPathing attached: " + go.name);
                }

                yield return new WaitForSeconds(timeBetweenEnemies);
            }

            // Countdown before next wave (except last wave)
            if (w < waves.Length - 1 && countdownText != null)
                yield return StartCoroutine(CountdownRoutine(nextWaveCountdown));
        }

        if (waveText != null)
            waveText.text = "All Waves Complete!";
        if (countdownText != null)
            countdownText.text = "";
    }

    private IEnumerator CountdownRoutine(float duration)
    {
        float remaining = duration;
        float pulseSpeed = 8f; // Pulse frequency

        // Initialize countdown text for fade-in
        if (countdownText != null)
        {
            countdownText.alpha = 0f;
            countdownText.gameObject.SetActive(true);
        }

        // Fade-in duration
        float fadeInDuration = 0.5f;
        float elapsedFadeIn = 0f;
        while (elapsedFadeIn < fadeInDuration)
        {
            elapsedFadeIn += Time.deltaTime;
            if (countdownText != null)
                countdownText.alpha = Mathf.Lerp(0f, 1f, elapsedFadeIn / fadeInDuration);
            yield return null;
        }

        // Countdown loop
        while (remaining > 0f)
        {
            if (countdownText != null)
            {
                countdownText.text = "Next Wave In: " + Mathf.CeilToInt(remaining);

                // Color green → red
                float t = 1f - (remaining / duration);
                countdownText.color = Color.Lerp(Color.green, Color.red, t);

                // Pulse animation
                float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * 0.05f;
                countdownText.rectTransform.localScale = Vector3.one * pulse;
            }

            remaining -= Time.deltaTime;
            yield return null;
        }

        // Fade out smoothly
        if (countdownText != null)
        {
            float fadeOutDuration = 0.5f;
            float elapsed = 0f;
            Color originalColor = countdownText.color;

            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                countdownText.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
                yield return null;
            }

            countdownText.text = "";
            countdownText.alpha = 1f;
            countdownText.rectTransform.localScale = Vector3.one;
            countdownText.color = Color.white;
        }
    }
}
