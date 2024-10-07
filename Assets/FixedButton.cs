using UnityEngine;
using UnityEngine.EventSystems;

public class FixedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Animator playerAnimator;  // Reference to the Animator component on MyPlayer

    // This method will be called when the button is pressed down
    public void OnPointerDown(PointerEventData eventData)
    {
        // Trigger the attack animation when the button is pressed
        playerAnimator.SetTrigger("AttackTrigger");
    }

    // This method will be called when the button is released (optional)
    public void OnPointerUp(PointerEventData eventData)
    {
        // Optionally, you can reset the attack or play another animation when the button is released
        // For now, we do nothing, but you could reset the animation or perform another action here
    }
}
