using Com.IsartDigital.Rush.GameObjects;
using Com.IsartDigital.Rush.Manager;
using System.Collections.Generic;
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

        private List<Cube> _Cubes = new List<Cube>();

        public static CollisionManager Instance { get; private set; }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            Instance = this;
        }

        public void RegisterCube(Cube pCube)
        {
            _Cube = pCube;
            _Cubes.Add(pCube);
            pCube.collisionSignal += CheckCollision;
        }

        private void CheckCollision(Cube pCube, GameObject pObject, ECollision pCollision)
        {
            switch (pCollision)
            {
                case ECollision.ARROW:
                    pCube.SetDirection(pObject.transform.forward);
                    pCube.SetStateMove();
                    break;
                case ECollision.GROUND:
                    pCube.SetStateMove();
                    break;
                case ECollision.STOP:
                    pCube.SetStateStop();
                    break;
                case ECollision.TURNSTILE:
                    break;
                case ECollision.CONVEYORS:
                    pCube.SetDirection(pObject.transform.forward);
                    pCube.SetStateSlide();
                    break;
                case ECollision.TELEPORTER:
                    TeleportCollisionManagement(pCube, pObject.GetComponent<Teleporter>());
                    pCube.SetStateTeleport(_NextTeleporter.transform.position);
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