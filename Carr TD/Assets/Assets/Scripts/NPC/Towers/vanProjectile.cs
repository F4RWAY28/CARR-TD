using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class vanProjectile : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 2f;
    public GameObject explosionPrefab;      // Explosion prefab
    public float explosionScale = 1f;       // Scale of the explosion
    public float collisionDelay = 0.5f;     // Time in seconds before collisions are active
    public AudioClip[] explosionSounds;     // Drag multiple sounds here

    private Rigidbody rb;
    private cameraShake shakeScript;        // Automatically assigned
    private AudioSource audioSource;        // Play explosion sound from prefab
    private bool hasCollided = false;
    private bool canCollide = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D sound

        // Ignore collisions with Van1 and Van2 objects
        Collider[] allColliders = FindObjectsOfType<Collider>();
        foreach (var col in allColliders)
        {
            if (col.CompareTag("Van1") || col.CompareTag("Van2"))
                Physics.IgnoreCollision(GetComponent<Collider>(), col);
        }
    }

    void Start()
    {
        Destroy(gameObject, lifetime); // Auto-destroy after lifetime

        // Automatically find the CameraShake script on Main Camera
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

        // Start the collision delay
        Invoke(nameof(EnableCollision), collisionDelay);
    }

    private void EnableCollision()
    {
        canCollide = true;
    }

    public void Launch(Vector3 direction, float force)
    {
        rb.AddForce(direction.normalized * force, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canCollide) return; // Ignore collisions until delay is over

        // Ignore Van1/Van2 tags
        if (other.CompareTag("Van1") || other.CompareTag("Van2"))
            return;

        if (!hasCollided)
        {
            hasCollided = true;

            // Spawn explosion at van position
            if (explosionPrefab != null)
            {
                GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                explosion.transform.localScale = Vector3.one * explosionScale;
            }

            // Play a random explosion sound from prefab itself
            if (explosionSounds != null && explosionSounds.Length > 0 && audioSource != null)
            {
                int index = Random.Range(0, explosionSounds.Length);
                audioSource.PlayOneShot(explosionSounds[index]);
            }

            // Trigger camera shake
            if (shakeScript != null)
                StartCoroutine(shakeScript.Shaking());
        }

        // Deal damage to enemy (unchanged)
        enemyPathing collidedEnemy = other.GetComponent<enemyPathing>();
        if (collidedEnemy != null)
        {
            collidedEnemy.TakeDamage(damage); // Only pass damage
        }

        // The van is NOT destroyed on impact
    }
}