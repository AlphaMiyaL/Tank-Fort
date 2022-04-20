using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public float delayInSeconds = 1.5f;          //Time between shots
    public Rigidbody projectile;                 //Reference to projectile object;
    public Transform fireTransform;              //Reference to location where projectile will be launched from
    public float launchForce;                    //Launch speed of the projectile
    public bool pause = true;                           //Boolean determining if the turret is paused on shooting

    private WaitForSeconds waiting;
    private float lastTimeFired=0;


    private void Update() {
        if (Time.time >lastTimeFired+delayInSeconds && pause==false) {
            Shoot();
            lastTimeFired = Time.time;
        }
    }

    private void Shoot() {
        // Create instance of shell and store a reference to it's rigidbody
        Rigidbody shellInstance =
            Instantiate(projectile, fireTransform.position, fireTransform.rotation) as Rigidbody;

        // Set shell's velocity to launch force in fire position's forward direction
        shellInstance.velocity = launchForce * fireTransform.forward; ;
    }

    public void changePause() {
        pause = !pause;
        lastTimeFired = Time.time;
    }

}
