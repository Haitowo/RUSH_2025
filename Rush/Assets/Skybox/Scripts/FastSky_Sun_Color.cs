using UnityEngine;
using Unity.Mathematics;

[ExecuteAlways]
public class FastSky_Sun_Color : MonoBehaviour
{
    [Header("Sun Settings")]
    [SerializeField] private Light _light;
    [SerializeField] private float sunIntensity = 5f;
    [SerializeField] private float minSunIntensity = 0.1f;
    [SerializeField] private float sunPower = 5f;
    [SerializeField] private float dotOffset = 0.9f;

    [Header("Colors")]
    [SerializeField] private Color dayColour = Color.white;
    [SerializeField] private Color eveningColour = new Color(1f, 0.6f, 0.4f);

    [Header("Ambient Settings")]
    [SerializeField] private float ambientDay = 2.5f;
    [SerializeField] private float ambientNight = 1f;

    private void Update()
    {
        if (_light == null) _light = GetComponent<Light>();

        float lDotProduct = Vector3.Dot(-transform.forward, Vector3.up);
        float lClampedDot = GetClampedDot(lDotProduct);
        float lTopDot = ComputeTopDot(lDotProduct);
        float lBottomDot = ComputeBottomDot(lDotProduct);

        UpdateLight(lClampedDot, lTopDot, lBottomDot);
        UpdateAmbient(lClampedDot);
    }

    private float GetClampedDot(float pDot) => Mathf.Clamp(pDot + dotOffset, 0f, 1f);
    
    float ComputeTopDot(float pDot)
    {
        float pTop = (1 - Mathf.Clamp01(pDot)) * Mathf.Clamp01(Mathf.Sign(pDot));
        pTop = math.smoothstep(0f, 0.9f, pTop);
        return Mathf.Pow(pTop, sunPower);
    }

    float ComputeBottomDot(float pDot)
    {
        float bottom = (1 - Mathf.Clamp01(-pDot)) * Mathf.Clamp01(Mathf.Sign(-pDot));
        return Mathf.Pow(bottom, sunPower);
    }

    void UpdateLight(float pClampedDot, float pTopDot, float pBottomDot)
    {
        float t = Mathf.Pow(pClampedDot, sunPower);
        _light.intensity = Mathf.Lerp(minSunIntensity, sunIntensity, t);
        _light.color = Color.Lerp(dayColour, eveningColour, pTopDot + pBottomDot);
    }

    void UpdateAmbient(float pClampedDot)
    {
        float t = Mathf.Pow(pClampedDot, sunPower);
        RenderSettings.ambientIntensity = Mathf.Lerp(ambientDay, ambientNight, t);
    }
}