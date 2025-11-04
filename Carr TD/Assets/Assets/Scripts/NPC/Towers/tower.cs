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
    [SerializeField] private float rotationSpeed = 8f; // how quickly it turns toward target
    [SerializeField] private float aimThreshold = 5f; // how close (in degrees) the turret must be to the target before firing

    private float fireCountdown = 0f;
    private GameObject currentTarget;

    void Update()
    {
        fireCountdown -= Time.deltaTime;

        GameObject target = GetFirstAliveEnemy();

        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);

            if (distance <= range)
            {
                currentTarget = target;
                bool isAimed = AimAt(target);

                // Only shoot when the turret is mostly facing the target
                if (isAimed && fireCountdown <= 0f)
                {
                    Shoot(target);
                    fireCountdown = fireRate;
                }
            }
            else
            {
                currentTarget = null;
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
            if (e == null || !e.activeInHierarchy) continue;

            enemyPathing path = e.GetComponent<enemyPathing>();
            if (path == null || path.waypoints == null || path.waypoints.Length == 0) continue;
            if (Vector3.Distance(transform.position, e.transform.position) > range) continue;
            if (path.lives <= 0) continue;

            if (path.WaypointIndex > maxWaypointIndex)
            {
                first = e;
                maxWaypointIndex = path.WaypointIndex;
            }
        }

        return first;
    }

    /// <summary>
    /// Rotates the turret toward the target and returns true if it's mostly facing it.
    /// </summary>
    bool AimAt(GameObject target)
    {
        if (turretHead == null || target == null) return false;

        Vector3 dir = target.transform.position - turretHead.position;
        dir.y = 0; // keep rotation horizontal

        if (dir.sqrMagnitude < 0.01f) return false;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        turretHead.rotation = Quaternion.Lerp(turretHead.rotation, targetRot, Time.deltaTime * rotationSpeed);

        // Check if the turret is roughly facing the target
        float angle = Quaternion.Angle(turretHead.rotation, targetRot);
        return angle < aimThreshold;
    }

    void Shoot(GameObject target)
    {
        if (vanPrefab == null || firePoint == null || target == null) return;
        if (Vector3.Distance(transform.position, target.transform.position) > range) return;

        GameObject vanGO = Instantiate(vanPrefab, firePoint.position, firePoint.rotation);
        vanProjectile van = vanGO.GetComponent<vanProjectile>();
        if (van != null)
        {
            van.damage = vanDamage;
            van.Launch(firePoint.forward, vanForce);
        }
    }
}