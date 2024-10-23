using UnityEngine;
using TMPro;

public class RestartButtonHandler : MonoBehaviour
{
    public TMP_Text restartButton;

    void Start()
    {
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false); // Hide the button initially
        }
    }

    public void ShowButton()
    {
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true); // Show the button when needed
        }
    }
}
