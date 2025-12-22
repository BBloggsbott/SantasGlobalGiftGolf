using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PowerbarController : MonoBehaviour
{
    

    public Slider powerSlider;
    public Image fillImage;
    public Gradient colorGradient;

    public PlayerController player;

    void Update()
    {
        float powerPercentage = player.currentPower / player.maxPower;

        powerSlider.value = powerPercentage;

        fillImage.color = colorGradient.Evaluate(powerPercentage);
    }

}
