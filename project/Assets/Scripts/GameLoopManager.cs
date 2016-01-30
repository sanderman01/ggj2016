using UnityEngine;
using System.Collections.Generic;

public class GameLoopManager : MonoBehaviour {

    const float CHALLENGE_REMOVAL_LIMIT = 5f;

    public Loki loki;

    public int playerCount = 4;
    public GameObject playerPrefab;
    public List<PlayerData> playerDatas;
    public float movementPerSecond = 4f;
    bool running = false;

    private int combo = 0;
    private int happyCombo = 15; //The required combo to make Loki happy

	// Use this for initialization
	void Start () {
        InitializeGame(4);
        StartGame();
	}
	
	// Update is called once per frame
	void Update () {
        if (running)
        {
            float seconds = Time.deltaTime;

            //Evaluate player levels
            for (int i = 0; i < playerCount; i++)
            {
                PlayerData data = playerDatas[i];

                //Handle challenges
                for (int j = 0; j < data.challenges.Count; j++)
                {
                    Challenge challenge = data.challenges[j];
                    //Change time
                    challenge.timeLeftUntilInput -= seconds;
                    challenge.timeLeftUntilJudgment -= seconds;
                    
                    //Set position of challenge object
                    SetChallengePosition(challenge, i);

                    if (challenge.requiredInput != Challenge.InputType.None)
                    {
                        //Check for failure
                        if (!challenge.failed)
                        {
                            if (challenge.timeLeftUntilJudgment <= 0)
                            {
                                if (!challenge.cleared)
                                {
                                    challenge.Fail();
                                    loki.SetMood(Loki.Mood.Angry, true);
                                    combo = 0;
                                }
                            }
                        }

                        //Check for input
                        if (!challenge.failed && !challenge.cleared)
                        {
                            if (challenge.timeLeftUntilInput <= 0)
                            {
                                Static.LogOnceVerbose("Challenge timing!", "challenge");
                                if (CheckIfInputEntered(i, challenge.requiredInput))
                                {
                                    challenge.Clear();
                                    combo++;
                                    if (combo >= happyCombo) loki.SetMood(Loki.Mood.Happy, true);
                                }
                            }
                        }

                        //Remove outdated objects
                        if (challenge.timeLeftUntilJudgment <= -CHALLENGE_REMOVAL_LIMIT)
                        {
                            data.challenges.Remove(challenge);
                            j--; //Make sure the removal doesn't affect the for loop
                        }
                    }
                }
            }
        }
	}

    void InitializeGame(int playerCount = 4)
    {
        this.playerCount = playerCount;
        playerDatas = new List<PlayerData>();
        for(int i = 0; i < playerCount; i++)
        { 
            //Create player
            PlayerData playerData = new PlayerData(i);
            playerData.yPos = i * 4 - 4;
            GameObject character = (GameObject)Instantiate(playerPrefab, new Vector3(playerData.xPos,playerData.yPos + 1f,0), Quaternion.identity);
            PlayerCharacter charaScript = (PlayerCharacter)character.GetComponent("PlayerCharacter");
            charaScript.playerID = i;

            //Generate level
            LevelManager lm = FindObjectOfType(typeof(LevelManager)) as LevelManager;
            lm.CreateNewLevel(playerData.yPos);
            foreach(Challenge c in lm.challengeList)
            {
                playerData.challenges.Add(c);
            }
                       
            //Store player data
            playerDatas.Add(playerData);
        }
    }

    void StartGame()
    {
        running = true;
        Static.Log("Game started!");
    }

    bool CheckIfInputEntered(int playerID, Challenge.InputType inputType)
    {
        int bn = playerID + 1; //Button number; 1-based, as opposed to playerID which is 0-based
        if (Input.GetButton("Jump" + bn) && inputType == Challenge.InputType.Jump) return true;
        return false;
    }

    void SetChallengePosition(Challenge challenge, int playerID)
    {
        GameObject go = challenge.attachedGameObject;
        if (go != null)
        {
            float targetX = playerDatas[playerID].xPos; //At judgment time, its pos is equal to player pos
            targetX += challenge.timeLeftUntilJudgment * movementPerSecond; //Move it away from player depending on judgment time
            //targetX -= challenge.xOffset; //Adjust for location on object where challenge occurs
            go.transform.localPosition = new Vector3(targetX, go.transform.localPosition.y, go.transform.localPosition.z);
        }
        else
        {
            Static.WarningOnce("Null object on challenge for player " + playerID, "nullobject");
        }
    }
}
