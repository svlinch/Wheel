using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.BezierLines;

namespace Assets.Scripts.Units
{
    public class EnemyMoveController
    {
        private BezierSpline _spline;
        private Transform _transform;
        private Transform _transform2;
        private float _progress = 0f;
        private float _speed = 0.15f;
        private float _angle = 0f;
        private float _rotSpeed = 0.5f;
        private bool _started;

        public void Setup(BezierSpline spline, Transform transform)
        {
            _spline = spline;
            _transform = transform;
            _transform2 = _transform.GetChild(0).transform;
            _transform.position = _spline.GetPoint(_progress);
        }

        public void HandleUpdate(float deltaTime)
        {
            if (_progress <= 1f)
            {
                _progress += deltaTime * _speed;
                _transform.position = _spline.GetPoint(_progress);
            }

            _angle += _rotSpeed;
            if (_angle >= 360f)
            {
                _angle = 0f;
            }
            _transform2.localRotation = Quaternion.Euler(0, -90f, _angle);
        }
    }
}