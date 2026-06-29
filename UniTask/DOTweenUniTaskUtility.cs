using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Whaledevelop.Utility
{
    public static class DOTweenUniTaskUtility
    {
        public static UniTask AwaitComplete(this Tween tween, CancellationToken cancellationToken = default)
        {
            if (tween == null || !tween.IsActive())
            {
                return UniTask.CompletedTask;
            }

            if (tween.IsComplete())
            {
                return UniTask.CompletedTask;
            }

            var completionSource = new UniTaskCompletionSource();

            tween.OnComplete(() =>
            {
                completionSource.TrySetResult();
            });

            if (cancellationToken.CanBeCanceled)
            {
                cancellationToken.Register(() =>
                {
                    if (tween.IsActive())
                    {
                        tween.Kill();
                    }

                    completionSource.TrySetCanceled(cancellationToken);
                });
            }

            return completionSource.Task;
        }
    }
}