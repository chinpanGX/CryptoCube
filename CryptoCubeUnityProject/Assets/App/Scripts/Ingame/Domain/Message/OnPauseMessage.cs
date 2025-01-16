namespace App.InGame.Domain.Message
{
    public struct PauseMessage
    {
        public readonly bool IsPause;

        public PauseMessage(bool isPause)
        {
            IsPause = isPause;
        }
    }
}