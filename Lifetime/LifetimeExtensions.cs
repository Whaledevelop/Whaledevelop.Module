using System.Threading;
using Cysharp.Threading.Tasks;

namespace Whaledevelop
{
    public static class LifetimeExtensions
    {
        public static async UniTask InitializeAsync(this ILifetime lifetime, CancellationToken cancellationToken)
        {
            if (lifetime is IAsyncInitializable asyncInitializable)
            {
                await asyncInitializable.InitializeAsync(cancellationToken);

                return;
            }

            if (lifetime is IInitializable initializable)
            {
                initializable.Initialize();

                return;
            }
        }

        public static async UniTask ReleaseAsync(this ILifetime lifetime, CancellationToken cancellationToken)
        {
            if (lifetime is IAsyncReleasable asyncReleasable)
            {
                await asyncReleasable.ReleaseAsync(cancellationToken);

                return;
            }

            if (lifetime is IReleasable releasable)
            {
                releasable.Release();

                return;
            }
        }
    }
}