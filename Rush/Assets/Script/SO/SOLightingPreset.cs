using Com.IsartDigital.Rush.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.Lighting
{
    [System.Serializable]
    [CreateAssetMenu(fileName = Utils.LIGHTING_PRESET, menuName = Utils.LIGHTING_MENU)]
    public class SOLightingPreset : ScriptableObject
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] public Gradient _AmbientColor;
        [SerializeField] public Gradient _DirectionalColor;
        [SerializeField] public Gradient _FogColor;
    }
}