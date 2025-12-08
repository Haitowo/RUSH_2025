using Com.IsartDigital.Rush.GameObjects;
using Com.IsartDigital.Rush.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.TargetManagement
{
    
    public class ColorableTile : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLE
        [Header(Utils.COLORS_MANAGEMENT)]
        [SerializeField] protected Color m_OrangeColor;
        [SerializeField] protected Color m_PurpleColor;
        [SerializeField] protected Color m_BlueColor;
        [SerializeField] protected Color m_CyanColor;
        [SerializeField] protected Color m_RedColor;

        protected Dictionary<EColorSetter, Color> m_ColorTable;
        protected Material m_SpawnMaterial;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        protected void Awake()
        {
            m_ColorTable = new Dictionary<EColorSetter, Color>(){
            { EColorSetter.RED, m_RedColor },
            { EColorSetter.BLUE, m_BlueColor },
            { EColorSetter.GREEN, Color.green },
            { EColorSetter.ORANGE, m_OrangeColor },
            { EColorSetter.CYAN, Color.cyan },
            { EColorSetter.PURPLE, m_PurpleColor },
            };
        }

        protected void GetAllMaterials(Renderer[] pRenderers, EColorSetter pColorToSet)
        {
            foreach (Renderer r in pRenderers)
            {
                Material[] lMaths = r.materials;

                for (int i = 0; i < lMaths.Length; i++)
                {
                    lMaths[i] = new Material(lMaths[i]);
                    ApplyColorMaterial(pColorToSet, lMaths[i]);
                }

                r.materials = lMaths;
            }
        }

        protected void ApplyColorMaterial(EColorSetter pCurrentColor, Material pMaterial)
        {
            if (m_ColorTable.TryGetValue(pCurrentColor, out Color lColor))
                pMaterial.color = lColor;
        }
    }
}