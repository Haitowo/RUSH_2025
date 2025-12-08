using Com.IsartDigital.Rush.CubeManagement;
using Com.IsartDigital.Rush.Manager;
using Com.IsartDigital.Rush.TargetManagement;
using Com.IsartDigital.Rush.Ticks;
using Com.IsartDigital.Rush.UI;
using Com.IsartDigital.Rush.Utilities;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 09/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.GameObjects
{
    public class CubeSpawner : ColorableTile
    {
        [Header(Utils.PARAMETERS_SPAWNER)]
        [SerializeField] private GameObject _CubePrefab;
        [SerializeField] private Transform _SpawnPoint;
        [SerializeField] private Transform _GameObjectContainer;
        [SerializeField] private ParticleSystem _TrailDirection;

        [Header(Utils.PARAMETERS_LEVEL)]
        [SerializeField] private EColorSetter _ColorSpawnerAndCube;
        [SerializeField] private ELevelToload _CurrentLevel;
        [SerializeField] private bool _DoesLevelNeedsToGetDelaySpawn;

        [SerializeField] private float _TickToDelay;

        public EColorSetter ColorSpawnerAndCube => _ColorSpawnerAndCube;
        private ITickProvider _TickProvider;

        private float _NextSpawnTick;
        private int _TickCount = 0;
        private int _TickNextSpawnToAdd = 9;
        private int _CubesSpawned = 0;

        private const int TOTAL_CUBES_TO_SPAWN = 8;

        private const float SPAWN_DECAY = .5f;
        private const float SPAWN_CUBE_TIME = .2f;

        private Vector3 _SpawnPos;

        private GameManager _GameManager => GameManager.Instance;
        private CollisionManager _CollisionManager => CollisionManager.Instance;

        private void Start()
        {
            Renderer lRend = GetComponentInChildren<Renderer>();
            ParticleSystem.MainModule lMain = _TrailDirection.main;

            enabled = false;
            _TickProvider = TickProviderLocator.Instance;
            _TickProvider.tickEvent += OnTick;

            _GameManager.activatePlayPhase += _DoesLevelNeedsToGetDelaySpawn ? ResetTick : SpawnCube;
            _GameManager.activatePlayPhase += DisablePreview;
            _GameManager.resetLevel += EnablePreview;

            m_SpawnMaterial = lRend.material;
            ApplyColorMaterial(_ColorSpawnerAndCube, m_SpawnMaterial);
            _TrailDirection.transform.localRotation = _TrailDirection.transform.localRotation;
            lMain.startColor = m_SpawnMaterial.color;
        }

        private void OnTick()
        {
            if(!_DoesLevelNeedsToGetDelaySpawn) return;
            _TickCount++;

            if (_TickCount >= _TickToDelay && _TickCount >= _NextSpawnTick)
            {
                SpawnCube();
                _NextSpawnTick = _TickCount + _TickNextSpawnToAdd;
            }
        }

        private void SpawnCube()
        {
            if (_CurrentLevel != _GameManager.selectHUDLevel || _CubesSpawned >= TOTAL_CUBES_TO_SPAWN) return;
            _SpawnPos = new Vector3(transform.position.x, transform.position.y + SPAWN_DECAY, transform.position.z);

            GameObject lPrefab = Instantiate(_CubePrefab, _SpawnPos, transform.rotation, _GameObjectContainer);
            Cube lCube = lPrefab.GetComponent<Cube>();
            Renderer[] lRenderers = lPrefab.GetComponentsInChildren<Renderer>();
            GetAllMaterials(lRenderers, _ColorSpawnerAndCube);

            lPrefab.transform.localScale = Vector3.zero;
            lPrefab.transform.DOScale(Vector3.one, SPAWN_CUBE_TIME);
            lCube.cubeColor = _ColorSpawnerAndCube;

            if (lCube != null) _CollisionManager.RegisterCube(lCube);

            _CubesSpawned++;
        }

        private void DisablePreview()
        {
            _TrailDirection.Stop();
        }

        private void EnablePreview(bool pBool)
        {
            _TrailDirection.Play();
        }

        private void ResetTick()
        {
            _TickCount = 0;
            _NextSpawnTick = 0;
            _CubesSpawned = 0;
        }

        private void OnDestroy()
        {
            if (_TickProvider != null)
                _TickProvider.tickEvent -= OnTick;

            _GameManager.activatePlayPhase -= SpawnCube;
            _GameManager.activatePlayPhase -= DisablePreview;
            _GameManager.resetLevel -= EnablePreview;
        }
    }
}