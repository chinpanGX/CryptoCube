using R3;
using UnityEngine.InputSystem;

namespace App.InGame.Player
{
    public static class UnityInputSystemExtensions
    {
        public static Observable<InputAction.CallbackContext> AsObservable(this InputAction inputAction)
        {
            return Observable.FromEvent<InputAction.CallbackContext>(
                h => inputAction.performed += h,
                h => inputAction.performed -= h
            );
        }
    }
}