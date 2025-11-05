using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.Manager
{
    
    public class GameManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private float _SpeedTime = 1f;

        private float _DurationBetweenTicks = 1f;
        private float _ElapsedTime = 0f;
        public float ratio { get; private set; } = 0f;

        public Action tickAction;

        public static GameManager Instance { get; private set; }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            if (Instance != this && Instance != null)
            {
                Destroy(this);
                Debug.LogError(nameof(Instance) + "Instance already exists. Destroying the current instance.");
                return;
            }

            Instance = this;
            //DontDestroyOnLoad(this);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            TickManager();
        }

        private void TickManager()
        {
            if (_ElapsedTime >= _DurationBetweenTicks)
            {
                _ElapsedTime = 0f;
                tickAction?.Invoke();
            }
            
            CalculateRatio();
        }

        private void CalculateRatio()
        {
            _ElapsedTime += Time.deltaTime * _SpeedTime;
            ratio = _ElapsedTime / _DurationBetweenTicks;
        }
    }
}