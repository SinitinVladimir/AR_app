using System.Collections;
using UnityEngine;

public class BotAnimationController : MonoBehaviour
{
    public Animator botAnimator;  // Reference to the Animator component on the bot
    public float minAttackInterval = 0.1f;  // Minimum time interval between attacks
    public float maxAttackInterval = 5f;  // Maximum time interval between attacks
    private float attackTimer;

    private void Start()
    {
        // Set the initial attack timer to a random value between min and max interval
        attackTimer = Random.Range(minAttackInterval, maxAttackInterval);
    }

    private void Update()
    {
        // Reduce the timer by the time since the last frame
        attackTimer -= Time.deltaTime;

        // When the timer runs out, trigger an attack and reset the timer
        if (attackTimer <= 0f)
        {
            if (botAnimator.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
            {
                StartCoroutine(WaitAndAttack(2f));
            }
            else
            {
                Attack();
            }
            
            // Reset the timer to a new random value between min and max interval
            attackTimer = Random.Range(minAttackInterval, maxAttackInterval);
        }
    }

    private void Attack()
    {
        // Trigger the attack animation
        botAnimator.SetTrigger("AttackTrigger");
    }

    private IEnumerator WaitAndAttack(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Attack();
    }
}
