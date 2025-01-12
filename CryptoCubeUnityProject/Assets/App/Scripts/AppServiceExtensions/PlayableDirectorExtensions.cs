using Cysharp.Threading.Tasks;
using UnityEngine.Playables;

namespace App.AppServiceExtensions
{
    public static class PlayableDirectorExtensions
    {
        public static UniTask PlayAsync(this PlayableDirector self)
        {
            self.Play();
            return UniTask.WaitUntil(() => self.state != PlayState.Playing);
        }
    }
}