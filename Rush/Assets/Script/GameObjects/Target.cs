using Com.IsartDigital.Rush.CubeManagement;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.GameObjects
{
    
    public class Target : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private EColorSetter _TargetColor;
        public EColorSetter CurrentColor => _TargetColor;

        private const float TWEEN_TIME_SCALE = .1f;

        private CollisionManager _CollisionManager => CollisionManager.Instance;

        private Dictionary<EColorSetter, Color> _ColorTable;
        private Material _SpawnMaterial;

        [SerializeField] private Color _OrangeColor;
        [SerializeField] private Color _PurpleColor;
        [SerializeField] private Color _BlueColor;

        private void Awake()
        {
            _ColorTable = new Dictionary<EColorSetter, Color>(){
            { EColorSetter.RED, Color.red },
            { EColorSetter.BLUE, _BlueColor },
            { EColorSetter.GREEN, Color.green },
            { EColorSetter.ORANGE, _OrangeColor },
            { EColorSetter.CYAN, Color.cyan },
            { EColorSetter.PURPLE, _PurpleColor },
            };

            Renderer[] lRenderers = GetComponentsInChildren<Renderer>();

            GetAllMaterials(lRenderers);
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

        private void GetAllMaterials(Renderer[] pRenderers)
        {
            foreach (Renderer r in pRenderers)
            {
                Material[] lMaths = r.materials;

                for (int i = 0; i < lMaths.Length; i++)
                {
                    lMaths[i] = new Material(lMaths[i]);
                    ApplyColorMaterial(_TargetColor, lMaths[i]);
                }

                r.materials = lMaths;
            }
        }

        private void ApplyColorMaterial(EColorSetter pCurrentColor, Material pMaterial)
        {
            if (_ColorTable.TryGetValue(pCurrentColor, out Color lColor))
                pMaterial.color = lColor;
        }

    }
}