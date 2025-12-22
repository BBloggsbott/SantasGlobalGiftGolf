using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance;
    
    [Header("Aimer & Globe")]
    public Transform aimerPivot;
    public Transform globeTransform;
    public float aimSpeed = 100f;
    public float globeSpeed = 50f;
    private float currentAimAngle = 0f;
    private float moveInput;


    [Header("Power System")]
    public Slider powerSlider;
    public Image fillImage;
    public Gradient colorGradient;
    public float maxPower = 25f;
    public float chargeSpeed = 15f;
    public float currentPower = 0f;
    private bool isCharging = false;


    [Header("Bell System")]
    public bool isBellActive = false;
    public LineRenderer trajectoryLine;
    public GameObject giftPrefab;

    [Header("Gift Flight")]
    public float flightDurationScaler = 0.1f;
    public float baseFlightTime = 0.5f;
    public int trajectorySteps = 50;
    public float gravityStrength = 10f;


    void Awake()
    {
        Instance = this;
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
    }

    public void OnBell(InputAction.CallbackContext context)
    {
        if (context.performed && GameManager.Instance.bellcharges > 0)
        {
            isBellActive = !isBellActive;
            trajectoryLine.enabled = isActiveAndEnabled;
        }
    }


    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCharging = true;
            currentPower = 0f;
        } else
        {
            if (isCharging)
            {
                ShootGift();
                isCharging = false;
                powerSlider.value = 0f;
            }
        }
    }
    
    
    void Update()
    {
        currentAimAngle -= moveInput * aimSpeed * Time.deltaTime;
        currentAimAngle = Mathf.Clamp(currentAimAngle, -180, 0);
        aimerPivot.localRotation = Quaternion.Euler(0, 0, currentAimAngle);

        if (isCharging)
        {
            currentPower = Mathf.Min(currentPower + chargeSpeed * Time.deltaTime, maxPower);
            float powerPercentage = currentPower / maxPower;
            powerSlider.value = powerPercentage;
            fillImage.color = colorGradient.Evaluate(powerPercentage);
            // if(isBellActive) UpdateTrajectory(currentPower);
        }
    }


    void ShootGift()
    {
        GameObject gift = Instantiate(giftPrefab, aimerPivot.position, Quaternion.identity);
        float verticalAngle = 90f - Mathf.Abs(currentAimAngle);
        gift.GetComponent<GiftFlight>().InitializeFlight(currentPower, verticalAngle, isBellActive);
        gift.GetComponent<Rigidbody2D>().AddForce(aimerPivot.right * currentPower, ForceMode2D.Impulse);

        if (isBellActive)
        {
            GameManager.Instance.UseBell();
            isBellActive = false;
            trajectoryLine.enabled = false;
        }
    }


    void UpdateTrajectory(float power)
    {
        float totalFlightTime = baseFlightTime + (power * flightDurationScaler);
        float timeStep = totalFlightTime / trajectorySteps;
        Vector2 pos = aimerPivot.position;
        Vector2 vel = (Vector2)aimerPivot.up * -1 * power;

        trajectoryLine.positionCount = trajectorySteps;
        for(int i = 0; i< trajectorySteps; i++)
        {
            trajectoryLine.SetPosition(i, pos);
            Vector2 gravDir = (Vector2.zero - pos).normalized;
            vel += gravDir * gravityStrength * timeStep;
            pos += vel * timeStep;
        }
    }

}
