namespace App.InGame.Domain.Message
{
    public struct PlayerControlPermissionMessage
    {
        public readonly bool CanControl;

        public PlayerControlPermissionMessage(bool canControl)
        {
            CanControl = canControl;
        }
    }
}