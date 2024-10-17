using PostProcess;
using UnityEngine;

namespace Character.Player
{
    public class PlayerVignette : MonoBehaviour
    {
        [SerializeField] private Transform _waterTransform;
        private bool _isSink;
        private float _diffScale;
        private VignetteHandler _vignetteHandler;
        [SerializeField] private float _sinkTime;
        private float _currentTime;
        private void Start()
        {
            _vignetteHandler = VignetteHandler.Instance;
            _diffScale = _waterTransform.localScale.y - transform.localScale.y;
        }

        public void Update()
        {
            var diffPosY = transform.position.y - _waterTransform.position.y - _diffScale / 2.0f;
            _isSink = diffPosY <= 0;
            if (_isSink)
            {
                _currentTime += Time.deltaTime;
            }
            else
            {
                _currentTime -= Time.deltaTime;
            }
            _currentTime = Mathf.Clamp(_currentTime, 0, _sinkTime);
            var ratio = _currentTime / _sinkTime;
            _vignetteHandler.SetVignetteIntensity(ratio);
        }
    }
}
