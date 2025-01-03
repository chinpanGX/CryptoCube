#nullable enable
using System;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace AppService.Runtime
{
    public class CustomButton : MonoBehaviour
    {
        [SerializeField] private Button? button;
        [SerializeField] private CustomText? text;

        private readonly TimeSpan cooldown = TimeSpan.FromSeconds(0.3);

        public void OnClicked(Action action)
        {
            if (button == null)
            {
                Debug.LogError("Button が null です。");
                return;
            }
            button.OnClickAsObservable()
                .ThrottleFirst(cooldown)
                .Subscribe(_ => action())
                .RegisterTo(destroyCancellationToken);
        }

        public void SetInteractableSafe(bool value)
        {
            if (button == null)
            {
                Debug.LogError("Button が null です。");
                return;
            }
            button.interactable = value;
        }

        public void SetTextSafe(string value)
        {
            if (text == null)
            {
                Debug.LogError("Text が null です。");
                return;
            }
            text.SetTextSafe(value);
        }
    }
}