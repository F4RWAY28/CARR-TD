using UnityEngine;

public class tower : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject vanPrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public int vanDamage = 1;
    public float vanForce = 20f;
    public Transform turretHead;
    [SerializeField] public float range = 15f;

    private float fireCountdown = 0f;
    private GameObject currentTarget;

    void Update()
    {
        fireCountdown -= Time.deltaTime;

        // Get a valid target
        GameObject target = GetFirstAliveEnemy();

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);

            // Only aim and shoot if within range
            if (distance <= range)
            {
                currentTarget = target;
                AimAt(target);

                if (fireCountdown <= 0f)
                {
                    Shoot(target);
                    fireCountdown = fireRate;
                }
            }
            else
            {
                currentTarget = null; // too far, stop tracking
            }
        }
        else
        {
            currentTarget = null;
        }
    }

    GameObject GetFirstAliveEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject first = null;
        int maxWaypointIndex = -1;

        foreach (GameObject e in enemies)
        {
            if (e == null) continue;
            if (!e.activeInHierarchy) continue;

            enemyPathing path = e.GetComponent<enemyPathing>();
            if (path == null || path.waypoints == null || path.waypoints.Length == 0)
                continue;

            // Skip if out of range
            if (Vector3.Distance(transform.position, e.transform.position) > range)
                continue;

            // Skip if dead (lives <= 0)
            if (path.lives <= 0)
                continue;

            if (path.waypointIndex > maxWaypointIndex)
            {
                first = e;
                maxWaypointIndex = path.waypointIndex;
            }
        }

        return first;
    }

    void AimAt(GameObject target)
    {
        if (turretHead == null || target == null) return;

        Vector3 dir = target.transform.position - turretHead.position;
        dir.y = 0;
        if (dir != Vector3.zero)
            turretHead.rotation = Quaternion.LookRotation(dir);
    }

    void Shoot(GameObject target)
    {
        if (vanPrefab == null || firePoint == null || target == null) return;

        // Double-check target is still within range before firing
        if (Vector3.Distance(transform.position, target.transform.position) > range)
            return;

        GameObject vanGO = Instantiate(vanPrefab, firePoint.position, firePoint.rotation);
        vanProjectile van = vanGO.GetComponent<vanProjectile>();
        if (van != null)
        {
            van.damage = vanDamage;
            van.Launch(firePoint.forward, vanForce);
        }
    }
}