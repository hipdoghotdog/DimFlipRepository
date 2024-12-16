using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArtifactManager : MonoBehaviour
{
    // **Artifact Movement Variables**
    [Header("Movement Settings")]
    public Transform followPos;       
    public Vector3 offset = Vector3.zero; 
    public float speed = 5f;         

    [Header("Text Display Settings")]
    public float displayDuration = 5f; 

    private bool displayActive = false; 
    private float displayCountdown = 0f; 
    private TextMesh tm;                

    // **Light Component Variables**
    [Header("Light Settings")]
    public LightType lightType = LightType.Point; 
    public Color lightColor = Color.white;        
    public float lightIntensity = 1f;             
    public float lightRange = 10f;                
    public bool enableShadows = false;            

    [Header("Dynamic Light Effects")]
    public bool enablePulsing = true;            
    public float pulseSpeed = 2f;                 
    public float pulseIntensityMin = 0.5f;        
    public float pulseIntensityMax = 2f;          

    private Light artifactLight; 

    public Transform camToLookAt;

    void Start()
    {
        tm = GetComponentInChildren<TextMesh>();
        if (tm == null)
        {
            Debug.LogError("TextMesh component not found in children of Artifact.");
        }

        artifactLight = GetComponent<Light>();
        if (artifactLight == null)
        {
            artifactLight = gameObject.AddComponent<Light>();
        }

        artifactLight.type = lightType;
        artifactLight.color = lightColor;
        artifactLight.intensity = lightIntensity;
        artifactLight.range = lightRange;
        artifactLight.shadows = enableShadows ? LightShadows.Soft : LightShadows.None;

        if (artifactLight.type == LightType.Spot)
        {
            artifactLight.spotAngle = 30f; 
        }

        camToLookAt = GameObject.Find("Camera").transform; // If camera object is renamed, this breaks
    }

    void Update()
    {
        // **Handle Artifact Movement**
        FollowPlayer();

        // **Handle Text Display Countdown**
        HandleTextDisplay();

        // **Make Text Look On Camera
        tm.transform.rotation = Quaternion.LookRotation(camToLookAt.forward);

        // **Handle Dynamic Light Effects**
        if (enablePulsing)
        {
            PulseLight();
        }
    }

    private void FollowPlayer()
    {
        if (followPos != null)
        {
            Vector3 targetPosition = followPos.position + offset;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        else
        {
            Debug.LogWarning("Follow Position is not assigned in ArtifactManager.");
        }
    }

    private void HandleTextDisplay()
    {
        if (displayActive)
        {
            displayCountdown -= Time.deltaTime;
            if (displayCountdown <= 0f)
            {
                displayActive = false;
                displayCountdown = 0f;
                if (tm != null)
                {
                    tm.text = "";
                }
            }
        }
    }

    public void DisplayText(string text)
    {
        if (tm != null)
        {
            displayActive = true;
            displayCountdown = displayDuration;
            tm.text = text;
            SoundManager.instance.PlaySound(Sound.ARTIFACT_TALK, 0.3f);
        }
        else
        {
            Debug.LogWarning("TextMesh component is not assigned.");
        }
    }

    private void PulseLight()
    {
        if (artifactLight != null)
        {
            // Calculate pulsing intensity using PingPong for smooth oscillation
            float pulsingIntensity = Mathf.PingPong(Time.time * pulseSpeed, pulseIntensityMax - pulseIntensityMin) + pulseIntensityMin;
            artifactLight.intensity = pulsingIntensity;
        }
    }

    #region Optional Light Control Methods

    public void SetLightColor(Color newColor)
    {
        if (artifactLight != null)
        {
            artifactLight.color = newColor;
        }
    }

    public void SetLightIntensity(float newIntensity)
    {
        if (artifactLight != null)
        {
            lightIntensity = newIntensity;
            artifactLight.intensity = newIntensity;
        }
    }

    public void ToggleLight(bool isOn)
    {
        if (artifactLight != null)
        {
            artifactLight.enabled = isOn;
        }
    }

    public void ChangeLightType(LightType newType)
    {
        if (artifactLight != null)
        {
            artifactLight.type = newType;
            // Optional: Adjust additional properties based on light type
            if (newType == LightType.Spot)
            {
                artifactLight.spotAngle = 30f; // Example value
            }
        }
    }

    #endregion
}
