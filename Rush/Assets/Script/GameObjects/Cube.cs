using Com.IsartDigital.Rush.GameObjects;
using Com.IsartDigital.Rush.Ticks;
using Com.IsartDigital.Rush.Utilities;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UIElements;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 04/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.CubeManagement
{
    
    public class Cube : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [Header(Utils.CUBE_MANAGEMENT)]
        [SerializeField] private float _Angle = 90f;
        [SerializeField] private float _UturnAngle = 180f;
        [SerializeField] private LayerMask _ObstacleLayer;

        private Vector3 _FromPos, _ToPos, _CrossProduct, _PivotPoint, _SlideDirection;
        private Vector3 _TpFinalPos;
        public Vector3 lastDirectionBeforeFall;
        public Vector3 direction = Vector3.forward;

        private Transform _SelfTransform;

        private const float DISTANCE_RAYCAST = 1f;
        private const float TELEPORT_DECAY = .5f;
        private const float TWEEN_TIME_SCALE = .2f;

        private float _GridSize = 1f;

        private int _StopCubeTickCount = 0;
        private int _TeleportationTickCount = 0;

        private const int MAX_TICK_COUNT = 2;

        private Quaternion _FromRotation, _ToRotation;

        public Action doAction { get; private set; }
        public Action<Cube, GameObject, ECollision> collisionSignal;

        public EColorSetter cubeColor;
        private ITickProvider _TickProvider;

        private bool _IsStop;
        private bool _IsTeleporting;
        private bool _JustTeleported;
        private bool _IsFalling;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            _SelfTransform = transform;

            direction = _SelfTransform.forward;

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
            if (direction == Vector3.down)
                direction = lastDirectionBeforeFall;
            _IsFalling = false;
            _JustTeleported = false;
            _PivotPoint = (direction + Vector3.down) / 2f + _SelfTransform.position; //Pivot point on the under + right of the cube
            _FromPos = _SelfTransform.position - _PivotPoint;
            _ToPos = _FromPos + direction * _GridSize;

            _CrossProduct = Vector3.Cross(Vector3.up, direction);
            _FromRotation = _SelfTransform.rotation;
            _ToRotation = Quaternion.AngleAxis(_Angle, _CrossProduct) * _FromRotation;
            lastDirectionBeforeFall = direction;

            doAction = DoActionMove;
        }

        private void SetStateFall()
        {
            _IsFalling = true;
            _FromPos = _SelfTransform.position;
            direction = Vector3.down;
            _ToPos = _FromPos + direction;
            doAction = DoActionFall;
        }

        public void SetStateSlide(Vector3 pSlideDirection)
        {
            _SlideDirection = pSlideDirection.normalized;

            _FromPos = _SelfTransform.position;
            _ToPos = _FromPos + _SlideDirection * _GridSize;
            lastDirectionBeforeFall = _SlideDirection;

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

        private void DoActionTeleport()
        {
            _SelfTransform.position = Vector3.Lerp(_FromPos, _TpFinalPos, _TickProvider.RatioTimeTick);
            _SelfTransform.DOScale(Vector3.zero, TWEEN_TIME_SCALE / _TickProvider.TickSpeed);
        }

        private void CheckCollision()
        {
            if (_JustTeleported) return;
            int lCollisionLayerObstacle = 1 << (int)ECollision.GROUND;
            
            Ray lRayDown = new Ray(_SelfTransform.position, Vector3.down);
            Ray lRayFront = new Ray(_SelfTransform.position, lastDirectionBeforeFall);
            RaycastHit lHit;

            ECollision lCollisionLayer;

            GameObject lCollided;

            if (Physics.Raycast(lRayDown, out lHit, DISTANCE_RAYCAST) && !_JustTeleported)
            {
                lCollided = lHit.collider.gameObject;
                lCollisionLayer = (ECollision)lCollided.layer;
                collisionSignal?.Invoke(this, lCollided, lCollisionLayer);
            }
            else SetStateFall();

            if (Physics.Raycast(lRayFront, out lHit, DISTANCE_RAYCAST, lCollisionLayerObstacle) && doAction != DoActionSlide && !_IsFalling && !_IsTeleporting)
                SetStateVoid();
        }

        private void IncreaseTickTeleport()
        {
            _TeleportationTickCount++;
            if (_TeleportationTickCount >= MAX_TICK_COUNT)
                EndTeleport();
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
            int lCollisionLayerObstacle = 1 << (int)ECollision.GROUND;
            Ray lRayFront = new Ray(_SelfTransform.position, lastDirectionBeforeFall);
            RaycastHit lHit;
            _JustTeleported = true;

            _SelfTransform.DOScale(Vector3.one, TWEEN_TIME_SCALE / _TickProvider.TickSpeed);

            ResetAllValues();

            if (lastDirectionBeforeFall != Vector3.zero)
                SetDirection(lastDirectionBeforeFall);
            else
                SetDirection(Vector3.forward);

            if (Physics.Raycast(lRayFront, out lHit, DISTANCE_RAYCAST, lCollisionLayerObstacle) && _JustTeleported) SetStateVoid();
            else
            {
                _JustTeleported = false;
                SetStateMove();
            }
        }

        public void SetDirection(Vector3 pDirection)
        {
            direction = pDirection.normalized;
            lastDirectionBeforeFall = direction;
        }

        private void CanMoveToDirection(Vector3 pDirection)
        {
            Vector3 lDirectionToCheck = Quaternion.AngleAxis(_Angle, Vector3.up) * pDirection;
            bool lIsObstacleOnTheRight = HasObstacle(lDirectionToCheck);
            float lAngleToUse = lIsObstacleOnTheRight ? _UturnAngle : _Angle;
            Vector3 lNewDirection = Quaternion.AngleAxis(lAngleToUse, Vector3.up) * pDirection;

            SetDirection(lNewDirection);
            SetStateMove();

            _StopCubeTickCount = 0;
        }

        private bool HasObstacle(Vector3 pDirection) 
        { 
            Ray lRay = new Ray(_SelfTransform.position, pDirection); 
            return Physics.Raycast(lRay, DISTANCE_RAYCAST, _ObstacleLayer); 
        }

        private void IncreaseStopTickCube()
        {
            _StopCubeTickCount++;
            if (_StopCubeTickCount >= MAX_TICK_COUNT && _IsStop)
                MoveInFront();
            else if (_StopCubeTickCount >= MAX_TICK_COUNT && !_IsStop)
                CanMoveToDirection(direction);
        }

        private void MoveInFront()
        {
            SetDirection(direction);
            SetStateMove();
        }

        private void ResetAllValues()
        {
            _IsTeleporting = false;
            _PivotPoint = _SelfTransform.position;
            _FromPos = _TpFinalPos;
            _ToPos = _TpFinalPos;
            direction = _SelfTransform.forward.normalized;
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