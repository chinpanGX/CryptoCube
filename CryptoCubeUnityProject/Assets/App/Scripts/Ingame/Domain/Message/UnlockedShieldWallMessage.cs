namespace App.InGame.Domain.Message
{
    public struct UnlockedShieldWallMessage
    {
        public readonly int ShieldWallId;

        public UnlockedShieldWallMessage(int shieldWallId)
        {
            ShieldWallId = shieldWallId;
        }
    }
}