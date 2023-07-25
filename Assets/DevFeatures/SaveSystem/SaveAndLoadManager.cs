using System;
using System.IO;
using System.Linq;
using DevFeatures.SaveSystem.Model;
using LiveLarson.DataTableManagement;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DevFeatures.SaveSystem
{
    public class SaveAndLoadManager : MonoBehaviour
    {
        public static SaveAndLoadManager Instance;
        public GameStat GameStat { get; set; } = new();
        public PlayerStat PlayerStat { get; set; } = new();
        public GadgetStat GadgetStat { get; set; } = new();
        public InventoryStat InventoryStat { get; set; } = new();
        public bool IsFirstTime => GameStat.isWatchedOpeningCutscene == false;

        private readonly IDataService _dataService = new JsonDataService();
        [SerializeField] private bool encryptionEnabled;
        private long _saveTime;
        private long _loadTime;

        private void Awake()
        {
            Instance = this;
            LoadAll();
            
            Application.quitting += OnquittingApplication;
        }

        private void OnquittingApplication()
        {
            SaveAll();
        }

        public static void LoadAll()
        {
            Instance.GameStat = Instance.Load<GameStat>();
            Instance.GadgetStat = Instance.Load<GadgetStat>();
            Instance.PlayerStat = Instance.Load<PlayerStat>();
            Instance.InventoryStat = Instance.Load<InventoryStat>();
        }
        
        private void SaveAll()
        {
            try
            {
                Save(GameStat);
                Save(GadgetStat);
                Save(PlayerStat);
                Save(InventoryStat);
                Debug.Log($"[SAVE LOAD]  All Saved");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        [Button]
        public void Save(ISavable savable)
        {
            var startTime = DateTime.Now.Ticks;
            if (_dataService.SaveData(savable.RelativePath, savable, encryptionEnabled))
            {
                _saveTime = DateTime.Now.Ticks - startTime;
                // Debug.Log($"[SAVE LOAD]  Save Time: {_saveTime / TimeSpan.TicksPerMillisecond:N4} ms");
                startTime = DateTime.Now.Ticks;
                try
                {
                    var data = _dataService.LoadData<PlayerStat>(savable.RelativePath, encryptionEnabled);
                    _loadTime = DateTime.Now.Ticks - startTime;
                    Debug.Log($"[SAVE LOAD]  Loaded from file ({_loadTime / TimeSpan.TicksPerMillisecond:N4}ms): \r\n {JsonConvert.SerializeObject(data, Formatting.Indented)}");
                }
                catch (Exception e)
                {
                    Debug.LogError("[SAVE LOAD]  Could not read file!");
                }
            }
            else
            {
                Debug.LogError("[SAVE LOAD]  Could not save file!");
            }
        }

        [Button]
        private T Load<T>() where T : ISavable, new()
        {
            Debug.Log($"[SAVE LOAD]  Loading {typeof(T).Name}...");

            var relativePath = new T().RelativePath; // Create a new instance to access the relative path
            try
            {
                var data = _dataService.LoadData<T>(relativePath, encryptionEnabled);
                Debug.Log($"[SAVE LOAD]  Loaded {typeof(T).Name}: \r\n {JsonConvert.SerializeObject(data, Formatting.Indented)}");
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SAVE LOAD]  Could not read {typeof(T).Name} file! (Ignore error if first play)");
                return new T(); // Return a new instance of the type if loading fails
            }
        }

        [Button]
        public void ClearData(ISavable savable)
        {
            var path = Application.persistentDataPath + savable.RelativePath;
            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"[SAVE LOAD]  Deleted {savable.RelativePath}");
            }
        }

        public static bool IsPlayerCreated()
        {
            return string.IsNullOrEmpty(Instance.PlayerStat.name) == false;
        }

        public static void CreateNewPlayer(string name, int agentLevel)
        {
            Instance.GameStat = new GameStat();
            Instance.GadgetStat = CreateInitialGadgetStat();
            Instance.PlayerStat = new PlayerStat(name, agentLevel);
            Instance.InventoryStat = new InventoryStat();
            Instance.SaveAll();
        }

        private static GadgetStat CreateInitialGadgetStat()
        {
            var stat = new GadgetStat();
            foreach (var gadgetInfo in DataTableManager.GadgetInfos.Values.Where(p => p.HasFromStart))
                stat.Gadgets.Add(gadgetInfo.ID, 1);
            return stat;
        }
    }
}