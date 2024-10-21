using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FixedButtonWithLogs : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Animator playerAnimator;  // Reference to the Animator component on MyPlayer
    public Button buttonComponent;   // Reference to the Button component

    void Start()
    {
        // Log if the animator is properly assigned
        if (playerAnimator == null)
        {
            Debug.LogError("Player Animator is not assigned!");
        }
        else
        {
            Debug.Log("Player Animator successfully assigned.");
        }

        // Log if the button component is properly assigned
        if (buttonComponent == null)
        {
            Debug.LogError("Button component is not assigned!");
        }
        else
        {
            Debug.Log("Button component successfully assigned.");
        }

        // Log if the button is interactable and has a raycast target
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

    // This method will be called when the button is pressed down
    public void OnPointerDown(PointerEventData eventData)
    {
        // Trigger the attack animation when the button is pressed
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

    // This method will be called when the button is released (optional)
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Button released.");
    }

    public void TestClick()
    {
        Debug.Log("Button clicked through UI OnClick() method.");
    }
}
