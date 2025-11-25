using Com.IsartDigital.Rush.Manager;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class LevelSelectTargetButton : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private GameObject _TargetPrefabToLoad;
        [SerializeField] private GameObject _GameObjectsParents;
        [SerializeField] private TextMeshProUGUI _TextUILevel;
        [SerializeField] private Button _LoadLevel;

        private List<GameObject> _InstantiatedLevel = new List<GameObject>();

        private GameManager _GameManager => GameManager.Instance;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            _LoadLevel.onClick.AddListener(LoadLevel);
            _GameManager.BackToMenu += OnBackToMenu;
        }

        private void LoadLevel()
        {
            GameObject lInstanciatedLevel = Instantiate(_TargetPrefabToLoad);
            lInstanciatedLevel.transform.SetParent(_GameObjectsParents.transform);
            lInstanciatedLevel.transform.localPosition = Vector3.zero;
            _InstantiatedLevel.Add(lInstanciatedLevel);
            _TextUILevel.text = _TargetPrefabToLoad.name;
        }

        private void OnBackToMenu(bool pBool)
        {
            foreach (GameObject level in _InstantiatedLevel)
            {
                if(level != null)
                {
                    DOTween.Kill(level.transform, complete: false);
                    Destroy(level);
                }
            }

            _InstantiatedLevel.Clear();
        }

    }
}