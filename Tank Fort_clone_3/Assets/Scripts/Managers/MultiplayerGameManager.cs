using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Colyseus;
using UnityEngine.UI;
using System;

public class MultiplayerGameManager : MonoBehaviour
{
    private bool allConnected = false;
    private int playersInitialized = 0;
    public int m_NumRoundsToWin = 5;            // Number of rounds a player has to win to win game
    public float m_StartDelay = 3f;             // Delay between start of RoundStarting and RoundPlaying phases
    public float m_EndDelay = 3f;               // Delay between end of RoundPlaying and RoundEnding phases
    public CameraControl m_CameraControl;       // Reference to CameraControl script for control during different phases
    public Text m_MessageText;                  // Reference to overlay Text to display winning text, etc.
    public GameObject m_TankPrefab;             // Reference to tank prefab
    public MultiplayerTankManager[] m_Tanks;               // Collection of managers for enabling/disabling different aspects of the tanks
    public int m_MyTankIndex;
    public GameObject[] m_SpawnPoints;

    private int m_RoundNumber;                  // Which round game is currently on
    private WaitForSeconds m_StartWait;         // Used to have a delay whilst round starts
    private WaitForSeconds m_EndWait;           // Used to have a delay whilst round or game ends
    private WaitForSeconds m_BuildWait;
    private MultiplayerTankManager m_RoundWinner;          // Reference to winner of current round; Used to make announcement of who won
    private MultiplayerTankManager m_GameWinner;           // Reference to winner of game; Used to make announcement of who won


    class PlayerInitializedMessage {
        public string id;
    }
    class FireMessage {
        public float force;
        public string id;
    }
    class DmgMessage {
        public float dmg;
        public string id;
    }
    private async void Start()
    {
        
        if (GameSettingsManager.testMode){
            print("Running in test mode");
            GameSettingsManager.client = new ColyseusClient(GameSettingsManager.serverAddress);
            GameSettingsManager.room = await GameSettingsManager.client.JoinOrCreate<MyRoomState>("room",
                new Dictionary<string, object>{
                    ["nickname"] = GameSettingsManager.nickname
                });
        } else if (GameSettingsManager.isOwner){
            await GameSettingsManager.lobby.Send("game-started", GameSettingsManager.room.Id);
        } else {
            print("You are not this game's owner.");
        }
        
        GameSettingsManager.room.OnMessage<PlayerInitializedMessage>("player-initialized", (message) => {
            print("A player initialized");
            playersInitialized++;
            print(playersInitialized);
        });
        GameSettingsManager.room.OnStateChange += async (state, isFirstState) => {
            if (state.networkedUsers.Count >= GameSettingsManager.minPlayerCount && !allConnected){
                allConnected = true;
                Debug.Log("All players have joined!");
            } 
        };

        m_SpawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        Debug.Log(m_SpawnPoints.Length);
        
        // Create delays so they only have to be made once
    }

