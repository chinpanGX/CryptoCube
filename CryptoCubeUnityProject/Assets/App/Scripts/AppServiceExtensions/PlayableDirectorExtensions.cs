using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Playables;

namespace App.AppServiceExtensions
{
    public static class PlayableDirectorExtensions
    {
        public static UniTask PlayAsync(this PlayableDirector self, CancellationToken cancellationToken)
        {
            self.Play();
            return UniTask.WaitUntil(() => self.state != PlayState.Playing, cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
        }
    }
}