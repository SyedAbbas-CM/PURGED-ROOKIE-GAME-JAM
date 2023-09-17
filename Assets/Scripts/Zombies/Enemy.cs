using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;
using System.Collections;

public enum EnemyType { Standard,Fast,Tank, MazeBreaker, SwarmCarrier }
public class Enemy : MonoBehaviour
{
    public EnemyType enemyType = EnemyType.Standard;
    public GameObject swarmPrefab;
    public float speed;
    public float maxSpeed;
    public float currentHealth;
    private float maxHealth;
    
    public float heightOffset = 0.5f; // To raise the enemy above the grid
    public PathManager pathManager;
    public EnemyManager enemyManager;
    public float rotationSpeed = 5f;
    public int numberOfSwarmUnits = 3;
    public int wallsToBreak = 1;
    public float currentSpeed;
    [Header("HealthBar Thing")]
    public Image HpBar;
    public Image HpBarBG;
    public float HpBarHeight;
    public GridGenerator gridGenerator;
    public SoundManager soundmanager;

    private List<Node> path;
    private bool childrenVisible = false;
    public Animator anim;
    private bool isAttacking = false;
    private bool isBreakingWall = false;
    private float wallBreakTimer = 0f;
    private float timeToBreakWall = 5f;
    private bool AbilityUsed = false;
    private Quaternion lockedRotation;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip spawnSound;
    public AudioClip deathSound;
    public AudioClip specialAbilitySound;
    public static AudioClip backgroundMusic;  // shared across all instances
    public static bool isBackgroundMusicPlaying = false;




    [ContextMenu("Toggle Children")]
    public void ToggleChildrenVisibility()
    {
        childrenVisible = !childrenVisible;
        foreach (Transform child in transform)
        {
            child.hideFlags = childrenVisible ? HideFlags.None : HideFlags.HideInHierarchy;
        }
    }
    private void Awake()
    {
        enemyManager = EnemyManager.Instance;
        audioSource = SoundManager.Instance.GetComponent<AudioSource>();

    }
    private void Start()
    {

        if(enemyType != EnemyType.Standard || enemyType != EnemyType.Standard)
        {
            SoundManager.instance.PlaySFX(spawnSound);
        }
        currentSpeed = speed;

        pathManager = PathManager.Instance;
        soundmanager = SoundManager.Instance;
        audioSource = soundmanager.sfxSource;
        path = pathManager.GetPrimaryPath();

        anim = GetComponent<Animator>();

        maxHealth = currentHealth;


        if (!isBackgroundMusicPlaying && backgroundMusic != null)
        {
            AudioSource.PlayClipAtPoint(backgroundMusic, transform.position);
            isBackgroundMusicPlaying = true;
        }

        if ((enemyType != EnemyType.Standard && enemyType != EnemyType.Fast) && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
        lockedRotation = Quaternion.Euler(90, 0, 0);
    }

    void FixedUpdate()
    {
        if (path == null || path.Count == 0)
        {
            if (pathManager.CurrentPathState == pathState.pathFound)
            {
                path = pathManager.GetPrimaryPath();
                return;
            }
            else
            {
                Debug.Log("No Path Generated!");
                return;
            }
        }

        Vector3 targetPosition = path[0].WorldPosition + Vector3.up * heightOffset; // Adjusted for height above grid
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Enemy Facing Direction
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            // Check if it's the last node in the path
            if (path.Count == 1)
            {
                Die();
                ResourceManager.Instance.LoseLife();
                return;
            }

            path.RemoveAt(0);
        }

        if (enemyType == EnemyType.MazeBreaker && !AbilityUsed && currentHealth <= maxHealth / 2)
        {
            StartCoroutine(BreakWallAbility());
        }

    }
    private void LateUpdate()
    {
        HpBar.transform.rotation = lockedRotation;
        HpBarBG.transform.rotation = lockedRotation;
    }
    IEnumerator BreakWallAbility()
    {
        // Stop the enemy for 5 seconds to use its ability
        speed = 0;

        // Find the closest wall
        NodeScript closestWall = FindClosestWall();

        if (closestWall != null)
        {
            anim.SetTrigger("Attack");
            // Darken the enemy's color
            GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.black, 0.5f);

            yield return new WaitForSeconds(5f);

            // Initiate the wall breaking process
            StartCoroutine(closestWall.BreakWall());

            SoundManager.instance.PlaySFX(specialAbilitySound);

            // Update the enemy color back to normal if needed
            GetComponent<Renderer>().material.color = Color.white;
        }

