using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public ScoreManager scoreManager;
    public Button restartButton;   
    public Animator playerAnimator;
    public Animator botAnimator;  

    private bool isGameOver = false;
    private bool playerDefeated = false; // Track which entity is defeated

    private void Start()
    {
        restartButton.gameObject.SetActive(false);
        restartButton.onClick.AddListener(RestartGame); 
    }

    private void Update()
    {
        if (!isGameOver && (scoreManager.playerScore >= 7 || scoreManager.botScore >= 7))
        {
            StartCoroutine(GameOver());
        }
    }

    private IEnumerator GameOver()
    {
        isGameOver = true;

        // Determine the defeated entity and set the appropriate "Fall" trigger
        if (scoreManager.playerScore >= 7 && botAnimator != null)
        {
            botAnimator.SetTrigger("Fall");  // Bot is defeated
            playerDefeated = false;
        }
        else if (scoreManager.botScore >= 7 && playerAnimator != null)
        {
            playerAnimator.SetTrigger("Fall");  // Player is defeated
            playerDefeated = true;
        }

        // Wait until the "Fall" animation has completed for the defeated entity
        yield return new WaitUntil(() =>
        {
            if (playerDefeated)
            {
                return playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("DramaticFall") && 
                       playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && 
                       !playerAnimator.IsInTransition(0);
            }
            else
            {
                return botAnimator.GetCurrentAnimatorStateInfo(0).IsName("DramaticFall") && 
                       botAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && 
                       !botAnimator.IsInTransition(0);
            }
        });

        // Freeze the game immediately after the animation completes
        Time.timeScale = 0f;

        // Show the Restart Button
        restartButton.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        // Only set the "Restart" trigger for the defeated entity
        if (playerDefeated)
        {
            playerAnimator.SetTrigger("Restart"); 
        }
        else
        {
            botAnimator.SetTrigger("Restart"); 
        }

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
