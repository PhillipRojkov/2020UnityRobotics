using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDelay : MonoBehaviour
{
    public float delay = 2;
    private float ti;
    private float t;

    private void Start()
    {
        ti = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        t = Time.time - ti;
        if (t > delay)
        {
            Destroy(this.gameObject);
        }
    }
}
