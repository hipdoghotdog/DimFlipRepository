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

    private bool _displayActive; 
    private float _displayCountdown; 
    private TextMesh _textMesh;                

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

    private Light _artifactLight; 

    public Transform camToLookAt;

    public void Initialize()
    {
        _textMesh = GetComponentInChildren<TextMesh>();
        if (_textMesh == null)
        {
            Debug.LogError("TextMesh component not found in children of Artifact.");
        }

        _artifactLight = GetComponent<Light>();
        if (_artifactLight == null)
        {
            _artifactLight = gameObject.AddComponent<Light>();
        }

        _artifactLight.type = lightType;
        _artifactLight.color = lightColor;
        _artifactLight.intensity = lightIntensity;
        _artifactLight.range = lightRange;
        _artifactLight.shadows = enableShadows ? LightShadows.Soft : LightShadows.None;

        if (_artifactLight.type == LightType.Spot)
        {
            _artifactLight.spotAngle = 30f; 
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
        _textMesh.transform.rotation = Quaternion.LookRotation(camToLookAt.forward);

        // **Handle Dynamic Light Effects**
        if (enablePulsing)
        {
            PulseLight();
        }
    }

    private void FollowPlayer()
    {
        if (followPos)
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
        if (!_displayActive) return;
        
        _displayCountdown -= Time.deltaTime;
        
        if (!(_displayCountdown <= 0f)) return;
        
        _displayActive = false;
        _displayCountdown = 0f;
        
        if (_textMesh)
        {
            _textMesh.text = "";
        }
    }

    public void DisplayText(string text)
    {
        if (_textMesh)
        {
            _displayActive = true;
            _displayCountdown = displayDuration;
            _textMesh.text = text;
            SoundManager.Instance.PlaySound(Sound.ArtifactTalk, 0.3f);
        }
        else
        {
            Debug.LogWarning("TextMesh component is not assigned.");
        }
    }

    private void PulseLight()
    {
        if (!_artifactLight) return;
        
        // Calculate pulsing intensity using PingPong for smooth oscillation
        float pulsingIntensity = Mathf.PingPong(Time.time * pulseSpeed, pulseIntensityMax - pulseIntensityMin) + pulseIntensityMin;
        _artifactLight.intensity = pulsingIntensity;
    }

    #region Optional Light Control Methods

    public void SetLightColor(Color newColor)
    {
        if (_artifactLight == null) return;
        
        _artifactLight.color = newColor;
    }

    public void SetLightIntensity(float newIntensity)
    {
        if (_artifactLight == null) return;
        
        lightIntensity = newIntensity;
        _artifactLight.intensity = newIntensity;
    }

    public void ToggleLight(bool isOn)
    {
        if (_artifactLight == null) return;
        
        _artifactLight.enabled = isOn;
    }

    public void ChangeLightType(LightType newType)
    {
        if (_artifactLight == null) return;
        
        _artifactLight.type = newType;
        
        // Optional: Adjust additional properties based on light type
        if (newType == LightType.Spot)
        {
            _artifactLight.spotAngle = 30f; // Example value
        }
    }

    #endregion
}
