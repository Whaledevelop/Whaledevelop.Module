using System;

namespace Game
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class GameModelAttribute : Attribute
    {
        public GameModelAttribute(params string[] scopeNames)
        {
            ScopeNames = scopeNames;
        }

        public string[] ScopeNames { get; }
    }
}
