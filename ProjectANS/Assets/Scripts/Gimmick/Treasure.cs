using UnityEngine;

namespace Gimmick
{
    public class Treasure : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            Debug.Log("宝箱を発見!!!!");
        }
    }
}
