using Com.IsartDigital.Rush.Manager;
using Com.IsartDigital.Rush.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class LevelSelectChooseSO : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private Button _LevelSelectCurrentButton;
        [SerializeField] private ELevelToload _SOLinkedToLevel;
        [SerializeField] private HUDTileToPlace _TileToPlace;

        private GameManager _GameManager => GameManager.Instance;

        private void Awake()
        {
            _LevelSelectCurrentButton.onClick.AddListener(() => OnLevelSelected(_SOLinkedToLevel));
        }

        private void OnLevelSelected(ELevelToload pSOToLoad) => _GameManager.SetSelectedHUD(pSOToLoad);
    }
}