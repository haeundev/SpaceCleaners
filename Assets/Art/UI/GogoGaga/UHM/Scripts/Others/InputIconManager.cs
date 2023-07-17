using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GogoGaga.UHM
{
    public class InputIconManager : MonoBehaviour
    {

        public TMP_SpriteAsset KeyboardLight;
        public TMP_SpriteAsset KeyboardDark;



        public InputIconData[] KeyboardData;

        Dictionary<KeyCode, InputIconData> InputIconsDic = new Dictionary<KeyCode, InputIconData>();


        void Awake()
        {
            for (int i = 0; i < KeyboardData.Length; i++)
            {
                InputIconsDic.Add(KeyboardData[i].Key, KeyboardData[i]);
            }

        }

        public string GetIconCode(KeyCode keycode)
        {
            string val = "<sprite=";

            if (InputIconsDic.ContainsKey(keycode))
            {
                val += InputIconsDic[keycode].Index;
                val += ">";
            }
            else
                val = "";

            return val;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Joystick2Button4))
                Debug.Log("asasa");
        }
    }
[System.Serializable]
public class InputIconData
{
    public int Index;
    public KeyCode Key;
}
}
