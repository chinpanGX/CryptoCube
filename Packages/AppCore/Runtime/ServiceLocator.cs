using System;
using System.Collections.Generic;
using UnityEngine;

namespace AppCore.Runtime
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> instances = new();

        /// <summary>
        ///     インスタンスを登録します。すでに同じ型のインスタンスが登録されている場合は登録できませんので、先に Unregister を行ってください。
        /// </summary>
        /// <param name="instance">登録するインスタンス。</param>
        /// <typeparam name="T">登録するインスタンスの型。</typeparam>
        public static void Register<T>(T instance) where T : class
        {
            var type = typeof(T);

            if (!instances.TryAdd(type, instance))
            {
                Debug.LogWarning($"すでに同じ型のインスタンスが登録されています：{type.Name}");
            }
        }

        /// <summary>
        ///     インスタンスの登録を解除します。インスタンスが登録されていなかった場合は警告が出ます。
        /// </summary>
        /// <param name="instance">登録を解除するインスタンス。</param>
        /// <typeparam name="T">登録を解除するインスタンスの型。</typeparam>
        public static void Unregister<T>(T instance) where T : class
        {
            var type = typeof(T);

            if (!instances.ContainsKey(type))
            {
                Debug.LogWarning($"要求された型のインスタンスが登録されていません：{type.Name}");
                return;
            }

            if (!Equals(instances[type], instance))
            {
                Debug.LogWarning($"登録されている要求された型のインスタンスと渡されたインスタンスが一致しません：{type.Name}");
                return;
            }

            instances.Remove(type);
        }

        public static void Clear()
        {
            instances.Clear();
        }

        /// <summary>
        ///     インスタンスを取得します。取得できなかった場合はエラーになります。
        /// </summary>
        /// <typeparam name="T">取得したいインスタンスの型。</typeparam>
        /// <returns>取得したインスタンスを返します。取得できなかった場合は null を返します。</returns>
        public static T GetOrNull<T>() where T : class
        {
            var type = typeof(T);

            if (!instances.TryGetValue(type, out var instance))
            {
                Debug.LogError($"要求された型のインスタンスが登録されていません：{type.Name}");
                return null;
            }
            return instance as T;
        }

        /// <summary>
        ///     インスタンスを取得し、渡された引数に代入します。取得できなかった場合は null が入ります。
        /// </summary>
        /// <param name="instance">取得したインスタンスを入れる変数。</param>
        /// <typeparam name="T">取得したいインスタンスの型。</typeparam>
        /// <returns>取得が成功したら true を返します。</returns>
        public static bool TryGet<T>(out T instance) where T : class
        {
            var type = typeof(T);
            instance = instances.TryGetValue(type, out var value) ? value as T : null;
            return instance != null;
        }

        /// <summary>
        ///     指定された型のインスタンスがすでに登録されているかをチェックします。
        /// </summary>
        /// <typeparam name="T">登録を確認するインスタンスの型。</typeparam>
        /// <returns>指定された型のインスタンスがすでに登録されている場合は true を返します。</returns>
        public static bool IsRegistered<T>() where T : class
        {
            return instances.ContainsKey(typeof(T));
        }

        /// <summary>
        ///     渡されたインスタンスがすでに登録されているかをチェックします。
        /// </summary>
        /// <param name="instance">登録を確認するインスタンス。</param>
        /// <typeparam name="T">登録を確認するインスタンスの型。</typeparam>
        /// <returns>渡されたインスタンスが既に登録されている場合は true を返します。</returns>
        public static bool IsRegistered<T>(T instance) where T : class
        {
            var type = typeof(T);
            return instances.ContainsKey(type) && Equals(instances[type], instance);
        }
    }
}