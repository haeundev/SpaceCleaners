using UnityEditor;
using System;
using LiveLarson.DataTableManagement.DataSheet.Editor;

namespace DataTables
{
    [CustomEditor(typeof(GameConst))]
    public class GameConstEditor : DataScriptEditor
    {
        public override string FileID => "15EwIIfOuPGi2uXNPlZmv2U4EUB5YY30if30UxfryNGY";
        public override string SheetName => "GameConst";
        public override DataScript.DataType DataType => DataScript.DataType.Const;
        public override Type SubClassType => typeof(GameConst.DataClass);

        public override void SetAssetData(string json)
        {
            var obj = target as GameConst;
            obj.Data = DataScript.MakeObjectFromJsonString<GameConst.DataClass>(json);
        }
    }
}

