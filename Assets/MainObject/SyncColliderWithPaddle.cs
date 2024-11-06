using UnityEngine;

public class SyncColliderWithPaddle : MonoBehaviour
{
    public CapsuleCollider myPaddleCollider;
    public Transform animatedPaddleTransform;

    void Update()
    {
        myPaddleCollider.transform.position = animatedPaddleTransform.position;
        myPaddleCollider.transform.rotation = animatedPaddleTransform.rotation;
    }
}
