using UnityEngine;
using System.Collections;

public class enemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject[] enemyPrefabs;
    public Transform pathParent;        // parent containing waypoint transforms
    public float spawnInterval = 1f;    // time between spawns
    public int enemiesPerWave = 5;      // how many per wave
    public float timeBetweenWaves = 3f; // break between waves

    private Transform[] waypoints;

    void Start()
    {
        if (pathParent == null)
        {
            Debug.LogError("EnemySpawner: PathParent not assigned!");
            return;
        }

        // Build waypoints
        int count = pathParent.childCount;
        waypoints = new Transform[count];
        for (int i = 0; i < count; i++)
            waypoints[i] = pathParent.GetChild(i);

        if (waypoints.Length == 0)
        {
            Debug.LogError("EnemySpawner: No waypoints found under PathParent!");
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        int wave = 1;

        while (true) // infinite waves
        {
            Debug.Log($"--- Starting Wave {wave} ---");

            for (int i = 0; i < enemiesPerWave; i++)
            {
                // Pick random prefab
                GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
                GameObject enemy = Instantiate(prefab, waypoints[0].position, Quaternion.identity);

                enemyPathing pathing = enemy.GetComponent<enemyPathing>();
                if (pathing != null)
                {
                    pathing.waypoints = waypoints;
                    Debug.Log($"Spawned {enemy.name} with {pathing.waypoints.Length} waypoints");
                }
                else
                {
                    Debug.LogError("EnemySpawner: Spawned prefab missing enemyPathing script!");
                }

                yield return new WaitForSeconds(spawnInterval);
            }

            Debug.Log($"--- Wave {wave} complete ---");

            wave++;
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }
}
