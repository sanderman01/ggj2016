using UnityEngine;
using System.Collections.Generic;

public class PlayerData : MonoBehaviour {

    public int playerID = 0;
    public List<Challenge> challenges;

    public PlayerData(int playerID)
    {
        this.playerID = playerID;
        challenges = new List<Challenge>();
    }
}
