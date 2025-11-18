using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private float _ScaleMultiplier = 1.1f;
        [SerializeField] private float _Duration = .5f;
        [SerializeField] private EMenuType _TargetMenu;

        public EMenuType TargetMenu => _TargetMenu;

        private Tween _CurrentTween;

        private Vector3 _OriginScale;

        private void Awake()
        {
            _OriginScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData pEventData)
        {
            _CurrentTween?.Kill();

            _CurrentTween = transform.DOScale(_OriginScale * _ScaleMultiplier, _Duration)
                .SetEase(Ease.OutElastic);
        }

        public void OnPointerExit(PointerEventData pEventData)
        {
            _CurrentTween?.Kill();

            _CurrentTween = transform.DOScale(_OriginScale, _Duration)
                .SetEase(Ease.OutElastic);
        }
    }
}