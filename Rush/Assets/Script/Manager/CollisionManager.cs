using Com.IsartDigital.Rush.GameObjects;
using Com.IsartDigital.Rush.Manager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 10/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.CubeManagement
{
    public class CollisionManager : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [SerializeField] private ParticleSystem _ApparitionAndTpParticles;
        [HideInInspector] public List<Cube> cubes = new List<Cube>();

        private Teleporter _CurrentTeleporter;
        private Teleporter _NextTeleporter;

        private GameManager _GameManager => GameManager.Instance;
        private TeleporterManager _TeleporterManager => TeleporterManager.Instance;

        public static CollisionManager Instance { get; private set; }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            #region Singleton Management
            if (Instance != this && Instance != null)
            {
                Destroy(this);
                Debug.LogError(nameof(CollisionManager) + "Instance already exists. Destroying the current instance.");
                return;
            }

            Instance = this;
            #endregion
        }

        public void RegisterCube(Cube pCube)
        {
            SpreadParticles(pCube);
            cubes.Add(pCube);
            pCube.collisionSignal += CheckCollision;
            pCube.onCubeColliding += OnCubeDeath;
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
                    SpreadParticles(pCube);
                    TeleportCollisionManagement(pCube, pObject.GetComponent<Teleporter>());
                    pCube.SetStateTeleport(_NextTeleporter.transform.position);
                    break;
                case ECollision.TERRAIN:
                    _GameManager.onGameLost?.Invoke();
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
            if (!_TeleporterManager.CanTeleport(pCube, _CurrentTeleporter)) return;
        }

        private void TurnTileManagement(Cube pCube, TurnTile pTurnTile)
        {
            if(pTurnTile is null) return;
            Vector3 lNewDirection = pTurnTile.GetNextDirection(pCube.lastDirectionBeforeFall, pCube);
            pCube.SetDirection(lNewDirection);
            pCube.lastDirectionBeforeFall = lNewDirection;
            pCube.SetStateMove();
        }

        private void SetTeleporter(Teleporter pTeleporter)
        {
            _CurrentTeleporter = pTeleporter;
            _NextTeleporter = _TeleporterManager.GetNext(_CurrentTeleporter);
        }

        private void TargetManagement(Cube pCube, Target pCurrentTarget)
        {
            CheckTarget(pCube, pCurrentTarget);
            CheckDisconnectCurrentCube(pCube, pCurrentTarget);
            pCurrentTarget.DetectCubeColor(pCube);
            SpreadParticles(pCube);
        }

        private void CheckTarget(Cube pCube, Target pCurrentTarget)
        {
            if (pCurrentTarget.CurrentColor != pCube.cubeColor)
                pCube.SetStateMove();
            else
                pCube.SetStateStop();
        }

        private void CheckAllCubesGone()
        {
            cubes.RemoveAll(c => c == null);

            if (cubes.Count == 0)
                _GameManager.LevelComplete();
        }

        private void CheckDisconnectCurrentCube(Cube pCube, Target pCurrentTarget)
        {
            if(pCube.cubeColor != pCurrentTarget.CurrentColor) return;
            pCube.collisionSignal -= CheckCollision;
        }
        
        public void DiconnectCube()
        {
            for (int i = cubes.Count - 1; i > 0; i--)
            {
                if (cubes[i] != null)
                    cubes[i].collisionSignal -= CheckCollision;
            }
        }

        public void RemoveCube(Cube pCube)
        {
            if (cubes.Contains(pCube))
                cubes.Remove(pCube);

            CheckAllCubesGone();
        }

        private void OnDestroy()
        {
            foreach (Cube lCube in cubes)
            {
                if (lCube != null)
                {
                    lCube.collisionSignal -= CheckCollision;
                    lCube.onCubeColliding -= OnCubeDeath;
                }
            }
        }

        private void SpreadParticles(Cube pCube)
        {
            ParticleSystem lParticles = Instantiate(_ApparitionAndTpParticles, pCube.transform.position, Quaternion.AngleAxis(-90f, Vector3.right));
            lParticles.Play();
            Destroy(lParticles.gameObject, lParticles.main.duration + lParticles.main.startLifetime.constantMax);
        }

        private void OnCubeDeath(Cube pCube) => _GameManager.onGameLost?.Invoke();
        
    }
}