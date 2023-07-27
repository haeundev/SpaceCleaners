using System;
using System.Collections.Generic;

namespace DevFeatures.SaveSystem.Model
{
    public interface ISavable
    {
        public string RelativePath { get; }
    }
    
    [Serializable]
    public class GameStat : ISavable
    {
        public string RelativePath => "/game-stat.json";
        public bool isDoneTutorial;
        public bool isWatchedOpeningCutscene;
    }
    
    [Serializable]
    public class PlayerStat : ISavable
    {
        public string RelativePath => "/player-stat.json";
        public string name; // agent name
        public int level; // agent level 1~7

        public PlayerStat(string name, int level)
        {
            this.name = name;
            this.level = level;
        }

        public PlayerStat() { }

        // public PlayerStat()
        // {
        //     var gameConst = DataTableManager.GameConst.Data;
        //     name = $"{gameConst.AgentNames.PeekRandom()} {gameConst.AgentNumbers.PeekRandom()} {gameConst.AgentGenerations.PeekRandom()}";
        //     level = 1;
        // }
    }
 
    [Serializable]
    public class GadgetStat : ISavable
    {
        public string RelativePath => "/gadget-stat.json";
        public SortedDictionary<int, int> Gadgets; // id, level
        public int lastSelectedID;

        public GadgetStat()
        {
            Gadgets = new();
        }
    }
 
    [Serializable]
    public class InventoryStat : ISavable
    {
        public string RelativePath => "/inventory-stat.json";
        public Dictionary<int, int> Items; // id, count

        public InventoryStat()
        {
            Items = new();
        }
    }
}