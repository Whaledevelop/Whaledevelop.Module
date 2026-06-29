using System.Threading;
using Cysharp.Threading.Tasks;

public interface IAsyncInitializable
{
    bool Initialized { get; }

    UniTask InitializeAsync(CancellationToken cancellationToken);
}