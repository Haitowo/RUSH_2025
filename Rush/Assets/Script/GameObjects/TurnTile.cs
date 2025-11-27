using Com.IsartDigital.Rush.CubeManagement;
using Com.IsartDigital.Rush.Manager;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 04/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.GameObjects
{
    public class TurnTile : MonoBehaviour
    {
        private bool _TurnRight;
        private float _Angle = 90f;

        private GameManager _GameManager => GameManager.Instance;

        private void Start() => _GameManager.activatePlayPhase += SetTurnRight;

        public Vector3 GetNextDirection(Vector3 pCurrentDirection, Cube pCube)
        {
            _TurnRight = !_TurnRight;

            float lAngle = _TurnRight ? _Angle : -_Angle;
            Quaternion lRotation = Quaternion.AngleAxis(lAngle, Vector3.up);

            return lRotation * pCurrentDirection;
        }

        private void SetTurnRight() => _TurnRight = false;
        
    }
}