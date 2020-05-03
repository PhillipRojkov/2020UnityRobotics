using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private float pistolDamage = 20;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private GameObject camera;
    [SerializeField] private EnemyScript enemyScript; //Default enemy
    [SerializeField] private SpiderBotScript spiderBotScript; //Spiderbot enemy
    [SerializeField] private Transform leftBarrel;
    [SerializeField] private Transform rightBarrel;
    private WaitForSeconds shotDuration = new WaitForSeconds(.05f);
    [SerializeField] private LineRenderer laserLineL;
    [SerializeField] private LineRenderer laserLineR;
    private float nextFire = 0;
    [SerializeField] private GameObject hitParticles;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private Animator pistolAnimator;

    // Update is called once per frame
    void Update()
    {
        laserLineL.SetPosition(0, leftBarrel.position);
        if (Input.GetButton("Fire1") && Time.time >= nextFire) //Shoot
        {
            var flash = Instantiate(muzzleFlash, leftBarrel.position, leftBarrel.rotation); //Create muzzle flash
            flash.transform.parent = leftBarrel;
            pistolAnimator.SetBool("Fire", true); //Set fire animation

            nextFire = Time.time + fireRate; //Timer for fire rate

            StartCoroutine(ShotEffect());

            RaycastHit hit;
            if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 100)) //Raycast shooting
            {
                laserLineL.SetPosition(1, hit.point);
                if (hit.transform.gameObject.CompareTag("Enemy")) //Check if hit enemy
                {
                    if (hit.transform.gameObject.GetComponent<EnemyScript>()) //Check if standard enemy
                    {
                        enemyScript = hit.transform.gameObject.GetComponent<EnemyScript>(); //Apply damage to enemy
                        enemyScript.health -= pistolDamage;
                    }
                    else if (hit.transform.gameObject.GetComponent<SpiderBotScript>())
                    {
                        spiderBotScript = hit.transform.gameObject.GetComponent<SpiderBotScript>(); //Apply damage to enemy
                        spiderBotScript.health -= pistolDamage;
                    }
                }

                Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal)); //create hit particles perpendicular to the surface normal of the hit object
            }
            else
            {
                laserLineL.SetPosition(1, camera.transform.position + (camera.transform.forward * 100)); //Shoot into thin air at a point 100 units away
            }
        } else if (Input.GetButtonUp("Fire1"))
        {
            pistolAnimator.SetBool("Fire", false); //Switch off fire animation
        }
    }

    private IEnumerator ShotEffect() //Create the line renderer from the barrel of the gun to the hit point
    {
        laserLineL.enabled = true;
        yield return shotDuration;
        laserLineL.enabled = false;
    }
}
