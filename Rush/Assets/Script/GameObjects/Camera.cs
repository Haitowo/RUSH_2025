using Com.IsartDigital.Rush.Utilities;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 04/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.Camera
{
    
    public class Camera : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES

        [Header(Utils.CAMERA_MANAGEMENT)]
        [SerializeField] private float _YMinAngle = -20f;
        [SerializeField] private float _YMaxAngle = 80f;
        [SerializeField] private float _ZoomSpeed = 3f;
        [SerializeField] private float _ZoomMin = 2f;
        [SerializeField] private float _ZoomMax = 20f;

        [SerializeField] private Transform _MiddlePoint;

        private float _DistanceCamera = 10f;
        private float _XAngles = 0f;
        private float _YAngles = 20f;

        private float _RotationSpeed = 5f;

        private const float MAX_ZOOM = .01f;

        private const int MOUSE_BUTTON_LEFT = 0;

        private Vector3 _BasePoint;
        private Vector3 _Angles;

        private Transform _SelfTransform;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Awake()
        {
            _SelfTransform = transform;
            _BasePoint = _MiddlePoint.transform.position;
            _Angles = _SelfTransform.eulerAngles;
            _XAngles = _Angles.x;
            _YAngles = _Angles.y;

            _SelfTransform.rotation = Quaternion.identity;
        }

        //// ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            MoveCamera();
            ZoomMouse();
        }

        private void MoveCamera()
        {
            Quaternion lRotation;
            Vector3 lDistance;
            Vector3 lPosition;

            if (Input.GetMouseButton(MOUSE_BUTTON_LEFT)) GetAxisMouseAndTouch();

            lRotation = Quaternion.Euler(_YAngles, _XAngles, 0f);
            lDistance = new Vector3(0f, 0f, -_DistanceCamera);
            lPosition = lRotation * lDistance + _BasePoint;

            transform.rotation = lRotation;
            transform.position = lPosition;
        }

        private void GetAxisMouseAndTouch()
        {
            _XAngles += Input.GetAxis(Utils.MOUSE_BUTTON_X) * _RotationSpeed;
            _YAngles += Input.GetAxis(Utils.MOUSE_BUTTON_Y) * _RotationSpeed;
            _YAngles = Mathf.Clamp(_YAngles, _YMinAngle, _YMaxAngle);
        }

        private void ZoomMouse()
        {
            float lScroll = Input.GetAxis(Utils.MOUSE_WHEEL);

            if(Mathf.Abs(lScroll) > MAX_ZOOM)
            {
                _DistanceCamera -= lScroll * _ZoomSpeed;
                _DistanceCamera = Mathf.Clamp(_DistanceCamera, _ZoomMin, _ZoomMax);
            }
        }
    }
}