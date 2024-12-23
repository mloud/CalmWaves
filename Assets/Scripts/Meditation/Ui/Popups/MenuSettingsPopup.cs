using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Meditation.States;
using OneDay.Core;
using OneDay.Core.Modules.Conditions.Ui;
using OneDay.Core.Modules.Sm;
using OneDay.Core.Modules.Ui;
using UnityEngine;
using UnityEngine.UI;


namespace Meditation.Ui
{
    public class MenuSettingsPopup : UiPopup
    {
        [SerializeField] private Button notificationsButtons;
        [SerializeField] private Button subscriptionButton;
        [SerializeField] private List<ConditionComponent> conditions;
        public override UniTask Initialize()
        {
            notificationsButtons.onClick.AddListener(OnNotificationsButton);
            subscriptionButton.onClick.AddListener(OnSubscription);
            return UniTask.CompletedTask;
        }

        private void OnSubscription()
        {
            Close();
            ServiceLocator.Get<IUiManager>().OpenPopup<SubscriptionPopup>(null);
        }

        private void OnNotificationsButton()
        {
            Close();
            ServiceLocator.Get<IUiManager>().OpenPopup<NotificationPopup>(null);
        }

        protected override async UniTask OnOpenStarted(IUiParameter parameter)
        {
            conditions.ForEach(x=>x.Refresh());
            ServiceLocator.Get<IUiManager>().HideView();
        }

        protected override async UniTask OnCloseStarted()
        {
            ServiceLocator.Get<IUiManager>().ShowView();
        }
    }
}