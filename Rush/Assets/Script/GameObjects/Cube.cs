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
        private Vector3 _SquashAndStretchScale;
        public Vector3 lastDirectionBeforeFall;
        public Vector3 direction = Vector3.forward;

        private Transform _SelfTransform;

        private float DISTANCE_RAYCAST = 1f;
        private const float TELEPORT_DECAY = .5f;
        private const float TWEEN_TIME = .2f;
        private const float SCALE_X_Z = 1.2f;
        private const float SCALE_Y = .8f;

        private float _GridSize = 1f;

        private int _SlideTickCount = 0;
        private int _SlideWaitTickCount = 0;
        private int _CheckTickCount = 0;
        private int _StopCubeTickCount = 0;
        private int _TeleportationTickCount = 0;

        private const int SLIDE_TICKS = 1;
        private const int SLIDE_WAIT_DURATION = 2;
        private const int STOP_TICK_COUNT = 2;
        private const int WALL_HIT_STOP_TICK_COUNT = 3;
        private const int NUMBER_OF_JUMPS = 1;

        private Quaternion _FromRotation, _ToRotation;

        public Action doAction { get; private set; }
        public Action<Cube, GameObject, ECollision> collisionSignal;
        public Action<Cube> onCubeColliding;

        public EColorSetter cubeColor;
        private ITickProvider _TickProvider;

        private bool _IsStop;
        private bool _IsWallAfterStop;
        private bool _IsTeleporting;
        private bool _JustTeleported;
        private bool _IsFalling;
        private bool _IsSliding;
        private bool _IsCubeJustSpawned;

        private Tween squashStretchTween;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            _SelfTransform = transform;

            direction = _SelfTransform.forward;
            _SquashAndStretchScale = new Vector3(SCALE_X_Z, SCALE_Y, SCALE_X_Z);
        }

        private void Start()
        {
            _TickProvider = TickProviderLocator.Instance;
            _TickProvider.tickEvent += ReceiveTick;
            _IsCubeJustSpawned = true;

            SetStateMove();
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            doAction();
        }

        private void ReceiveTick()
        {
            if (CheckCurrentTeleportation()) return;
            else if (_IsSliding)
            {
                WaitForSlideToEnd();
                return;
            }
            CheckCollision();
            if (doAction == DoActionVoid || doAction == DoActionStop) IncreaseStopTickCube();
        }

        public void SetStateStop()
        {
            _IsWallAfterStop = CheckFrontAfterStop();
            _CheckTickCount = _IsWallAfterStop ? WALL_HIT_STOP_TICK_COUNT : STOP_TICK_COUNT;
            _IsStop = true;
            doAction = DoActionStop;
        }

        public void SetStateVoid()
        {
            doAction = DoActionVoid;
        }

        public void SetStateMove()
        {
            if (direction == Vector3.down)
                direction = lastDirectionBeforeFall;

            PlaySquashStretch(_SquashAndStretchScale, TWEEN_TIME);
            _IsCubeJustSpawned = false;
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
            _SlideTickCount = 0;
            _SlideDirection = pSlideDirection.normalized;

            _FromPos = _SelfTransform.position;
            _ToPos = _FromPos + _SlideDirection * _GridSize;
            _IsSliding = true;

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

        public void SetStateSlideWait()
        {
            _SlideWaitTickCount = 0;
            doAction = DoActionSlideWait;
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
            _SelfTransform.DOScale(Vector3.zero, TWEEN_TIME / _TickProvider.TickSpeed);
        }

        private void DoActionSlideWait()
        {
            _SlideWaitTickCount++;

            if (_SlideWaitTickCount >= SLIDE_WAIT_DURATION)
            {
                CheckCollision();
                _SlideWaitTickCount = 0;
            }
        }

        private void CheckCollision()
        {
            if (_JustTeleported || _IsSliding) return;

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

            if (Physics.Raycast(lRayFront, out lHit, DISTANCE_RAYCAST, _ObstacleLayer) && !_IsSliding && !_IsFalling && !_IsTeleporting)
                SetStateVoid();
        }

        private void IncreaseTickTeleport()
        {
            _TeleportationTickCount++;
            if (_TeleportationTickCount >= STOP_TICK_COUNT) EndTeleport();
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
            Ray lRayFront = new Ray(_SelfTransform.position, lastDirectionBeforeFall);
            RaycastHit lHit;
            _JustTeleported = true;

            _SelfTransform.DOScale(Vector3.one, TWEEN_TIME / _TickProvider.TickSpeed);

            ResetAllValues();

            if (lastDirectionBeforeFall != Vector3.zero) SetDirection(lastDirectionBeforeFall);
            else SetDirection(Vector3.forward);

            if (Physics.Raycast(lRayFront, out lHit, DISTANCE_RAYCAST, _ObstacleLayer) && _JustTeleported) SetStateVoid();
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
            if (_IsCubeJustSpawned)
            {
                SetStateMove();
                return;
            }
            else if (_StopCubeTickCount >= _CheckTickCount && _IsStop)
                MoveInFront();
            else if (_StopCubeTickCount >= STOP_TICK_COUNT && !_IsStop)
                CanMoveToDirection(direction);
            else _StopCubeTickCount++;
        }

        private void MoveInFront()
        {
            bool lWasWallAndStop = _IsWallAfterStop;
            _StopCubeTickCount = 0;
            _IsWallAfterStop = false;
            _IsStop = false;
            if (lWasWallAndStop)
            {
                CanMoveToDirection(direction);
                return;
            }
            SetStateMove();
            SetDirection(direction);
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

        private bool CheckFrontAfterStop()
        {
            Ray lRayFront = new Ray(_SelfTransform.position, direction);
            RaycastHit lHit;

            if (Physics.Raycast(lRayFront, out lHit, DISTANCE_RAYCAST, _ObstacleLayer)) return true;
            else return false;
        }

        private void WaitForSlideToEnd()
        {
            SetStateVoid();
            if (_SlideTickCount >= SLIDE_TICKS)
                EndSlide();
            else _SlideTickCount++;
        }

        private void EndSlide()
        {
            _IsSliding = false;
            _FromPos = _SelfTransform.position;
            _ToPos = _FromPos;
            _PivotPoint = _SelfTransform.position;
            SetStateSlideWait();
            _SlideTickCount = 0;
        }

        private void OnDestroy()
        {
            if (_TickProvider != null)
                _TickProvider.tickEvent -= ReceiveTick;
            onCubeColliding = null;
        }

        private void OnTriggerEnter(Collider pOther)
        {
            if (pOther.CompareTag(Utils.TAG_CUBE)) onCubeColliding?.Invoke(this);
        }


        private void PlaySquashStretch(Vector3 pTargetScale, float pDuration)
        {
            if (squashStretchTween != null && squashStretchTween.IsActive())
                squashStretchTween.Kill();

            squashStretchTween = _SelfTransform.DOScale(pTargetScale, pDuration * 0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _SelfTransform.DOScale(Vector3.one, pDuration * 0.5f)
                        .SetEase(Ease.InQuad);
                });
        }

    }
}