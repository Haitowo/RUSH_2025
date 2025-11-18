using Com.IsartDigital.Rush.CubeManagement;
using UnityEngine;

// Author : Florian MAJCHER - Isart DIGITAL
// DATE : 04/11/2025 - Beginning of the class

namespace Com.IsartDigital.Rush.GameObjects
{
    public class TurnTile : PlaceableTiles
    {
        private bool _TurnRight;
        private float _Angle = 90f;

        public Vector3 GetNextDirection(Vector3 pCurrentDirection, Cube pCube)
        {
            _TurnRight = !_TurnRight;

            float lAngle = _TurnRight ? _Angle : -_Angle;
            Quaternion lRotation = Quaternion.AngleAxis(lAngle, Vector3.up);

            return lRotation * pCurrentDirection;
        }
    }
}