using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SelectionPlayer", menuName ="ScriptableObjects/SelectionPlayer")]
public class SelectionPlayer : ScriptableObject 
{
    public int ID;
    public SelectionItem item;
}
