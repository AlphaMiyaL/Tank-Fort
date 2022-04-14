using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour {
    public Camera selectionCamera;
    public Camera cam1;
    public Camera cam2;
    public bool Horizontal;

    private void Start() {
        //ChangeSplitScreen();
    }

    private void ChangeSplitScreen() {
        Horizontal = !Horizontal;
        if (Horizontal) {
            cam1.rect = new Rect(0, 0, 1, 0.5f);
            cam2.rect = new Rect(0, 0.5f, 1, 0.5f);
        }
        else {
            cam1.rect = new Rect(0, 0, 0.5f, 1);
            cam2.rect = new Rect(0.5f, 0, 0.5f, 1);
        }
    }

    public void SetCameras(bool isSelection)
    {
        if (isSelection)
        {
            selectionCamera.gameObject.SetActive(true);
            cam1.gameObject.SetActive(false);
            cam2.gameObject.SetActive(false);
        }
        else
        {
            selectionCamera.gameObject.SetActive(false);
            cam1.gameObject.SetActive(true);
            cam2.gameObject.SetActive(true);
            ChangeSplitScreen();
        }
    }
}

