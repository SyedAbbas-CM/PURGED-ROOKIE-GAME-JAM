using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Bullet : MonoBehaviour
{
    private Vector3 targetPosition;
    private float speed = 10f; // Adjust this value based on how fast you want the bullet to travel.
    private float closeEnoughDistance = 0.5f; // The distance at which the bullet is considered to have hit the target.
    public GameObject explosionEffect;
    private void Update()
    {
        // Move the bullet towards the target position.
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

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
        // Here, you can add a visual effect or sound effect if you want.

        // Once hit, you can either:
        // 1. Destroy the bullet
        // Destroy(gameObject);
        // OR
        // 2. Deactivate the bullet if you're using an object pooling mechanism (recommended for performance).


        //destroy impact effect after 2 seconds
        GameObject explosionInstance = Instantiate(explosionEffect, transform.position, transform.rotation);



        Destroy(explosionInstance, 1.5f);
        gameObject.SetActive(false);
    }
}