using System;
using UnityEngine;
using UnityEngine.UI;

namespace Pickable
{
    public class PickableCanvas : MonoBehaviour
    {

        [Header("Config")]
        [SerializeField] private Vector2 sizeDieDelta = new Vector2(150, 150);
        public bool isDie = false;

        [Header("Object 1")]
        [SerializeField] private RectTransform ositoShow;
        [SerializeField] private RectTransform ositoHide;
        [SerializeField] private RectTransform ositoDie;
        [SerializeField] private Text ositoText;
        public bool ositoCollected;
        
        [Header("Object 2")]
        [SerializeField] private RectTransform pinguiShow;
        [SerializeField] private RectTransform pinguiHide;
        [SerializeField] private RectTransform pinguiDie;
        [SerializeField] private Text pinguiText;
        public bool pinguiCollected;
        
        [Header("Object 3")]
        [SerializeField] private RectTransform rabbitShow;
        [SerializeField] private RectTransform rabbitHide;
        [SerializeField] private RectTransform rabbitDie;
        [SerializeField] private Text rabbitText;
        public bool rabbitCollected;
        
        public static PickableCanvas Instance;
        private Color colorAlive;
        private Color colorDead;
        private Color colorCollected;
        public bool gameEnded;

        
        private void Awake()
        {
            Instance = this;
            colorAlive = ositoText.color;
            colorDead = pinguiText.color;
            colorCollected = rabbitText.color;
        }

        private void Update()
        {
            OnDie(isDie);
            if (ositoCollected && pinguiCollected && rabbitCollected)
            {
                gameEnded = true;
            }
        }

        public void OnPick(int id)
        {
            switch (id)
            {
                case 1:
                    ositoShow.gameObject.SetActive(true);
                    ositoHide.gameObject.SetActive(false);
                    ositoCollected = true;
                    break;
                case 2:
                    pinguiShow.gameObject.SetActive(true);
                    pinguiHide.gameObject.SetActive(false);
                    pinguiCollected = true;
                    break;
                case 3:
                    rabbitShow.gameObject.SetActive(true);
                    rabbitHide.gameObject.SetActive(false);
                    rabbitCollected = true;
                    break;
                default:
                    break;
            }
        }

        public void OnDie(bool state)
        {
            if (state)
            {
                ositoText.color = colorDead;
                pinguiText.color = colorDead;
                rabbitText.color = colorDead;
                ositoDie.sizeDelta = Vector2.Lerp(ositoDie.sizeDelta, sizeDieDelta, 0.125f);
                pinguiDie.sizeDelta = Vector2.Lerp(pinguiDie.sizeDelta, sizeDieDelta, 0.125f);
                rabbitDie.sizeDelta = Vector2.Lerp(rabbitDie.sizeDelta, sizeDieDelta, 0.125f);
            }
            else
            {
                ositoText.color =  (ositoCollected) ? colorCollected : colorAlive;
                pinguiText.color = (pinguiCollected) ? colorCollected : colorAlive;
                rabbitText.color = (rabbitCollected) ? colorCollected : colorAlive;
                ositoDie.sizeDelta = Vector2.Lerp(ositoDie.sizeDelta, Vector2.zero, 0.125f);
                pinguiDie.sizeDelta = Vector2.Lerp(pinguiDie.sizeDelta, Vector2.zero, 0.125f);
                rabbitDie.sizeDelta = Vector2.Lerp(rabbitDie.sizeDelta, Vector2.zero, 0.125f);
            }
        }
    }
}
