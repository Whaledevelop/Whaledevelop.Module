using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace QuizRPG.Utility
{
    public static class UniTaskUtility
    {
        public static async UniTask<T> WaitCompletionAsync<T>(UniTaskCompletionSource<T> completionSource, CancellationToken cancellationToken, Action callback = null)
        {
            await using var registration = cancellationToken.Register(() => completionSource.TrySetCanceled());
            try
            {
                var result = await completionSource.Task;
                
                return result;
            }
            finally
            {
                callback?.Invoke();
                await registration.DisposeAsync();
            }
        }
        
        public static CancellationTokenSource StartTask(UniTask task, Action onComplete, CancellationToken cancellationToken)
        {
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            UniTask.Void(async () =>
            {
                try
                {
                    await task.AttachExternalCancellation(cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                }

                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }
                onComplete?.Invoke();
            });
            return cancellationTokenSource;
        }

        public static void ExecuteAfterFrames(int frames, Action onComplete, CancellationToken cancellationToken = default)
        {
            UniTask.Void(async () =>
            {
                await UniTask.DelayFrame(frames, cancellationToken: cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                onComplete.Invoke();
            });
        }
        
        public static void ExecuteAfterSeconds(float seconds, Action onComplete, CancellationToken cancellationToken = default)
        {
            UniTask.Void(async () =>
            {
                await UniTask.WaitForSeconds(seconds, cancellationToken: cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                onComplete.Invoke();
            });
        }
    }
}