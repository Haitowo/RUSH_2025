using Com.IsartDigital.Rush.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class HUDTileToPlace : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private Transform _PrefabTileSlot;
        [SerializeField] private Transform _Container;
        [SerializeField] private SOLevelPrefabTilesHUD _SpawnHUDPrefab;

        private const int HUD_DECAY = 75;

        private List<TileEntryRuntime> _AllElementsToSpawn = new List<TileEntryRuntime>();

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            GameManager.Instance.SwitchToGame += SetPrefabOnSpawn;
            LinkSOWithHUD();
            TileSelectionManager.Instance.Init(_AllElementsToSpawn);
        }

        private void SetPrefabOnSpawn(bool pBool)
        {
            GenerateHUD();
        }

        private void LinkSOWithHUD()
        {
            _AllElementsToSpawn.Clear();

            foreach(TileDefinition def in _SpawnHUDPrefab.tilesToPlaceForLevel)
            {
                _AllElementsToSpawn.Add(new TileEntryRuntime(def));
            }
        }

        private void GenerateHUD()
        {
            TileEntryRuntime lEntry;
            Transform lSlot;
            float lOffset;

            for (int i = 0; i < _AllElementsToSpawn.Count; i++)
            {
                lEntry = _AllElementsToSpawn[i];

                lSlot = Instantiate(_PrefabTileSlot, _Container);
                lOffset = i * HUD_DECAY;
                lSlot.localPosition = new Vector3(0f, -lOffset, 0f);

                lSlot.GetComponent<UITileSlot>().Init(lEntry);
                lSlot.SetAsLastSibling();
            }
        }
    }
}