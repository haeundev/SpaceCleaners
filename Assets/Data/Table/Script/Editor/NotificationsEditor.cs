using UnityEditor;
using System;
using LiveLarson.DataTableManagement.DataSheet.Editor;

namespace DataTables
{
    [CustomEditor(typeof(Notifications))]
    public class NotificationsEditor : DataScriptEditor
    {
        public override string FileID => "15EwIIfOuPGi2uXNPlZmv2U4EUB5YY30if30UxfryNGY";
        public override string SheetName => "Notification";
        public override DataScript.DataType DataType => DataScript.DataType.Table;
        public override Type SubClassType => typeof(Notification);

        public override void SetAssetData(string json)
        {
            var obj = target as Notifications;
            obj.Values = DataScript.MakeJsonListObjectString<Notification>(json).values;
        }
    }
}

