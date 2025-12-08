using Com.IsartDigital.Rush.CameraManagement;
using Com.IsartDigital.Rush.LevelDesign;
using Com.IsartDigital.Rush.Manager;
using Com.IsartDigital.Rush.Utilities;
using DG.Tweening;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{

    public class MenuManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [Header(Utils.PARAMETERS_MENU)]
        [SerializeField] private Transform _CurrentCameraGame;
        [SerializeField] private AudioClip _TransitionSound;
        [SerializeField] private Button _PauseButton;
        [SerializeField] private Button[] _BackLevelSelectButtonsArray;

        private const float TRANSITION_TIME = 1f;
        private const float TWEEN_SCALE = 1f;
        private const float TWEEN_DURATION = .5f;

        private Dictionary<EMenuType, GameObject> _Menus = new Dictionary<EMenuType, GameObject>();

        private GameManager _GameManager => GameManager.Instance;
        private HUDManager _HUDManager => HUDManager.Instance;
        private SoundManager _SoundManager => SoundManager.Instance;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            RegisterMenus();
            RegisterButtons();

            ShowMenu(EMenuType.MAIN);
            _GameManager.gameFinished += ShowMenu;

            _PauseButton.onClick.AddListener(() => SetPauseCamera(true));

            foreach (Button lBackButton in _BackLevelSelectButtonsArray)
                lBackButton.onClick.AddListener(() => SetMenuCamera(false));
            
        }

        private void RegisterMenus()
        {
            _Menus.Clear();

            foreach (MenuType lMenu in FindObjectsByType<MenuType>(FindObjectsSortMode.None))
                _Menus[lMenu.Type] = lMenu.gameObject;
        }

        private void RegisterButtons()
        {
            MenuButton[] lButtons = FindObjectsByType<MenuButton>(FindObjectsSortMode.None);

            foreach (MenuButton lButton in lButtons)
            {
                Button lUIButton = lButton.GetComponent<Button>();
                EMenuType lTarget = lButton.TargetMenu;

                lUIButton.onClick.AddListener(() => SwitchCameraPos(lTarget));
            }
        }

        private void SwitchCameraPos(EMenuType pType)
        {
            CheckTypeOfQuit(pType);

            MenuType lNextMenuToShow;
            foreach (GameObject lMenu in _Menus.Values)
                lMenu.SetActive(false);

            if (_Menus.TryGetValue(pType, out GameObject lNextMenu))
            {
                lNextMenuToShow = lNextMenu.GetComponent<MenuType>();
                _CurrentCameraGame.DOMove(lNextMenuToShow.CameraPos.position, TRANSITION_TIME).From(_CurrentCameraGame.position).SetEase(Ease.OutCubic);
                _CurrentCameraGame.DORotateQuaternion(lNextMenuToShow.CameraPos.rotation, TRANSITION_TIME).From(_CurrentCameraGame.rotation).SetEase(Ease.OutCubic).OnComplete(() => ShowMenu(pType));
            }
            else Debug.LogWarning(Utils.ERR_CAMERA_MENU);
        }

        public void ShowMenu(EMenuType pType)
        {
            Transform lTransform = _Menus[pType].transform;
            foreach (GameObject lMenu in _Menus.Values)
                lMenu.SetActive(false);

           _Menus[pType].SetActive(true);

            lTransform = _Menus[pType].transform;
            lTransform.localScale = Vector3.zero;
            lTransform.DOScale(TWEEN_SCALE, TWEEN_DURATION).SetEase(Ease.OutBack);

            if (pType == EMenuType.PLAY) 
                SetGameCamera(false);
        }

        private void CheckTypeOfQuit(EMenuType pType)
        {
            if (pType == EMenuType.QUIT)
            {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
                return;
            }

            _SoundManager.PlaySound(_TransitionSound, transform.position);
        }

        private void SetGameCamera(bool pIsPause)
        {
            CameraGame lCam = _CurrentCameraGame.GetComponent<CameraGame>();
            Vector3 lBasePoint = _CurrentCameraGame.position + (_CurrentCameraGame.rotation * Vector3.forward * lCam.distanceCamera);

            if (lCam != null)
                lCam.SetStartTransform(_CurrentCameraGame.position, _CurrentCameraGame.rotation, lCam.distanceCamera, lBasePoint);

            if(!pIsPause) _GameManager.switchToGame?.Invoke(true);
        }

        private void SetMenuCamera(bool pBool)
        {
            HUDTileToPlace lHud = _HUDManager.hudTileToPlace;
            if (lHud != null)
                lHud.ClearAllSlots(pBool);

            _GameManager.backToMenu?.Invoke(pBool);
            SwitchCameraPos(EMenuType.LEVEL_SELECT);
        }

        private void SetPauseCamera(bool pBool)
        {
            _GameManager.pauseGame?.Invoke(pBool);
            SwitchCameraPos(EMenuType.PAUSE);
        }

    }
}