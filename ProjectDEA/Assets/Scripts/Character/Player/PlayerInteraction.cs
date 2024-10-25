using Gimmick;
using UnityEngine;

namespace Character.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        private bool _isInteractable = true;
        private IInteractable _currentInteractable;
        [SerializeField] private GameObject _indicationUI;
        private void Update()
        {
            if (!_isInteractable) return;
            if (_currentInteractable == null) return;
            _indicationUI.SetActive(_currentInteractable.IsInteractable);
            // インタラクションキーのチェック
            if (!Input.GetKeyDown(KeyCode.E) || !_currentInteractable.IsInteractable) return;
            _currentInteractable.Interact();
        }

        private void OnTriggerEnter(Collider other)
        {
            var interactable = other.GetComponent<IInteractable>();
            if (interactable == null) return;
            _currentInteractable = interactable;
            _currentInteractable.Destroyed += ResetCurrentTarget;
        }

        private void OnTriggerExit(Collider other)
        {
            var interactable = other.GetComponent<IInteractable>();
            if (interactable == null || _currentInteractable != interactable) return;
            ResetCurrentTarget();
        }

        private void ResetCurrentTarget()
        {
            _currentInteractable.Destroyed -= ResetCurrentTarget;
            _currentInteractable = null;
            _indicationUI.SetActive(false);
        }
        
        public void SetInteractableState(bool active)
        {
            _isInteractable = active;
        }
    }
}