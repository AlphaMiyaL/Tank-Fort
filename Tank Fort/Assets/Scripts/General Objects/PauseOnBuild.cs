using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseOnBuild : MonoBehaviour
{
    //public bool pause = false;
    public GameObject turnOnOff;

    public void turnOn() {
        turnOnOff.SetActive(true);
        Rotate rotateScript = this.gameObject.GetComponent<Rotate>();
        //if current gameobject has a rotate script, turn it on
        if (rotateScript) {
            rotateScript.enabled = true;
        }

    }

    public void turnOff() {
        turnOnOff.SetActive(false);
        Rotate rotateScript = this.gameObject.GetComponent<Rotate>();
        //if current gameobject has a rotate script, turn it off
        if (rotateScript) {
            rotateScript.enabled = false;
        }
    }
}
