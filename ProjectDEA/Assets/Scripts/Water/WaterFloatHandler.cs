using System;
using System.Collections.Generic;
using UnityEngine;

namespace Water
{
    public class WaterFloatHandler : MonoBehaviour
    {
        [SerializeField] private float _terrainY;
        private WaterMover _waterMover;
        private int _fieldObjLayer;
        private int _otherLayer;
        private float _waterHalfHeight;
        private float _floatMax;
        private readonly List<Transform> _objList = new();
        
        private void Start()
        {
            _fieldObjLayer = LayerMask.NameToLayer("FieldObj");
            _otherLayer = LayerMask.NameToLayer("Other");
            _waterMover = GetComponent<WaterMover>();
            _waterHalfHeight = transform.localScale.y / 2.0f;
            _floatMax = _waterMover.YPosMax + _waterHalfHeight - 0.1f;
        }

        private void LateUpdate()
        {
            if (_objList.Count == 0) return;
            var waterSurfaceY = transform.position.y + _waterHalfHeight;

            foreach (var t in _objList)
            {
                var objHalfHeight = t.localScale.y / 2.0f;
                var objFloatMax = _floatMax - objHalfHeight;
                var objFloatMin = _terrainY + objHalfHeight;
                var newY = Math.Clamp(waterSurfaceY, objFloatMin, objFloatMax);
                var objPos = t.position;
                objPos.y = newY;
                t.position = objPos;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == _fieldObjLayer) return;
            if (other.gameObject.layer == _otherLayer) return;
            _objList.Add(other.transform);
        }
    }
}