using System.Collections;
using UnityEngine;

namespace Gimmick
{
    public class Door : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform _parent;
        private bool _isOpen;
        private Quaternion _closedRotation;
        private Quaternion _openRotation;
        [SerializeField] private float _rotationDuration = 1.0f;
        private bool _isRotating;
        [SerializeField] private GameObject _hiddenArea;
        private Material _hiddenMaterial;
        private Color _hiddenMaterialColor;
        
        private void Start()
        {
            _closedRotation = _parent.rotation;
            _openRotation = _closedRotation * Quaternion.Euler(0, -90, 0);
            _hiddenMaterial = _hiddenArea.GetComponent<Renderer>().material;
            _hiddenMaterialColor = _hiddenMaterial.color;
        }

        public void Interact()
        {
            if (_isRotating) return;

            if (_isOpen)
            {
                StartCoroutine(RotateDoor(_closedRotation));
                StartCoroutine(FadeMaterialAlpha(1f));
            }
            else
            {
                StartCoroutine(RotateDoor(_openRotation));
                StartCoroutine(FadeMaterialAlpha(0f));
            }

            _isOpen = !_isOpen;
        }

        private IEnumerator RotateDoor(Quaternion toRotation)
        {
            _isRotating = true;
            float elapsedTime = 0;
            var startingRotation = _parent.rotation;

            while (elapsedTime < _rotationDuration)
            {
                _parent.rotation = Quaternion.Slerp(startingRotation, toRotation, elapsedTime / _rotationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _parent.rotation = toRotation;
            _isRotating = false;
        }

        private IEnumerator FadeMaterialAlpha(float targetAlpha)
        {
            float elapsedTime = 0;
            var currentColor = _hiddenMaterialColor;
            var startAlpha = currentColor.a;

            while (elapsedTime < _rotationDuration)
            {
                var newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / _rotationDuration);
                _hiddenMaterialColor.a = newAlpha;
                _hiddenMaterial.color = _hiddenMaterialColor;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            _hiddenMaterialColor.a = targetAlpha;
            _hiddenMaterial.color = _hiddenMaterialColor;
        }
    }
}
