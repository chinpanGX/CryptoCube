using System.Collections.Generic;
using App.InGame.Application;
using UnityEngine;
using VContainer;

namespace App.InGame.Presentation.Props
{
    public class TrapLineDirector : MonoBehaviour
    {
        [SerializeField] private List<TrapLine> trapLines;
        private TrapLineApplicationService ApplicationService { get; set; }
        
        [Inject]
        public void Construct(TrapLineApplicationService applicationService)
        {
            ApplicationService = applicationService;
            foreach (var trapLine in trapLines)
            {
                trapLine.Construct(applicationService.OnTriggerEnterPublish);
            }
        }

        private void OnDestroy()
        {
            ApplicationService?.Dispose();
        }
    }
}