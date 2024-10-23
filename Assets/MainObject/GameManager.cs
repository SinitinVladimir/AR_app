using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public ScoreManager scoreManager;
    public Button restartButton;          // Reference to the Restart Button
    public Animator playerAnimator;       // Animator for the player's boat
    public Animator botAnimator;          // Animator for the bot's boat

    private bool isGameOver = false;

    private void Start()
    {
        // Set the Restart Button to inactive initially
        restartButton.gameObject.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);  // Add listener for the restart button
    }

    private void Update()
    {
        if (!isGameOver && (scoreManager.playerScore >= 4 || scoreManager.botScore >= 4))
        {
            StartCoroutine(GameOver());
        }
    }

    private IEnumerator GameOver()
    {
        isGameOver = true;

        // Trigger the fall animation for the loser
        if (scoreManager.playerScore >= 4 && botAnimator != null)
        {
            botAnimator.SetTrigger("Fall");  // Set the "Fall" trigger for the bot
        }
        else if (scoreManager.botScore >= 4 && playerAnimator != null)
        {
            playerAnimator.SetTrigger("Fall");  // Set the "Fall" trigger for the player
        }

        // Wait until the "Fall" animation has completed
        yield return new WaitUntil(() =>
        {
            if (scoreManager.playerScore >= 4 && botAnimator != null)
            {
                return botAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !botAnimator.IsInTransition(0);
            }
            else if (scoreManager.botScore >= 4 && playerAnimator != null)
            {
                return playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !playerAnimator.IsInTransition(0);
            }
            return false;
        });

        // // Wait for an additional 2 seconds
        // yield return new WaitForSeconds(2f);

        // Freeze the game
        Time.timeScale = 0f;

        // Show the Restart Button
        restartButton.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        botAnimator.SetTrigger("Restart"); // Set the "Restart" trigger
        playerAnimator.SetTrigger("Restart"); // Set the "Restart" trigger

        // Reset the scores
        scoreManager.playerScore = 0;
        scoreManager.botScore = 0;
        scoreManager.UpdateScoreUI();

        // Reset Game Over status
        isGameOver = false;

        // Hide the Restart Button
        restartButton.gameObject.SetActive(false);

        // Unfreeze the game
        Time.timeScale = 1f;
    }
}
