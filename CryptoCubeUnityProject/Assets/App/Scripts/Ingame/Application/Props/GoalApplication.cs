using App.InGame.Domain.Message;
using MessagePipe;
using VContainer;

namespace App.InGame.Application
{
    public class GoalApplicationService
    {
        private readonly IPublisher<PlayerControlPermissionMessage> playerControlPermissionPublisher;
        private readonly IPublisher<OnTriggerEnterWithGoalMessage> onTriggerEnterWithGoalPublisher;

        [Inject]
        public GoalApplicationService(
            IPublisher<PlayerControlPermissionMessage> playerControlPermissionPublisher,
            IPublisher<OnTriggerEnterWithGoalMessage> onTriggerEnterWithGoalPublisher)
        {
            this.playerControlPermissionPublisher = playerControlPermissionPublisher;
            this.onTriggerEnterWithGoalPublisher = onTriggerEnterWithGoalPublisher;
        }

        public void OnTriggerEnterPublish()
        {
            playerControlPermissionPublisher.Publish(new PlayerControlPermissionMessage(false));
            onTriggerEnterWithGoalPublisher.Publish(new OnTriggerEnterWithGoalMessage());
        }
    }
}