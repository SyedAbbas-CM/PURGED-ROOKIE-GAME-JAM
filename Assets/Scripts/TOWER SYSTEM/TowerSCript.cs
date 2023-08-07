using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public float range;
    public float damage;
    public float fireRate;
    public float AOE = 0;

    private float fireCountdown = 0f;
    private List<GameObject> enemiesInRange = new List<GameObject>();
    void Update()
    {
        // find the nearest enemy
        GameObject enemy = FindClosestEnemy();

        if (enemy != null && Vector3.Distance(transform.position, enemy.transform.position) <= range)
        {
            if (fireCountdown <= 0f)
            {
                AttackEnemy(enemy);
                fireCountdown = 1f / fireRate;
            }

            
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

    GameObject FindClosestEnemy()
    {




        return null;
    }

    void AttackEnemy(GameObject enemy)
    {
        if (enemiesInRange.Count == 0) return;
        GameObject target = enemiesInRange[0];

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
}