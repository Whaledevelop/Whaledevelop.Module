using System.Threading;
using Cysharp.Threading.Tasks;

public interface IAsyncReleasable
{
    UniTask ReleaseAsync(CancellationToken cancellationToken);
}