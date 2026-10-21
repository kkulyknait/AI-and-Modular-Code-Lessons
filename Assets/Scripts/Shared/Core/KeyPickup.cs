using UnityEngine;
public class KeyPickup : MonoBehaviour
{
    #region Unity Methods
    private void OnTriggerEnter(Collider other)
    {
        AgentContext context = other.GetComponentInParent<AgentContext>();

        if (context == null) {
            return;
        }

        context.HasKey = true;

        Debug.Log($"{context.name} collected the Key.", context);

        gameObject.SetActive(false);
    }
    #endregion
}
