using UnityEngine;
using Com.IsartDigital.Rush.Ticks;
using Com.IsartDigital.Rush.CubeManagement;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 09/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.GameObjects
{
    public class CubeSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _CubePrefab;
        [SerializeField] private Transform _SpawnPoint;
        [SerializeField] private Transform _GameObjectContainer;

        private ITickProvider _TickProvider;
        private int _TickCount = 0;

        private const int TICK_PER_SPAWN = 2;

        private const float SPAWN_DECAY = .5f;

        private Vector3 _SpawnPos;

        private void Start()
        {
            _TickProvider = TickProviderLocator.Instance;
            _TickProvider.TickEvent += OnTick;

            _SpawnPos = new Vector3(transform.position.x, transform.position.y + SPAWN_DECAY, transform.position.z);

            SpawnCube();
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
            GameObject lPrefab = Instantiate(_CubePrefab, _SpawnPos, transform.rotation, _GameObjectContainer);
            Cube lCube = lPrefab.GetComponent<Cube>();
            if(lCube != null) CollisionManager.Instance.RegisterCube(lCube);
        }

        private void OnDestroy()
        {
            if (_TickProvider != null)
                _TickProvider.TickEvent -= OnTick;
        }
    }
}