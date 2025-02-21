using System;
using UnityEngine;

namespace Manager
{
    public class TutorialArea1 : MonoBehaviour
    {
        public bool IsReaching {get; set;}

        private void Start()
        {
            IsReaching = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                IsReaching = true;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                IsReaching = false;
            }
        }
    }
}
