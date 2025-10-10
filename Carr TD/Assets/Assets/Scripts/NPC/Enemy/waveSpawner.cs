using System.Collections;
using TMPro;
using UnityEngine;

public class waveSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public Transform spawnPoint;
    public Transform pathParent;
    public float timeBetweenEnemies = 0.5f;
    public float timeBetweenWaves = 3f;

    [Header("Waves")]
    public waveData[] waves;

    [Header("UI")]
    public TMP_Text waveText;

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
        for (int w = 0; w < waves.Length; w++)
        {
            // Use the wave index (w+1) so you always see Wave 1, Wave 2, etc.
            if (waveText != null)
                waveText.text = "Wave: " + (w + 1);

            waveData wave = waves[w];

            for (int i = 0; i < wave.enemiesInWave; i++)
            {
                // Round-robin enemy selection
                enemyData data = wave.enemies[i % wave.enemies.Count];
                if (data == null || data.enemyPrefab == null)
                {
                    Debug.LogWarning("waveSpawner: enemyData or prefab is null in wave " + (w + 1));
                    continue;
                }

                GameObject go = Instantiate(data.enemyPrefab, spawnPoint.position, spawnPoint.rotation);

                // Apply enemy stats + waypoints
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

            // Wait until all enemies are gone before starting next wave
            while (GameObject.FindGameObjectsWithTag("Enemy").Length > 0)
                yield return null;

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        if (waveText != null)
            waveText.text = "All Waves Complete!";
    }
}
