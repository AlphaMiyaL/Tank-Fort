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
    public Camera SelectionCamera;
    public float groundOffset;
    public float afterSelectionDelay = 1;
    private float afterSelectionDelayStart;
    bool isSelecting;
    public Vector2 itemSize;
    public Transform SelectedItem { set { selectedItem = value; } }
    private Transform selectedItem;

    // Start is called before the first frame update
    void Start()
    {
        //buildGrid();
    }
    private void OnEnable()
    {
        StartSelecting();
    }
    // Update is called once per frame
    void Update()
    {
        if (isSelecting && selectedItem)
        {
            handleGridSelection();
            //display item in correct quad
            if (selectedQuad && !selectedQuad.HasItem())
            {
                selectedItem.gameObject.SetActive(transform);
                selectedItem.position = new Vector3(selectedQuad.transform.position.x, selectedItem.position.y, selectedQuad.transform.position.z);
            }
            else
            {

                selectedItem.gameObject.SetActive(false);
            }

            //rotate item with right click
            if (selectedQuad && !selectedQuad.HasItem() && Input.GetMouseButtonUp(1))
            {
                selectedItem.Rotate(new Vector3(0, 90, 0));
                
            }

            //place item with left click
            if (selectedQuad && !selectedQuad.HasItem() && Input.GetMouseButtonDown(0))
            {
                FindObjectOfType<SelectionHandler>().PlayerSelectsQuad(selectedQuad);
                isSelecting = false;
                selectedQuad.SetSelected(selectedItem.gameObject);
                StartCoroutine(finishSelection());
                
            }

            


        }
        

    }
    public void StartSelecting()
    {
        
        isSelecting = true;
    }
    private IEnumerator finishSelection()
    {
        yield return new WaitForSeconds(afterSelectionDelay);
        FindObjectOfType<SelectionHandler>().SelectionGrid();
    }
    public LayerMask GridLayer;
    private void handleGridSelection()
    {
        RaycastHit hit;
        Ray ray = SelectionCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000, GridLayer))
        //if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.GetComponent<SelectionQuad>())
            {

                SelectionQuad current = hit.transform.GetComponent<SelectionQuad>();
                if (!selectedQuad)
                {
                    selectedQuad = current;
                    
                }
                else if(current != selectedQuad)
                {
                    selectedQuad.SetSelected(false);
                    selectedQuad = current;
                    //current.SetSelected(true);
                }
                if(!current.HasItem() )current.SetSelected(true);
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
