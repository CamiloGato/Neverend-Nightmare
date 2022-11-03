using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace Player
{
    public class VolumeController : MonoBehaviour
    {
        [Header("Constants")]
        [SerializeField] private float _vignetteConstant = 20f;
        [SerializeField] private float _chromaticAberrationConstant = 20f;
        [SerializeField] private float _filmGainConstant = 10f;
        [SerializeField] private float _colorAdjustmentConstant = 5f;
        [SerializeField] private float _bloomConstant = 5f;
        [SerializeField] private float _bloomIntensityConstant = 5f;
        [SerializeField] private float _lerpConstant = 0.1f;

        class ConstantUpdates
        {
            public bool _isBloomActive;
            public bool _isVignetteActive;
            public bool _isChromaticAberrationActive;
        }

        private ConstantUpdates _constantUpdates = new();
        
        public static VolumeController Instance { get; private set; }
        private Volume _volume;
    
        private void Awake()
        {
            Instance = this;
            _volume = GetComponent<Volume>();
        }
    
        public void ChangeVignette(float value)
        {
            _volume.profile.TryGet(out Vignette vignette);
            vignette.intensity.value = (float) Math.Tanh(1/_vignetteConstant * value) + 0.3f;
        }

        public void ChangeVignette(float intensity, float smooth, Vector2 position)
        {
            _volume.profile.TryGet(out Vignette vignette);
            vignette.intensity.value = (float) Math.Tanh(intensity);
            vignette.smoothness.value = (float) Math.Tanh(smooth);
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

        private void Update()
        {
            
            if (!_constantUpdates._isBloomActive)
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
            }

            if (!_constantUpdates._isVignetteActive)
            {
                _volume.profile.TryGet(out Vignette vignette);
                if (vignette.center.value != new Vector2(0.5f, 0.5f))
                {
                    vignette.center.value = Vector2.Lerp(vignette.center.value, new Vector2(0.5f, 0.5f), 
                        _lerpConstant);
                }
            }

            if (!_constantUpdates._isChromaticAberrationActive)
            {
                _volume.profile.TryGet(out ChromaticAberration chromaticAberration);
                if (chromaticAberration.intensity.value > 0.05f)
                {
                    chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, 0.05f, _lerpConstant);
                }
            }
            
        }
    }
}
