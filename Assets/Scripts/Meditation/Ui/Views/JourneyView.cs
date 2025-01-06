using System;
using System.Linq;
using Meditation.Data.Breathing;
using Meditation.Ui.Components;
using OneDay.Core.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Meditation.Ui.Views
{
    public class JourneyView : UiView
    {
        public Action<int> JourneyButtonClicked { get; set; }
        public CanvasGroup MissionsCanvasGroup => missionsCanvasGroup;

        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private JourneyButtonsPanel journeyButtonsPanel;
        [SerializeField] private ScrollRect missionsScrollrect;
        [SerializeField] private ChapterInfoComponent chapterInfoPrefab;
        [SerializeField] private CanvasGroup missionsCanvasGroup;
        [SerializeField] private RectTransform centeredTransform;
        public void Initialize(JourneySettingsDb journeySettings)
        {
            titleLabel.text = journeySettings.Name;
            var missions = journeySettings.GetMissions().ToList();
            var chapterInfos = journeySettings.GetChapterInfos().ToList();
            
            journeyButtonsPanel.Prepare(missions.Count);
            for (var index = 0; index < missions.Count; index++)
            {
                int chapterIndex = chapterInfos.FindIndex(x => x.FromMission == index);
                if (chapterIndex != -1)
                { 
                    var chapterInfo = Instantiate(chapterInfoPrefab, journeyButtonsPanel.Container);
                    chapterInfo.transform.SetSiblingIndex(journeyButtonsPanel.Get(index).transform.GetSiblingIndex());
                    chapterInfo.Set(
                        chapterInfos[chapterIndex].Name, 
                        chapterInfos[chapterIndex].Description);
                }

                journeyButtonsPanel.Get(index).gameObject.name = $"JourneyButton_{index}";
                journeyButtonsPanel.Get(index).Set(index, missions.Count, missions[index]);
                journeyButtonsPanel.Get(index).SetState(JourneyButton.State.Locked);
                journeyButtonsPanel.Get(index).ClickedAction += (order)=>JourneyButtonClicked(order);
            }
        }
        
        public void Set(int currentMission)
        {
            int index = 0;
            foreach (var item  in journeyButtonsPanel.Items)
            {
                var state = index == currentMission
                    ? JourneyButton.State.Current
                    : index < currentMission
                        ? JourneyButton.State.Finished
                        : JourneyButton.State.Locked;
                item.SetState(state);
                index++;
            }

            var centeredItem = journeyButtonsPanel.Items.ElementAt(currentMission);
            missionsScrollrect.CenterOnItem(centeredItem.transform as RectTransform, centeredTransform);

            // int index = 0;
            // foreach (var item in journeyButtonsPanel.Items)
            // {
            //       item.StartFloatingEffect(index * 0.1f);
            //       index++;
            //}
        }
    }
}