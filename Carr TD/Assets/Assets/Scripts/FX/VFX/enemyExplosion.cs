using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject explosionPrefab; // Drag your explosion prefab here
    public float explosionScale = 1f;  // Adjust explosion size
    public enemyPathing enemyScript;   // Reference to the enemyPathing script

    private cameraShake shakeScript;   // Will be automatically assigned
    private bool hasExploded = false;

    void Start()
    {
        if (enemyScript == null)
            enemyScript = GetComponent<enemyPathing>();

        // Automatically find the CameraShake script on the Main Camera
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            shakeScript = mainCam.GetComponent<cameraShake>();
            if (shakeScript == null)
                Debug.LogWarning("No CameraShake component found on Main Camera!");
        }
        else
        {
            Debug.LogWarning("No Main Camera found in the scene!");
        }
    }

    void Update()
    {
        if (enemyScript == null) return;

        // Check if enemy is dead
        if (!hasExploded && IsDead(enemyScript))
        {
            Explode();
        }
    }

    bool IsDead(enemyPathing enemy)
    {
        // Access private 'dead' field using reflection
        var deadField = typeof(enemyPathing).GetField("dead",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return deadField != null && (bool)deadField.GetValue(enemy);
    }

    void Explode()
    {
        hasExploded = true;

        // Spawn explosion prefab
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * explosionScale;
        }
        else
        {
            Debug.LogWarning($"{name}: No explosion prefab assigned!");
        }

        // Trigger camera shake if available
        if (shakeScript != null)
        {
            StartCoroutine(shakeScript.Shaking());
        }
    }
}
