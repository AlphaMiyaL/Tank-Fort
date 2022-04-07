using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {
        setupPlayers();
        setupItems();
        Board.gameObject.SetActive(true);
        Grid.BuildGrid();
        Grid.gameObject.SetActive(false);
    }

    
    public void SelectionBoard()
    {
        Board.gameObject.SetActive(false);
        Grid.gameObject.SetActive(true);
    }
    public void SelectionGrid()
    {
        Grid.gameObject.SetActive(false);
        getNextCurrentSelectionPlayer();
        if (currentSelectionPlayer)
        {
            Board.gameObject.SetActive(true);
        }
        else
        {
            //game start

        }
    }
    public void PlayerSelectsQuad(SelectionQuad quad)
    {
        PlayerSelectsQuad(currentSelectionPlayer.ID, quad);
    }

    public void PlayerSelectsQuad(int playerID, SelectionQuad quad)
    {
        SelectionPlayer player = GetSelectionPlayer(playerID);
        Vector3 quadPos = quad.transform.position;
        Instantiate(player.item.Prefab, quadPos, Quaternion.identity);

    }
    //private void handleMouseClick()
    //{
    //    RaycastHit hit;
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        if (hit.transform.GetComponent<SelectionObject>())
    //        {
                
    //            PlayerSelectItem(hit.transform.GetComponent<SelectionObject>());
    //            getNextCurrentSelectionPlayer();
                
    //        }
    //    }
    //}

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
        for(int i = 0; i < PlayerCount; i += 1)
        {
            SelectionPlayer newPlayer = PlayerPrefabs[i];
            //newPlayer.ID = i;
            Players.Add(newPlayer);
        } 
        
        for(int i = 0; i < Players.Count; i += 1)
        {
            int rand = Random.Range(0, Players.Count);
            SelectionPlayer temp = Players[i];
            Players[i] = Players[rand];
            Players[rand] = temp;
        }
        currentSelectionPlayer = Players[0];
    }

    void setupItems()
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

    public SelectionPlayer GetSelectionPlayer(int playerID)
    {
        foreach(SelectionPlayer player in Players)
        {
            if (player.ID == playerID) return player;
        }
        Debug.LogWarning($"playerID {playerID} not found.");
        return null;
    }

    private void removeSelectionItem(SelectionObject item)
    {
        item.gameObject.SetActive(false);
    }
}
