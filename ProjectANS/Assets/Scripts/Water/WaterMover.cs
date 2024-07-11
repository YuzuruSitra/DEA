using System;
using UnityEngine;

namespace Water
{
    public class WaterMover : MonoBehaviour
    {
        [SerializeField] private float _yPosMin;
        [SerializeField] private float _yPosMax;
        [SerializeField] private float _upTime;
        private float _upSpeed;

        private void Start()
        {
            var pos = transform.position;
            pos.y = _yPosMin;
            transform.position = pos;
            _upSpeed = (_yPosMax - _yPosMin) / _upTime;
        }

        private void Update()
        {
            var pos = transform.position;
            if (pos.y >= _yPosMax) return;
            var newY = pos.y + _upSpeed * Time.deltaTime;
            pos.y = Math.Clamp(newY, _yPosMin, _yPosMax);
            transform.position = pos;
        }
    }
}
