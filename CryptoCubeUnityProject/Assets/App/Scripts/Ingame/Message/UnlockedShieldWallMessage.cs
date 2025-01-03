namespace App.InGame.Message
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