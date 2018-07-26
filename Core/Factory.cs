using System;
using System.Collections.Generic;

namespace XAS.Core {

    /// <summary>
    /// A generic factory class.
    /// </summary>
    /// <typeparam name="K">The keyword to use.</typeparam>
    /// <typeparam name="T">The function pointer.</typeparam>
    /// 
    public class Factory<K, T> {

        // taken from http://www.gutgames.com/post/Factory-Pattern-using-Generics-in-C.aspx

        private Dictionary<K, Func<T>> items = new Dictionary<K, Func<T>>();

        /// <summary>
        /// Register a key to a function pointer.
        /// </summary>
        /// <param name="key">The keyword to user.</param>
        /// <param name="result">The function delegate.</param>
        /// 
        public void Register(K key, T result) {

            if (items.ContainsKey(key)) {

                items[key] = new Func<T>(() => result);

            } else {

                items.Add(key, new Func<T>(() => result));

            }

        }

        /// <summary>
        /// Register a key for a function pointer.
        /// </summary>
        /// <param name="key">The keyword to user.</param>
        /// <param name="item">The function pointer.</param>
        /// 
        public void Register(K key, Func<T> item) {

            if (items.ContainsKey(key)) {

                items[key] = item;

            } else {

                items.Add(key, item);

            }

        }

        /// <summary>
        /// Retrieve the function, based on key.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <returns>A function pointer.</returns>
        /// 
        public T Create(K key) {

            if (items.ContainsKey(key)) {

                return items[key]();

            }

            return default(T);

        }

    }

}
