namespace App.InGame.Domain.Message
{
    public struct OnTriggerEnterWithGoalMessage
    {
    }

    public struct OnTriggerEnterWithPlayerRestartMessage
    {
    }
    
    public struct OnTriggerEnterWithCharacterSpawnerMessage
    {
    }

    public struct OnTriggerEnterWithShieldWallMessage
    {
        public readonly int ShieldWallId;

        public OnTriggerEnterWithShieldWallMessage(int shieldWallId)
        {
            ShieldWallId = shieldWallId;
        }
    }

}