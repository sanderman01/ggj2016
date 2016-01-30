﻿using UnityEngine;
using System.Collections.Generic;

public class PlayerData : MonoBehaviour {

    public int playerID = 0;
    public List<Challenge> challenges;
    public float xPos = 0; //X location of player

    public PlayerData(int playerID)
    {
        this.playerID = playerID;
        challenges = new List<Challenge>();
    }
}
