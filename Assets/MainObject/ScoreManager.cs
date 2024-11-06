using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int playerScore = 0;
    public int botScore = 0;

    public TMP_Text scoreText; 

    public void Start()
    {
        UpdateScoreUI();
    }

    public void AddPointToPlayer()
    {
        playerScore++;
        UpdateScoreUI();
        CheckGameOver();
    }

    public void AddPointToBot()
    {
        botScore++;
        UpdateScoreUI();
        CheckGameOver();
    }

    public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Player: " + playerScore + "\nBot: " + botScore;
        }
    }

    private void CheckGameOver()
    {
        if (playerScore >= 7)
        {
            Debug.Log("Player wins by reaching 7 points.");
        }
        else if (botScore >= 7)
        {
            Debug.Log("Bot wins by reaching 7 points.");
        }
    }

}
