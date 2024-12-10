using UnityEngine;
using System.Collections;
namespace SGGame
{

    /// <summary>
    /// Singleton.
    /// </summary>
    public class Singleton<T> where T : new()
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static T Instance
        {
            get
            {
                // If instance is null, create one.
                if (_instance == null)
                    _instance = new T();

                return _instance;
            }
        }

        /// <summary>
        /// Hide the constructor.
        /// </summary>
        protected Singleton()
        {

        }

        // Singleton instance.
        protected static T _instance;
    }
}