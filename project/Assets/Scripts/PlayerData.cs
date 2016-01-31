using System.Collections.Generic;

public class PlayerData {

    public int playerID = 0;
    public List<Challenge> challenges;
    public float xPos = -7.37f; //X location of player
    public float yPos = 0; //Y location of player
    public PlayerCharacter character;

    public PlayerData(int playerID)
    {
        this.playerID = playerID;
        challenges = new List<Challenge>();
    }
}
