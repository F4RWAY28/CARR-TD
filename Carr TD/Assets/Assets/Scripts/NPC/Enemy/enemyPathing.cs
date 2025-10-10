using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class enemyPathing : MonoBehaviour
{
    [Header("Pathing")]
    public Transform[] waypoints;
    public float moveSpeed = 5f;
    public float turnSpeed = 5f;

    [Header("Stats")]
    public int lives = 1;
    public int moneyReward = 1;
    public int moneyLoss = 1;

    [Header("Death Knockback")]
    public float deathKnockbackForce = 5f;
    public float deathKnockbackUp = 2f;

    [Header("Death Sounds")]
    public AudioClip[] deathSounds; // Drag multiple sounds here

    [HideInInspector] public int waypointIndex = 0;

    private Rigidbody rb;
    private bool dead = false;
    private AudioSource mainAudioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        mainAudioSource = GetComponent<AudioSource>();
        mainAudioSource.playOnAwake = false;
        mainAudioSource.spatialBlend = 1f; // 3D sound

        if (waypoints == null || waypoints.Length == 0)
            Debug.LogError("enemyPathing: No waypoints assigned!");
    }

    void Update()
    {
        if (dead || waypoints == null || waypointIndex >= waypoints.Length) return;
        MoveAlongPath();
    }

    void MoveAlongPath()
    {
        Transform target = waypoints[waypointIndex];
        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            waypointIndex++;
            if (waypointIndex >= waypoints.Length)
                ReachEnd();
        }
    }

    public void TakeDamage(int damage)
    {
        if (dead) return;
        lives -= damage;
        if (lives <= 0) Die();
    }

    void Die()
    {
        dead = true;
        rb.isKinematic = false;

        // 🛑 Stop all currently playing AudioSources on this object
        AudioSource[] allSources = GetComponents<AudioSource>();
        foreach (var src in allSources)
            src.Stop();

        // Apply knockback
        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        Vector3 force = randomDir * deathKnockbackForce + Vector3.up * deathKnockbackUp;
        rb.AddForce(force, ForceMode.Impulse);

        // 🎵 Play a random death sound using the main AudioSource
        if (deathSounds != null && deathSounds.Length > 0 && mainAudioSource != null)
        {
            int index = Random.Range(0, deathSounds.Length);
            mainAudioSource.PlayOneShot(deathSounds[index]);
        }

        // Reward player
        if (gameManager.Instance != null)
            gameManager.Instance.AddMoney(moneyReward);

        Destroy(gameObject, 5f); // Wait so the sound can finish
    }

    void ReachEnd()
    {
        if (gameManager.Instance != null)
            gameManager.Instance.LoseMoney(moneyLoss);

        Destroy(gameObject);
    }
}