#nullable enable

namespace AppCore.Runtime
{
    public class UpdatablePresenter
    {
        private IPresenter? presenter;
        private IPresenter? request;

        public void Execute()
        {
            if (request != null)
            {
                presenter?.Dispose();
                presenter = request;
                request = null!;
            }

            presenter?.Execute();
        }

        public void SetRequest(IPresenter presenter)
        {
            request = presenter;
        }
    }
}