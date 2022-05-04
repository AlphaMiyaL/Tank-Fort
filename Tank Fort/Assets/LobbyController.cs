using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class LobbyController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TextMeshProUGUI LobbyCode;
    [SerializeField] TextMeshProUGUI PlayerCount;
    Button StartGameButton;
    void Start()
    {
        StartGameButton = GameObject.Find("StartButton").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLobbyId(string id){
        LobbyCode.SetText(id);
    }

    public void SetPlayerCount(int current, int max){
        PlayerCount.SetText(string.Format("{0}/{1}", current, max));
    }

}
