using System;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 05/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.Manager
{
    
    public class GameManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private float _SpeedTime = 1f;

        private float _DurationBetweenTicks = 1f;
        private float _ElapsedTime = 0f;

        public float ratioTimeTick { get; private set; } = 0f;

        public Action tickAction { get; set; }

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
            ratioTimeTick = _ElapsedTime / _DurationBetweenTicks;
        }
    }
}