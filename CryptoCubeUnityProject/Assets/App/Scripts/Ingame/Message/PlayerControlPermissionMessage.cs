namespace App.InGame.Message
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