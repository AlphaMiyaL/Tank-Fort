using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electrocute : MonoBehaviour{
    public bool electrocuting = false;   // Boolean determining whether it electric floor is on
    public float onOffDelay = 3f;        // Time spent on/off before swapping to the other
    public GameObject electric;          // Electric object to be turned on or off

    private WaitForSeconds onOffWait;

    private void Start() {
        onOffWait = new WaitForSeconds(onOffDelay);
        StartCoroutine(electricLoop());
    }

    private IEnumerator electricLoop() {
        reverseElectric();
        yield return onOffWait;

        StartCoroutine(electricLoop());
    }

    private void reverseElectric() {
        electrocuting = !electrocuting;
        electric.SetActive(electrocuting);
    }
}
