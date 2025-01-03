﻿using AppCore.Runtime;
using AppService.Runtime;
using R3;
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
        
        private readonly Subject<string> request = new();
        public Observable<string> Request => request;

        public void Setup()
        {
            inputField.onSubmit.RemoveAllListeners();
            inputField.onSubmit.AddListener((text) => request.OnNext(text));
        }
        
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