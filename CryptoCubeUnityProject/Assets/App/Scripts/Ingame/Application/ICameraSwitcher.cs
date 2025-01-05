namespace App.InGame.Application
{
    public enum ActivateCamera
    {
        ForwardLock,
        RightLock,
        LeftLock
    }
    
    public interface ICameraSwitcher
    {
        void SwitchCamera(ActivateCamera activateCamera);
    }
}