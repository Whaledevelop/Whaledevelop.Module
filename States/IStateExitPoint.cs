using System.Threading;
using Cysharp.Threading.Tasks;

namespace Whaledevelop.States
{
    public interface IStateExitPoint
    {
        UniTask ExitAsync(CancellationToken cancellationToken);
    }
}
