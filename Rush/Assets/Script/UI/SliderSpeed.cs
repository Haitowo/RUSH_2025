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

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            _CurrentSlider.SetValueWithoutNotify(GameManager.Instance.TickSpeed);
            _CurrentSlider.onValueChanged.AddListener(ChangeSpeed);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {

        }

        private void ChangeSpeed(float pValue)
        {
            GameManager.Instance.TickSpeed = pValue;
        }
    }
}