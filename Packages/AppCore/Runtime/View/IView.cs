using UnityEngine;

namespace AppCore.Runtime
{
    public interface IView
    {
        Canvas Canvas { get; }
        void Push();
        void Pop();
        void Open();
        void Close();
    }
}