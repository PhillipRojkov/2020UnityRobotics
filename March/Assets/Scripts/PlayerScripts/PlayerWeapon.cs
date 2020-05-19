using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private float useDistance = 1.5f;
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

    public bool hasShotgun = false;

    public int shotgunAmmo = 8; //8 is max

    private int selectedWeapon = 0; //0 = pistol, 1 = shotgun

    [SerializeField] private float shotgunDamage = 130;
    [SerializeField] private float shotgunDamageFalloff = .1f; //Percent damage taken off after 1 unit of distance
    [SerializeField] private float shotgunFireRate =  0.6f;
    [SerializeField] private float shotgunRangeMultiplier = .8f;
    [SerializeField] private GameObject shotgunMuzzleFlash;
    [SerializeField] private GameObject shotgunConeEffect;

    [SerializeField] private Material shotgunLoaded;
    [SerializeField] private Material shotgunBlank;

    private Material lightsNormal;

    [SerializeField] private GameObject lightsParent;
    private GameObject[] lights;


    private void Start()
    {
        lights = new GameObject[8]; //Intialize lights array

        for (int i = 0; i < lights.Length; i++)
        {
            lights[i] = lightsParent.transform.GetChild(i).gameObject; //Array lights is an array of the lights under lightsParent
        }

        lightsNormal = lights[1].GetComponent<MeshRenderer>().material; //Store the normal pistol blue material for the lights
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("q") && hasShotgun)
        {
            SwapWeapon();
        }

        laserLineL.SetPosition(0, leftBarrel.position);
        if (Input.GetButton("Fire1") && Time.time >= nextFire) //Shoot
        {
            if (selectedWeapon == 0) //Blaster
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
            }

            else if (selectedWeapon == 1 && shotgunAmmo > 0) //Shotgun
            {
                shotgunAmmo--;
                ShotgunAmmoCheck();

                var flash = Instantiate(shotgunMuzzleFlash, leftBarrel.position, leftBarrel.rotation); //Create muzzle flash
                flash.transform.parent = leftBarrel;

                var coneEffect = Instantiate(shotgunConeEffect, leftBarrel.position, leftBarrel.rotation); //Create shot effect flash
                coneEffect.transform.parent = leftBarrel;

                pistolAnimator.SetBool("Fire", true); //Set fire animation

                nextFire = Time.time + shotgunFireRate; //Timer for fire rate

                RaycastHit sgHit;
                if (Physics.Raycast(camera.transform.position, camera.transform.forward, out sgHit, 100)) //Raycast shooting
                {
                    if (sgHit.transform.gameObject.CompareTag("Enemy")) //Check if hit enemy
                    {
                        float distance = Vector3.Distance(sgHit.transform.position, transform.position);

                        if (sgHit.transform.gameObject.GetComponent<EnemyScript>()) //Check if standard enemy
                        {
                            enemyScript = sgHit.transform.gameObject.GetComponent<EnemyScript>(); //Apply damage to enemy
                            enemyScript.health -= shotgunDamage - shotgunDamage * shotgunDamageFalloff * distance; //Linearly decrease damage taken as distance increases. for a shotgunDamageFalloff of .1, the damage taken after 1 unit of distance is 90%
                        }
                        else if (sgHit.transform.gameObject.GetComponent<SpiderBotScript>())
                        {
                            spiderBotScript = sgHit.transform.gameObject.GetComponent<SpiderBotScript>(); //Apply damage to enemy
                            spiderBotScript.health -= shotgunDamage - shotgunDamage * shotgunDamageFalloff * distance; //Linearly decrease damage taken as distance increases. for a shotgunDamageFalloff of .1, the damage taken after 1 unit of distance is 90%
                        }
                    }
                }
            }
        } //End of fire if statement

        else if (Input.GetButtonUp("Fire1"))
        {
            pistolAnimator.SetBool("Fire", false); //Switch off fire animation
        }

        if (Input.GetButton("Use")) //Use button
            {
                RaycastHit use;
                if (Physics.Raycast(camera.transform.position, camera.transform.forward, out use, useDistance))
                {
                    if (use.transform.gameObject.CompareTag("Puzzle")) //Is puzzle
                    {
                        PuzzleScript puzzleScript = use.transform.gameObject.GetComponent<PuzzleScript>();

                        puzzleScript.enabled = true; //Turn on puzzle

                        puzzleScript.TurnOnInstructions(); //Turns on the instruction graphic thingy

                        PlayerMove playerMove = this.gameObject.GetComponent<PlayerMove>();
                        playerMove.inPuzzle = true; //Disable movement
                    }
                }
            }
        }


    private IEnumerator ShotEffect() //Create the line renderer from the barrel of the gun to the hit point
    {
        laserLineL.enabled = true;
        yield return shotDuration;
        laserLineL.enabled = false;
    }

    private void ShotgunAmmoCheck()
    {
        for (int i = 0; i < lights.Length; i++)
        {
            if (i < shotgunAmmo)
            {
                lights[i].GetComponent<MeshRenderer>().material = shotgunLoaded; //Change material to loaded
            }
            else if (i >= shotgunAmmo)
            {
                lights[i].GetComponent<MeshRenderer>().material = shotgunBlank; //Change material to blank
            }
        }
    }

    public void SwapWeapon()
    {
        if (selectedWeapon == 0)
        {
            selectedWeapon = 1; //Select shotgun
            ShotgunAmmoCheck();
        }
        else if (selectedWeapon == 1)
        {
            selectedWeapon = 0; //Select pistol
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].GetComponent<MeshRenderer>().material = lightsNormal; //Change material to normal blue lights
            }
        }
    }
}
