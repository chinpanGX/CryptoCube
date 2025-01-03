namespace App.InGame.Message
{
    public struct OnTriggerEnterWithShieldWallMessage
    {
        public readonly int ShieldWallId;

        public OnTriggerEnterWithShieldWallMessage(int shieldWallId)
        {
            ShieldWallId = shieldWallId;
        }
    }
}