using Com.IsartDigital.Rush.CubeManagement;
using Com.IsartDigital.Rush.UI;
using Com.IsartDigital.Rush.Utilities;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.Manager
{
    
    public class TilePreviewManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private HUDTileToPlace _HUDTileToPlace;
        [SerializeField] private LayerMask _ObstacleMask;

        private GameObject _GhostTile;
        private Action<GameObject> _DoActionUI;

        private const int LEFT_CLICK_BUTTON_AND_TOUCH = 0;

        private const float DECAY_TILE = .5f;
        private const float TWEEN_TIME = .5f;
        private const float PAUSE_TIME = .05f;
        private const float FULL_TURN = 360f;

        private List<GameObject> _PlacedTiles = new List<GameObject>();
        private List<Vector3?> _PositionUsed = new List<Vector3?>();

        private bool _HasSnapErrorBeenShown = false;

        public static TilePreviewManager Instance { get; private set; }

        private GameManager _GameManager => GameManager.Instance;
        private TileSelectionManager _TileSelectionManager => TileSelectionManager.Instance;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            SetStateVoid();
            enabled = false;
            _GameManager.switchToGame += Activate;
            _GameManager.backToMenu += Disable;
            _GameManager.pauseGame += Disable;
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
            CheckClickInputs();
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
            Vector3? lSnapPos = SnapOnGrid();

            if (!lSnapPos.HasValue && _GhostTile.activeSelf)
            {
                _GhostTile.SetActive(false);
                return;
            }
            else if (!_GhostTile.activeSelf && lSnapPos.HasValue)
                _GhostTile.SetActive(true);

            _HasSnapErrorBeenShown = false;
            if (lSnapPos.HasValue) 
                pTile.transform.position = lSnapPos.Value;
        }

        private Vector3? SnapOnGrid()
        {
            Vector3 lMousePos = GetPointerPosition();
            Vector3 lGlobalIndexToIndex;
            Ray lRay = Camera.main.ScreenPointToRay(lMousePos);

            if (Physics.Raycast(lRay, out RaycastHit lHitInfo, Mathf.Infinity, _ObstacleMask))
            {
                lGlobalIndexToIndex = lHitInfo.point;
                int lX = Mathf.FloorToInt(lGlobalIndexToIndex.x + DECAY_TILE);
                int lZ = Mathf.FloorToInt(lGlobalIndexToIndex.z + DECAY_TILE);
                float lY = Mathf.FloorToInt(lGlobalIndexToIndex.y) + DECAY_TILE;

                lGlobalIndexToIndex = new Vector3(lX, lY, lZ);
                return lGlobalIndexToIndex;
            }
            else return null;
        }

        private void CheckValidation()
        {
            Vector3? lCurrentMousePos = SnapOnGrid();
            if (_GhostTile != null && GetPrimaryDown() && SnapOnGrid() != null && !_PositionUsed.Contains(lCurrentMousePos))
                ValidateTilePlacement();
            else if (_GhostTile == null && GetPrimaryDown())
                TryRemoveTile();
        }

        private void ValidateTilePlacement()
        {
            if (_GhostTile == null) return;

            TileRuntimeIdentifier lIdentifier;
            GameObject lPlacedTile = Instantiate(_GhostTile);

            Vector3 lPos = _GhostTile.transform.position;
            _PositionUsed.Add(lPos);
                
            lPlacedTile.transform.position = lPos;
            lIdentifier = lPlacedTile.AddComponent<TileRuntimeIdentifier>();
            lIdentifier.tileEntry = _TileSelectionManager.CurrentEntry;
            Destroy(_GhostTile);
            _GhostTile = null;
            lPlacedTile.transform.DOLocalRotate(new Vector3(0f, FULL_TURN, 0f), TWEEN_TIME, RotateMode.FastBeyond360).SetRelative(true).OnComplete(() => PlaceTile(lPlacedTile));

            _TileSelectionManager.UseOne();
            GameObject lNextPrefab = _TileSelectionManager.GetCurrentPrefab();
            CheckNextPrefab(lNextPrefab);
        }

        private void PlaceTile(GameObject pTile)
        {
            _PlacedTiles.Add(pTile);
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

        private void CheckClickInputs()
        {
            if (_GhostTile != null && Input.GetMouseButton(1))
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
                Destroy(lTile);
            
            SetStateVoid();
        }

        private void TryRemoveTile()
        {
            Vector3 lMousePos = GetPointerPosition();
            Ray lRay = Camera.main.ScreenPointToRay(lMousePos);

            if (Physics.Raycast(lRay, out RaycastHit hit, Mathf.Infinity))
            {
                GameObject lTargetTile = hit.collider.gameObject;

                if(lTargetTile.layer == (int)ECollision.GROUND)
                    return;

                TileRuntimeIdentifier lIdentifier = lTargetTile.GetComponent<TileRuntimeIdentifier>();

                if (lIdentifier != null && _PlacedTiles.Contains(lTargetTile))
                {
                    _PlacedTiles.Remove(lTargetTile);
                    AnimateTileRemoval(lTargetTile, lIdentifier);
                }
            }
        }

        private void Activate(bool pEnable) => enabled = pEnable;

        private void Disable(bool pEnable)
        {
            enabled = pEnable;

            if (_GameManager.isGameOnPause) return;

            foreach (GameObject tile in _PlacedTiles)
            {
                if(tile != null)
                    Destroy(tile);  
            }

            _PlacedTiles.Clear();
            _PositionUsed.Clear();
        }

        private void AnimateTileRemoval(GameObject pTile, TileRuntimeIdentifier pIdentifier)
        {
            _TileSelectionManager.AddOne(pIdentifier.tileEntry);

            Sequence lSequence = DOTween.Sequence();

            lSequence.Append(pTile.transform.DOMoveY(pTile.transform.position.y + DECAY_TILE, TWEEN_TIME / 2f).SetEase(Ease.OutQuad));
            lSequence.Join(pTile.transform.DORotate(new Vector3(0f, FULL_TURN, 0f), TWEEN_TIME, RotateMode.FastBeyond360).SetRelative(true));
            lSequence.AppendInterval(PAUSE_TIME);
            lSequence.OnComplete(() => Destroy(pTile));
        }

        private bool GetPrimaryDown()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return Input.GetMouseButtonDown(LEFT_CLICK_BUTTON_AND_TOUCH);
#else
    return Touchscreen.current?.primaryTouch.press.wasPressedThisFrame ?? false;
#endif
        }

        private Vector2 GetPointerPosition()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return Mouse.current.position.ReadValue();
#else
     if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        return Touchscreen.current.primaryTouch.position.ReadValue();
    return Vector2.zero;
#endif
        }
    }
}