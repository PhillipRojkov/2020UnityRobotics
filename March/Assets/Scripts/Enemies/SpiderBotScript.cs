using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBotScript : MonoBehaviour
{
    public float health = 50;
    //Objects
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject deathExplosion;
    [SerializeField] private GameObject warningSphere;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject healthDrop;
    [SerializeField] private GameObject ammoDrop;
    //Monobehaviours
    private PlayerMove playerMove;
    //Player Detection
    [SerializeField] private Transform linecastOrigin; //Object from which a linecast is made for player visibility detection
    [SerializeField] private bool seePlayer = false; //True if player is visible
    private Vector3 direction; //Euler from enemy to player
    //Combat
    [SerializeField] private float explosionTriggerDistance = 2;
    [SerializeField] private float explosionRadius = 2;
    [SerializeField] private float explosionDamage = 100;
    [SerializeField] private float explosionForce = 5;
    [SerializeField] private float vertPlayerExplosionForce = 4;
    [SerializeField] private float playerInactiveDuration = 0.8f;
    [SerializeField] private float explosionDamageDistanceMultiplier = 10; //Higher value = less damage taken from explosion based on distance
    //Movement
    [SerializeField] private float obstacleAvoidanceDistance = 4f;
    [SerializeField] private float obstacleAvoidingMagnitude = 10f; //Lower value will avoid more drastically
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float walkHeight = 0.3f;
    private Rigidbody rb;
    private Vector3 moveDirection;
    [SerializeField] private int detectionRays = 30; //Number of rays used for obstacle detection
    private bool canMove = true; //Disable movement during explosion

    private IEnumerator yeetCoroutine;

    private int layerMask = 1 << 2; //Layermask for ignore raycast layer

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMove = player.GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        //Look for player raycast
        RaycastHit hit;

        direction = player.transform.position - linecastOrigin.position; //Direction from enemy to player
        direction = direction.normalized; //Set magnitude = 1

        if (Physics.Raycast(linecastOrigin.position, direction, out hit, 100, ~layerMask)) //Raycasts towards player; player detection
        {
            if (hit.transform.gameObject.CompareTag("Player")) //The player is visible
            {
                seePlayer = true;
                //Look at player
                Vector3 lookTransform = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z); //This sets the y component of the look vector to the y position of the enemy, so the enemy doesn't look up or down
                transform.LookAt(lookTransform, Vector3.up); //Points enemy towards player along y axis (body always stays flat)

                moveDirection = transform.forward;
            }
            else //The player is not visible
            {
                seePlayer = false;
                moveDirection = Vector3.zero;
            }
        }

        RaycastHit groundCheck; //For distance alignment with the ground normal
        if (Physics.Raycast(transform.position, Vector3.down, out groundCheck, 5, ~layerMask))
        {
            transform.position = new Vector3(transform.position.x, groundCheck.point.y + walkHeight, transform.position.z); //Place the body  at walkHeight units above the ground
        }

        for (int i = 0; i < 30; i++) //Obstacle detection
        {
            float angle = (360/detectionRays) * i; //Creates angles based on the number of detection rays
            Vector3 raycastDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)); //Creates a vector3 with x and z coords to point in the angle direction for raycast

            RaycastHit obstacleHit;
            if (Physics.Raycast(transform.position, raycastDirection, out obstacleHit, obstacleAvoidanceDistance, ~layerMask)) //Raycast to see if anything is closer than obstacleAvoidanceDistance in the angle direction
            {
                //Debug.DrawLine(transform.position, obstacleHit.point, Color.red, Time.deltaTime); //Visualize the obstalce detection rays
                Vector3 obstacletoEnemyDirection = transform.position - obstacleHit.point; //Creates a vector3 direction that points from the obstacle to the enemy
                obstacletoEnemyDirection = obstacletoEnemyDirection.normalized;
                //Debug.Log(obstacletoEnemyDirection);
                Vector3 oteMagnitude = obstacletoEnemyDirection / (obstacleAvoidingMagnitude * Vector3.Distance(transform.position, obstacleHit.point)); //set magnitude of vector pushing the enemy away from the obstacle relative to the inverse of the distance (farther away = less force)

                moveDirection = moveDirection + oteMagnitude; //Add the moveDirection and oteMagnitude vector which pushes the enemy away from obstacles while keeping it moving towards the player
            }
        }

        moveDirection.y = 0; //The enemy should not fluctuate in the y position as that is controlled by the groundCheck raycast

        if (canMove)
        {
            rb.velocity = moveDirection * moveSpeed; //Apply a velocity change
        } else if (!canMove)
        {
            rb.velocity = Vector3.zero; //Stop movement during explosion
        }

        if (health <= 0) //Death event
        {
            Instantiate(deathExplosion, transform.position, transform.rotation);

            //Health drop
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            float dropChance = 100 - playerHealth.health; //Gives a % chance for dropping health that increases as health is depleted
            float random = Random.Range(0f, 100f);
            if (random < dropChance)
            {
                Instantiate(healthDrop, transform.position, transform.rotation);
            }

            //Shotgun ammo drop
            PlayerWeapon playerWeapon = player.GetComponent<PlayerWeapon>();
            if (playerWeapon.hasShotgun)
            {
                float ammoDropChance = 1 / (playerWeapon.shotgunAmmo + 1);
                float ammoRandom = Random.Range(0.1f, 1f); //Min is .1 because at 8/8 ammo, the ammoDropChance is 1% as opposed to 11%. At 0/8 ammo, drop rate is 100%

                if (ammoRandom <= ammoDropChance)
                {
                    Instantiate(ammoDrop, transform.position, transform.rotation);
                }
            }

            Destroy(this.gameObject);
        }

        if (Vector3.Distance(transform.position, player.transform.position) < explosionTriggerDistance) //Explosion attack when player comes close
        {
            StartCoroutine(ExplodeCoroutine());
        }
    }

    private IEnumerator ExplodeCoroutine()
    {
        anim.enabled = true; //Start the flashing light animation
        canMove = false; //Disable movement
        warningSphere.SetActive(true); //Turn on the warning sphere effect

        yield return new WaitForSeconds(3);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius); //Create a sphere that encompasses the explosion
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].CompareTag("Player"))
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>(); //Get player health component
                explosionDamage -= (explosionDamageDistanceMultiplier * Vector3.Distance(transform.position, player.transform.position)); //Decrease damage based on distance
                playerHealth.health -= explosionDamage; //Apply damage

                yeetCoroutine = YeetPlayer();
                StartCoroutine(yeetCoroutine);
            }
        }

        //Debug.Log("Boom");
        Instantiate(explosion, transform.position, transform.rotation);

        Destroy(this.gameObject);
    }

    IEnumerator YeetPlayer()
    {
        playerMove.canMove = false; //Disable player movement during the yeet duration
        Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
        Vector3 yeetDirection = player.transform.position - transform.position; //Creates a vector3 pointing from the enemy towards the player
        Vector3 newYeetDirection = new Vector3(yeetDirection.x * explosionForce, vertPlayerExplosionForce, yeetDirection.z * explosionForce); //Apply force multipliers and new y value
        newYeetDirection = newYeetDirection / Vector3.Distance(transform.position, player.transform.position);
        playerRigidbody.AddForce(newYeetDirection, ForceMode.Impulse); //Yeet the player

        yield return new WaitForSeconds(playerInactiveDuration);

        playerMove.canMove = true; //Reenable player movement
    }
}
