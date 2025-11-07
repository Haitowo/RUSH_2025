using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 04/11/2025 - Beginning of the class

namespace Com.IsartDigital.ProjectName
{
    public class FPS : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private TextMeshProUGUI _FpsDisplayText;

        private float _FpsCounter;
        private float _ElapsedTime;

        private int _MaxCountTime = 2;

        private const string FPS_TEXT = "FPS : ";

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            ShowFPS();
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            RefreshFPS();
        }

        private void ShowFPS()
        {
            _FpsCounter = 1f / Time.deltaTime;
            _FpsDisplayText.text = FPS_TEXT + (int)_FpsCounter;
            _ElapsedTime = 0f;
        }

        private void RefreshFPS()
        {
            _ElapsedTime += Time.deltaTime;

            if (_ElapsedTime >= _MaxCountTime) ShowFPS(); 
        }
    }
}
