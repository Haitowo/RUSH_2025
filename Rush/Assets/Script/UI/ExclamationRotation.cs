using Com.IsartDigital.Rush.Manager;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    
    public class ExclamationRotation : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        private Camera _GameCamera;

        private GameManager _GameManager => GameManager.Instance;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            _GameManager.resetLevel += DestroyExclamation;
            _GameCamera = Camera.main;
            transform.LookAt(_GameCamera.transform.position);
            transform.localScale = Vector3.one * .05f;
        }

        private void DestroyExclamation(bool pBool)
        {
            transform.DOScale(Vector3.zero, .5f).From(transform.localScale).SetEase(Ease.Linear).OnComplete(() => Destroy(gameObject));
        }

        private void OnDestroy()
        {
            _GameManager.resetLevel -= DestroyExclamation;
        }
    }
}