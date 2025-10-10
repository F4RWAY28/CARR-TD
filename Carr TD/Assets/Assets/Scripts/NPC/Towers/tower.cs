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

    void Update()
    {
        fireCountdown -= Time.deltaTime;

        GameObject target = GetFirstEnemy();
        if (target != null)
        {
            AimAt(target);

            if (fireCountdown <= 0f)
            {
                Shoot(target);
                fireCountdown = fireRate;
            }
        }
    }

    GameObject GetFirstEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject first = null;
        int maxWaypointIndex = -1;

        foreach (GameObject e in enemies)
        {
            enemyPathing path = e.GetComponent<enemyPathing>();
            if (path == null || path.waypoints == null || path.waypoints.Length == 0) continue;
            if (Vector3.Distance(transform.position, e.transform.position) > range) continue;

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
        if (vanPrefab == null || firePoint == null) return;

        GameObject vanGO = Instantiate(vanPrefab, firePoint.position, firePoint.rotation);
        vanProjectile van = vanGO.GetComponent<vanProjectile>();
        if (van != null)
        {
            van.damage = vanDamage;
            van.Launch(firePoint.forward, vanForce);
        }
    }
}