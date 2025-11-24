using Com.IsartDigital.Rush.Manager;
using Com.IsartDigital.Rush.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class UITileSlot : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private Button _Button;
        [SerializeField] private Transform _PrefabHolder;
        [SerializeField] private Camera _PreviewCamera;
        [SerializeField] private RawImage _CurrentImage;

        private TileEntryRuntime _RuntimeEntry;

        private GameObject _SpawnPrefab;

        private const int RENDER_TEXTURE_SIZE = 100;
        private const int RENDER_TEXTURE_DEPTH = 10;

        private int _Index;

        private bool _IsTileAlreadySelected;

        private TileSelectionManager _TileSelectionManager => TileSelectionManager.Instance;
        private TilePreviewManager _TilePreviewManager => TilePreviewManager.Instance;

        public void Init(TileEntryRuntime pEntry, int pIndex)
        {
            _RuntimeEntry = pEntry;

            _Index = pIndex;

            SetRenderTexture();
            ShowPrefab();
            UpdateUI();

            _Button.onClick.AddListener(OnClick);
        }

        private void OnEnable() => _TileSelectionManager.OnAmountChanged += UpdateUI;

        private void OnDisable() => _TileSelectionManager.OnAmountChanged -= UpdateUI;

        private void OnClick()
        {
            GameObject lPreviewTile;

            if (_RuntimeEntry.remaining > 0 && !_IsTileAlreadySelected)
            {
                _TileSelectionManager.SetIndex(_Index);
                _IsTileAlreadySelected = true;
                lPreviewTile = Instantiate(_RuntimeEntry.prefab);
                lPreviewTile.SetActive(true);
                _TilePreviewManager.SetStateSelectTile(lPreviewTile);
            }
            else if(_IsTileAlreadySelected)
                _TilePreviewManager.SetStateVoid();
            else return;
        }

        private void UpdateUI()
        {
            amountText.text = _RuntimeEntry.remaining.ToString();
        }

        private void ShowPrefab()
        {
            _SpawnPrefab = Instantiate(_RuntimeEntry.prefab, _PrefabHolder);
            _SpawnPrefab.transform.localRotation = Quaternion.Euler(0f, _RuntimeEntry.angleToTurn, 0f);
            Canvas lCanvas = _SpawnPrefab.GetComponentInChildren<Canvas>();
        }

        private void SetRenderTexture()
        {
            RenderTexture lRenderTexture = new RenderTexture(RENDER_TEXTURE_SIZE, RENDER_TEXTURE_SIZE, RENDER_TEXTURE_DEPTH);
            _PreviewCamera.targetTexture = lRenderTexture;
            _CurrentImage.texture = lRenderTexture;
        }

        public void SetRotation(int angle)
        {
            if (_SpawnPrefab != null)
                _SpawnPrefab.transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }


        public void ResetSlot(TileEntryRuntime newEntry, int index)
        {
            _TileSelectionManager.OnAmountChanged -= UpdateUI;

            _RuntimeEntry = newEntry;
            _Index = index;
            _IsTileAlreadySelected = false;

            UpdateUI();

            _TileSelectionManager.OnAmountChanged += UpdateUI;
        }
    }
}