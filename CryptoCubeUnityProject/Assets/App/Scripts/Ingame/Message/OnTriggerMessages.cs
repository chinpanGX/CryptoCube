namespace App.InGame.Message
{
    public struct OnTriggerEnterWithGoalMessage
    {
    }

    public struct OnTriggerEnterWithPatrolEnemyMessage
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