using Com.IsartDigital.Rush.Manager;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.LevelDesign
{
    
    public class LevelDesignSpawner : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private Transform _LevelContainer;

        private List<GameObject> _TilesForLevel = new List<GameObject>();

        private const float SPAWN_TILE_TIME_MIN = .5f;
        private const float SPAWN_TILE_TIME_MAX = 1.5f;

        private const int DECAY_DOWN = 10;

        private GameManager _GameManager => GameManager.Instance;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            GetLevel();
            _LevelContainer.gameObject.SetActive(false);
            _GameManager.SwitchToGame += SpawnLevel;
        }

        private void GetLevel()
        {
            foreach (Transform tile in _LevelContainer)
            {
                _TilesForLevel.Add(tile.gameObject);
            }
        }

        private void SpawnLevel(bool pShow)
        {
            _LevelContainer.gameObject.SetActive(pShow);
            foreach (GameObject tile in _TilesForLevel)
            {
                tile.transform.DOMove(tile.transform.position, Random.Range(SPAWN_TILE_TIME_MIN, SPAWN_TILE_TIME_MAX)).From(tile.transform.position + (Vector3.down * DECAY_DOWN)).SetEase(Ease.OutBack);
            }   
        }
    }
}