using Com.IsartDigital.Rush.Manager;
using System.Collections;
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
        private Dictionary<EMenuType, GameObject> _Menus = new Dictionary<EMenuType, GameObject>();

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            RegisterMenus();
            RegisterButtons();

            ShowMenu(EMenuType.MAIN);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {

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

                lUIButton.onClick.AddListener(() => ShowMenu(lTarget));
            }
        }

        public void ShowMenu(EMenuType pType)
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
            else if (pType == EMenuType.LEVEL_SELECT) GameManager.Instance.SwitchToGame?.Invoke(true);

                foreach (GameObject lMenu in _Menus.Values)
                    lMenu.SetActive(false);

            _Menus[pType].SetActive(true);
        }
    }
}