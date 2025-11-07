using Com.IsartDigital.Rush.Manager;
using Com.IsartDigital.Rush.Ticks;
using Com.IsartDigital.Rush.Utilities;
using System;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 04/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.Cube
{
    
    public class Cube : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [Header(Utils.CUBE_MANAGEMENT)]
        [SerializeField] private float _Angle = 90f;
        [SerializeField] private LayerMask _ObstacleLayer;

        private Vector3 _FromPos, _ToPos, _CrossProduct, _PivotPoint, _Axis;
        private Vector3 _Direction = Vector3.forward;
        private Vector3 _LastDirectionBeforeFall;

        private Transform _SelfTransform;

        private const float DISTANCE_RAYCAST = 1f;

        private float _GridSize = 1f;

        private int _StopCubeTickCount = 0;
        private int _MaxTickCount = 2;

        private Quaternion _FromRotation, _ToRotation;

        public Action doAction { get; private set; }

        private ITickProvider _TickProvider;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            _SelfTransform = transform;

            _Direction = Vector3.forward;
            _Axis = Vector3.right;

            doAction = DoActionVoid;
        }

        private void Start()
        {
            _TickProvider = TickProviderLocator.Instance;
            _TickProvider.TickEvent += ReceiveTick;
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            doAction();
        }

        private void ReceiveTick()
        {
            CheckCollision();
            if (doAction == DoActionVoid) IncreaseStopTickCube();
        }

        private void SetStateVoid() => doAction = DoActionVoid;

        private void SetStateMove()
        {
            if (_Direction == Vector3.down)
                _Direction = _LastDirectionBeforeFall;
            _Direction = Vector3.forward;
            _PivotPoint = (_Direction + Vector3.down) / 2f + _SelfTransform.position; //Pivot point on the under + right of the cube
            _FromPos = _SelfTransform.position - _PivotPoint;
            _ToPos = _FromPos + _Direction * _GridSize;

            _CrossProduct = Vector3.Cross(Vector3.up, _Direction);
            _FromRotation = _SelfTransform.rotation;
            _ToRotation = Quaternion.AngleAxis(_Angle, _CrossProduct) * _FromRotation;

            doAction = DoActionMove;
        }

        private void SetStateFall()
        {
            _FromPos = transform.position;
            _Direction = Vector3.down;
            _ToPos = _FromPos + _Direction;
            doAction = DoActionFall;
        }

        private void DoActionVoid(){}

        private void DoActionMove()
        {
            _SelfTransform.position = Vector3.Slerp(_FromPos, _ToPos, _TickProvider.RatioTimeTick) + _PivotPoint;
            _SelfTransform.rotation = Quaternion.Slerp(_FromRotation, _ToRotation, _TickProvider.RatioTimeTick);
        }

        private void DoActionFall()
        {
            transform.position = Vector3.Lerp(_FromPos, _ToPos, _TickProvider.RatioTimeTick);
        }

        private void CheckCollision()
        {
            int lCollisionLayerObstacle = 1 << (int)ECollision.OBSTACLE;
            
            Ray lRayDown = new Ray(transform.position, Vector3.down);
            Ray lRayFront = new Ray(transform.position, _Direction);
            RaycastHit lHit;

            ECollision lCollisionLayer;

            GameObject lCollided;

            if (Physics.Raycast(lRayDown, out lHit, DISTANCE_RAYCAST))
            {
                lCollided = lHit.collider.gameObject;
                lCollisionLayer = (ECollision)lCollided.layer;

                switch (lCollisionLayer)
                {
                    case ECollision.ARROW:
                        SetDirection(lCollided.transform.forward);
                        SetStateMove();
                        break;
                    case ECollision.GROUND:
                        SetStateMove();
                        break;
                    case ECollision.STOP:
                        SetStateVoid();
                        break;
                    case ECollision.TELEPORTER:
                        break;
                    case ECollision.TURNSTILE:
                        break;
                    case ECollision.CONVEYORS:
                        break;
                    default:
                        break;
                }
            }
            else SetStateFall();

            if (Physics.Raycast(lRayFront, out lHit, DISTANCE_RAYCAST, lCollisionLayerObstacle)) SetStateVoid();
        }

        private void SetDirection(Vector3 pDirection)
        {
            _Direction = pDirection.normalized;
            _LastDirectionBeforeFall = _Direction;
        }

        private void IncreaseStopTickCube()
        {
            _StopCubeTickCount++;
            if(_StopCubeTickCount >= _MaxTickCount)
            {
                SetDirection(Vector3.right);
                SetStateMove();
                _StopCubeTickCount = 0;
            }
        }

        private void OnDestroy()
        {
            if (_TickProvider != null)
                _TickProvider.TickEvent -= ReceiveTick;
        }
    }
}