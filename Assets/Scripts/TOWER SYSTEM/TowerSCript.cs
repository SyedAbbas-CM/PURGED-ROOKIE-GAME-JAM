using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum TowerType { DirectFire, Cannon }

public class Tower : PlaceableItem
{
    public float range= 10f;
    public float damage=1f;
    public float fireRate=0.5f;
    public float AOE = 0;
    public Transform partToRotate;
    public float rotationSpeed = 2f;
    public GameObject bulletPrefab;
    public LineRenderer lineRenderer;
    public float lineDisplayTime = 0.1f;
    public Color lineTrailColor = Color.red;
    public TowerType towerType;

    private EnemyManager enemyManager;
    private GameObject target;
    private float fireCountdown = 0f;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    private Transform targetTransform;

    private void Awake()
    {
        enemyManager = EnemyManager.Instance;
        if (towerType == TowerType.DirectFire)
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.startColor = lineTrailColor;
            lineRenderer.endColor = lineTrailColor;
            lineRenderer.enabled = false;
        }
    }
    void Update()
    {
        if (towerType == TowerType.DirectFire && lineRenderer != null)
        {
            lineRenderer.startColor = lineTrailColor;
            lineRenderer.endColor = lineTrailColor;
        }


        FindClosestEnemy();
        if (target == null)
        {
            Debug.Log("no target");
            return;
        }
        // find the nearest enemy
        
        Vector3 dir = target.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation,lookRotation,Time.deltaTime*rotationSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(0f,rotation.y,0f);



            if (fireCountdown <= 0f)
            {
                AttackEnemy(target);
                fireCountdown = 1f / fireRate;
            }

            
        fireCountdown -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }

    void FindClosestEnemy()
    {
        if(enemyManager.CurrentWaveState == WaveState.InActive)
        {
            Debug.Log("Wave Inactive! not finding target!");
            return;
        }
        enemiesInRange = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        float shortDistance = Mathf.Infinity;
        GameObject closestEnemy = null;
        foreach (GameObject enemy in enemiesInRange)
        {
            Debug.Log("Found :"+enemy.tag);
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortDistance)
            {
                shortDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        if(closestEnemy != null && shortDistance <= range)
        {
            Debug.Log("Target set!");
            target = closestEnemy;
        }
        else
        {
            target = null;
        }



        return;
    }

    void AttackEnemy(GameObject enemy)
    {
        if (enemiesInRange.Count == 0) return;

        if (towerType == TowerType.DirectFire)
        {
            StartCoroutine(showLineTrail(target.transform.position));
        }
        else if (towerType == TowerType.Cannon)
        {
            LaunchBullet(target.transform.position);
        }


        if (AOE == 0)
        {
            var enemyBeingAttacked = target.GetComponent < Enemy>();
            if(enemyBeingAttacked != null)
            {
                enemyBeingAttacked.TakeDamage(damage);
            }
        }
        else
        {

        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,range);
    }
    private IEnumerator showLineTrail(Vector3 targetPosition)
    {
        lineRenderer.SetPosition(0, partToRotate.position);
        lineRenderer.SetPosition(1, targetPosition);
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(0.1f); // Adjust this duration as needed

        lineRenderer.enabled = false;
    }
    private void LaunchBullet(Vector3 targetPosition)
    {
        GameObject bullet = Instantiate(bulletPrefab, partToRotate.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Initialize(targetPosition); // Assuming your bullet prefab has a Bullet script that moves it to the target.
    }
}