using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionGrid : MonoBehaviour
{
    public SelectionQuad QuadPrefab;
    public float width, height;
    public Vector2 gridSize;
    private SelectionQuad[,] selectionQuads;
    private SelectionQuad selectedQuad;

    // Start is called before the first frame update
    void Start()
    {
        //buildGrid();
    }

    // Update is called once per frame
    void Update()
    {
        handleGridSelection();
        if(selectedQuad && Input.GetMouseButtonDown(0))
        {
            FindObjectOfType<SelectionHandler>().PlayerSelectsQuad(selectedQuad);
            FindObjectOfType<SelectionHandler>().SelectionGrid();
        }
    }

    private void handleGridSelection()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.GetComponent<SelectionQuad>())
            {

                SelectionQuad current = hit.transform.GetComponent<SelectionQuad>();
                if (!selectedQuad)
                {
                    selectedQuad = current;
                    current.SetSelected(true);
                }
                else if(current != selectedQuad)
                {
                    selectedQuad.SetSelected(false);
                    selectedQuad = current;
                    current.SetSelected(true);
                }

            }
            else if(selectedQuad)
            {
                selectedQuad.SetSelected(false);
                selectedQuad = null;
            }
        }
    }
    public void BuildGrid()
    {
        buildGrid();
    }

    private void buildGrid()
    {
        int xSteps = (int) (width / gridSize.x);
        int zSteps = (int)(height / gridSize.y);
        selectionQuads = new SelectionQuad[xSteps, zSteps];
        for (int x = 0; x < xSteps; x += 1)
        {
            for(int z = 0; z < zSteps; z += 1)
            {
                SelectionQuad quad = Instantiate(QuadPrefab);
                quad.transform.parent = transform;
                selectionQuads[x, z] = quad;
                Vector3 pos = new Vector3(x * gridSize.x - width / 2, transform.position.y, z * gridSize.y - height / 2);
                quad.Setup(gridSize, pos, x, z);
            }
        }
    }
}
