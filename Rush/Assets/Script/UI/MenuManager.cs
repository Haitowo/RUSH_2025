using Com.IsartDigital.Rush.CameraManagement;
using Com.IsartDigital.Rush.Manager;
using Com.IsartDigital.Rush.Utilities;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class MenuManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        private Dictionary<EMenuType, GameObject> _Menus = new Dictionary<EMenuType, GameObject>();

        [SerializeField] private Transform _CurrentCameraGame;
        [SerializeField] private Button _BackMenuGame;

        private const float TRANSITION_TIME = .75f;

        private GameManager _GameManager => GameManager.Instance;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            RegisterMenus();
            RegisterButtons();

            ShowMenu(EMenuType.MAIN);

            _BackMenuGame.onClick.AddListener(() => SetMenuCamera(false));
        }

        private void RegisterMenus()
        {
            _Menus.Clear();

            foreach (MenuType lMenu in FindObjectsByType<MenuType>(FindObjectsSortMode.None))
            {
                _Menus[lMenu.Type] = lMenu.gameObject;
            }
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
            foreach (GameObject lMenu in _Menus.Values)
                lMenu.SetActive(false);
            if (pType == EMenuType.PLAY) SetGameCamera();

                _Menus[pType].SetActive(true);
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
        }

        private void SetGameCamera()
        {
            CameraGame lCam = _CurrentCameraGame.GetComponent<CameraGame>();
            Vector3 lBasePoint = _CurrentCameraGame.position + (_CurrentCameraGame.rotation * Vector3.forward * lCam.distanceCamera);

            if (lCam != null)
                lCam.SetStartTransform(_CurrentCameraGame.position, _CurrentCameraGame.rotation, lCam.distanceCamera, lBasePoint);

            _GameManager.SwitchToGame?.Invoke(true);
        }

        private void SetMenuCamera(bool pBool)
        {
            _GameManager.BackToMenu?.Invoke(pBool);
            SwitchCameraPos(EMenuType.LEVEL_SELECT);
        }

    }
}