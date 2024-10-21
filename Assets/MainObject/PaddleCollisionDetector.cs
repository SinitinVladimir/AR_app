using UnityEngine;

public class PaddleCollisionDetector : MonoBehaviour
{
    public GameObject myPlayerPaddle; // Drag and drop your player paddle in the inspector
    public GameObject botPaddle; // Drag and drop the bot paddle in the inspector
    public ScoreManager scoreManager; // Drag and drop the ScoreManager object in the inspector

    private void OnTriggerEnter(Collider other)
    {
        // If player's paddle triggers with the bot
        if (other.CompareTag("bot") && gameObject == myPlayerPaddle)
        {
            scoreManager.AddPointToPlayer();
            Debug.Log("Player scored!");
        }
        // If bot's paddle triggers with the player
        else if (other.CompareTag("player") && gameObject == botPaddle)
        {
            scoreManager.AddPointToBot();
            Debug.Log("Bot scored!");
        }
    }
}
