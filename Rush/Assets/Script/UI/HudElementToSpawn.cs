using Com.IsartDigital.Rush.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.UI
{
    public class HUDElementToSpawn : MonoBehaviour
    {
        public RectTransform objectToTransform;
        public EHUDElementPos pos;
        public Vector3 currentPos;
        public bool ignoreHide;
    }
}