    private void Awake(){
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);
        m_BuildWait = new WaitForSeconds(4f);
        // print(room.State.networkedUsers.Count);
        StartCoroutine(Pregame());
    }

    private IEnumerator Pregame(){
        while(!allConnected)
        {
            yield return new WaitForSeconds(2f);
            print("waiting for all players to join...");
        }
        SpawnAllTanks();
        while(playersInitialized < GameSettingsManager.minPlayerCount)
        {
            yield return new WaitForSeconds(2f);
            print("waiting for all players to initialize...");
        }


        // Once tanks have been created and camera is using them as targets, start game
        StartCoroutine(GameLoop());
    }

    void OnApplicationQuit()
    {
        GameSettingsManager.room.Leave();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }

    private async void SpawnAllTanks()
    {
        m_Tanks = new MultiplayerTankManager[GameSettingsManager.room.State.networkedUsers.Count];
        
        // For all the tanks, create them, set player number and references needed for control
        for (int i = 0; i < m_Tanks.Length; i++)
        {   
            NetworkedUser user = ((NetworkedUser)(GameSettingsManager.room.State.networkedUsers.GetByIndex(i)));
            if (GameSettingsManager.room.SessionId == user.sessionId){
                m_Tanks[i] = new MultiplayerTankManager();
                m_MyTankIndex = i;
                m_Tanks[i].m_PlayerColor = Color.green;
                m_Tanks[i].m_SessionId = user.sessionId;
                m_Tanks[i].m_SpawnPoint = m_SpawnPoints[i].transform;
                m_Tanks[i].m_Instance = (GameObject)Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation);
                m_Tanks[i].m_Instance.name = user.sessionId;
                m_Tanks[i].m_PlayerNumber = i + 1;
                m_Tanks[i].Setup();
                await GameSettingsManager.room.Send("move", new {
                    init = true,
                    isPlayer = true,

                    xPos = m_Tanks[i].m_Instance.transform.position.x,
                    yPos = m_Tanks[i].m_Instance.transform.position.y,
                    zPos = m_Tanks[i].m_Instance.transform.position.z,
                    
                    xRot = m_Tanks[i].m_Instance.transform.rotation.x,
                    yRot = m_Tanks[i].m_Instance.transform.rotation.y,
                    zRot = m_Tanks[i].m_Instance.transform.rotation.z,
                    wRot = m_Tanks[i].m_Instance.transform.rotation.w,
                });
            } else {
                m_Tanks[i] = new MultiplayerTankManager();
                m_Tanks[i].m_PlayerColor = Color.green;
                m_Tanks[i].m_SessionId = user.sessionId;
                m_Tanks[i].m_SpawnPoint = m_SpawnPoints[i].transform;
                m_Tanks[i].m_Instance = (GameObject)Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation);
                m_Tanks[i].m_Instance.name = user.sessionId;
                m_Tanks[i].m_PlayerNumber = i + 1;
                Destroy(m_Tanks[i].m_Instance.GetComponent<TankMovementMultiplayer>());
                // Destroy(m_Tanks[i].m_Instance.GetComponent<TankHealth>());
                m_Tanks[i].m_Instance.GetComponent<TankShooting>().m_IsDummy = true;
                
            }
            Debug.Log(user.sessionId);
            
        }
        GameSettingsManager.room.OnStateChange += (state, isFirstState) => {
            if (playersInitialized >= GameSettingsManager.minPlayerCount){
                for (int i = 0; i < m_Tanks.Length; i++){
                    if (GameSettingsManager.room.SessionId != m_Tanks[i].m_SessionId){
                        NetworkedEntity player = state.networkedEntities.GetByIndex(i) as NetworkedEntity;
                        m_Tanks[i].m_Instance.transform.position = Vector3.Lerp(m_Tanks[i].m_Instance.transform.position,new Vector3(
                            player.xPos,
                            player.yPos,
                            player.zPos
                        ), 0.4f);

                        m_Tanks[i].m_Instance.transform.rotation = Quaternion.Lerp(m_Tanks[i].m_Instance.transform.rotation, new Quaternion(
                            player.xRot,
                            player.yRot,
                            player.zRot,
                            player.wRot
                        ),0.7f);
                    }
                }
            }
        };
        GameSettingsManager.room.OnMessage<FireMessage>("fire", (message) => {
            print(message.force);
            for (int j = 0; j < m_Tanks.Length; j++){
                if (m_Tanks[j].m_SessionId == message.id){
                    m_Tanks[j].DummyFire(message.force);
                    break;
                }
            }
        });
        GameSettingsManager.room.OnMessage<DmgMessage>("dmg", (message) => {
            for (int j = 0; j < m_Tanks.Length; j++){
                if (m_Tanks[j].m_SessionId == message.id){
                    m_Tanks[j].m_Instance.GetComponent<TankHealth>().TakeDamage(message.dmg);

                }
            }
        });
    }



    // Called from start and will run each phase of game one after another
    private IEnumerator GameLoop()
    {
        // Start off running 'RoundStarting' coroutine but don't return until finished
        yield return StartCoroutine(RoundStarting());
        // Once 'RoundStarting' coroutine finishes, run 'RoundPlaying' coroutine but don't return until finished
        yield return StartCoroutine(RoundPlaying());
        // Once execution has returned, run 'RoundEnding' coroutine, don't return until finished
        yield return StartCoroutine(RoundEnding());
        // Once that other thing finishes, run 'RoundBuilding' coroutine but don't return until finished
        yield return StartCoroutine(RoundBuilding());

        // Code is not run until 'RoundEnding' finished; Check if game winner has been found
        if (m_GameWinner != null)
        {
            // TODO: go back to the multiplayer lobby
            Application.LoadLevel(Application.loadedLevel);
            GameSettingsManager.room.Leave();
        }
        else
        {
            // If there isn't winner yet, restart coroutine so loop continues
            // Note this coroutine doesn't yield;  This means current version of GameLoop will end
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        // As soon as round starts reset tanks and make sure they can't move
        ResetAllTanks();
        DisableTankControl();

        // Snap camera's zoom and position to something appropriate for reset tanks
        // m_CameraControl.SetStartPositionAndSize();

        // Increment round number and display text showing players what round it is
        m_RoundNumber++;
        m_MessageText.text = "BATTLE ROUND " + m_RoundNumber;

        // Wait for specified length of time until yielding control back to game loop
        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying() {
        // As soon as round begins, let players control tanks
        EnableTankControl();

        // Clear text from screen
        m_MessageText.text = string.Empty;

        // While not one tank left, return on next frame
        while (!OneTankLeft()) {
            yield return null;
        }
    }

    private IEnumerator RoundBuilding() {
        // Stop tanks from moving
        DisableTankControl();
        m_MessageText.text = "BUILD PHASE \n(i didn't program this yet)" + m_RoundNumber;
        yield return m_StartWait;

    }

    private IEnumerator RoundEnding()
    {
        // Stop tanks from moving
        DisableTankControl();

        // Clear winner from previous round
        m_RoundWinner = null;

        // See if there is winner since round is over
        m_RoundWinner = GetRoundWinner();

        // If there is winner, increment their score
        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;

        // See if someone has won the game
        m_GameWinner = GetGameWinner();

        // Get message based on scores and whether there is game winner and display it
        string message = EndMessage();
        m_MessageText.text = message;

        // Wait for specified length of time until yielding control back to game loop
        yield return m_EndWait;
    }

    // Used to check if one or fewer tanks remaining -> round should end
    private bool OneTankLeft()
    {
        // Initialize count of tanks left at zero
        int numTanksLeft = 0;

        // Go through all tanks, if active, increment counter
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        // If one or fewer tanks remain, return true, otherwise return false
        return numTanksLeft <= 1;
    }

    // Find out if there is winner of round
    // Called with assumption that 1 or fewer tanks are currently active
    private MultiplayerTankManager GetRoundWinner()
    {
        // Go through all tanks, if one is active, it is winner; return it
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        // If none of tanks are active it is draw; return null
        return null;
    }

    // Find out if there is winner of game
    private MultiplayerTankManager GetGameWinner()
    {
        // Go through all tanks, if one has enough rounds to win game, return it
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        // If no tanks have enough rounds to win, return null
        return null;
    }

    // Returns string message to display at end of each round
    private string EndMessage()
    {
        // By default when round ends there are no winners, so default end message is draw
        string message = "DRAW!";

        // If there is winner then change message
        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        // Add some line breaks after initial message
        message += "\n\n\n\n";

        // Go through all tanks and add each of their scores to message
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        // If there is game winner, change message
        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }

    // Turn all the tanks back on and reset positions and properties
    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        
        m_Tanks[m_MyTankIndex].EnableControl();
    }


    private void DisableTankControl()
    {
        m_Tanks[m_MyTankIndex].DisableControl();
    }
}