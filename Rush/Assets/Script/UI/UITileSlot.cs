using Com.IsartDigital.Rush.Manager;
using Com.IsartDigital.Rush.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class UITileSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private TMP_Text amountText;
        [SerializeField] private Button _Button;
        [SerializeField] private Transform _PrefabHolder;
        [SerializeField] private Camera _PreviewCamera;
        [SerializeField] private RawImage _CurrentImage;
        [SerializeField] private AudioClip _ClickSound;

        private float _ScaleMultiplier = 1.1f;
        private float _Duration = .5f;

        private Vector3 _OriginalScale;

        private Tween _CurrentTween;

        private TileEntryRuntime _RuntimeEntry;

        private GameObject _SpawnPrefab;

        private const int RENDER_TEXTURE_SIZE = 100;
        private const int RENDER_TEXTURE_DEPTH = 10;

        private int _Index;

        private bool _IsTileAlreadySelected;

        private TileSelectionManager _TileSelectionManager => TileSelectionManager.Instance;
        private TilePreviewManager _TilePreviewManager => TilePreviewManager.Instance;
        private SoundManager _SoundManager => SoundManager.Instance;

        public void Init(TileEntryRuntime pEntry, int pIndex)
        {
            _RuntimeEntry = pEntry;

            _Index = pIndex;

            SetRenderTexture();
            ShowPrefab();
            UpdateUI();

            _Button.onClick.AddListener(OnClick);
            _OriginalScale = transform.localScale;
            _TileSelectionManager.RegisterSlot(this);
        }

        private void OnClick()
        {
            GameObject lPreviewTile;

            _SoundManager.PlaySound(_ClickSound, transform.position);
            if (_TileSelectionManager.CurrentSlot != null && _TileSelectionManager.CurrentSlot != this)
                _TileSelectionManager.CurrentSlot.Deselect();
            
            if (_RuntimeEntry.remaining > 0 && !_IsTileAlreadySelected)
            {
                _TileSelectionManager.SetIndex(_Index);
                _IsTileAlreadySelected = true;
                lPreviewTile = Instantiate(_RuntimeEntry.prefab);
                lPreviewTile.SetActive(true);
                _TilePreviewManager.SetStateSelectTile(lPreviewTile);
            }
            else if (_IsTileAlreadySelected)
            {
                _IsTileAlreadySelected = false;
                _TilePreviewManager.SetStateVoid();
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
            _SpawnPrefab.transform.localRotation = Quaternion.Euler(0f, _RuntimeEntry.angleToTurn, 0f);
            Canvas lCanvas = _SpawnPrefab.GetComponentInChildren<Canvas>();
        }

        private void SetRenderTexture()
        {
            RenderTexture lRenderTexture = new RenderTexture(RENDER_TEXTURE_SIZE, RENDER_TEXTURE_SIZE, RENDER_TEXTURE_DEPTH);
            _PreviewCamera.targetTexture = lRenderTexture;
            _CurrentImage.texture = lRenderTexture;
        }

        public void SetRotation(int pAngle)
        {
            if (_SpawnPrefab != null)
                _SpawnPrefab.transform.localRotation = Quaternion.Euler(0f, pAngle, 0f);
        }


        public void ResetSlot(TileEntryRuntime pNewEntry, int pIndex)
        {
            _TileSelectionManager.OnAmountChanged -= UpdateUI;

            _RuntimeEntry = pNewEntry;
            _Index = pIndex;
            _IsTileAlreadySelected = false;

            UpdateUI();

            _TileSelectionManager.OnAmountChanged += UpdateUI;
        }

        public void OnPointerEnter(PointerEventData pEventData)
        {
            _CurrentTween?.Kill();

            _CurrentTween = transform.DOScale(_OriginalScale * _ScaleMultiplier, _Duration)
                .SetEase(Ease.OutElastic);
        }

        public void OnPointerExit(PointerEventData pEventData)
        {
            _CurrentTween?.Kill();

            _CurrentTween = transform.DOScale(_OriginalScale, _Duration)
                .SetEase(Ease.OutElastic);
        }

        public void Deselect() => _IsTileAlreadySelected = false;
        
        private void OnEnable() => _TileSelectionManager.OnAmountChanged += UpdateUI;

        private void OnDestroy()
        {
            _CurrentTween?.Kill();
            _TileSelectionManager.OnAmountChanged -= UpdateUI;

            if (_PreviewCamera != null && _PreviewCamera.targetTexture != null)
            {
                _PreviewCamera.targetTexture.Release();
                Destroy(_PreviewCamera.targetTexture);
            }
            if (_SpawnPrefab != null)
                Destroy(_SpawnPrefab);
        }
    }
}