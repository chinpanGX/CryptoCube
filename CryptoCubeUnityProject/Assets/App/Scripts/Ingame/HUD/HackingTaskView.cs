using AppCore.Runtime;
using AppService.Runtime;
using TMPro;
using UnityEngine;

namespace App.InGame.HUD
{
    public class HackingTaskView : MonoBehaviour, IView
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private CustomText hackingText;
        [SerializeField] private TMP_InputField inputField;

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
            inputField.ActivateInputField();
        }
        
        public void PreOpenSetup(string text)
        {
            hackingText.SetTextSafe(text);
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
    }
}