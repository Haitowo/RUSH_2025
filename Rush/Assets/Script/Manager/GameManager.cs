using Com.IsartDigital.Rush.CubeManagement;
using Com.IsartDigital.Rush.Ticks;
using Com.IsartDigital.Rush.UI;
using Com.IsartDigital.Rush.Utilities;
using DG.Tweening;
using System;
using System.Collections;
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
        [SerializeField] private EMenuType _WinScreenToShow;

        [Header(Utils.SOUND)]
        [SerializeField] private AudioClip _GameMusic;
        [SerializeField] private AudioClip _WinScreenSound;

        private float _DurationBetweenTicks = 1f;
        private float _ElapsedTime = 0f;
        private float _TimeToWaitOnGameEnd = 2f;

        public event Action tickEvent;
        public Action onGameLost;
        public Action activatePlayPhase;
        public Action<bool> resetLevel;
        public Action<bool> switchToGame;
        public Action<bool> backToMenu;
        public Action<EMenuType> gameFinished;

        private bool _IsGamePlaying;

        public ELevelToload selectHUDLevel { get; private set; }

        private CollisionManager _CollisionManager => CollisionManager.Instance;
        private SoundManager _SoundManager => SoundManager.Instance;

        public static GameManager Instance { get; private set; }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            TickProviderLocator.Register(this);
            activatePlayPhase += ActivateLevel;
            onGameLost += DisactivateLevel;
            resetLevel += ResetCube;
            _IsGamePlaying = false;

            #region Singleton Management
            if (Instance != this && Instance != null)
            {
                Destroy(this);
                Debug.LogError(nameof(GameManager) + "Instance already exists. Destroying the current instance.");
                return;
            }

            Instance = this;
            #endregion
        }

        private void Start()
        {
            _SoundManager.PlayMusic(_GameMusic);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            if (_IsGamePlaying) TickManager();
        }

        private void TickManager()
        {
            if (_ElapsedTime >= _DurationBetweenTicks)
            {
                tickEvent?.Invoke();
                _ElapsedTime = 0f;
            }

            CalculateRatio();
        }

        private void CalculateRatio()
        {
            _ElapsedTime += Time.deltaTime * TickSpeed;
            RatioTimeTick = _ElapsedTime / _DurationBetweenTicks;
        }

        public void LevelComplete()
        {
            _SoundManager.PlaySound(_WinScreenSound, transform.position);
            gameFinished?.Invoke(_WinScreenToShow);
            _IsGamePlaying = false;
        }

        public void SetSelectedHUD(ELevelToload pLevel) => selectHUDLevel = pLevel;
        
        private void ActivateLevel() => _IsGamePlaying = true;

        private void DisactivateLevel()
        {
            _IsGamePlaying = false;
            StartCoroutine(WaitForLevelToReset(true));
        }

        private IEnumerator WaitForLevelToReset(bool pShow)
        {
            yield return new WaitForSeconds(_TimeToWaitOnGameEnd);
            resetLevel?.Invoke(pShow);
        }

        private void ResetCube(bool pShow)
        {
            foreach (Cube pCubes in _CollisionManager.cubes)
                pCubes.transform.DOScale(Vector3.zero, .5f).SetEase(Ease.Linear).OnComplete(() => DestroyCurrentCubes(pCubes));

            _CollisionManager.cubes.Clear();

        }

        private void DestroyCurrentCubes(Cube pCube) => Destroy(pCube.gameObject);

    }
}