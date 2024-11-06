using UnityEngine;

public class PaddleCollisionDetector : MonoBehaviour
{
    public GameObject myPlayerPaddle; 
    public GameObject botPaddle; 
    public ScoreManager scoreManager; 

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("bot") && gameObject == myPlayerPaddle)
        {
            scoreManager.AddPointToPlayer();
            Debug.Log("Player scored!");
        }

        else if (other.CompareTag("player") && gameObject == botPaddle)
        {
            scoreManager.AddPointToBot();
            Debug.Log("Bot scored!");
        }
    }
}
