using System;

namespace Gimmick
{
    public interface IInteractable
    {
        void Interact();
        event Action Destroyed;
    }
}