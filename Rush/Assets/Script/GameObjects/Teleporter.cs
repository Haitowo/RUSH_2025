using Com.IsartDigital.Rush.Manager;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 08/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.GameObjects
{
    
    public class Teleporter : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private ETeleportColor _TeleportColor;
        public ETeleportColor CurrentColor => _TeleportColor;

        public int Index { get; set; }

        private void Start()
        {
            TeleporterManager.Instance.Register(this);
        }
    }
}
