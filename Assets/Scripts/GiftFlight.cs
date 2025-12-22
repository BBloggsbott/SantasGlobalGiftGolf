using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;

public class GiftFlight : MonoBehaviour
{
    
    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] giftSprites;
    public Sprite bellSprite;
    public float pseudo3DScaleFactor = 0.15f;
    public float rotationSpeed = 1f;


    [Header("Flight Settings")]
    public AnimationCurve heightCurve;
    public GameObject impactLightPrefab;
    public float baseMaxHeight = 4f;
    public float flightDurationScaler = 0.1f;
    public float baseFlightTime = 0.5f;
    
    
    [Header("Boundary Settings")]
    public float destroyDistance = 15f;
    private Vector3 globeCenter = Vector3.zero;

    [Header("Collision Detection")]
    public float textureRectBuffer = 0.5f;
    public float alphaThreshold = 0.1f;


    [Header("Progress Control")]
    public float progressIncrement = 5f;
    public float bellBaseIncrement = 7f;
    public int longshotMultiplier = 2;
    public float longshotThreshold = 5f;
    
    
    [Header("Light Blinking")]
    public float blinkInterval = 0.5f;
    public int blinkCount = 3;

    private float flightDuration, verticalAngle, timer;
    private bool initialized = false;
    private bool isBell = false;

    void Awake()
    {
        Debug.Log("Gift is Awake");
        if (spriteRenderer == null)
        {
            Debug.Log("Gift Sprite Renderer is empty. Getting component.");
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }


    public void InitializeFlight(float power, float vAngle, bool isBellActive)
    {
        if (isBellActive)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.bellLaunchSound);
        } else
        {
        AudioManager.Instance.PlaySFXWithRandomPitch(AudioManager.Instance.launchSound);
        }
        isBell = isBellActive;
        if (isBellActive)
        {
            spriteRenderer.sprite = bellSprite;
            spriteRenderer.transform.localScale = new Vector3(1.5f, 1.5f, 1);
        } else if(giftSprites.Length > 0)
        {
            spriteRenderer.sprite = giftSprites[Random.Range(0, giftSprites.Length)];
        }

        flightDuration = baseFlightTime + (power * flightDurationScaler);
        verticalAngle = vAngle;
        initialized = true;

    }


    void Update()
    {
        if (!initialized) return;

        float deltaTime = Time.deltaTime;
        RotateObject(deltaTime);
        
        timer += deltaTime;
        float progress = timer / flightDuration;
        float h = heightCurve.Evaluate(progress) * baseMaxHeight * (verticalAngle / 90f);

        if(transform.childCount > 0)
        {
            transform.GetChild(0).localPosition = new Vector3(0, h, 0);
            transform.GetChild(0).localScale = Vector3.one * (1 + (h * pseudo3DScaleFactor));
        }
        else
        {
            spriteRenderer.transform.localPosition = new Vector3(0, h, 0);
        }

        float distanceToCenter = Vector3.Distance(transform.position, globeCenter);
        
        if (progress >= 0.5 && distanceToCenter > destroyDistance)
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.vanishSound);
            Destroy(gameObject);
            return;
        }

        if (progress >= 1.0f) Land();
    }


    void RotateObject(float deltaTime)
    {
        Quaternion rotation = transform.rotation;
        rotation.z += deltaTime * rotationSpeed;
        transform.rotation = rotation;
    }

    void Land()
    {
        
        GameObject globe = GameObject.Find("Globe");
        SpriteRenderer globeRenderer = globe.GetComponent<SpriteRenderer>();

        Vector2 localPos = globe.transform.InverseTransformPoint(transform.position);

        Texture2D tex = globeRenderer.sprite.texture;
        Rect rect = globeRenderer.sprite.textureRect;
        int x = (int)((localPos.x / globeRenderer.sprite.bounds.size.x + textureRectBuffer) * rect.width + rect.x);
        int y = (int)((localPos.y / globeRenderer.sprite.bounds.size.y + textureRectBuffer) * rect.height + rect.y);

        Color hitColor = tex.GetPixel(x, y);
        Debug.Log(x + ", " + y + " | " + tex.width + ", " + tex.height);

        if(hitColor.g > hitColor.r && hitColor.g > hitColor.b && hitColor.a > alphaThreshold)
        {
            AudioManager.Instance.PlaySFXWithRandomPitch(AudioManager.Instance.landSound);
            GameObject l = Instantiate(impactLightPrefab, transform.position, Quaternion.identity);
            l.transform.SetParent(GameObject.Find("Globe").transform);
            GameManager.Instance.StartCoroutine(PauseAndBlinkLight(l));
            float playerDistance = Vector3.Distance(
                PlayerController.Instance.transform.position,
                transform.position
            );

            Debug.Log("Is Bell: " + isBell);
            float score = isBell ? bellBaseIncrement : progressIncrement;
            Debug.Log("Hit Distance: " + playerDistance);

            if (playerDistance > longshotThreshold)
                score *= longshotMultiplier;
        
            GameManager.Instance.AddProgress(score);
                
        }
        else if(hitColor.a > 0.1f)
        {
            AudioManager.Instance.PlayRandomSfx(AudioManager.Instance.waterLandSounds);
        } else
        {
            AudioManager.Instance.PlaySfx(AudioManager.Instance.vanishSound);
        }
        Destroy(gameObject);

    }


    private IEnumerator PauseAndBlinkLight(GameObject lightObject)
    {
        PlayerInput playerInput = FindFirstObjectByType<PlayerInput>();
        Light2D light = lightObject.GetComponent<Light2D>();
        Time.timeScale = 0f;
        playerInput.DeactivateInput();

        for (int i = 0; i < blinkCount; i++)
        {
            light.enabled = true;
            yield return new WaitForSecondsRealtime(blinkInterval);
            light.enabled = false;
            yield return new WaitForSecondsRealtime(blinkInterval);
        }
        Destroy(light);
        playerInput.ActivateInput();
        Time.timeScale = 1f;
    }

}
