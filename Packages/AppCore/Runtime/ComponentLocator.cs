using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AppCore.Runtime
{
    public static class ComponentLocator
    {
        private const float GCInterval = 31f;
        private static readonly Dictionary<Type, IStaticCache> cache = new();

        public static bool UseGC = true;
        private static float lastGCAt;

        private static readonly List<Type> compTypesToRemove = new();

        public static T GetOrNull<T>() where T : Component
        {
            GCIfNeeded();

            var value = StaticCache<T>.Value;
            if (value)
            {
                return value;
            }

            Uncache<T>();

            value = Object.FindObjectOfType<T>();
            if (value)
            {
                Cache(value);
                return value;
            }

            return null;
        }

        public static T Get<T>() where T : Component
        {
            var comp = GetOrNull<T>();
            if (comp) return comp;

            var compType = typeof(T);
            var gameObject = new GameObject(compType.Name, compType);
            comp = gameObject.GetComponent<T>();
            if (comp)
            {
                Uncache<T>();
                Cache(comp);
                return comp;
            }

            return null;
        }

        public static void Cache<T>(T value) where T : Component
        {
            StaticCache<T>.Value = value;
            cache.Add(typeof(T), StaticCache<T>.Instance);
        }

        public static void Uncache<T>() where T : Component
        {
            StaticCache<T>.Value = null;
            cache.Remove(typeof(T));
        }

        private static void GCIfNeeded()
        {
            if (!UseGC) return;

            var now = Time.unscaledTime;
            if (GCInterval < now - lastGCAt)
            {
                lastGCAt = now;
                GC();
            }
        }

        public static void GC()
        {
            foreach (var kv in cache)
            {
                if (kv.Value == null)
                {
                    compTypesToRemove.Add(kv.Key);
                }
            }
            foreach (var compType in compTypesToRemove)
            {
                cache.Remove(compType);
            }
            compTypesToRemove.Clear();
        }
    }

    internal interface IStaticCache
    {
        Component Component { get; }
    }

    internal class StaticCache<T> : IStaticCache where T : Component
    {
        internal static StaticCache<T> Instance = new();

        internal static T Value;
        public Component Component
        {
            get => Value;
            set => Value = value as T;
        }
    }
}