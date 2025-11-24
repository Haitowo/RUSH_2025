using Com.IsartDigital.Rush.GameObjects;
using Com.IsartDigital.Rush.Manager;
using System.Collections.Generic;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 10/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.CubeManagement
{
    public class CollisionManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        private Teleporter _Teleporter;
        private Teleporter _NextTeleporter;

        private List<Cube> _Cubes = new List<Cube>();

        public static CollisionManager Instance { get; private set; }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            Instance = this;
        }

        public void RegisterCube(Cube pCube)
        {
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
                    pCube.lastDirectionBeforeFall = pCube.direction;
                    break;
                case ECollision.TURNSTILE:
                    TurnTileManagement(pCube, pObject.GetComponent<TurnTile>());
                    break;
                case ECollision.CONVEYORS:
                    pCube.SetStateSlide(pObject.transform.forward);
                    break;
                case ECollision.TELEPORTER:
                    TeleportCollisionManagement(pCube, pObject.GetComponent<Teleporter>());
                    pCube.SetStateTeleport(_NextTeleporter.transform.position);
                    break;
                case ECollision.TERRAIN:
                    //TODO : Exclamation and call game manager to reset level
                    break;
                case ECollision.TARGET:
                    TargetManagement(pCube, pObject.GetComponent<Target>());
                    break;
                case ECollision.SPAWNER:
                    pCube.SetStateMove();
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

        private void TurnTileManagement(Cube pCube, TurnTile pTurnTile)
        {
            if(pTurnTile is null) return;
            Vector3 lNewDirection = pTurnTile.GetNextDirection(pCube.direction, pCube);
            pCube.SetDirection(lNewDirection);
            pCube.lastDirectionBeforeFall = lNewDirection;
            pCube.SetStateMove();
        }

        private void SetTeleporter(Teleporter pTeleporter)
        {
            _Teleporter = pTeleporter;
            _NextTeleporter = TeleporterManager.Instance.GetNext(_Teleporter);
        }

        private void TargetManagement(Cube pCube, Target pCurrentTarget)
        {
            CheckTarget(pCube, pCurrentTarget);
            CheckDisconnectCurrentCube(pCube, pCurrentTarget);
            pCurrentTarget.DetectCubeColor(pCube);
            GameManager.Instance.CubeReachTarget(pCube);
        }

        private void CheckTarget(Cube pCube, Target pCurrentTarget)
        {
            if (pCurrentTarget.CurrentColor != pCube.cubeColor)
                pCube.SetStateMove();
            else
                pCube.SetStateStop();

        }

        private void CheckDisconnectCurrentCube(Cube pCube, Target pCurrentTarget)
        {
            if(pCube.cubeColor != pCurrentTarget.CurrentColor) return;
            pCube.collisionSignal -= CheckCollision;
        }
        
        public void DiconnectCube()
        {
            for (int i = _Cubes.Count; i > 0; i--)
            {
                if (_Cubes[i] != null)
                    _Cubes[i].collisionSignal -= CheckCollision;
            }
        }

        private void OnDestroy()
        {
            foreach (Cube lCube in _Cubes)
            {
                if (lCube != null)
                    lCube.collisionSignal -= CheckCollision;
            }
        }
    }
}