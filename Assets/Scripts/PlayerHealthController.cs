using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    [SerializeField] private FloatVariable playerHealth;
    [SerializeField] private GameEvent takeDamageEvent;
    [SerializeField] private float damageAmount = 0.2f;

    public void TakeDamage()
    {
        playerHealth.Value = Mathf.Clamp01(playerHealth.Value - damageAmount);
        takeDamageEvent.Raise();
    }
}