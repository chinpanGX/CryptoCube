using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace App.InGame.Domain
{
    public class GameControlEntity
    {
        private readonly ReactiveProperty<int> hackingRemainTime = new();
        private readonly ReactiveProperty<bool> pausing = new(false);

        public ReadOnlyReactiveProperty<int> HackingRemainTime => hackingRemainTime;
        public ReadOnlyReactiveProperty<bool> Pausing => pausing;

        public GameControlEntity(int limitTime)
        {
            hackingRemainTime.Value = limitTime;
        }

        public async UniTaskVoid StartCountDown(CancellationToken cancellationToken)
        {
            while (hackingRemainTime.Value > 0)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                if (pausing.Value)
                {
                    await UniTask.Yield(cancellationToken: cancellationToken);
                }

                await UniTask.Delay(1000, cancellationToken: cancellationToken);
                hackingRemainTime.Value--;
            }
        }
        
        public void Pause()
        {
            pausing.Value = true;
        }
        
        public void Resume()
        {
            pausing.Value = false;
        }
    }
}