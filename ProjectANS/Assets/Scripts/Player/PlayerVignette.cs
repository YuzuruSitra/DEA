using UnityEngine;

namespace Player
{
    public class PlayerVignette : MonoBehaviour
    {
        [SerializeField] private Transform _waterTransform;
        private bool _isSink;
        private float _diffScale;
        
        private void Start()
        {
            _diffScale = _waterTransform.localScale.y - transform.localScale.y;
        }

        public void Update()
        {
            var diffPosY = transform.position.y - _waterTransform.position.y - _diffScale / 2.0f;
            _isSink = diffPosY <= 0;
            if (!_isSink) return;
            Debug.Log("a");
        }
    }
}
