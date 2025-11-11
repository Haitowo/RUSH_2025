using Com.IsartDigital.Rush.Ticks;
using Com.IsartDigital.Rush.Utilities;
using DG.Tweening;
using System;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 04/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.CubeManagement
{
    
    public class Cube : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [Header(Utils.CUBE_MANAGEMENT)]
        [SerializeField] private float _Angle = 90f;
        [SerializeField] private LayerMask _ObstacleLayer;

        private Vector3 _FromPos, _ToPos, _CrossProduct, _PivotPoint, _Axis;
        private Vector3 _FromPosTP, _TpFinalPos;
        private Vector3 _RightAngle;
        private Vector3 _LastDirectionBeforeFall;
        public Vector3 _Direction = Vector3.forward;

        private Transform _SelfTransform;

        private const float DISTANCE_RAYCAST = 1f;
        private const float TELEPORT_DECAY = .5f;
        private const float TELEPORT_TIME_SCALE = .2f;

        private float _GridSize = 1f;

        private int _StopCubeTickCount = 0;
        private int _TeleportationTickCount = 0;

        private const int MAX_TICK_COUNT = 2;

        private Quaternion _FromRotation, _ToRotation;

        public Action doAction { get; private set; }
        public Action<Cube, GameObject, ECollision> collisionSignal;

        private ITickProvider _TickProvider;

        public bool JustTeleported {  get; private set; }
        private bool _IsStop;
        private bool _IsTeleporting;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            _SelfTransform = transform;

            _Axis = Vector3.right;
            _RightAngle = Quaternion.AngleAxis(_Angle, Vector3.up) * _Direction;
            _Direction = _SelfTransform.forward;

            SetStateMove();
        }

        private void Start()
        {
            _TickProvider = TickProviderLocator.Instance;
            _TickProvider.TickEvent += ReceiveTick;

            SetDirection(_SelfTransform.forward);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            doAction();
        }

        private void ReceiveTick()
        {
            if (CheckCurrentTeleportation()) return;
            CheckCollision();
            if (doAction == DoActionVoid || doAction == DoActionStop) IncreaseStopTickCube();
        }

        public void SetStateStop() => doAction = DoActionStop;

        public void SetStateVoid() => doAction = DoActionVoid;

        public void SetStateMove()
        {
            if (_Direction == Vector3.down)
                _Direction = _LastDirectionBeforeFall;
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
            _FromPos = _SelfTransform.position;
            _Direction = Vector3.down;
            _ToPos = _FromPos + _Direction;
            doAction = DoActionFall;
        }

        public void SetStateSlide()
        {
            if (_Direction == Vector3.down)
                _Direction = _LastDirectionBeforeFall;

            _FromPos = _SelfTransform.position;
            _ToPos = _FromPos + _Direction * _GridSize;

            doAction = DoActionSlide;
        }

        public void SetStateTeleport(Vector3 pFinalPos)
        {
            _IsTeleporting = true;
            _TeleportationTickCount = 0;
            _FromPos = _SelfTransform.position;
            _TpFinalPos = pFinalPos + Vector3.up * TELEPORT_DECAY;
            doAction = DoActionTeleport;
        }

        private void DoActionStop() => _IsStop = true;

        private void DoActionVoid() => _IsStop = false;

        private void DoActionMove()
        {
            _SelfTransform.position = Vector3.Slerp(_FromPos, _ToPos, _TickProvider.RatioTimeTick) + _PivotPoint;
            _SelfTransform.rotation = Quaternion.Slerp(_FromRotation, _ToRotation, _TickProvider.RatioTimeTick);
        }

        private void DoActionFall() => _SelfTransform.position = Vector3.Lerp(_FromPos, _ToPos, _TickProvider.RatioTimeTick);
        
        private void DoActionSlide() => _SelfTransform.position = Vector3.Lerp(_FromPos, _ToPos, _TickProvider.RatioTimeTick);

        private void DoActionTeleport() => _SelfTransform.DOScale(Vector3.zero, TELEPORT_TIME_SCALE);

        private void CheckCollision()
        {
            int lCollisionLayerObstacle = 1 << (int)ECollision.GROUND;
            
            Ray lRayDown = new Ray(transform.position, Vector3.down);
            Ray lRayFront = new Ray(transform.position, _Direction);
            RaycastHit lHit;

            ECollision lCollisionLayer;

            GameObject lCollided;

            if (Physics.Raycast(lRayDown, out lHit, DISTANCE_RAYCAST))
            {
                lCollided = lHit.collider.gameObject;
                lCollisionLayer = (ECollision)lCollided.layer;
                collisionSignal?.Invoke(this, lCollided, lCollisionLayer);
            }
            else SetStateFall();

            if (Physics.Raycast(lRayFront, out lHit, DISTANCE_RAYCAST, lCollisionLayerObstacle)) SetStateVoid();
        }

        private bool CheckCurrentTeleportation()
        {
            if (_IsTeleporting)
            {
                IncreaseTickTeleport();
                return true;
            }
            else return false;
        }

        private void EndTeleport()
        {
            _SelfTransform.position = _TpFinalPos;
            _SelfTransform.DOScale(Vector3.one, .2f * _TickProvider.TickSpeed);

            ResetAllValues();

            if (_LastDirectionBeforeFall != Vector3.zero)
                SetDirection(_LastDirectionBeforeFall);
            else
                SetDirection(Vector3.forward);

            SetStateMove();
        }

        public void SetDirection(Vector3 pDirection)
        {
            _Direction = pDirection.normalized;
            _RightAngle = Quaternion.AngleAxis(_Angle, Vector3.up) * _Direction;
            _LastDirectionBeforeFall = _Direction;
        }

        private void IncreaseStopTickCube()
        {
            _StopCubeTickCount++;
            if (_StopCubeTickCount >= MAX_TICK_COUNT && !_IsStop)
                CanMoveToDirection(_RightAngle);
            else if(_StopCubeTickCount >= MAX_TICK_COUNT && _IsStop) 
                CanMoveToDirection(_Direction);
        }

        private void IncreaseTickTeleport()
        {
            _TeleportationTickCount++;
            if (_TeleportationTickCount >= MAX_TICK_COUNT)
                EndTeleport();
        }

        private void CanMoveToDirection(Vector3 pDirection)
        {
            SetDirection(pDirection);
            SetStateMove();
            _StopCubeTickCount = 0;
        }

        private void ResetAllValues()
        {
            _IsTeleporting = false;
            JustTeleported = true;
            _PivotPoint = Vector3.zero;
            _FromPos = _TpFinalPos;
            _ToPos = _TpFinalPos;
            _Direction = _SelfTransform.forward.normalized;
            _FromRotation = _SelfTransform.rotation;
            _ToRotation = _SelfTransform.rotation;
        }

        private void OnDestroy()
        {
            if (_TickProvider != null)
                _TickProvider.TickEvent -= ReceiveTick;
        }
    }
}