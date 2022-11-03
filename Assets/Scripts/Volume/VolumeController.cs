using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Volume
{
    public class VolumeController : MonoBehaviour
    {
        [Header("Default Volume")]
        [SerializeField] private float _lerpConstant = 0.1f;
        
        
        public static VolumeController Instance { get; private set; }
        
        public VolumeEffects volumeEffects;
        private UnityEngine.Rendering.Volume _volume;
    
        private void Awake()
        {
            Instance = this;
            _volume = GetComponent<UnityEngine.Rendering.Volume>();
            volumeEffects = _volume.GetComponent<VolumeEffects>();
        }
        
        private void Update()
        {

            _volume.profile.TryGet(out Bloom bloom);
            if (bloom.threshold.value < 1.0f)
            {
                bloom.threshold.value = Mathf.Lerp(bloom.threshold.value, 1.0f, _lerpConstant);
            }

            if (bloom.intensity.value > 1.0f)
            {
                bloom.intensity.value = Mathf.Lerp(bloom.intensity.value, 1.0f, _lerpConstant);
            }
            
            _volume.profile.TryGet(out Vignette vignette);
            if (vignette.center.value != new Vector2(0.5f, 0.5f))
            {
                vignette.center.value = Vector2.Lerp(vignette.center.value, new Vector2(0.5f, 0.5f), 
                    _lerpConstant);
            }
            
            _volume.profile.TryGet(out ChromaticAberration chromaticAberration);
            if (chromaticAberration.intensity.value > 0.05f)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, 0.05f, _lerpConstant);
            }
            
        }

        private void DefaultValues<T, TK>(T effect, TK value) where T : VolumeEffects
        {
            
        }
        
    }
}
