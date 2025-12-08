using Com.IsartDigital.Rush.CubeManagement;
using Com.IsartDigital.Rush.TargetManagement;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.GameObjects
{
    
    public class Target : ColorableTile
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private EColorSetter _TargetColor;
        public EColorSetter CurrentColor => _TargetColor;

        private const float TWEEN_TIME_SCALE = .1f;

        private CollisionManager _CollisionManager => CollisionManager.Instance;

        private void Start()
        {
            Renderer[] lRenderers = GetComponentsInChildren<Renderer>();

            GetAllMaterials(lRenderers, _TargetColor);
        }

        public void DetectCubeColor(Cube pCurrentCube)
        {
            if (pCurrentCube.cubeColor == _TargetColor)
                pCurrentCube.transform.DOScale(Vector3.zero, TWEEN_TIME_SCALE).SetEase(Ease.Linear).OnComplete(() => DestroyCube(pCurrentCube));
            else return;
        }

        private void DestroyCube(Cube pCurrentCube)
        {
            _CollisionManager.RemoveCube(pCurrentCube);
            Destroy(pCurrentCube.gameObject);
        }

    }
}