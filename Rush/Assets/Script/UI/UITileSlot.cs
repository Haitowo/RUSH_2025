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

        private bool _IsTileAlreadySelected;

        public void Init(TileEntryRuntime pEntry)
        {
            _RuntimeEntry = pEntry;

            SetRenderTexture();
            ShowPrefab();
            UpdateUI();

            TileSelectionManager.Instance.OnAmountChanged += UpdateUI;

            _Button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            GameObject lPreviewTile;
            if (_RuntimeEntry.remaining > 0 && !_IsTileAlreadySelected)
            {
                _IsTileAlreadySelected = true;
                lPreviewTile = Instantiate(_RuntimeEntry.prefab);
                lPreviewTile.SetActive(true);
                SetGhostPreview(lPreviewTile);
                TilePreviewManager.Instance.SetStateSelectTile(lPreviewTile);
            }
            else if(_IsTileAlreadySelected)
            {
                TilePreviewManager.Instance.SetStateVoid();
                _RuntimeEntry.ResetOne();
            }
            else return;
        }

        private void UpdateUI()
        {
            amountText.text = _RuntimeEntry.remaining.ToString();
        }

        private void ShowPrefab()
        {
            _SpawnPrefab = Instantiate(_RuntimeEntry.prefab, _PrefabHolder);
            Canvas lCanvas = _SpawnPrefab.GetComponentInChildren<Canvas>();
        }

        private void SetRenderTexture()
        {
            RenderTexture lRenderTexture = new RenderTexture(RENDER_TEXTURE_SIZE, RENDER_TEXTURE_SIZE, RENDER_TEXTURE_DEPTH);
            _PreviewCamera.targetTexture = lRenderTexture;
            _CurrentImage.texture = lRenderTexture;
        }

        private void SetGhostPreview(GameObject pTile)
        {
            Renderer lRend = pTile.GetComponentInChildren<Renderer>();
            Color lColor = lRend.material.color;
            lColor.a = .5f;
            lRend.material.color = lColor;
        }
    }
}