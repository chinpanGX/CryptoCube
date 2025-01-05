#nullable enable
using System;
using App.InGame.Application;
using Unity.Cinemachine;
using UnityEngine;

namespace App.InGame.Presentation.Player
{
    public class CameraGroup : MonoBehaviour, ICameraSwitcher
    {
        [SerializeField] private CinemachineCamera forwardLockCamera = null!;
        [SerializeField] private CinemachineCamera rightLockCamera = null!;
        [SerializeField] private CinemachineCamera leftLockCamera = null!;

        private GameObject? activeCamera;

        public void SwitchCamera(ActivateCamera activateCamera)
        {
            switch (activateCamera)
            {
                case ActivateCamera.ForwardLock:
                    SetActiveCamera(forwardLockCamera.gameObject);
                    break;
                case ActivateCamera.RightLock:
                    SetActiveCamera(rightLockCamera.gameObject);
                    break;
                case ActivateCamera.LeftLock:
                    SetActiveCamera(leftLockCamera.gameObject);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(activateCamera), activateCamera, null);
            }
        }
        
        private void SetActiveCamera(GameObject newActiveCamera)
        {
            if (activeCamera != null)
            {
                activeCamera.SetActive(false);
            }

            activeCamera = newActiveCamera;
            activeCamera.SetActive(true);
        }
    }
}