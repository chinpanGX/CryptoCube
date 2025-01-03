#nullable enable
using System;
using System.Collections.Generic;
using Codice.CM.Common;

namespace AppCore.Runtime
{
    public class StateMachine<T> where T : class
    {
        private readonly List<IState> cache = new();

        private readonly T owner;
        private IState? request;
        private IState? Current { get; set; }

        public StateMachine(T owner)
        {
            this.owner = owner;
        }
        
        public void Dispose()
        {
            if (Current != null)
            {
                Current.End(owner);
                Free(Current);   
            }
            
            Current = null;

            foreach (var state in cache)
            {
                state.Dispose();
            }
            cache.Clear();
        }
        
        public void Change<TState>() where TState : IState, new()
        {
            request = Alloc<TState>();
        }
        
        public void Execute()
        {
            if (request != null)
            {
                if (Current != null)
                {
                    Current.End(owner);
                    Free(Current);
                }

                Current = request;
                request = null!;

                Current.Begin(owner);
            }

            Current?.Execute(owner);
        }
        
        public bool IsState<TState>() where TState : IState
        {
            return Current is TState;
        }
        
        private IState Alloc<TState>() where TState : IState, new()
        {
            foreach (var value in cache)
            {
                if (value is TState state)
                {
                    return state;
                }
            }
            return new TState();
        }
        
        private void Free<TState>(TState state) where TState : IState
        {
            state.Clear();
            cache.Add(state);
        }

        public interface IState : IDisposable
        {
            void Clear();
            void Begin(T owner);
            void Execute(T owner);
            void End(T owner);
        }

        public class State : IState
        {
            public virtual void Dispose()
            {

            }
            public virtual void Clear()
            {

            }
            public virtual void Begin(T owner)
            {

            }
            public virtual void Execute(T owner)
            {

            }
            public virtual void End(T owner)
            {

            }
        }
    }
}