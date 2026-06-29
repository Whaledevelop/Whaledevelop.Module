using System;using System.Threading;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using Whaledevelop.Services;
using Whaledevelop.Systems;

namespace Whaledevelop.VContainer
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public abstract class EntryPointBase : IAsyncStartable
    {
        public abstract Awaitable StartAsync(CancellationToken cancellation = new CancellationToken());
    }
}
