using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace App.InGame.Domain
{
    public class GameControlEntity
    {
        private readonly ReactiveProperty<int> hackingRemainTime = new();

        public ReadOnlyReactiveProperty<int> HackingRemainTime => hackingRemainTime;
        public bool Pausing { get; private set; }

        public GameControlEntity(int limitTime)
        {
            hackingRemainTime.Value = limitTime;
        }

        public async UniTaskVoid StartCountDown(CancellationToken cancellationToken)
        {
            while (hackingRemainTime.Value > 0)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                await UniTask.Delay(1000, cancellationToken: cancellationToken);
                while (Pausing)
                    await UniTask.Yield(cancellationToken: cancellationToken);

                hackingRemainTime.Value--;
            }
        }

        public void OnChangePause()
        {
            Pausing = !Pausing;
        }
    }
}