using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionBoardUI : SelectionBoard
{
    public SelectionObjectUI prefab;
    public Transform UIPanel;
    public Vector2 buttonSize = new Vector2(125, 125);
    //protected override void handleMouseClick()
    //{
    //    //base.handleMouseClick();
    //}
    public override void PlaceObjects(SelectionItem[] items)
    {
        foreach (SelectionObject item in UIPanel.GetComponentsInChildren<SelectionObject>())
        {
            Destroy(item.gameObject);
        }
        List<Vector2Int> positions = new List<Vector2Int>();
        //float maxRadius = 0;
        //foreach (SelectionItem item in items)
        //{
        //    if (maxRadius < item.SelectionPrefab.GetComponent<SelectionObject>().Radius) maxRadius = item.SelectionPrefab.GetComponent<SelectionObject>().Radius;
        //}
        width = UIPanel.GetComponent<RectTransform>().sizeDelta.x;
        depth = UIPanel.GetComponent<RectTransform>().sizeDelta.y;
        int xGrid = (int)(width / buttonSize.x);
        int yGrid = (int)(depth / buttonSize.y);

        for (int x = 0; x < xGrid; x += 1)
        {
            for (int y = 0; y < yGrid; y += 1)
            {
                positions.Add(new Vector2Int(x, y));
            }
        }

        foreach (SelectionItem item in items)
        {
            int posIndex = Random.Range(0, positions.Count);
            Vector2Int posInt = positions[posIndex];
            positions.RemoveAt(posIndex);
            SelectionObjectUI button = Instantiate(prefab, UIPanel);
            button.GetComponent<Image>().sprite = item.sprite;
            button.item = item;
            button.GetComponent<Image>().enabled = true;
            button.GetComponent<Button>().enabled = true;
            Vector3 panelPos = UIPanel.GetComponent<RectTransform>().position;
            button.GetComponent<RectTransform>().position = new Vector3(posInt.x * buttonSize.x - width / 2, posInt.y * buttonSize.y - depth / 2, 0) + panelPos;
        }
    }
}
