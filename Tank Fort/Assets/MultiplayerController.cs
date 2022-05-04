using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MultiplayerController : MonoBehaviour
{
    [SerializeField] TMP_InputField NicknameField;
    [SerializeField] TMP_InputField LobbyIdField;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable(){
        NicknameField.text = "";
        LobbyIdField.text = "";
    }

    public void SetLobbyId(){
        GameSettingsManager.lobbyId = LobbyIdField.text;
    }
    public void SetNickname(){
        GameSettingsManager.nickname = NicknameField.text;
    }
}
