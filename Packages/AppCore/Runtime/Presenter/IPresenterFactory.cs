using UnityEngine;

namespace AppCore.Runtime
{
    public interface IPresenterFactory
    {
        Awaitable<IPresenter> CreateAsync();
    }
}