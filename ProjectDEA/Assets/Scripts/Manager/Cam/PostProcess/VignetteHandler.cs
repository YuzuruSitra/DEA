using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Manager.Cam.PostProcess
{
    public class VignetteHandler
    {
        private readonly Vignette _vignette;

        public VignetteHandler()
        {
            var postProcessVolume = GameObject.FindWithTag("PostprocessVolume").GetComponent<PostProcessVolume>();
            // PostProcessVolumeからVignetteの設定を取得
            postProcessVolume.profile.TryGetSettings(out _vignette);
        }

        public void SetVignetteIntensity(float intensity)
        {
            if (_vignette != null)
            {
                _vignette.intensity.value = intensity;
            }
        }
    }
}