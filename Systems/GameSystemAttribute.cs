using System;

namespace Whaledevelop.Systems
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class GameSystemAttribute : Attribute
    {
        public GameSystemAttribute(params string[] scopeNames)
        {
            ScopeNames = scopeNames;
        }

        public string[] ScopeNames { get; }
    }
}
