using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;            // Number of rounds a player has to win to win game
    public float m_StartDelay = 3f;             // Delay between start of RoundStarting and RoundPlaying phases
    public float m_EndDelay = 3f;               // Delay between end of RoundPlaying and RoundEnding phases
    public CameraControl m_CameraControl;       // Reference to CameraControl script for control during different phases
    public Text m_MessageText;                  // Reference to overlay Text to display winning text, etc.
    public GameObject m_TankPrefab;             // Reference to tank prefab
    public TankManager[] m_Tanks;               // Collection of managers for enabling/disabling different aspects of the tanks


    private int m_RoundNumber;                  // Which round game is currently on
    private WaitForSeconds m_StartWait;         // Used to have a delay whilst round starts
    private WaitForSeconds m_EndWait;           // Used to have a delay whilst round or game ends
    private TankManager m_RoundWinner;          // Reference to winner of current round; Used to make announcement of who won
    private TankManager m_GameWinner;           // Reference to winner of game; Used to make announcement of who won

    public SelectionHandler SelectionHandler;
    public CameraSetup CameraSetup;
    [SerializeField]private Animator tankTransition;

    private void Start()
    {
        //StartCoroutine(TankTransition());
        StartCoroutine(StartSelection());
    }

    public void TankTransition(bool closed)
    {
        tankTransition.SetBool("Close", closed);
        //yield return new WaitForSeconds(1);
    }
    public IEnumerator StartSelection()
    {
        TankTransition(true);
        yield return new WaitForSeconds(1);
        m_MessageText.text = "";
        CameraSetup.SetCameras(true);
        SelectionHandler.StartSelection();
        TankTransition(false);
        yield return new WaitForSeconds(1);
    }
    public void StartGame()
    {
        StartCoroutine(GameStart());
    }
    private IEnumerator GameStart()
    {
        TankTransition(true);
        yield return new WaitForSeconds(1);
        CameraSetup.SetCameras(false);

        // Create delays so they only have to be made once
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        if(m_RoundNumber == 0) SpawnAllTanks();

        foreach (Shooting shoot in FindObjectsOfType<Shooting>()) {
            shoot.changePause();
        }
        
        //SetCameraTargets();
        // Once tanks have been created and camera is using them as targets, start game
        StartCoroutine(GameLoop());
    }


    private void SpawnAllTanks()
    {
        // For all the tanks, create them, set player number and references needed for control
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }

    private void SetCameraTargets()
    {
        // Create collection of transforms same size as number of tanks
        Transform[] targets = new Transform[m_Tanks.Length];

        // For each of these transforms, set it to appropriate tank transform
        for (int i = 0; i < targets.Length; i++){
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        // Targets camera should follow
        m_CameraControl.m_Targets = targets;
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

        // Code is not run until 'RoundEnding' finished; Check if game winner has been found
        if (m_GameWinner != null)
        {
            //If there is game winner, restart level
            //Application.LoadLevel(Application.loadedLevel);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            // If there isn't winner yet, restart coroutine so loop continues
            // Note this coroutine doesn't yield;  This means current version of GameLoop will end
            //StartCoroutine(GameLoop());
            StartCoroutine(StartSelection());
        }
    }


    private IEnumerator RoundStarting()
    {
        // As soon as round starts reset tanks and make sure they can't move
        ResetAllTanks();
        DisableTankControl();

        // Snap camera's zoom and position to something appropriate for reset tanks
        //m_CameraControl.SetStartPositionAndSize();

        // Increment round number and display text showing players what round it is
        m_RoundNumber++;
        m_MessageText.text = "BATTLE ROUND " + m_RoundNumber;

        TankTransition(false);
        //yield return new WaitForSeconds(1);
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

    private IEnumerator RoundEnding()
    {
        // Stop tanks from moving, stops turrets from shooting
        DisableTankControl();
        foreach (Shooting shoot in FindObjectsOfType<Shooting>()) {
            shoot.changePause();
        }

        // Clear winner from previous round
        m_RoundWinner = null;

        // See if there is winner since round is over
        m_RoundWinner = GetRoundWinner();

        // If there is winner, increment their score
        if (m_RoundWinner != null) {
            m_RoundWinner.m_Wins++;
            foreach (Coin coin in FindObjectsOfType<Coin>()) {
                if (coin.addPointTo != -1) {
                    m_Tanks[coin.addPointTo - 1].m_Wins += 0.5;
                    Destroy(coin.gameObject);
                }
            }
        }

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
    private TankManager GetRoundWinner()
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
    private TankManager GetGameWinner()
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
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}