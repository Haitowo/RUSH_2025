using Com.IsartDigital.Rush.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 04/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.Cube
{
    
    public class Cube : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES
        [Range(0.1f, 5f)]
        [SerializeField] private float _Speed = 1f;
        [SerializeField] private float _RaycastOffset = .4f;

        [SerializeField] private string _GroundTag = "Ground";
        [SerializeField] private string _ArrowTag = "Arrow";

        [SerializeField] private float _Angle = 90f;
        [SerializeField] private LayerMask _ObstacleLayer;

        private Vector3 _FromPos, _ToPos, _CrossProduct, _PivotPoint, _Axis;
        private Vector3 _Direction = Vector3.forward;

        private Transform _SelfTransform;

        private const float DISTANCE_RAYCAST = 1f;

        private float _ElapsedTime = 0f;
        private float _DurationBetweenTicks = 1f;
        private float _Ratio = 0f;
        private float _GridSize = 1f;

        private int _StopCubeTickCount = 0;
        private int _MaxTickCount = 2;

        private Quaternion _FromRotation, _ToRotation, _MovementRotation;

        public Action doAction {  get; private set; }

        private GameManager _GameManager;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            _GameManager = GameManager.Instance;
            if (_GameManager == null)
            {
                Debug.LogError("GameManager.Instance is null in Cube!");
                return;
            }

            _GameManager.tickAction += ReceiveTick;
            _SelfTransform = transform;

            _Direction = Vector3.forward;
            _Axis = Vector3.right;
            doAction = DoActionVoid;
            _MovementRotation = Quaternion.AngleAxis(_Angle, transform.right);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            //Tick();
            doAction();
        }

        private void Tick()
        {
            if (_ElapsedTime >= _DurationBetweenTicks)
            {
                CheckCollision();
                _ElapsedTime = 0f;

                if (doAction == DoActionVoid) IncreaseStopTickCube();
            }

            CalculateRatio();
        }

        private void ReceiveTick()
        {
            CheckCollision();
            if (doAction == DoActionVoid) IncreaseStopTickCube();
        }

        private void SetStateVoid() => doAction = DoActionVoid;

        private void SetStateMove()
        {
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
            _SelfTransform.position = Vector3.Slerp(_FromPos, _ToPos, _GameManager.ratio) + _PivotPoint;
            _SelfTransform.rotation = Quaternion.Slerp(_FromRotation, _ToRotation, _GameManager.ratio);
        }

        private void DoActionFall()
        {
            transform.position = Vector3.Lerp(_FromPos, _ToPos, _GameManager.ratio);
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
                Debug.Log(lCollided.layer);

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

        private void CalculateRatio()
        {
            _ElapsedTime += Time.deltaTime * _Speed;
            _Ratio = _ElapsedTime / _DurationBetweenTicks;
        }

        private void SetDirection(Vector3 pDirection)
        {
            _Direction = pDirection;
            _MovementRotation = Quaternion.AngleAxis(_Angle, Vector3.Cross(Vector3.up, pDirection));
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
            if (_GameManager != null)
                _GameManager.tickAction -= ReceiveTick;
        }
    }
}