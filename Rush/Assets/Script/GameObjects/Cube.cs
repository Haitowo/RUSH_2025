using System;
using System.Collections;
using System.Collections.Generic;
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

        [SerializeField] private float _TickSpeed = 2f;
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

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            _SelfTransform = transform;
            _Direction = Vector3.forward;
            _Axis = Vector3.right;
            doAction = DoActionVoid;
            _MovementRotation = Quaternion.AngleAxis(_Angle, transform.right);
        }

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            Tick();
            doAction();
        }

        private void Tick()
        {
            if (_ElapsedTime >= _DurationBetweenTicks)
            {
                CheckCollision();
                _ElapsedTime = 0f;

                if (doAction == DoActionStopCube) IncreaseStopTickCube();
            }

            CalculateRatio();
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

        private void SetStateStopCube()
        {
            doAction = DoActionStopCube;
        }

        private void DoActionStopCube()
        {

        }

        private void DoActionVoid(){}

        private void DoActionMove()
        {
            _SelfTransform.position = Vector3.Slerp(_FromPos, _ToPos, _Ratio) + _PivotPoint;
            _SelfTransform.rotation = Quaternion.Slerp(_FromRotation, _ToRotation, _Ratio);
        }

        private void DoActionFall()
        {
            transform.position = Vector3.Lerp(_FromPos, _ToPos, _Ratio);
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
                        SetStateStopCube();
                        break;
                    case ECollision.TELEPORTER:
                        break;
                        break;
                    case ECollision.TURNSTILE:
                        break;
                    default:
                        break;
                }
            }
            else SetStateFall();

            if (Physics.Raycast(lRayFront, out lHit, DISTANCE_RAYCAST, lCollisionLayerObstacle)) SetStateStopCube();
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
    }
}