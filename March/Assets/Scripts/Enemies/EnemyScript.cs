using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float health = 100;
    //Objects and Effects
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject projectile;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject deathExplosion;
    private PlayerMove playerMove;
    [SerializeField] private WaveScript waveScript;
    [SerializeField] private GameObject healthDrop;
    //Movement
    [SerializeField] private float t;
    [SerializeField] private float timeBetweenMoves = 5;
    //Searching for Player
    [SerializeField] private bool seePlayer;
    [SerializeField] private Transform linecastOrigin;
    //Combat
    [SerializeField] private float fireRate = 2;
    [SerializeField] private float playerPushbackForce = 1f;
    [SerializeField] private float vertPlayerPushbackForce = 1f;
    private Vector3 direction; //Euler from the enemy to the player
    [SerializeField] private float playerInactiveDuration = 1;
    private IEnumerator yeetCoroutine;

    //logic vars
    private float y;
    private int i;
    private float angle;
    private float time;
    private float speed;
    [SerializeField] private bool isPartofWave = true;

    private void Start()
    {
        playerMove = player.GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        //Wander
        if (t > timeBetweenMoves && seePlayer == false)
        {
            if (i == 0)
            { //Randomize
                angle = Random.Range(0, 359);
                time = Random.Range(1, 3);
                speed = Random.Range(3, 7);
                time += t;
                transform.rotation = Quaternion.Euler(0, angle, 0);
            }
            i = 1;
            if (t < time)
            {
                Vector3 velocity = rb.velocity;
                velocity.x = transform.forward.x * speed;
                velocity.z = transform.forward.z * speed;
                rb.velocity = velocity;
            } else
            {
                t = 0;
                i = 0;
                timeBetweenMoves = Random.Range(1, 5);
            }
        }
            t += Time.deltaTime;

        //Look for player
        RaycastHit hit;

        direction = player.transform.position - linecastOrigin.position;
        direction = direction.normalized;

        if (Physics.Raycast(linecastOrigin.position, direction, out hit, 100))
        {
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                seePlayer = true;
            }
            else
            {
                seePlayer = false;
            }
        }

        //Shoot at player
        if (seePlayer == true)
        {
            Vector3 lookAtPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.LookAt(lookAtPos);

            if (Time.time > (y + fireRate))
            {
                Instantiate(projectile, transform.position + Vector3.up * 0.5f, transform.rotation);
                y = Time.time;
            }
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

            if (isPartofWave) //Wave logic
            {
                waveScript.enemyDeaths++;
            }
            Destroy(this.gameObject);
        }
    }

    //Push Player away if they come too close (collide with the enemy)
    private void OnCollisionEnter(Collision collision)
    {
        yeetCoroutine = YeetPlayer();
        if (collision.transform.gameObject.CompareTag("Player"))
        {
            //Debug.Log("YEET");
            StartCoroutine(yeetCoroutine);
        }
    }

    IEnumerator YeetPlayer()
    {
        playerMove.canMove = false; //Disable player movement during the yeet duration
        Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
        Vector3 yeetDirection = player.transform.position - transform.position; //Creates a vector3 pointing from the enemy towards the player
        Vector3 newYeetDirection = new Vector3(yeetDirection.x * playerPushbackForce, vertPlayerPushbackForce, yeetDirection.z * playerPushbackForce); //Apply force multipliers and new y value
        playerRigidbody.AddForce(newYeetDirection, ForceMode.Impulse); //Yeet the player

        yield return new WaitForSeconds(playerInactiveDuration);

        playerMove.canMove = true; //Reenable player movement
    }
}