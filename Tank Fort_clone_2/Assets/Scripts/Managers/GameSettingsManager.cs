using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Colyseus;
using UnityEngine.SceneManagement;

public class GameSettingsManager: MonoBehaviour {

    [SerializeField] LobbyController lobbyController;
    public static bool testMode = false;
    public static bool isOwner = false;
    public static string serverAddress = "ws://localhost:8000";
    public static ColyseusClient client;
    public static ColyseusRoom<MyRoomState> room;
    public static ColyseusRoom<MyLobbyState> lobby;
    public static string lobbyId = "";

    // "multiplayer" or "local"
    public static string gamemode = "";

    // set before loading into scene. remember this will be map dependent! 
    public static int minPlayerCount = 2;
    public static int maxPlayerCount = 4;

    // set before loading into scene
    public static string nickname = "generic nickname";

    private void Start(){
        client = new ColyseusClient(serverAddress);
    }


    public async void CreateLobby(){
        lobby = await client.Create<MyLobbyState>("lobby",
                new Dictionary<string, object>{
                    ["nickname"] = GameSettingsManager.nickname
                });
        isOwner = true;
        lobby.OnStateChange += async (state, isFirstState) => {
            lobbyController.SetPlayerCount(state.networkedUsers.Count, maxPlayerCount);
        };
        lobbyController.SetLobbyId(lobby.Id);
        
    }
    class GameStartMessage {
        public string roomId;
    }
    IEnumerator Wait()
    {

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(3);
    }
    public async void JoinLobbyById(){
        lobby = await client.JoinById<MyLobbyState>(lobbyId,
                new Dictionary<string, object>{
                    ["nickname"] = GameSettingsManager.nickname
                });
        isOwner = false;
        lobby.OnStateChange += async (state, isFirstState) => {
            lobbyController.SetPlayerCount(state.networkedUsers.Count, maxPlayerCount);
        };
        lobby.OnMessage<GameStartMessage>("game-started",  async (message) => {

            print("Owner started game.");
            print(message.roomId);
            StartCoroutine(Wait());
            GameSettingsManager.room = await GameSettingsManager.client.JoinById<MyRoomState>(message.roomId,
                new Dictionary<string, object>{
                    ["nickname"] = GameSettingsManager.nickname
                });
            // Eventually, different scenes will be loaded based on map
            gamemode = "multiplayer";
            SceneManager.LoadScene("Multiplayer");
        });
        lobbyController.SetLobbyId(lobby.Id);
    }

    public void LeaveLobby(){
        lobby.Leave();
    }


    public async void StartGame(){
        testMode = false;
        if (!isOwner){
            print("You aren't the lobby owner!");
        } else if (lobby.State.networkedUsers.Count < minPlayerCount) {
            print("Not enough people have joined yet!");
        } else {
            print("You are this game's owner.");
            GameSettingsManager.room = await GameSettingsManager.client.Create<MyRoomState>("room",
                new Dictionary<string, object>{
                    ["nickname"] = GameSettingsManager.nickname
                });
            // Eventually, different scenes will be loaded based on map
            gamemode = "multiplayer";
            SceneManager.LoadScene("Multiplayer");
        }
    }
}   