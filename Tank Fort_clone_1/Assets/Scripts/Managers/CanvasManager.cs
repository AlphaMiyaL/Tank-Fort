using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public Canvas PauseCanvas;

    //Need to disable and reenable pauseCanvas for the game to detect the buttons working
    void Awake()
    {
        PauseCanvas.enabled = false;
        PauseCanvas.enabled = true;
    }
}
