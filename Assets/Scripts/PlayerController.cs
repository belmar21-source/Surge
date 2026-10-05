using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Combat")]
    public float fireRate = 2f;
    public float attackRange = 7f;
    public float projectileSpeed = 10f;
    public float projectileLife = 2f;
    public int damage = 1;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private Rigidbody2D rb;
    private float fireCooldown;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (firePoint == null)
            firePoint = transform;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(h, v).normalized;
        rb.velocity = input * moveSpeed;

        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;

        Transform nearestEnemy = GetNearestEnemy();
        if (nearestEnemy != null)
        {
            Vector3 dir = nearestEnemy.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            firePoint.rotation = Quaternion.Euler(0f, 0f, angle);

            if (fireCooldown <= 0f)
            {
                Shoot(nearestEnemy.position);
                fireCooldown = 1f / fireRate;
            }
        }
    }

    Transform GetNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform closest = null;
        float bestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float d = Vector2.Distance(transform.position, enemy.transform.position);
            if (d < attackRange && d < bestDistance)
            {
                bestDistance = d;
                closest = enemy.transform;
            }
        }

        return closest;
    }

    void Shoot(Vector3 targetPos)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Assign a projectile prefab to PlayerController.");
            return;
        }

        Vector3 direction = (targetPos - firePoint.position).normalized;
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projectile = bullet.GetComponent<Projectile>();

        if (projectile != null)
            projectile.Initialize(direction, projectileSpeed, damage, "Player", projectileLife);
    }
}
