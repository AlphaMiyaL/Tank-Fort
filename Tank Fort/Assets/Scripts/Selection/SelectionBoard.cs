using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBoard : MonoBehaviour
{
    public float width = 20, depth = 20, bottomOffset = 0.01f;
    private List<SelectionItem> placedItems = new List<SelectionItem>();
    

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) handleMouseClick();
    }
    protected virtual void handleMouseClick()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.GetComponent<SelectionObject>())
            {

                SelectionObjectClicked(hit.transform.GetComponent<SelectionObject>());
                
            }
        }
    }
    public void SelectionObjectClicked(SelectionObject selectionObject)
    {
        FindObjectOfType<SelectionHandler>().PlayerSelectItem(selectionObject);
        FindObjectOfType<SelectionHandler>().SelectionBoard();
    }
    public virtual void PlaceObjects(SelectionItem[] items)
    {
        foreach(SelectionObject item in GetComponentsInChildren<SelectionObject>())
        {
            Destroy(item.gameObject);
        }
        List<Vector2Int> positions = new List<Vector2Int>();
        float maxRadius = 0;
        foreach (SelectionItem item in items)
        {
            if (maxRadius < item.SelectionPrefab.GetComponent<SelectionObject>().Radius) maxRadius = item.SelectionPrefab.GetComponent<SelectionObject>().Radius;
        }

        int xGrid = (int)(width / (2 * maxRadius));
        int zGrid = (int)(depth / (2 * maxRadius));

        for (int x = 0; x < xGrid; x += 1)
        {
            for (int z = 0; z < zGrid; z += 1)
            {
                positions.Add(new Vector2Int(x, z));
            }
        }

        foreach (SelectionItem item in items)
        {
            int posIndex = Random.Range(0, positions.Count);
            Vector2Int posInt = positions[posIndex];
            positions.RemoveAt(posIndex);
            Vector2 pos = new Vector2(transform.position.x - width/ 2 + posInt.x * 2 * maxRadius + maxRadius, transform.position.z - depth/2 + posInt.y * 2 * maxRadius + maxRadius);
            placeSelectionItem(item, pos);
        }
    }

    void placeSelectionItem(SelectionItem prefab, Vector2 pos)
    {
        SelectionObject item = Instantiate(prefab.SelectionPrefab, new Vector3(pos.x, bottomOffset + transform.position.y, pos.y), Quaternion.identity).GetComponent<SelectionObject>();
        item.transform.parent = transform;
        placedItems.Add(prefab);
    }
}
