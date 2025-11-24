using Com.IsartDigital.Rush.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class SliderSpeed : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private Slider _CurrentSlider;

        private GameManager _GameManager => GameManager.Instance;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            _CurrentSlider.SetValueWithoutNotify(_GameManager.TickSpeed);
            _CurrentSlider.onValueChanged.AddListener(ChangeSpeed);
        }

        private void ChangeSpeed(float pValue)
        {
            _GameManager.TickSpeed = pValue;
        }
    }
}