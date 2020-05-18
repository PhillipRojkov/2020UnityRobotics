using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2CustomAnimScript : MonoBehaviour
{
    public GameObject glass;
    public GameObject shatterGlass;

    public GameObject explosion;

    // Start is called before the first frame update
    void Start()
    {
        glass.SetActive(false);
        shatterGlass.SetActive(true);


        Instantiate(explosion, transform.position, transform.rotation);

        this.gameObject.SetActive(false);
    }
}
