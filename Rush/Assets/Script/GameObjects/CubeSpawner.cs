using Com.IsartDigital.Rush.CubeManagement;
using Com.IsartDigital.Rush.Manager;
using Com.IsartDigital.Rush.Ticks;
using Com.IsartDigital.Rush.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 09/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.GameObjects
{
    public class CubeSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _CubePrefab;
        [SerializeField] private Transform _SpawnPoint;
        [SerializeField] private Transform _GameObjectContainer;
        [SerializeField] private Color _OrangeColor;
        [SerializeField] private Color _PurpleColor;
        [SerializeField] private Color _BlueColor;
        [SerializeField] private EColorSetter _ColorSpawnerAndCube;
        [SerializeField] private ELevelToload _CurrentLevel;
        [SerializeField] private bool _DoesLevelNeedsToGetDelaySpawn;

        [SerializeField] private float _TickToDelay;

        public EColorSetter ColorSpawnerAndCube => _ColorSpawnerAndCube;
        private ITickProvider _TickProvider;

        private int _TickCount = 0;
        private float _NextSpawnTick;
        private int _TickNextSpawnToAdd = 9;
        private int _CubesSpawned = 0;

        private const int TOTAL_CUBES_TO_SPAWN = 8;

        private const float SPAWN_DECAY = .5f;
        private const float SPAWN_CUBE_TIME = .2f;

        private Vector3 _SpawnPos;

        private Dictionary<EColorSetter, Color> _ColorTable;
        private Material _SpawnMaterial;

        private GameManager _GameManager => GameManager.Instance;
        private CollisionManager _CollisionManager => CollisionManager.Instance;

        private void Awake()
        {
            _ColorTable = new Dictionary<EColorSetter, Color>(){
            { EColorSetter.RED, Color.red },
            { EColorSetter.BLUE, _BlueColor },
            { EColorSetter.GREEN, Color.green },
            { EColorSetter.ORANGE, _OrangeColor },
            { EColorSetter.CYAN, Color.cyan },
            { EColorSetter.PURPLE, _PurpleColor },
            };
        }

        private void Start()
        {
            enabled = false;
            _TickProvider = TickProviderLocator.Instance;
            _TickProvider.tickEvent += OnTick;

            _GameManager.activatePlayPhase += _DoesLevelNeedsToGetDelaySpawn ? ResetTick : SpawnCube;

            Renderer lRend = GetComponentInChildren<Renderer>();
            _SpawnMaterial = lRend.material;
            ApplyColorMaterial(_ColorSpawnerAndCube, _SpawnMaterial);
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
            GetAllMaterials(lRenderers);

            lPrefab.transform.localScale = Vector3.zero;
            lPrefab.transform.DOScale(Vector3.one, SPAWN_CUBE_TIME);
            lCube.cubeColor = _ColorSpawnerAndCube;

            if (lCube != null) _CollisionManager.RegisterCube(lCube);

            _CubesSpawned++;
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
        }

        private void GetAllMaterials(Renderer[] pRenderers)
        {
            foreach (Renderer r in pRenderers)
            {
                Material[] lMaths = r.materials;

                for (int i = 0; i < lMaths.Length; i++)
                {
                    lMaths[i] = new Material(lMaths[i]);
                    ApplyColorMaterial(_ColorSpawnerAndCube, lMaths[i]);
                }

                r.materials = lMaths;
            }
        }

        private void ApplyColorMaterial(EColorSetter pCurrentColor, Material pMaterial)
        {
            if (_ColorTable.TryGetValue(pCurrentColor, out Color lColor))
                pMaterial.color = lColor;
        }
    }
}