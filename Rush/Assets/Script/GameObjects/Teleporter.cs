using Com.IsartDigital.Rush.Manager;
using Com.IsartDigital.Rush.TargetManagement;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 08/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.GameObjects
{
    
    public class Teleporter : ColorableTile
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private EColorSetter _TeleportColor;
        public EColorSetter CurrentColor => _TeleportColor;

        public int Index { get; set; }

        private TeleporterManager _TeleporterManager => TeleporterManager.Instance;

        private void Start()
        {
            _TeleporterManager.Register(this);

            Renderer lRend = GetComponentInChildren<Renderer>();
            m_SpawnMaterial = lRend.material;
            ApplyColorMaterial(_TeleportColor, m_SpawnMaterial);
        }
    }
}
