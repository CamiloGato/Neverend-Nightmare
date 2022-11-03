using Cinemachine;
using UnityEngine;

namespace Player
{
    public class CinemachineEffects : MonoBehaviour
    {
        public static CinemachineEffects Instance { get; private set; }
        private CinemachineVirtualCamera _cinemachineVirtualCamera;
        private float _shakeTimer;
    
        private float _defaultFixedDeltaTime = 0.02f;
        private float _defaultTimeScale = 1f;
        
        private void Awake()
        {
            Instance = this;
            _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        }
    
        public void ShakeCamera(float amplitude, float duration)
        {
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = amplitude;
            _shakeTimer = duration;
        }

        public void SlowMotion(float scale,float duration)
        {
            Time.timeScale = scale;
            Time.fixedDeltaTime = _defaultFixedDeltaTime * Time.timeScale;
            if (duration > 0) Invoke("RestartGameTime", duration);
        }
        
        private void RestartGameTime()
        {
            Time.timeScale = _defaultTimeScale;
            Time.fixedDeltaTime = _defaultFixedDeltaTime;
        }
        
        private void Update()
        {
            if (_shakeTimer > 0)
            {
                _shakeTimer -= Time.deltaTime;
                if (_shakeTimer <= 0f)
                {
                    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                    cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
                }
            }
        }
    }
}
