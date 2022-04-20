using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float speed;
    public Vector3 Direction { get; set; }

    //Projectile movement
    void Update()
    {
        transform.Translate(Direction * speed * Time.deltaTime, Space.World);
    }
}
