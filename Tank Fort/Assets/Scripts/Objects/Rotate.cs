using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour{
    public float turnSpeed = 45f;      // How fast the take turns in degrees per sec


    void Update() {
        float turn = turnSpeed * Time.deltaTime;
        transform.Rotate(0, turn, 0);
    }
}
