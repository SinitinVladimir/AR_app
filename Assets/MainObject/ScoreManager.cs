using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int playerScore = 0;
    public int botScore = 0;

    public TMP_Text scoreText; // Reference to the score display text

    public void Start()
    {
        // Initialize the UI with the starting scores
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
        if (playerScore >= 4)
        {
            Debug.Log("Player wins by reaching 4 points.");
        }
        else if (botScore >= 4)
        {
            Debug.Log("Bot wins by reaching 4 points.");
        }
    }

}
