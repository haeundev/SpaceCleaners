using UnityEditor;
using System;
using LiveLarson.DataTableManagement.DataSheet.Editor;

namespace DataTables
{
    [CustomEditor(typeof(AudioConst))]
    public class AudioConstEditor : DataScriptEditor
    {
        public override string FileID => "15EwIIfOuPGi2uXNPlZmv2U4EUB5YY30if30UxfryNGY";
        public override string SheetName => "AudioConst";
        public override DataScript.DataType DataType => DataScript.DataType.Const;
        public override Type SubClassType => typeof(AudioConst.DataClass);

        public override void SetAssetData(string json)
        {
            var obj = target as AudioConst;
            obj.Data = DataScript.MakeObjectFromJsonString<AudioConst.DataClass>(json);
        }
    }
}

