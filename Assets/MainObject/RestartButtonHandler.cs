using UnityEngine;
using TMPro;

public class RestartButtonHandler : MonoBehaviour
{
    public TMP_Text restartButton;

    void Start()
    {
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false); 
        }
    }

    public void ShowButton()
    {
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true); 
        }
    }
}
