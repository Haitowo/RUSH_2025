using Com.IsartDigital.Rush.CubeManagement;
using Com.IsartDigital.Rush.Manager;
using System.Collections;
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

        private bool _IsLocked;

        private Collider _Collider;

        private void Start()
        {
            _Collider = GetComponent<Collider>();
            TeleporterManager.Instance.Register(this);
        }

        private void OnTriggerEnter(Collider pOther)
        {
            if (_IsLocked) return;
            Cube lCube = pOther.GetComponent<Cube>();
            if (lCube is null || lCube.JustTeleported) return;

            if (!TeleporterManager.Instance.CanTeleport(lCube, this)) return;

            Teleporter lNextTeleporter = TeleporterManager.Instance.GetNext(this);
            if (lNextTeleporter == null) return;

            lNextTeleporter.DisableColliderTemporarily(8f);

            lCube.PrepareTeleport(lNextTeleporter.transform.position);
        }

        public void DisableColliderTemporarily(float pDuration)
        {
            StartCoroutine(DisableColliderCoroutine(pDuration));
        }

        private IEnumerator DisableColliderCoroutine(float pDuration)
        {
            if (_Collider == null) yield break;

            _Collider.enabled = false;
            yield return new WaitForSeconds(pDuration);
            _Collider.enabled = true;
        }
    }
}
