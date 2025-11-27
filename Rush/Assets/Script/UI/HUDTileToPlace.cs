using Com.IsartDigital.Rush.Manager;
using Com.IsartDigital.Rush.Utilities;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class HUDTileToPlace : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private Transform _PrefabTileSlot;
        [SerializeField] private Transform _Container;
        [SerializeField] private List<SOLevelPrefabTilesHUD> _AllHUD;

        private SOLevelPrefabTilesHUD _SpawnHUDPrefab;

        private const int HUD_DECAY = 75;

        private List<TileEntryRuntime> _AllElementsToSpawn = new List<TileEntryRuntime>();
        private List<UITileSlot> _UISlots = new List<UITileSlot>();

        private GameManager _GameManager => GameManager.Instance;
        private TileSelectionManager _TileSelectionManager => TileSelectionManager.Instance;

        private void OnEnable() => _GameManager.switchToGame += SetPrefabOnSpawn;

        private void OnDisable() => _GameManager.switchToGame -= SetPrefabOnSpawn;

        private void SetPrefabOnSpawn(bool pBool)
        {
            ELevelToload lLevelHUDToload = _GameManager.selectHUDLevel;
            LinkSOWithHUD(lLevelHUDToload);
            GenerateHUD();
        }

        public void LinkSOWithHUD(ELevelToload pLevel)
        {
            int lIndexSOLevel = (int)pLevel;
            if(lIndexSOLevel >= 0 && lIndexSOLevel < _AllHUD.Count)
                _SpawnHUDPrefab = _AllHUD[lIndexSOLevel];
            else 
                Debug.LogWarning(Utils.ERR_HUD_SPAWN);

            _AllElementsToSpawn.Clear();

            foreach(TileDefinition def in _SpawnHUDPrefab.tilesToPlaceForLevel)
                _AllElementsToSpawn.Add(new TileEntryRuntime(def));
            
            _TileSelectionManager.Init(_AllElementsToSpawn);
        }

        private void GenerateHUD()
        {
            TileEntryRuntime lEntry;
            UITileSlot lTileSlot;
            Transform lSlot;
            float lOffset;

            for (int i = 0; i < _AllElementsToSpawn.Count; i++)
            {
                lEntry = _AllElementsToSpawn[i];

                lSlot = Instantiate(_PrefabTileSlot, _Container);
                lOffset = i * HUD_DECAY;
                lSlot.localPosition = new Vector3(0f, -lOffset, 0f);
                lTileSlot = lSlot.GetComponent<UITileSlot>();
                _UISlots.Add(lTileSlot);

                lSlot.GetComponent<UITileSlot>().Init(lEntry, i);
                lSlot.SetAsLastSibling();
            }
        }

        public void ResetSlot()
        {
            _AllElementsToSpawn.Clear();

            foreach (TileDefinition def in _SpawnHUDPrefab.tilesToPlaceForLevel)
                _AllElementsToSpawn.Add(new TileEntryRuntime(def));
            
            for (int i = 0; i < _UISlots.Count; i++)
                _UISlots[i].ResetSlot(_AllElementsToSpawn[i], i);
            
            _TileSelectionManager.ResetAll();
        }

        public void ClearAllSlots(bool pBool)
        {
            foreach (UITileSlot slot in _UISlots)
            {
                if (slot != null)
                    Destroy(slot.gameObject);
            }

            _UISlots.Clear();
            _AllElementsToSpawn.Clear();
        }

        public void UpdateSlot(TileEntryRuntime pEntry)
        {
            int lCurrentIndex = _AllElementsToSpawn.IndexOf(pEntry);
            if (lCurrentIndex >= 0 && lCurrentIndex < _UISlots.Count)
            {
                _UISlots[lCurrentIndex].ResetSlot(pEntry, lCurrentIndex);
            }
        }
    }
}