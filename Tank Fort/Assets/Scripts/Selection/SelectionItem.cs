using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SelectionItem", menuName = "ScriptableObjects/SelectionItem")]
public class SelectionItem : ScriptableObject
{
    public Sprite sprite;
    public GameObject Prefab;
    public GameObject ObstaclePrefab;
    //public float ItemRadius = 1;


}
