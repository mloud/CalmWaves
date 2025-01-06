using System;
using System.Collections.Generic;
using Core.Modules.Ui.Effects;
using Meditation.Data.Breathing;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Components
{
    public class JourneyButton : MonoBehaviour
    {
        public Action<int> ClickedAction;
        
        public enum State
        {
            Locked,
            Current,
            Finished
        }
        
        [SerializeField] private Button button;
        [SerializeField] private List<TMP_Text> orderLabels;
        [SerializeField] private TMP_Text breathCycleLengtLabel;
        [SerializeField] private GameObject openGo;
        [SerializeField] private GameObject finishedGo;
        [SerializeField] private GameObject currentGo;
        [SerializeField] private RectTransform connector;
        [SerializeField] private Image progressImage;
        [SerializeField] private BreathingSettingsPanel breathingSettingsPanel;
        [SerializeField] private FloatingEffect floatingEffect;
        
        private State state;
        private int order;
        
        private void Awake()
        {
            button.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            if (state == State.Locked)
            {
                Debug.Log($"Clicked on locked mission {order}");
                return;
            }

            ClickedAction(order);
        }


        public JourneyButton Set(int order, int total, JourneyBreathingSettings settings)
        {
            this.order = order;
            orderLabels.ForEach(x=>x.text = (order + 1).ToString());
            breathCycleLengtLabel.text = $"{Mathf.RoundToInt(settings.GetOneBreatheTime())} sec";
            progressImage.fillAmount = (float)order / total;
            breathingSettingsPanel.Set(settings);
            return this;
        }

        public void StartFloatingEffect(float delay)
        {
            floatingEffect.Delay = delay;
            floatingEffect.Run();
        }
        
        public void SetState(State state)
        {
            this.state = state;
            
            switch (state)
            {
                case State.Current:
                    currentGo.SetActive(true);
                    finishedGo.SetActive(false);
                    openGo.SetActive(false);
                    breathingSettingsPanel.gameObject.SetActive(true);
                    break;
                
                case State.Finished:
                    currentGo.SetActive(false);
                    finishedGo.SetActive(true);
                    openGo.SetActive(false);
                    breathingSettingsPanel.gameObject.SetActive(false);
                    break;
                
                case State.Locked:
                    currentGo.SetActive(false);
                    finishedGo.SetActive(false);
                    openGo.SetActive(true);
                    breathingSettingsPanel.gameObject.SetActive(false);
                    break;
            }
        }
    }
}