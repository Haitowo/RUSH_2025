using Com.IsartDigital.Rush.GameObjects;
using Com.IsartDigital.Rush.Manager;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 00/00/0000 - Beginning of the class

namespace Com.IsartDigital.Rush.CubeManagement
{

    public class CollisionManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        private Cube _Cube;
        private Teleporter _Teleporter;
        private Teleporter _NextTeleporter;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            _Cube.collisionSignal += CheckCollision;
        }

        private void CheckCollision(GameObject pObject, ECollision pCollision)
        {
            switch (pCollision)
            {
                case ECollision.ARROW:
                    _Cube.SetDirection(pObject.transform.forward);
                    _Cube.SetStateMove();
                    break;
                case ECollision.GROUND:
                    _Cube.SetStateMove();
                    break;
                case ECollision.STOP:
                    _Cube.SetStateStop();
                    break;
                case ECollision.TURNSTILE:
                    break;
                case ECollision.CONVEYORS:
                    _Cube.SetDirection(pObject.transform.forward);
                    _Cube.SetStateSlide();
                    break;
                case ECollision.TELEPORTER:
                    TeleportCollisionManagement(_Cube, pObject.GetComponent<Teleporter>());
                    _Cube.PrepareTeleport(_NextTeleporter.transform.position);
                    break;
                default:
                    break;
            }
        }

        private void TeleportCollisionManagement(Cube pCube, Teleporter pTeleporter)
        {
            if (pCube is null) return;
            SetTeleporter(pTeleporter);
            if (!TeleporterManager.Instance.CanTeleport(pCube, _Teleporter)) return;
        }

        private void SetTeleporter(Teleporter pTeleporter)
        {
            _Teleporter = pTeleporter;
            _NextTeleporter = TeleporterManager.Instance.GetNext(_Teleporter);
        }

    }
}