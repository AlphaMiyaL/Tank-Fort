using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectionItem", menuName = "ScriptableObjects/SelectionItem")]
public class SelectionItem : ScriptableObject
{
    public Sprite sprite;
    public GameObject SelectionPrefab;
    public GameObject ObstaclePrefab;
    public Vector3 centerOffset;
    //public float ItemRadius = 1;
    public int size;
    public int damage = -1;

}
