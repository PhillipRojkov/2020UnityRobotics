using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileScript : MonoBehaviour
{
    public float damage = 30;
    [SerializeField] private float speed = 10;
    [SerializeField] private Rigidbody rb;
    private float t;
    [SerializeField] private float inactiveStartTime = 0.1f;
    [SerializeField] private GameObject explosionParticles;

    private void Start()
    {
        t += Time.time;
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        GameObject player = GameObject.Find("Player");
        if (player.CompareTag("Player"))
        {
            transform.LookAt(player.transform.position);
        }
    }

    private void Update()
    {
        if (Time.time > (t + inactiveStartTime))
        {
            Collider collider = GetComponent<Collider>();
            collider.enabled = true;
        }

            rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            playerHealth.health -= damage;
        }
        Instantiate(explosionParticles, transform.position, transform.rotation);
            Destroy(this.gameObject);
    }
}
