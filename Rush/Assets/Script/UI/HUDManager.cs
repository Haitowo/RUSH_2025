using Com.IsartDigital.Rush.Manager;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class HUDManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private HudElementToSpawn[] _HudElementsToSpawn;
        [SerializeField] private float _Offset = 100f;

        private const float TWEEN_DURATION = 1.2f;
        private const float TWEEN_DELAY = .75f;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            GameManager.Instance.SwitchToGame += ToggleHUD;
            transform.gameObject.SetActive(false);
        }

        private void ToggleHUD(bool pShow)
        {
            if (pShow)
                gameObject.SetActive(true);

            foreach (HudElementToSpawn lElements in _HudElementsToSpawn)
            {
                RectTransform lTransform = lElements.objectToTransform;
                Vector2 lBasePos = lTransform.anchoredPosition;
                Vector2 lOffset = lBasePos;

                switch (lElements.pos)
                {
                    case EHudElementPos.TOP:
                        lOffset = lBasePos + new Vector2(0f, _Offset);
                        break;
                    case EHudElementPos.BOTTOM:
                        lOffset = lBasePos + new Vector2(0f, -_Offset);
                        break;
                    case EHudElementPos.LEFT:
                        lOffset = lBasePos + new Vector2(-_Offset, 0f);
                        break;
                    case EHudElementPos.RIGHT:
                        lOffset = lBasePos + new Vector2(_Offset, 0f);
                        break;
                    default:
                        break;
                }

                if (pShow)
                    lTransform.anchoredPosition = lOffset;

                lTransform.DOAnchorPos(lBasePos, TWEEN_DURATION)
                    .SetEase(Ease.OutElastic);
            }

            if(!pShow) DOVirtual.DelayedCall(TWEEN_DURATION * TWEEN_DELAY, () => gameObject.SetActive(false));
        }
    }
}