        AbilityUsed = true;

        // Resume the enemy's movement
        speed = maxSpeed; // assuming you have an initialSpeed variable to store the original speed
    }

    NodeScript FindClosestWall()
    {
        // Get the enemy's current position node
        NodeScript currentNode = gridGenerator.GetNodeFromWorldPoint(transform.position);

        if (currentNode == null) return null;

        // Get all neighboring nodes
        List<NodeScript> neighbours = gridGenerator.GetNeighbours(currentNode);

        // Find the closest wall among the neighbors
        float minDistance = Mathf.Infinity;
        NodeScript closestWall = null;

        foreach (NodeScript node in neighbours)
        {
            if (!node.node.Walkable) // If it's a wall
            {
                float distance = Vector3.Distance(transform.position, node.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestWall = node;
                }
            }
        }

        return closestWall;
    }

    public void TakeDamage(float damage)
    {
        maxHealth -= damage;
        HpBar.fillAmount = maxHealth / currentHealth;
        if (maxHealth <= 0f)
        {
            enemyManager.enemyCount--;
            Die();
        }
    }

    public void Die()
    {
        anim.SetTrigger("Die");
        if (enemyType == EnemyType.SwarmCarrier)
        {
            // Determine the current node of the SwarmCarrier
            Node currentCarrierNode = null;
            if (path.Count > 0)
            {
                currentCarrierNode = path[0];
            }

            for (int i = 0; i < numberOfSwarmUnits; i++)
            {
                GameObject newSwarmUnit = Instantiate(swarmPrefab, transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f)), Quaternion.identity);

                // Set the path for the new swarm unit to start from the current node of the SwarmCarrier
                Enemy newSwarmUnitEnemy = newSwarmUnit.GetComponent<Enemy>();
                if (newSwarmUnitEnemy && currentCarrierNode != null)
                {
                    newSwarmUnitEnemy.path = new List<Node>(this.path);  // Make a copy of the current path
                    newSwarmUnitEnemy.TrimPathUntilNode(currentCarrierNode);
                }
            }
        }

        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        SoundManager.instance.PlaySFX(deathSound);
        Debug.Log("Enemy died.");
        Destroy(this.gameObject);
        
    }

    private Node FindClosestNodeOnPath(Vector3 position, List<Node> path)
    {
        float shortestDistance = Mathf.Infinity;
        Node closestNode = null;

        foreach (Node node in path)
        {
            float currentDistance = Vector3.Distance(position, node.WorldPosition);
            if (currentDistance < shortestDistance)
            {
                shortestDistance = currentDistance;
                closestNode = node;
            }
        }
        return closestNode;
    }

    private void TrimPathUntilNode(Node targetNode)
    {
        while (path.Count > 0 && path[0] != targetNode)
        {
            path.RemoveAt(0);
        }
    }
    private void accelerate()
    {
        if (currentSpeed >= speed)
        {
            currentSpeed = speed;
        }
        else
        {
            currentSpeed += currentSpeed / 10;
        }
    }

    private void AttackMode()
    {
        // Stop moving.
        anim.SetBool("isWalking", false);

        // Start attack animation.
        anim.SetTrigger("attack");

        // Logic for attacking here.
        isAttacking = true;

        // Perhaps add a timer, after which the enemy stops attacking and returns to walking.
    }
}