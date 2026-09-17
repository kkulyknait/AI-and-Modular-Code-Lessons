using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private FloatVariable playerHealth;
    [SerializeField] private Slider slider;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        slider.value = playerHealth.Value;
    }
}
