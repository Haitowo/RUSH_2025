using Com.IsartDigital.Rush.CubeManagement;
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

            Debug.Log($"TP Name : {name} with Index : {Index} with the color : {CurrentColor}");
        }

        private void Update()
        {
            
        }

        private void OnTriggerEnter(Collider pOther)
        {
            Cube lCube = pOther.GetComponent<Cube>();
            if (lCube is null || lCube.JustTeleported) return;

            Teleporter lNextTeleporter = TeleporterManager.Instance.GetNext(this);
            if (lNextTeleporter == null) return;

            lCube.PrepareTeleport(lNextTeleporter.transform.position);
        }
    }
}
