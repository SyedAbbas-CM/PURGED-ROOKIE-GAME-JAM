using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float range= 100;
    public float damage=1;
    public float fireRate;
    public float AOE = 0;
    public Transform partToRotate;
    private EnemyManager enemyManager;

    private GameObject target;
    private float fireCountdown = 0f;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    private Transform targetTransform;

    private void Awake()
    {
        enemyManager = EnemyManager.Instance;
    }
    void Update()
    {
        FindClosestEnemy();
        if (target == null)
        {
            Debug.Log("no target");
            return;
        }
        // find the nearest enemy
        
        Vector3 dir = target.transform.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = lookRotation.eulerAngles;
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
        if (other.CompareTag("Enemiy"))
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

        if(target != null && shortDistance <= range)
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
        if(AOE == 0)
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
}