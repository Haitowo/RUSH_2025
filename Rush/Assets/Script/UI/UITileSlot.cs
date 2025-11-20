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
        [SerializeField] private RenderTexture _RenderTexture;

        private TileEntryRuntime _RuntimeEntry;

        private GameObject _SpawnPrefab;

        public void Init(TileEntryRuntime pEntry)
        {
            _RuntimeEntry = pEntry;

            ShowPrefab();
            UpdateUI();

            _Button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            if (_RuntimeEntry.remaining > 0)
            {
                GameManager.Instance.SelectTileToPlace(_RuntimeEntry.prefab);
                _RuntimeEntry.UseOne();
                UpdateUI();
            }
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
    }
}