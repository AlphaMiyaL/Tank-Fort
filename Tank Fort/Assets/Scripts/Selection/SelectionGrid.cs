using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionGrid : MonoBehaviour {
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
    public int itemSize;
    public Transform SelectedItem { set { selectedItem = value; } }
    private Transform selectedItem;
    private SelectionItem selectionItem;
    public SelectionItem SelectionItem { set { selectionItem = value; } }
    public int RemoveRadius {
        set {
            removeRadius = value;
            if (removeRadius >= 0) removingObstacles = true;
            else removingObstacles = false;
        }
    }
    private int removeRadius;
    private bool removingObstacles;

    // Start is called before the first frame update
    void Start() {
        //buildGrid();
    }
    private void OnEnable() {
        StartSelecting();
    }
    // Update is called once per frame
    void Update() {
        if (isSelecting && selectedItem) {
            handleGridSelection();
            //display item in correct quad
            if (selectedQuad && (checkSize(selectedQuad) || removingObstacles)) {
                selectedItem.gameObject.SetActive(transform);
                selectedItem.position = new Vector3(selectedQuad.transform.position.x, selectedItem.position.y, selectedQuad.transform.position.z);
            }
            else {
                selectedItem.gameObject.SetActive(false);
            }

            //rotate item with right click
            if (selectedQuad && !selectedQuad.HasItem() && Input.GetMouseButtonUp(1)) {
                selectedItem.transform.Rotate(0, 90, 0, Space.World);
            }

            //place item with left click
            if ((selectedQuad || checkSize(selectedQuad)) && Input.GetMouseButtonDown(0)) {
                if (removingObstacles) {
                    Destroy(selectedItem.gameObject);
                    int x = selectedQuad.x;
                    int z = selectedQuad.z;
                    for (int r = 0; r <= removeRadius; r += 1) {
                        destroyObstacle(x + r, z);
                        destroyObstacle(x, z + r);
                        destroyObstacle(x - r, z);
                        destroyObstacle(x, z - r);
                        destroyObstacle(x + r, z + r);
                        destroyObstacle(x + r, z - r);
                        destroyObstacle(x - r, z + r);
                        destroyObstacle(x - r, z - r);
                    }
                    isSelecting = false;

                    StartCoroutine(finishSelection());
                }
                else if (selectedQuad && checkSize(selectedQuad)) {
                    List<SelectionQuad> selectedQuads = getSelectedQuads(selectedQuad);
                    foreach (SelectionQuad quad in selectedQuads) {
                        FindObjectOfType<SelectionHandler>().PlayerSelectsQuad(quad);
                        quad.SetSelected(selectedItem.gameObject, selectionItem);
                    }
                    isSelecting = false;

                    StartCoroutine(finishSelection());
                }


            }
        }
    }

    public void StartSelecting() {

        isSelecting = true;
    }
    private IEnumerator finishSelection() {
        yield return new WaitForSeconds(afterSelectionDelay);
        FindObjectOfType<SelectionHandler>().SelectionGrid();
    }
    public LayerMask GridLayer;
    private void handleGridSelection() {
        RaycastHit hit;
        Ray ray = SelectionCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000, GridLayer))
        //if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.GetComponent<SelectionQuad>()) {

                SelectionQuad current = hit.transform.GetComponent<SelectionQuad>();
                if (!selectedQuad) {
                    selectedQuad = current;

                }
                else if (current != selectedQuad) {
                    selectedQuad.SetSelected(false);
                    selectedQuad = current;
                    //current.SetSelected(true);
                }
                if (checkSize(current) || removingObstacles) current.SetSelected(true);
            }
            else if (selectedQuad) {
                selectedQuad.SetSelected(false);
                selectedQuad = null;
            }
        }
    }
    private List<SelectionQuad> getSelectedQuads(SelectionQuad current) {
        List<SelectionQuad> selectedQuads = new List<SelectionQuad>();
        int radius = itemSize / 2;
        for (int x = current.x - radius; x <= current.x + radius; x += 1) {
            for (int z = current.z - radius; z <= current.z + radius; z += 1) {
                selectedQuads.Add(selectionQuads[x, z]);
            }
        }
        return selectedQuads;
    }
    private bool checkSize(SelectionQuad current) {
        bool validSelection = true;
        int radius = itemSize / 2;

        if (current) {
            for (int x = current.x - radius; x <= current.x + radius; x += 1) {
                for (int z = current.z - radius; z <= current.z + radius; z += 1) {
                    if (!selectionQuads[x, z].validQuad || selectionQuads[x, z].HasItem()) {
                        validSelection = false;
                    }
                }
            }
        }
        else return false;

        //Debug.Log($"itemSize: {radius} validSelection: {validSelection}");
        return validSelection;
    }
    public void BuildGrid() {
        buildGrid();
    }

    private void buildGrid() {
        int xSteps = (int)(width / gridSize.x);
        int zSteps = (int)(height / gridSize.y);
        selectionQuads = new SelectionQuad[xSteps, zSteps];
        for (int x = 0; x < xSteps; x += 1) {
            for (int z = 0; z < zSteps; z += 1) {
                SelectionQuad quad = Instantiate(QuadPrefab);
                quad.transform.parent = transform;
                selectionQuads[x, z] = quad;
                Vector3 pos = new Vector3(x * gridSize.x - width / 2, transform.position.y, z * gridSize.y - height / 2);
                quad.Setup(gridSize, pos, x, z);
            }
        }
    }
    private void destroyObstacle(int x, int z) {
        if (x >= 0 && x <= width && z >= 0 && z <= height && selectionQuads[x, z] != null) {
            //Debug.Log($"Destroying {selectionQuads[x, z].name}");
            Destroy(selectionQuads[x, z].myItem);
            selectionQuads[x, z].myItem = null;

        }

    }

    public Vector3[] FindSafePlayerSpawn(int playerCount) {
        Vector3[] spawnPoints = new Vector3[playerCount];
        List<SelectionQuad> validQuads = new List<SelectionQuad>();
        List<SelectionQuad> itemQuads = new List<SelectionQuad>();
        for (int x = 0; x <= selectionQuads.GetUpperBound(0); x += 1) {
            for (int y = 0; y <= selectionQuads.GetUpperBound(1); y += 1) {
                if (selectionQuads[x, y].validQuad) {
                    if (selectionQuads[x, y].myItem == null) validQuads.Add(selectionQuads[x, y]);
                    else itemQuads.Add(selectionQuads[x, y]);
                }

            }
        }
        Debug.Log($"selectionQuads.Length: {selectionQuads.Length} validQuads.Count: {validQuads.Count} itemQuads.Count: {itemQuads.Count}");
        bool done = false;
        int timesRun = 0;
        while (!done) {
            List<SelectionQuad> spawnQuads = new List<SelectionQuad>();
            for (int i = 0; i < playerCount; i += 1) {
                int randQuad = Random.Range(0, validQuads.Count);
                spawnQuads.Add(validQuads[randQuad]);
            }

            bool validPlacement = true;
            for (int i = 0; i < spawnQuads.Count; i += 1) {
                for (int j = 0; j < spawnQuads.Count; j += 1) {
                    if (i != j) {
                        SelectionQuad quad1 = spawnQuads[i];
                        SelectionQuad quad2 = spawnQuads[j];
                        if (quad1 != quad2 && distance(quad1, quad2) < 5) validPlacement = false;
                        if (quad1 == quad2) validPlacement = false;
                    }
                }
            }

            foreach (SelectionQuad quad1 in spawnQuads) {
                foreach (SelectionQuad item in itemQuads) {
                    if (checkDirection(quad1, item) || distance(quad1, item) < item.mySelectionItem.safeRadius) validPlacement = false;
                }
            }
            if (validPlacement) {
                for (int i = 0; i < playerCount; i += 1) {
                    spawnPoints[i] = spawnQuads[i].transform.position - groundOffset * Vector3.up;
                }
                done = true;
            }
            else if(timesRun>200){
                for (int i = 0; i < playerCount; i += 1) {
                    spawnPoints[i] = new Vector3(i*15, 15, i*15);
                }
                done = true;
            }
            else {
                timesRun++;
            }
        }
        return spawnPoints;
    }

    private bool checkDirection(SelectionQuad playerQuad, SelectionQuad objectQuad) {
        if (objectQuad.mySelectionItem.FireDirection.y < 0) {
            Debug.Log(-objectQuad.myItem.transform.up);
            Vector3 forward = -objectQuad.myItem.transform.up;
            if (forward.x > 0) {
                if (playerQuad.x > objectQuad.x && playerQuad.z == objectQuad.z) return true;
            }
            else if (forward.x < 0) {
                if (playerQuad.x < objectQuad.x && playerQuad.z == objectQuad.z) return true;
            }
            else if (forward.z > 0) {
                if (playerQuad.x == objectQuad.x && playerQuad.z > objectQuad.z) return true;
            }
            else if (forward.z < 0) {
                if (playerQuad.x == objectQuad.x && playerQuad.z < objectQuad.z) return true;
            }
            return false;
        }
        else {
            return false;
        }
    }
    private float distance(SelectionQuad grid1, SelectionQuad grid2) {
        return Vector2.Distance(grid1.transform.position, grid2.transform.position);
    }
}
