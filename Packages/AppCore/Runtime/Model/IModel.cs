using System;

namespace AppCore.Runtime
{
    public interface IModel : IDisposable
    {
        void Setup();
        void Execute();
    }
}
