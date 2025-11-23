using Com.IsartDigital.Rush.CubeManagement;
using Com.IsartDigital.Rush.Manager;
using Com.IsartDigital.Rush.Ticks;
using DG.Tweening;
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
        [SerializeField] private EColorSetter _ColorSpawnerAndCube;

        public EColorSetter ColorSpawnerAndCube => _ColorSpawnerAndCube;
        private ITickProvider _TickProvider;

        private int _TickCount = 0;
        private const int TICK_PER_SPAWN = 2;

        private const float SPAWN_DECAY = .5f;
        private const float SPAWN_CUBE_TIME = .2f;

        private Vector3 _SpawnPos;

        private GameManager _GameManager => GameManager.Instance;

        private void Start()
        {
            _TickProvider = TickProviderLocator.Instance;
            _TickProvider.TickEvent += OnTick;

            _GameManager.ActivatePlayPhase += SpawnCube;
        }

        private void OnTick()
        {
            _TickCount++;
            if (_TickCount >= TICK_PER_SPAWN)
            {
                //SpawnCube();
                _TickCount = 0;
            }
        }

        private void SpawnCube()
        {
            _SpawnPos = new Vector3(transform.position.x, transform.position.y + SPAWN_DECAY, transform.position.z);

            GameObject lPrefab = Instantiate(_CubePrefab, _SpawnPos, transform.rotation, _GameObjectContainer);
            Cube lCube = lPrefab.GetComponent<Cube>();

            lPrefab.transform.localScale = Vector3.zero;
            lPrefab.transform.DOScale(Vector3.one, SPAWN_CUBE_TIME);
            lCube.cubeColor = _ColorSpawnerAndCube;

            if (lCube != null) CollisionManager.Instance.RegisterCube(lCube);
        }

        private void OnDestroy()
        {
            if (_TickProvider != null)
                _TickProvider.TickEvent -= OnTick;
        }
    }
}