using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionHandler : MonoBehaviour
{
    public int PlayerCount = 2;
    public Sprite ItemSpritePrefab;
    public SelectionPlayer[] PlayerPrefabs;
    public List<SelectionPlayer> Players;
    public SelectionItem[] SelectionItems;
    private SelectionPlayer currentSelectionPlayer;
    public SelectionBoard Board;
    public SelectionGrid Grid;
    public GameObject PlayerSelectingPanel;
    public Text PlayerSelectingText;
    private List<SelectionQuad> SelectedQuads = new List<SelectionQuad>();

    // Start is called before the first frame update
    void Start()
    {
       
    }
    

    public void StartSelection()
    {
        setupPlayers();
        setupItems();
        Board.gameObject.SetActive(true);
        setupGrid();
        Grid.gameObject.SetActive(false);
        //Grid.SelectionCamera.enabled = true;
    }
    void setupItems()
    {
        setupBoard(SelectionItems);
    }

    #region SelectionBoard

    public void SelectionBoard()
    {
        Board.gameObject.SetActive(false);
        PlayerSelectingPanel.SetActive(false);
        Grid.gameObject.SetActive(true);
    }

    private void setupBoard(SelectionItem[] SelectionItems)
    {
        
        Board.PlaceObjects(SelectionItems);
    }

    public void PlayerSelectItem(SelectionObject item)
    {
        PlayerSelectItem(currentSelectionPlayer.ID, item);
    }

    public void PlayerSelectItem(int playerID, SelectionObject item)
    {
        SelectionPlayer player = GetSelectionPlayer(playerID);
        player.item = item.item;
        Debug.Log($"Player {player.ID} item {item.item} set.");
        removeSelectionItem(item);

    }

    private void removeSelectionItem(SelectionObject item)
    {
        item.gameObject.SetActive(false);
    }
    #endregion

    #region SelectionGrid
    private void setupGrid()
    {
        if(SelectedQuads.Count > 0)
        {
            foreach(SelectionQuad quad in SelectedQuads)
            {
                quad.RemoveItem();
            }
            SelectedQuads.Clear();
        }
        else
        {
            Grid.BuildGrid();
        }
        
    }
    public void SelectionGrid()
    {
        Grid.gameObject.SetActive(false);
        getNextCurrentSelectionPlayer();
        if (currentSelectionPlayer)
        {
            Board.gameObject.SetActive(true);
            displayPlayerSelection();
        }
        else
        {
            //game start
            FindObjectOfType<GameManager>().StartGame();

        }
    }
    public void PlayerSelectsQuad(SelectionQuad quad)
    {
        PlayerSelectsQuad(currentSelectionPlayer.ID, quad);
    }

    public void PlayerSelectsQuad(int playerID, SelectionQuad quad)
    {
        SelectionPlayer player = GetSelectionPlayer(playerID);
        Vector3 quadPos = quad.transform.position - Vector3.up * Grid.groundOffset;
        GameObject item = Instantiate(player.item.Prefab, quadPos, Quaternion.identity);
        quad.SetSelected(item);
        SelectedQuads.Add(quad);
    }

    #endregion

    void getNextCurrentSelectionPlayer()
    {
        SelectionPlayer nextCurrent = null;
        for(int i = 1; i < Players.Count; i += 1)
        {
            if (currentSelectionPlayer == Players[i - 1])
            {
                nextCurrent = Players[i];
            }
        }
        currentSelectionPlayer = nextCurrent;
    }

    void setupPlayers()
    {
        if(Players.Count == 0)
        {
            for (int i = 0; i < PlayerCount; i += 1)
            {
                SelectionPlayer newPlayer = PlayerPrefabs[i];
                //newPlayer.ID = i;
                Players.Add(newPlayer);
            }
        }
        
        
        for(int i = 0; i < Players.Count; i += 1)
        {
            int rand = Random.Range(0, Players.Count);
            SelectionPlayer temp = Players[i];
            Players[i] = Players[rand];
            Players[rand] = temp;
        }
        currentSelectionPlayer = Players[0];
        displayPlayerSelection();
    }

    void displayPlayerSelection()
    {
        PlayerSelectingPanel.SetActive(true);
        PlayerSelectingText.text = $"Player {currentSelectionPlayer.ID+1} Selecting";
    }

    public SelectionPlayer GetSelectionPlayer(int playerID)
    {
        foreach (SelectionPlayer player in Players)
        {
            if (player.ID == playerID) return player;
        }
        Debug.LogWarning($"playerID {playerID} not found.");
        return null;
    }
}