using UniRx;
using UnityEditor;
using UnityEngine;

namespace LiveLarson.DataTableManagement
{
    [ExecuteAlways]
    public class DataTableManager : MonoBehaviour
    {
        [Header("Example")] [Space] 
        // [SerializeField] private Items items;
        // [SerializeField] private Messages messages;
        // [SerializeField] private PlayerConst playerConst;

        private static DataTableManager _instance;

        public static DataTableManager Instance
        {
            get
            {
#if UNITY_EDITOR
                if (_instance == null)
                    _instance = AssetDatabase.LoadAssetAtPath<DataTableManager>(
                        "Assets/LiveLarson/DataTableManagement/DataTableManager.prefab");
#endif
                return _instance;
            }
            set => _instance = value;
        }

        public static readonly ReactiveProperty<bool> IsLoaded = new(false);

        private static bool _needToInit = true;

        private void Awake()
        {
            if (_needToInit)
                Init();
        }

        private void OnDestroy()
        {
            Debug.Log("DataTableManager OnDestroy!");

            Instance = null;
            _needToInit = true;
        }

        public void Init()
        {
            Debug.Log("DataTableManager Init!");

            _needToInit = false;
#if UNITY_EDITOR
            _instance = this;

            if (!Application.isPlaying)
            {
                Debug.Log("Application is not playing!");
            }
#endif
        }

        // public static Messages Messages => Instance.messages;
        // public static PlayerConst PlayerConst => Instance.playerConst;
        // public static Items Items => Instance.items;
    }
}