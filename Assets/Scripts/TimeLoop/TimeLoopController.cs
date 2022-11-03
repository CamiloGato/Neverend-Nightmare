using System;
using UnityEngine;
using UnityEngine.UI;

namespace TimeLoop
{
    public class TimeLoopController : MonoBehaviour
    {
        [SerializeField] private GameObject _player;
        [Header("Time")]
        [SerializeField] private int min = 1;
        [SerializeField] private int sec = 0;
        
        [Header("Time Text")]
        [SerializeField] private Text time;
        [SerializeField] private RectTransform timeTransform;
        [SerializeField] private RectTransform clockTransform;
        [SerializeField] public bool die;
        public float leftTime;
        private bool timeIn;

        [Header("Timer Position")]
        [SerializeField] private Vector2 timerToPosition = new Vector2(-600, -525);
        [SerializeField] private float timerToFontSize = 350;
        [SerializeField] private Vector2 clockToPosition = new Vector2(0, -128);
        [SerializeField] private Vector2 clockToScale = new Vector2(300f, 300f);
        
        private Vector2 timeStartPosition;
        private Vector2 clockStartPosition;
        
        private float timeStartFontSize;
        private Vector2 clockStartSize;
        
        public static TimeLoopController Instance { get; private set; }

        public void SetTime()
        {
            leftTime = (min * 60) + sec;
        }
        
        private void Awake()
        {
            Instance = this;
            SetTime();
            timeIn = true;
        }

        private void Start()
        {
            timeStartPosition = timeTransform.anchoredPosition;
            clockStartPosition = clockTransform.anchoredPosition;
            clockStartSize = clockTransform.sizeDelta;
            timeStartFontSize = time.fontSize;
        }

        void Update()
        {

            if (timeIn)
            {
                leftTime -= Time.deltaTime;

                if (leftTime <= 0)
                {
                    leftTime = 0;
                    die = true;
                }
                
                int tempMin = Mathf.FloorToInt(leftTime / 60);
                int tempSeg = Mathf.FloorToInt(leftTime % 60);
                time.text = $"{tempMin:00}:{tempSeg:00}";

                Die(die);
            }
        }

        private void Die(bool state)
        {
            if (state)
            {
                time.fontSize = (int)Mathf.Lerp(time.fontSize, timerToFontSize, 0.125f);
                timeTransform.anchoredPosition = Vector2.Lerp(timeTransform.anchoredPosition, timerToPosition, 0.125f);
                clockTransform.anchoredPosition = Vector2.Lerp(clockTransform.anchoredPosition, clockToPosition, 0.125f);
                clockTransform.sizeDelta = Vector2.Lerp(clockTransform.sizeDelta, clockToScale, 0.125f);
            }
            else
            {
                time.fontSize = (int)Mathf.Lerp(time.fontSize, timeStartFontSize, 0.125f);
                timeTransform.anchoredPosition = Vector2.Lerp(timeTransform.anchoredPosition, timeStartPosition, 0.125f);
                clockTransform.anchoredPosition = Vector2.Lerp(clockTransform.anchoredPosition, clockStartPosition, 0.125f);
                clockTransform.sizeDelta = Vector2.Lerp(clockTransform.sizeDelta, clockStartSize, 0.125f);
            }
        }
    }
}
