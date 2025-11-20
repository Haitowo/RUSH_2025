using Com.IsartDigital.Rush.Manager;
using Com.IsartDigital.Rush.Utilities;
using UnityEngine;
using UnityEngine.EventSystems;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 04/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.CameraManagement
{
    public class CameraGame : MonoBehaviour
    {
        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // VARIABLES

        [Header(Utils.CAMERA_MANAGEMENT)]
        [SerializeField] private float _YMinAngle = -20f;
        [SerializeField] private float _YMaxAngle = 80f;
        [SerializeField] private float _ZoomSpeed = 3f;
        [SerializeField] private float _ZoomMin = 2f;
        [SerializeField] private float _ZoomMax = 20f;

        [SerializeField] public Transform middlePoint;

        public float distanceCamera = 10f;
        private float _XAngles = 0f;
        private float _YAngles = 20f;
        private float _RotationSpeed = 5f;

        private const float MAX_ZOOM = .01f;

        private const int MOUSE_BUTTON_LEFT = 0;

        private Vector3 _BasePoint;
        private Vector3 _Angles;

        private Transform _SelfTransform;
        private GameManager _GameManager;

        // ----------------~~~~~~~~~~~~~~~~~~~==========================# // READY
        private void Start()
        {
            enabled = false;
            _SelfTransform = transform;
            _GameManager = GameManager.Instance;

            _GameManager.SwitchToGame += EnableCameraForGame;
            _GameManager.BackToMenu += DisableCameraForGame;
        }

        //// ----------------~~~~~~~~~~~~~~~~~~~==========================# // PROCESS
        private void Update()
        {
            if (PointerOverUI()) return;

            MoveCamera();
            ZoomMouse();
        }

        private bool PointerOverUI()
        {
            #if UNITY_EDITOR || UNITY_STANDALONE
                        return EventSystem.current.IsPointerOverGameObject();
            #elif UNITY_ANDROID || UNITY_IOS
                if (Input.touchCount > 0)
                    return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            
                return false;
            #else
                return false;
            #endif
        }

        private void MoveCamera()
        {
            Quaternion lRotation;
            Vector3 lDistance;
            Vector3 lPosition;

            if (Input.GetMouseButton(MOUSE_BUTTON_LEFT)) GetAxisMouseAndTouch();

            lRotation = Quaternion.Euler(_YAngles, _XAngles, 0f);
            lDistance = new Vector3(0f, 0f, -distanceCamera);
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
                distanceCamera -= lScroll * _ZoomSpeed;
                distanceCamera = Mathf.Clamp(distanceCamera, _ZoomMin, _ZoomMax);
            }
        }

        private void EnableCameraForGame(bool pEnableCamera) => enabled = pEnableCamera;

        private void DisableCameraForGame(bool pEnableCamera) => enabled = pEnableCamera;

        public void SetStartTransform(Vector3 pPosition, Quaternion pRotation, float pDistance, Vector3 pBasePoint)
        {
            _SelfTransform.position = pPosition;
            _SelfTransform.rotation = pRotation;

            Vector3 pEuler = pRotation.eulerAngles;
            _XAngles = pEuler.y;
            _YAngles = pEuler.x;

            _BasePoint = pBasePoint;
            distanceCamera = pDistance;
        }
    }
}