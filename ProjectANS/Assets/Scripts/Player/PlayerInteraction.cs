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
        }

        private void OnTriggerExit(Collider other)
        {
            var interactable = other.GetComponent<IInteractable>();
            if (interactable == null || _currentInteractable != interactable) return;
            _currentInteractable = null;
        }
    }
}