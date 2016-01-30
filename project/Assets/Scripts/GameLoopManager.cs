using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameLoopManager : MonoBehaviour {

    const float CHALLENGE_REMOVAL_LIMIT = 5f;

    public int playerCount = 4;
    public List<PlayerData> playerDatas;
    public float movementPerSecond = 100f;
    bool running = false;

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
                    //TODO: Update position of challenge object

                    //Check for failure
                    if (!challenge.failed)
                    {
                        if (challenge.timeLeftUntilJudgment <= 0)
                        {
                            if (!challenge.cleared) challenge.Fail();
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

    void InitializeGame(int playerCount = 4)
    {
        this.playerCount = playerCount;
        playerDatas = new List<PlayerData>();
        for(int i = 0; i < playerCount; i++)
        {
            //Create player
            PlayerData playerData = new PlayerData(i);

            //Generate level
            Challenge challenge = new Challenge();
            challenge.requiredInput = Challenge.InputType.Jump;
            challenge.timeLeftUntilInput = 4.5f;
            challenge.timeLeftUntilJudgment = 5f;
            playerData.challenges.Add(challenge);
            
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
        GameObject go = challenge.gameObject;
        if (go != null)
        {
            float targetX = playerDatas[playerID].xPos; //At judgment time, its pos is equal to player pos
            targetX += challenge.timeLeftUntilJudgment *= movementPerSecond; //Move it away from player depending on judgment time
            targetX -= challenge.xOffset; //Adjust for location on object where challenge occurs
            go.transform.localPosition = new Vector3(targetX, go.transform.localPosition.y, go.transform.localPosition.z);
        }
        else
        {
            Static.WarningOnce("Null object on challenge for player " + playerID, "nullobject");
        }
    }
}
