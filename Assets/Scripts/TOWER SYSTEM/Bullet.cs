using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bullet : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed = 10f; // Adjust this value based on how fast you want the bullet to travel.
    private float closeEnoughDistance = 0.5f; // The distance at which the bullet is considered to have hit the target.
    public GameObject explosionEffect;
    public float damage = 1f;
public float AOE = 0f;
    private void Update()
    {
        // Move the bullet towards the target position.
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Point the bullet in the direction it's moving
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero) // To avoid LookRotation viewing vector being zero error
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = lookRotation;
        }

        // Check if the bullet is close enough to the target.
        if (Vector3.Distance(transform.position, targetPosition) <= closeEnoughDistance)
        {
            HitTarget();
        }
    }

    public void Initialize(Vector3 targetPos)
    {
        targetPosition = targetPos;
    }


    private void HitTarget()
    {
        // If the bullet has an AOE effect
        if (AOE > 0)
        {
            // Get all colliders within the AOE radius around the bullet's impact position
            Collider[] colliders = Physics.OverlapSphere(transform.position, AOE);

            foreach (Collider nearbyObject in colliders)
            {
                Enemy enemy = nearbyObject.GetComponent<Enemy>();

                // If the object is an enemy
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);

                }
            }
        }

        // Visual effect for bullet impact
        GameObject explosionInstance = Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(explosionInstance, 1.5f);

        // Deactivate or destroy the bullet
        gameObject.SetActive(false);
    }
}