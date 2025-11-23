using Com.IsartDigital.Rush.CubeManagement;
using Com.IsartDigital.Rush.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.EventTrigger;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.Manager
{
    
    public class TilePreviewManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        private GameObject _GhostTile;
        private Action<GameObject> _DoActionUI;

        private const int ERR_VALUE = -1;
        private const float DECAY_TILE = .5f;
        private List<GameObject> _PlacedTiles = new List<GameObject>();

        public static TilePreviewManager Instance { get; private set; }

        private GameManager _GameManager => GameManager.Instance;
        private TileSelectionManager _TileSelectionManager => TileSelectionManager.Instance;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            SetStateVoid();
            enabled = false;
            _GameManager.SwitchToGame += Activate;
            _GameManager.BackToMenu += Disable;
            _TileSelectionManager.OnInventoryEmpty += HandleInventoryEmpty;

            #region Singleton Management
            if (Instance != this && Instance != null)
            {
                Destroy(this);
                Debug.LogError(nameof(TilePreviewManager) + "Instance already exists. Destroying the current instance.");
                return;
            }

            Instance = this;
            #endregion
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            _DoActionUI(_GhostTile);
            CheckValidation();
            HandleMouseWheel();
            CheckRightClick();
        }

        public void SetStateVoid()
        {
            _DoActionUI = DoActionVoid;
        }

        public void SetStateSelectTile(GameObject pTile)
        {
            _GhostTile = pTile;
            TileEntryRuntime lEntry = _TileSelectionManager.CurrentEntry;
            _GhostTile.transform.rotation = Quaternion.Euler(0f, lEntry.angleToTurn, 0f);
            _DoActionUI = DoActionTileOnGrid;
        }

        private void DoActionVoid(GameObject pTile) { }

        private void DoActionTileOnGrid(GameObject pTile)
        {
            Vector3 lSnapPos = SnapOnGrid();
            if (lSnapPos.x != -1)
                pTile.transform.position = lSnapPos;
        }

        private Vector3 SnapOnGrid()
        {
            Vector3 lMousePos = Mouse.current.position.ReadValue();
            Vector3 lGlobalIndexToIndex;
            Ray lRay = Camera.main.ScreenPointToRay(lMousePos);

            int lCollisionLayerObstacle = 1 << (int)ECollision.GROUND;

            if (Physics.Raycast(lRay, out RaycastHit lHitInfo, Mathf.Infinity, lCollisionLayerObstacle))
            {
                lGlobalIndexToIndex = lHitInfo.point;
                Vector3 lPlaceInfo = lHitInfo.point;
                int lX = Mathf.FloorToInt(lPlaceInfo.x + DECAY_TILE);
                int lZ = Mathf.FloorToInt(lPlaceInfo.z + DECAY_TILE);
                float lY = Mathf.FloorToInt(lPlaceInfo.y) + DECAY_TILE;

                lGlobalIndexToIndex = new Vector3(lX, lY, lZ);
                return lGlobalIndexToIndex;
            }
            else return lGlobalIndexToIndex = new Vector3(ERR_VALUE, ERR_VALUE, ERR_VALUE);
        }

        private void CheckValidation()
        {
            if (_GhostTile != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                ValidateTilePlacement();
            }
        }

        private void ValidateTilePlacement()
        {
            if (_GhostTile == null) return;
            GameObject lPlacedTile = Instantiate(_GhostTile);
            Vector3 lPos = _GhostTile.transform.position;
            lPlacedTile.transform.position = lPos;

            _TileSelectionManager.UseOne();
            GameObject lNextPrefab = _TileSelectionManager.GetCurrentPrefab();

            Destroy(_GhostTile);
            _PlacedTiles.Add(lPlacedTile);

            CheckNextPrefab(lNextPrefab);
        }

        private void HandleInventoryEmpty()
        {
            if (_GhostTile != null)
            {
                Destroy(_GhostTile);
                _GhostTile = null;
            }

            SetStateVoid();
        }

        private void HandleMouseWheel()
        {
            float lScroll = Mouse.current.scroll.ReadValue().y;

            if (lScroll > 0)
                NextTile();
            else if (lScroll < 0)
                PreviousTile();
        }

        private void SwitchPreviewTile()
        {
            GameObject lNextPrefab = _TileSelectionManager.GetCurrentPrefab();
            Destroy(_GhostTile);
            _GhostTile = Instantiate(lNextPrefab);
            SetStateSelectTile(_GhostTile);
        }

        private void CheckNextPrefab(GameObject pPrefab)
        {
            if (pPrefab != null)
            {
                _GhostTile = Instantiate(pPrefab);
                SetStateSelectTile(_GhostTile);
            }
            else
            {
                SetStateVoid();
                _GhostTile = null;
            }
        }

        private void CheckRightClick()
        {
            if (_GhostTile != null && Mouse.current.rightButton.wasPressedThisFrame)
            {
                _GhostTile.SetActive(false);
                SetStateVoid();
            }
        }

        private void NextTile()
        {
            _TileSelectionManager.Next();
            SwitchPreviewTile();
        }

        private void PreviousTile()
        {
            _TileSelectionManager.Previous();
            SwitchPreviewTile();
        }

        public void ResetPreview()
        {
            if (_GhostTile != null)
            {
                Destroy(_GhostTile);
                _GhostTile = null;
            }

            foreach (GameObject lTile in _PlacedTiles)
            {
                Destroy(lTile);
            }

            SetStateVoid();
        }

        private void Activate(bool pEnable) => enabled = pEnable;

        private void Disable(bool pEnable) => enabled = pEnable;
        
    }
}