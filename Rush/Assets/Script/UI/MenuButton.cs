using Com.IsartDigital.Rush.Manager;
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
        [SerializeField] private AudioClip _ClickButton;
        [SerializeField] private Button _CurrentButton;

        public EMenuType TargetMenu => _TargetMenu;

        private Tween _CurrentTween;

        private Vector3 _OriginScale;

        private SoundManager _SoundManager => SoundManager.Instance;

        private void Awake()
        {
            _OriginScale = transform.localScale;
            _CurrentButton.onClick.AddListener(() => _SoundManager.PlaySound(_ClickButton, transform.position));
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