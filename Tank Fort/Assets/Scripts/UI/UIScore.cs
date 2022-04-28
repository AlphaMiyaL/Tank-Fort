using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIScore : MonoBehaviour
{
    [SerializeField] UIScoreBar RoundWinBarPrefab, CoinBarPrefab, UnderDogBarPrefab, TrapPointBarPrefab;
    [SerializeField] float roundWinLength = 200, coinBarLength = 100, underDogLength = 300, trapPointLength = 100;
    [SerializeField] float growSpeed = 1;
    [SerializeField] float barMaxHeight = 380, barMinHeight = -454, barWidthStart = 160;
    [SerializeField] Text PlayerTextPrefab;
    [SerializeField] Text roundScoreText;
    int currentRound = 0;

    List<UIScorePlayer> playerScores;
    Vector2[] playerCurrentPos;
    bool displaying;
    UIScoreBar currentAnimation;
    [SerializeField] GameObject CompletedTarget;

    //private void Start()
    //{
    //    playerScores = new List<UIScorePlayer>();
    //    playerScores.Add(new UIScorePlayer(1, 2, 0, 1));
    //    playerScores.Add(new UIScorePlayer(0, 2, 1, 0));
    //    playerScores.Add(new UIScorePlayer(0, 0, 0, 0));
    //    playerScores.Add(new UIScorePlayer(0, 2, 0, 0));
    //    DisplayScores(playerScores);
        
    //}

    private void Update()
    {
        if (displaying)
        {
            if (!currentAnimation)
            {
                setupNextScoreBar();
            }
            else if(currentAnimation && !currentAnimation.growing){
                if(playerIndex >= playerCurrentPos.Length - 1 && scoreIndex >= 3)
                {
                    currentAnimation = null;
                    displaying = false;
                }
                else
                {
                    if(scoreIndex == 3)
                    {
                        playerIndex += 1;
                        scoreIndex = 0;
                    }
                    else
                    {
                        scoreIndex += 1;
                    }
                    setupNextScoreBar();
                }
                
            }

        }else if (Input.GetMouseButtonUp(0))
        {
            if(CompletedTarget)CompletedTarget.SendMessage("UIScoreCompleted");
            gameObject.SetActive(false);
        }
        
    }
    int playerIndex;
    int scoreIndex;
    public void DisplayScores(List<UIScorePlayer> playerScores)
    {
        gameObject.SetActive(true);
        if(playerCurrentPos == null)
        {
            playerCurrentPos = new Vector2[playerScores.Count];
            float ySpacing = (barMaxHeight - barMinHeight) / (playerScores.Count + 1);
            for(int i = 0; i < playerScores.Count; i += 1)
            {
                playerCurrentPos[i] = new Vector2(barWidthStart, barMaxHeight - (i + 1) * ySpacing);
                Text playerText = Instantiate(PlayerTextPrefab, transform);
                playerText.text = $"Tank {i + 1}";
                playerText.GetComponent<RectTransform>().localPosition = playerCurrentPos[i] - Vector2.right * 30;
            }
        }
        playerIndex = 0;
        scoreIndex = 0;
        displaying = true;
        this.playerScores = playerScores;
        currentRound += 1;
        roundScoreText.text = $"Round {currentRound} Scores";
    }

    private void setupNextScoreBar()
    {
        switch (scoreIndex)
        {
            case 0:
                if(playerScores[playerIndex].roundWin > 0)
                {
                    UIScoreBar bar = Instantiate(RoundWinBarPrefab, transform);
                    bar.setup(playerScores[playerIndex].roundWin * roundWinLength, playerCurrentPos[playerIndex]);
                    playerCurrentPos[playerIndex] = playerCurrentPos[playerIndex] + Vector2.right * playerScores[playerIndex].roundWin * roundWinLength;
                    currentAnimation = bar;
                }
                break;
            case 1:
                if (playerScores[playerIndex].coinCount > 0)
                {
                    UIScoreBar bar = Instantiate(CoinBarPrefab, transform);
                    bar.setup(playerScores[playerIndex].coinCount * coinBarLength, playerCurrentPos[playerIndex]);
                    playerCurrentPos[playerIndex] = playerCurrentPos[playerIndex] + Vector2.right * playerScores[playerIndex].coinCount * coinBarLength;
                    currentAnimation = bar;
                }
                break;
            case 2:
                if (playerScores[playerIndex].underDogWin > 0)
                {
                    UIScoreBar bar = Instantiate(UnderDogBarPrefab, transform);
                    bar.setup(playerScores[playerIndex].underDogWin * underDogLength, playerCurrentPos[playerIndex]);
                    playerCurrentPos[playerIndex] = playerCurrentPos[playerIndex] + Vector2.right * playerScores[playerIndex].underDogWin * underDogLength;
                    currentAnimation = bar;
                }
                break;
            case 3:
                if (playerScores[playerIndex].trapPoints > 0)
                {
                    UIScoreBar bar = Instantiate(TrapPointBarPrefab, transform);
                    
                    bar.setup(playerScores[playerIndex].trapPoints * trapPointLength, playerCurrentPos[playerIndex]);
                    playerCurrentPos[playerIndex] = playerCurrentPos[playerIndex] + Vector2.right * playerScores[playerIndex].trapPoints * trapPointLength;
                    currentAnimation = bar;
                }
                break;
            default:
                if (playerIndex == playerCurrentPos.Length - 1 && scoreIndex == 3)
                {
                    currentAnimation = null;
                    displaying = false;
                }
                else
                {
                    if (scoreIndex == 3)
                    {
                        playerIndex += 1;
                        scoreIndex = 0;
                    }
                    else
                    {
                        scoreIndex += 1;
                    }
                    setupNextScoreBar();
                }
                break;
        }
    }
}



public class UIScorePlayer
{
    public int roundWin;
    public int coinCount;
    public int underDogWin;
    public int trapPoints;

    public bool roundDisplayed, coinDisplayed, underDogDisplayed, trapPointDisplayed;

    public UIScorePlayer(int roundWin, int coinCount, int underDogWin, int trapPoints)
    {
        this.roundWin = roundWin;
        this.coinCount = coinCount;
        this.underDogWin = underDogWin;
        this.trapPoints = trapPoints;
    }
}
