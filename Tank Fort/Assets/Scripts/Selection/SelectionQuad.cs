using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionQuad : MonoBehaviour
{
    public GameObject myItem;
    public Material m_Normal, m_Selected;
    public int x, z;
    public Transform[] corners;
    public bool validQuad;
    public void Setup(Vector2 size, Vector3 pos, int x, int z)
    {
        this.x = x;
        this.z = z;
        transform.localScale = new Vector3(size.x, size.y, 1);
        transform.position = pos;

        RaycastHit hit;
        
        if (!Physics.Raycast(transform.position + Vector3.up * 100, Vector3.down, out hit) || hit.transform != transform) gameObject.SetActive(false);
        if (!Physics.Raycast(corners[0].position + Vector3.up * 100, Vector3.down, out hit) || hit.transform != transform) gameObject.SetActive(false);
        if (!Physics.Raycast(corners[1].position + Vector3.up * 100, Vector3.down, out hit) || hit.transform != transform) gameObject.SetActive(false);
        if (!Physics.Raycast(corners[2].position + Vector3.up * 100, Vector3.down, out hit) || hit.transform != transform) gameObject.SetActive(false);
        if (!Physics.Raycast(corners[3].position + Vector3.up * 100, Vector3.down, out hit) || hit.transform != transform) gameObject.SetActive(false);

        if (gameObject.activeSelf) validQuad = true;
        else validQuad = false;
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
        return myItem != null;
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
