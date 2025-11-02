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
    public int lives = 3;            // Number of life segments
    public int maxLives = 3;         // Total life segments
    public int moneyReward = 1;
    public int moneyLoss = 1;

    [Header("Death Knockback")]
    public float deathKnockbackForce = 5f;
    public float deathKnockbackUp = 2f;

    [Header("Death Sounds")]
    public AudioClip[] deathSounds;

    [Header("Health Bar (Slider)")]
    public GameObject healthBarPrefab;
    public Vector3 healthBarOffset = new Vector3(0, 2f, 0);
    public float damageLerpSpeed = 4f;
    public float fadeDuration = 1f;

    private Rigidbody rb;
    private bool dead = false;
    private AudioSource mainAudioSource;

    private GameObject healthBarInstance;
    private Slider healthSlider;
    private Image fillImage;
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

        SetupHealthBar();
    }

    void SetupHealthBar()
    {
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + healthBarOffset, Quaternion.identity);
            healthBarInstance.transform.SetParent(null);

            canvasGroup = healthBarInstance.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = healthBarInstance.AddComponent<CanvasGroup>();

            healthSlider = healthBarInstance.GetComponentInChildren<Slider>();
            if (healthSlider == null)
            {
                Debug.LogError("HealthBar prefab must contain a Slider component!");
                return;
            }

            fillImage = healthSlider.fillRect?.GetComponent<Image>();
            if (fillImage != null)
                fillImage.color = Color.green;

            // Set slider max value to maxLives for segmented display
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxLives;
            healthSlider.value = lives;
        }
    }

    void Update()
    {
        if (!dead && waypoints != null && waypointIndex < waypoints.Length)
            MoveAlongPath();

        UpdateHealthBarPosition();
    }

    void MoveAlongPath()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[waypointIndex];
        Vector3 direction = (target.position - transform.position).normalized;

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

        if (healthSlider != null)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothHealthChange());
        }

        if (lives <= 0)
            Die();
    }

    private IEnumerator SmoothHealthChange()
    {
        float startValue = healthSlider.value;
        float targetValue = lives;
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * damageLerpSpeed;
            healthSlider.value = Mathf.Lerp(startValue, targetValue, elapsed);

            UpdateHealthBarColor();
            yield return null;
        }

        healthSlider.value = targetValue;
        UpdateHealthBarColor();
    }

    private void UpdateHealthBarColor()
    {
        if (fillImage == null || healthSlider == null) return;

        float healthPercent = healthSlider.value / maxLives;

        // Green → Yellow → Red based on total lives
        if (healthPercent > 0.5f)
            fillImage.color = Color.Lerp(Color.yellow, Color.green, (healthPercent - 0.5f) * 2f);
        else
            fillImage.color = Color.Lerp(Color.red, Color.yellow, healthPercent * 2f);
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
            gameManager.Instance.ForceSpendMoney(moneyLoss);

        if (healthBarInstance != null)
            Destroy(healthBarInstance);

        Destroy(gameObject);
    }
}
