using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.Lighting
{
    [ExecuteAlways]
    public class LightingManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private Light _DirectionalLight;
        [SerializeField] private SOLightingPreset _PresetLight;

        [SerializeField, Range(0, 24)] private float _TimeOfDay;

        private float _HoursOfADay = 24f;
        private float _FullCircle = 360f;
        private float _QuaterCircle = 90f;
        private float _Height = 170f;
        private float _DayTimeSpeed = .25f;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            CheckTime();
        }

        private void OnValidate()
        {
            SetupLight();
        }

        private void SetupLight()
        {
            Light[] lLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            if (_DirectionalLight != null) return;

            if (RenderSettings.sun != null)
                _DirectionalLight = RenderSettings.sun;
            else
            {
                foreach (Light light in lLights)
                {
                    if (light.type == LightType.Directional)
                    {
                        _DirectionalLight = light;
                        return;
                    }
                }
            }
        }

        private void UpdateLighting(float pTimeOfDay)
        {
            RenderSettings.ambientLight = _PresetLight._AmbientColor.Evaluate(pTimeOfDay);
            RenderSettings.fogColor = _PresetLight._FogColor.Evaluate(pTimeOfDay);

            if (_DirectionalLight == null) return;
            _DirectionalLight.color = _PresetLight._DirectionalColor.Evaluate(pTimeOfDay);
            _DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((_FullCircle * pTimeOfDay) - _QuaterCircle, _Height, 0f)); 
        }

        private void CheckTime()
        {
            if (_PresetLight == null) return;

            if(Application.isPlaying)
            {
                _TimeOfDay += Time.deltaTime * _DayTimeSpeed;
                _TimeOfDay %= _HoursOfADay;
                UpdateLighting(_TimeOfDay / _HoursOfADay);
            }
            else UpdateLighting(_TimeOfDay / _HoursOfADay);
        }
    }
}