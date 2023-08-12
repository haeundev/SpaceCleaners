using UnityEditor;
using System;
using LiveLarson.DataTableManagement.DataSheet.Editor;

namespace DataTables
{
    [CustomEditor(typeof(DialogueInfos))]
    public class DialogueInfosEditor : DataScriptEditor
    {
        public override string FileID => "15EwIIfOuPGi2uXNPlZmv2U4EUB5YY30if30UxfryNGY";
        public override string SheetName => "DialogueInfo";
        public override DataScript.DataType DataType => DataScript.DataType.Table;
        public override Type SubClassType => typeof(DialogueInfo);

        public override void SetAssetData(string json)
        {
            var obj = target as DialogueInfos;
            obj.Values = DataScript.MakeJsonListObjectString<DialogueInfo>(json).values;
        }
    }
}

