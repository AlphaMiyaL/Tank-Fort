using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectDamage : MonoBehaviour
{
    public float delay;
    public float FireDamage;
    public float IceDamage;

    private float lastTimeDamaged;

    private void Start() {
        lastTimeDamaged = Time.time;
    }

    private void OnParticleCollision(GameObject other) {
        if (other.tag == "Fire" && Time.time > lastTimeDamaged + delay) {
            this.gameObject.GetComponent<TankHealth>().TakeDamage(FireDamage);
            lastTimeDamaged = Time.time;
        }
        if (other.tag == "Ice") {
            this.gameObject.GetComponent<TankHealth>().TakeDamage(IceDamage);
            lastTimeDamaged = Time.time;
        }
    }
}
