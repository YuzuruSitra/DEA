using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;

namespace Manager.Cam.PostProcess
{
    public class ChromaticAberrationOscillator : MonoBehaviour
    {
        [FormerlySerializedAs("postProcessVolume")] [SerializeField] private PostProcessVolume _postProcessVolume;
        [SerializeField] private float _minIntensity;
        [SerializeField] private float _maxIntensity;
        [SerializeField] private float _cycleDuration;

        private ChromaticAberration _chromaticAberration;
        
        private void Start()
        {
            if (_postProcessVolume.profile.TryGetSettings(out _chromaticAberration))
            {
                _chromaticAberration.active = true; // エフェクトを有効にする
            }
        }
        
        private void Update()
        {
            var t = Mathf.PingPong(Time.time / _cycleDuration, 1f);
            _chromaticAberration.intensity.value = Mathf.Lerp(_minIntensity, _maxIntensity, t);
        }
    }
}