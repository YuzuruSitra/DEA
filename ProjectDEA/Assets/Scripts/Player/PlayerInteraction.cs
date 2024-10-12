using System;
using Gimmick;
using UnityEngine;

namespace Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        private IInteractable _currentInteractable;
        private void Update()
        {
            // インタラクションキーのチェック
            if (!Input.GetKeyDown(KeyCode.E) || _currentInteractable == null) return;
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
        }
    }
}