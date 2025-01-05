using AppCore.Runtime;
using AppService.Runtime;
using UnityEngine;

namespace App.InGame.Presentation.HUD
{
    public class InGameHudView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CustomText remainingTimeText;

        private static ViewScreen ViewScreen => ComponentLocator.Get<ViewScreen>();
        public Canvas Canvas => canvas;

        public void Push()
        {
            ViewScreen.Push(this);
        }

        public void Pop()
        {
            ViewScreen.Pop();
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        public void Close()
        {
            Destroy(gameObject);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetRemainingTimeText(int remainingTime)
        {
            remainingTimeText.SetTextSafe($"{remainingTime}");
        }
    }
}