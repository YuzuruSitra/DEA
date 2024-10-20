using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Manager.Cam.PostProcess
{
    public class VignetteHandler
    {
        private static VignetteHandler _instance;
        public static VignetteHandler Instance => _instance ??= new VignetteHandler();
        private readonly Vignette _vignette;

        private VignetteHandler()
        {
            var postProcessVolume = GameObject.FindWithTag("PostprocessVolume").GetComponent<PostProcessVolume>();
            // PostProcessVolumeからVignetteの設定を取得
            if (postProcessVolume.profile.TryGetSettings(out _vignette))
            {
                _vignette.intensity.value = 0.0f;
            }
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