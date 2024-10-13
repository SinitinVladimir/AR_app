using TMPro; // Make sure to import TextMeshPro
using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    public int playerScore = 0;
    public int botScore = 0;

    // References to the TMP Text components
    public TMP_Text playerScoreText;
    public TMP_Text botScoreText;

    void Start()
    {
        // Initialize the UI with the starting scores
        UpdateScoreUI();
    }

    public void AddPointToPlayer()
    {
        playerScore++;
        UpdateScoreUI();
    }

    public void AddPointToBot()
    {
        botScore++;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        // Update the TMP UI text with the current scores
        if (playerScoreText != null)
            playerScoreText.text = "Player: " + playerScore;

        if (botScoreText != null)
            botScoreText.text = "Bot: " + botScore;
    }
}
