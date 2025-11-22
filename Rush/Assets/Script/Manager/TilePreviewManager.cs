using Com.IsartDigital.Rush.CubeManagement;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.Manager
{
    
    public class TilePreviewManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        private GameObject _TileSelected;
        private Action<GameObject> _DoActionUI;

        private const int ERR_VALUE = -1;
        private const float DECAY_TILE = .5f;

        public static TilePreviewManager Instance { get; private set; }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            SetStateVoid();
            enabled = false;
            GameManager.Instance.SwitchToGame += Activate;
            GameManager.Instance.BackToMenu += Disable;

            #region Singleton Management
            if (Instance != this && Instance != null)
            {
                Destroy(this);
                Debug.LogError(nameof(TilePreviewManager) + "Instance already exists. Destroying the current instance.");
                return;
            }

            Instance = this;
            //DontDestroyOnLoad(this);
            #endregion
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            _DoActionUI(_TileSelected);
            CheckValidation();
        }

        public void SetStateVoid()
        {
            _DoActionUI = DoActionVoid;
        }

        public void SetStateSelectTile(GameObject pTile)
        {
            _TileSelected = pTile;
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

            float lY;
            int lX;
            int lZ;
            int lCollisionLayerObstacle = 1 << (int)ECollision.GROUND;

            if (Physics.Raycast(lRay, out RaycastHit lHitInfo, Mathf.Infinity, lCollisionLayerObstacle))
            {
                lGlobalIndexToIndex = lHitInfo.point;
                lX = Mathf.RoundToInt(lGlobalIndexToIndex.x);
                lY = Mathf.RoundToInt(lGlobalIndexToIndex.y) + DECAY_TILE;
                lZ = Mathf.RoundToInt(lGlobalIndexToIndex.z);

                lGlobalIndexToIndex = new Vector3(lX, lY, lZ);
                return lGlobalIndexToIndex;
            }
            else return lGlobalIndexToIndex = new Vector3(ERR_VALUE, ERR_VALUE, ERR_VALUE);
        }

        private void CheckValidation()
        {
            if (_TileSelected != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                ValidateTilePlacement();
            }
        }

        private void ValidateTilePlacement()
        {
            TileSelectionManager.Instance.UseOne();

            GameObject lNextPrefab = TileSelectionManager.Instance.GetCurrentPrefab();

            if (lNextPrefab != null)
            {
                _TileSelected = Instantiate(lNextPrefab);
                SetStateSelectTile(_TileSelected);
            }
            else
            {
                SetStateVoid();
                _TileSelected = null;
            }
        }

        private void Activate(bool pEnable) => enabled = pEnable;

        private void Disable(bool pEnable) => enabled = pEnable;
        
    }
}