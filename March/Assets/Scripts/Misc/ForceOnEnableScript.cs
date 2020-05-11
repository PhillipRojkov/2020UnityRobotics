using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceOnEnableScript : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 direction;
    public float forceMultiplier;

    public bool randomize;
    public float randomizeRange;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();


        if (randomize)
        {
            float random = Random.value;
            random *= randomizeRange;

            direction *= random;
        }

        direction *= forceMultiplier;
    }

    private void Update()
    {
            rb.AddForce(direction);
    }
}
