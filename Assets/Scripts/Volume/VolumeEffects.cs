using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Volume
{
    public class VolumeEffects : MonoBehaviour
    {
        [SerializeField] private float _vignetteConstant = 20f;
        [SerializeField] private float _chromaticAberrationConstant = 20f;
        [SerializeField] private float _filmGainConstant = 10f;
        [SerializeField] private float _colorAdjustmentConstant = 5f;
        [SerializeField] private float _bloomConstant = 5f;
        [SerializeField] private float _bloomIntensityConstant = 5f;
        [SerializeField] private float _lerpConstant = 0.1f;

        
        private UnityEngine.Rendering.Volume _volume;

        private void Awake()
        {
            _volume = GetComponent<UnityEngine.Rendering.Volume>();
        }
        
        public void ChangeVignette(float value)
        {
            _volume.profile.TryGet(out Vignette vignette);
            vignette.intensity.value = (float) Math.Tanh(1/_vignetteConstant * value) + 0.3f;
        }

        public void ChangeVignetteIntensity(float intensity)
        {
            _volume.profile.TryGet(out Vignette vignette);
            vignette.intensity.value = (float) Math.Tanh(intensity);
        }
        
        public void ChangeVignetteSmoothness(float value)
        {
            _volume.profile.TryGet(out Vignette vignette);
            vignette.smoothness.value = (float) Math.Tanh(value);
        }
        
        public void ChangeVignetteCenter(Vector2 position)
        {
            _volume.profile.TryGet(out Vignette vignette);
            vignette.center.value = Vector2.Lerp(vignette.center.value ,position, _lerpConstant);
        }
    
        public void ChangeFilmGrain(float value)
        {
            _volume.profile.TryGet(out FilmGrain filmGrain);
            filmGrain.intensity.value = (float) Math.Tanh(1/_filmGainConstant * value);
        }
        
        public void ChangeChromaticAberration(float value)
        {
            _volume.profile.TryGet(out ChromaticAberration chromaticAberration);
            chromaticAberration.intensity.value = (float) Math.Tanh(1/_chromaticAberrationConstant * value) + 0.05f;
        }
        
        public void ChangeColorAdjustments(float value)
        {
            _volume.profile.TryGet(out ColorAdjustments colorAdjustments);
            colorAdjustments.saturation.value = -100 * (float) Math.Tanh(1/_colorAdjustmentConstant * value);
        }

        public void ChangeBloom(float value)
        {
            _volume.profile.TryGet(out Bloom bloom);
            bloom.threshold.value = 1.0f - (float) Math.Tanh(1/_bloomConstant * value * _bloomIntensityConstant);
            bloom.intensity.value = 1.0f + (float) Math.Tanh(1/_bloomConstant * value * _bloomIntensityConstant);
        }

        public void EnemyTheme(float value)
        {
            ChangeVignette(value);
            ChangeFilmGrain(value);
            ChangeChromaticAberration(value);
            ChangeColorAdjustments(value);
        }
        
    }
}