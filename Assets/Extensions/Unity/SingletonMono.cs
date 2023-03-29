using UnityEngine;

namespace Extensions.Unity
{
    public abstract class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null) _instance = Instantiate(MonoPrefabContainer.Instance.GetPrefab<T>());
                }

                return _instance;
            }
        }
    }
}