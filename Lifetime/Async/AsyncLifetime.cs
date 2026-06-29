using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
// ReSharper disable MethodHasAsyncOverloadWithCancellation

namespace Whaledevelop
{
    public abstract class AsyncLifetime : IAsyncLifetime
    {
        private CancellationTokenSource _cancellationTokenSource;
        private bool _initialized;

        bool IAsyncInitializable.Initialized => _initialized;

        async UniTask IAsyncInitializable.InitializeAsync(CancellationToken cancellationToken)
        {
            if (_initialized)
            {
                return;
            }

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                Application.exitCancellationToken,
                cancellationToken
            );

            await OnInitializeAsync(_cancellationTokenSource.Token);

            _initialized = true;
        }

        async UniTask IAsyncReleasable.ReleaseAsync(CancellationToken cancellationToken)
        {
            if (!_initialized)
            {
                return;
            }

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;

            await OnReleaseAsync(cancellationToken);

            _initialized = false;
        }

        protected virtual UniTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask OnReleaseAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
    }

}