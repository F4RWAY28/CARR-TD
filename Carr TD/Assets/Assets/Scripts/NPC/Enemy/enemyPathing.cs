using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class enemyPathing : MonoBehaviour
{
    [Header("Pathing")]
    public Transform[] waypoints;
    public float moveSpeed = 5f;
    public float turnSpeed = 5f;

    [Header("Stats")]
    public int lives = 100;
    public int maxLives = 100;
    public int moneyReward = 1;
    public int moneyLoss = 1;

    [Header("Death Knockback")]
    public float deathKnockbackForce = 5f;
    public float deathKnockbackUp = 2f;

    [Header("Death Sounds")]
    public AudioClip[] deathSounds;

    [Header("Health Bar")]
    [Tooltip("Prefab with two Image children: red (damage) and green (health)")]
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, 2f, 0);
    public Color healthColor = Color.green;
    public Color damageColor = Color.red;
    public float damageLerpSpeed = 4f;
    public float fadeDuration = 1f;

    private Rigidbody rb;
    private bool dead = false;
    private AudioSource mainAudioSource;

    private GameObject healthBarInstance;
    private Image healthFill;
    private Image damageFill;
    private CanvasGroup canvasGroup;
    private Camera cam;

    private int waypointIndex = 0;
    public int WaypointIndex => waypointIndex;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;

        mainAudioSource = GetComponent<AudioSource>();
        mainAudioSource.playOnAwake = false;
        mainAudioSource.spatialBlend = 1f;

        cam = Camera.main;

        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity);
            healthBarInstance.transform.SetParent(null);

            canvasGroup = healthBarInstance.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = healthBarInstance.AddComponent<CanvasGroup>();

            Image[] imgs = healthBarInstance.GetComponentsInChildren<Image>(true);
            if (imgs.Length >= 2)
            {
                damageFill = imgs[0];
                healthFill = imgs[1];
                damageFill.color = damageColor;
                healthFill.color = healthColor;
                healthFill.fillAmount = 1f;
                damageFill.fillAmount = 1f;
            }
            else
            {
                Debug.LogError("HealthBar prefab must have two Image children (red for damage, green for health).");
            }
        }
    }

    void Update()
    {
        if (!dead && waypoints != null && waypointIndex < waypoints.Length)
            MoveAlongPath();

        UpdateHealthBarPosition();

        if (damageFill != null && healthFill != null && damageFill.fillAmount > healthFill.fillAmount)
        {
            damageFill.fillAmount = Mathf.Lerp(damageFill.fillAmount, healthFill.fillAmount, Time.deltaTime * damageLerpSpeed);
        }
    }

    void MoveAlongPath()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[waypointIndex];
        Vector3 direction = (target.position - transform.position).normalized;

        // ✅ Allow rotation to follow full 3D direction (including up/down tilt)
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            waypointIndex++;
            if (waypointIndex >= waypoints.Length)
                ReachEnd();
        }
    }

    void UpdateHealthBarPosition()
    {
        if (healthBarInstance == null) return;
        healthBarInstance.transform.position = transform.position + healthBarOffset;
        if (cam != null)
            healthBarInstance.transform.forward = cam.transform.forward;
    }

    public void TakeDamage(int damage)
    {
        if (dead || damage <= 0) return;

        lives -= damage;
        lives = Mathf.Clamp(lives, 0, maxLives);

        if (healthFill != null)
            healthFill.fillAmount = (float)lives / maxLives;

        if (lives <= 0)
            Die();
    }

    void Die()
    {
        dead = true;
        rb.isKinematic = false;

        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        Vector3 force = randomDir * deathKnockbackForce + Vector3.up * deathKnockbackUp;
        rb.AddForce(force, ForceMode.Impulse);

        if (deathSounds != null && deathSounds.Length > 0 && mainAudioSource != null)
        {
            int index = Random.Range(0, deathSounds.Length);
            mainAudioSource.PlayOneShot(deathSounds[index]);
        }

        if (gameManager.Instance != null)
            gameManager.Instance.AddMoney(moneyReward);

        if (healthBarInstance != null)
            StartCoroutine(FadeOutHealthBar());

        Destroy(gameObject, 5f);
    }

    private IEnumerator FadeOutHealthBar()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        if (healthBarInstance != null)
            Destroy(healthBarInstance);
    }

    void ReachEnd()
    {
        if (gameManager.Instance != null)
            gameManager.Instance.LoseMoney(moneyLoss);

        if (healthBarInstance != null)
            Destroy(healthBarInstance);

        Destroy(gameObject);
    }
}
