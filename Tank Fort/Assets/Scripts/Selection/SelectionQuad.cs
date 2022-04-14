﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionQuad : MonoBehaviour
{
    private GameObject myItem;
    public Material m_Normal, m_Selected;
    int x, z;
    public void Setup(Vector2 size, Vector3 pos, int x, int z)
    {
        this.x = x;
        this.z = z;
        transform.localScale = new Vector3(size.x, size.y, 1);
        transform.position = pos;
    }

    public Vector2Int GetIndex()
    {
        return new Vector2Int(x, z);
    }
    public void SetSelected(GameObject myItem)
    {
        this.myItem = myItem;
        SetSelected(false);
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected) GetComponent<Renderer>().material = m_Selected;
        else GetComponent<Renderer>().material = m_Normal;
    }
    public bool HasItem()
    {
        return myItem;
    }

    public void RemoveItem()
    {
        if (myItem)
        {
            Destroy(myItem);
            myItem = null;
        }
    }
}