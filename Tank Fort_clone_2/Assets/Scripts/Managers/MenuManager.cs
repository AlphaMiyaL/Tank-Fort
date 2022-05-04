using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public Animator ani;
    void Start()
    {
        ani.SetBool("Close", false);
    }
}
