using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MyBoat : MonoBehaviour
{
    public float moveSpeed = 4f;  // Set your move speed
    public float rotationSpeed = 40f;  // Set your rotation speed
    public RectTransform joystickBackground; // Assign the background of your joystick UI
    public RectTransform joystickHandle; // Assign the handle of your joystick UI
    public float strokeDuration = 3.6f;

    private Vector2 joystickInput; // Store joystick movement
    private TouchControls controls;
    private Vector3 initialPosition; // Starting position for boundary constraint

    private void Awake()
    {
        controls = new TouchControls();
    }

    private void Start()
    {
        // Store the initial position of the boat
        initialPosition = transform.position;
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.control.touch.performed += OnTouchInput;
        controls.control.touch.canceled += OnTouchCanceled;
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.control.touch.performed -= OnTouchInput;
        controls.control.touch.canceled -= OnTouchCanceled;
    }

    private void OnTouchInput(InputAction.CallbackContext context)
    {
        Vector2 touchPosition = context.ReadValue<Vector2>();

        // Check if the touch is inside the joystick's background area
        if (RectTransformUtility.RectangleContainsScreenPoint(joystickBackground, touchPosition))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBackground, touchPosition, null, out Vector2 localPoint);

            joystickInput = localPoint / (joystickBackground.sizeDelta / 2);
            joystickInput = Vector2.ClampMagnitude(joystickInput, 1.0f);  // Limit joystick input to a circle

            joystickHandle.anchoredPosition = joystickInput * (joystickBackground.sizeDelta / 2);
        }
    }

    private void OnTouchCanceled(InputAction.CallbackContext context)
    {
        joystickInput = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero; // Reset joystick handle to center
    }

    private void Update()
    {
        if (joystickInput.magnitude > 0.1f)
        {
            Vector3 direction = new Vector3(joystickInput.x, 0, joystickInput.y);

            // Calculate the potential new position with movement applied
            Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;

            // Check if the new position exceeds 20 meters from the initial position
            if (Vector3.Distance(newPosition, initialPosition) <= 20f)
            {
                // Update position if within boundary
                transform.position = newPosition;

                // Apply 180-degree offset to the rotation so the boat's nose stays in front
                Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                // Optional: adjust position back within bounds if it slightly exceeds
                transform.position = Vector3.MoveTowards(transform.position, initialPosition, moveSpeed * Time.deltaTime);
            }
        }
    }
}
