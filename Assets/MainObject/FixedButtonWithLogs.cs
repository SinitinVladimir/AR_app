using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FixedButtonWithLogs : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Animator playerAnimator;  
    public Button buttonComponent;   

    void Start()
    {
        if (playerAnimator == null)
        {
            Debug.LogError("Player Animator is not assigned!");
        }
        else
        {
            Debug.Log("Player Animator successfully assigned.");
        }
        if (buttonComponent == null)
        {
            Debug.LogError("Button component is not assigned!");
        }
        else
        {
            Debug.Log("Button component successfully assigned.");
        }
        if (buttonComponent != null)
        {
            Debug.Log("Button interactable status: " + buttonComponent.interactable);
        }

        Image imageComponent = GetComponent<Image>();
        if (imageComponent != null && imageComponent.raycastTarget)
        {
            Debug.Log("Image Raycast Target is set correctly.");
        }
        else
        {
            Debug.LogError("Image component or Raycast Target is missing or not enabled.");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("AttackTrigger");
            Debug.Log("Button pressed! Attack animation triggered.");
        }
        else
        {
            Debug.LogError("Player Animator is not assigned, can't trigger attack animation.");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Button released.");
    }

    public void TestClick()
    {
        Debug.Log("Button clicked through UI OnClick() method.");
    }
}
