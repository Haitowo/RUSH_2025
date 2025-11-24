using Com.IsartDigital.Rush.Manager;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class HUDManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private HUDElementToSpawn[] _HudElementsToSpawn;
        [SerializeField] public HUDTileToPlace hudTileToPlace;
        [SerializeField] private Button _PlayButton;
        [SerializeField] private float _Offset = 100f;

        private const float TWEEN_DURATION = 1.2f;
        private const float TWEEN_DELAY = .75f;

        private GameManager _GameManager => GameManager.Instance;

        public static HUDManager Instance { get; private set; }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            _GameManager.SwitchToGame += ToggleHUD;
            _GameManager.BackToMenu += RemoveHUD;
            transform.gameObject.SetActive(false);

            _PlayButton.onClick.AddListener(() => OnClick());

            #region Singleton Management
            if (Instance != this && Instance != null)
            {
                Destroy(this);
                Debug.LogError(nameof(HUDManager) + "Instance already exists. Destroying the current instance.");
                return;
            }

            Instance = this;
            #endregion
        }

        private void ToggleHUD(bool pShow)
        {
            if (pShow)
                gameObject.SetActive(true);

            foreach (HUDElementToSpawn lElements in _HudElementsToSpawn)
            {
                if (!pShow && lElements.ignoreHide)
                    continue;

                RectTransform lTransform = lElements.objectToTransform;
                Vector2 lBasePos = lTransform.anchoredPosition;
                Vector2 lOffset = lBasePos;

                switch (lElements.pos)
                {
                    case EHUDElementPos.TOP:
                        lOffset = lBasePos + new Vector2(0f, _Offset);
                        break;
                    case EHUDElementPos.BOTTOM:
                        lOffset = lBasePos + new Vector2(0f, -_Offset);
                        break;
                    case EHUDElementPos.LEFT:
                        lOffset = lBasePos + new Vector2(-_Offset, 0f);
                        break;
                    case EHUDElementPos.RIGHT:
                        lOffset = lBasePos + new Vector2(_Offset, 0f);
                        break;
                    default:
                        break;
                }

                if (pShow)
                {
                    lTransform.anchoredPosition = lOffset;
                    lTransform.DOAnchorPos(lBasePos, TWEEN_DURATION)
                        .SetEase(Ease.OutBack);
                }
                else
                {
                    lTransform.DOAnchorPos(lOffset, TWEEN_DURATION)
                        .SetEase(Ease.OutBounce);
                }
            }

            if(!pShow) DOVirtual.DelayedCall(TWEEN_DURATION * TWEEN_DELAY, () => StartGame());
        }

        private void RemoveHUD(bool pHide) => transform.gameObject.SetActive(false);

        private void OnClick() => ToggleHUD(false);

        private void StartGame() => _GameManager.ActivatePlayPhase?.Invoke();
        
    }
}