using System.Threading;
using Cysharp.Threading.Tasks;

namespace Whaledevelop.States
{
    public interface IStateEntryPoint
    {
        UniTask EnterAsync(CancellationToken cancellationToken);
    }
}
