using UnityEngine;

public class SyncColliderWithPaddle : MonoBehaviour
{
    public CapsuleCollider myPaddleCollider;
    public Transform animatedPaddleTransform; // Assign the transform of the paddle

    void Update()
    {
        // Synchronize the capsule collider position and rotation with the paddle's position and rotation
        myPaddleCollider.transform.position = animatedPaddleTransform.position;
        myPaddleCollider.transform.rotation = animatedPaddleTransform.rotation;
    }
}
