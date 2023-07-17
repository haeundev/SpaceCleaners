﻿using UnityEngine;

namespace LiveLarson.Camera.Auto_Letterbox.Demos.Game_Demo.Scripts {

    public class GameManager : MonoBehaviour {
        public DragonClass player;
        public int meatToWin = 3;
        public int fruitToWin = 3;


        // Use this for initialization
        void Start() {



        }

        // Update is called once per frame
        void Update() {

            if (player.GetMeat() == meatToWin || player.GetFruit() == fruitToWin) {
                Debug.Log("You've Won Meat " + player.GetMeat() + " Fruit " + player.GetFruit());
            }

        }
    }
}