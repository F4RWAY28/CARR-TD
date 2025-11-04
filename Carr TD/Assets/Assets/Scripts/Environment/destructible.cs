using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class Destructible : MonoBehaviour
{
    [Header("Destructible Settings")]
    public float hitForceMultiplier = 5f;
    public float torqueMultiplier = 3f;
    public float despawnDelay = 5f;
    public AudioClip[] destructionSounds;

    private Rigidbody rb;
    private AudioSource audioSource;
    private Vector3 startPos;
    private Quaternion startRot;
    private bool destroyed = false;
    private bool isRespawning = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }

        startPos = transform.position;
        startRot = transform.rotation;

        destructibleManager.RegisterDestructible(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (destroyed) return;

        // Check if colliding object is an Enemy, Van, or another Destructible
        bool hitByRelevantObject =
            collision.gameObject.CompareTag("Enemy") ||
            collision.gameObject.CompareTag("Van1") ||
            collision.gameObject.CompareTag("Van2") ||
            collision.gameObject.CompareTag("Van3") ||
            collision.gameObject.CompareTag("Destructible") ||

            collision.gameObject.GetComponent<Destructible>() != null;

        if (!hitByRelevantObject) return;

        destroyed = true;

        // Make Rigidbody non-kinematic to react physically
        rb.isKinematic = false;

        // Apply force and random torque for tumbling
        Vector3 force = collision.relativeVelocity * hitForceMultiplier;
        rb.AddForce(force, ForceMode.Impulse);

        Vector3 torque = Random.insideUnitSphere * torqueMultiplier;
        rb.AddTorque(torque, ForceMode.Impulse);

        // Play destruction sound
        if (destructionSounds.Length > 0)
        {
            int index = Random.Range(0, destructionSounds.Length);
            audioSource.PlayOneShot(destructionSounds[index]);
        }

        StartCoroutine(DespawnAfterDelay());
    }

    private IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(despawnDelay);
        gameObject.SetActive(false);
    }

    public void Respawn()
    {
        if (isRespawning) return;
        isRespawning = true;
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(0.5f); // Prevent spawn during tower placement

        // Check if any tower is blocking the spawn
        Collider[] hits = Physics.OverlapSphere(startPos, 0.5f);
        foreach (var h in hits)
        {
            if (h.CompareTag("Tower"))
            {
                isRespawning = false;
                yield break;
            }
        }

        // Reset position, rotation, and physics
        transform.position = startPos;
        transform.rotation = startRot;
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        destroyed = false;
        gameObject.SetActive(true);

        isRespawning = false;
    }

    private void OnDestroy()
    {
        if (destructibleManager.Instance != null)
            destructibleManager.UnregisterDestructible(this);
    }
}
