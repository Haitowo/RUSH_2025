using Com.IsartDigital.ProjectName;
using Com.IsartDigital.Rush.CubeManagement;
using Com.IsartDigital.Rush.Ticks;
using Com.IsartDigital.Rush.Utilities;
using System;
using UnityEngine;
// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 05/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.Manager
{
    
    public class GameManager : MonoBehaviour, ITickProvider
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [Header(Utils.GAME_SPEED)]
        [SerializeField] public float TickSpeed { get; set; } = 1f;
        [HideInInspector] public float RatioTimeTick {  get; private set; }

        private float _DurationBetweenTicks = 1f;
        private float _ElapsedTime = 0f;

        public event Action TickEvent;
        public Action ActivatePlayPhase;
        public Action<bool> SwitchToGame;

        private GameObject _TileSelected;

        public static GameManager Instance { get; private set; }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            TickProviderLocator.Register(this);
            ActivatePlayPhase += ActivateLevel;
            enabled = false;

            #region Singleton Management
            if (Instance != this && Instance != null)
            {
                Destroy(this);
                Debug.LogError(nameof(Instance) + "Instance already exists. Destroying the current instance.");
                return;
            }

            Instance = this;
            //DontDestroyOnLoad(this);
            #endregion
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
                TickEvent?.Invoke();
                _ElapsedTime = 0f;
            }

            CalculateRatio();
        }

        private void CalculateRatio()
        {
            _ElapsedTime += Time.deltaTime * TickSpeed;
            RatioTimeTick = _ElapsedTime / _DurationBetweenTicks;
        }

        public void CubeReachTarget(Cube pCube)
        {
            //TODO : Level Complete Management
        }

        private void ActivateLevel() => enabled = true;

        public void SelectTileToPlace(GameObject pTile)
        {
            _TileSelected = pTile;
            Debug.Log("Tuile sélectionnée : " + pTile.name);
        }

    }
}