﻿using UnityEngine;
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
    private int ritualCount=0;
    private int happyCombo = 15; //The required combo to make Loki happy
    private float extraDetectionDistance = 0.7f; //The distance from the center of the sprite at which judgment should start

	// Use this for initialization
	void Start () {
        InitializeGame(1);
        happyCombo = 4 * playerCount;
        //StartGame();       
	}
    void OnGUI()
    {
        Reset();
    }
    private void Reset()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(0);
        }
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
                PlayerCharacter charac = playerDatas[i].character;

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
                        //Judge challenge
                        if (!challenge.failed && !challenge.cleared)
                        {
                            if (challenge.timeLeftUntilJudgment <= 0)
                            {

                                if (JudgePlayerState(challenge, i)) //SUCCESS
                                {
                                    challenge.Clear();
                                    combo++;
                                    if (combo >= happyCombo) loki.SetMood(Loki.Mood.Happy, true);
                                }
                                else //FAILURE
                                {
                                    challenge.Fail();
                                    loki.SetMood(Loki.Mood.Angry, true);
                                    combo = 0;
                                }
                            }
                        }
                    }
                    bool dancingInput = Input.GetAxis(string.Format("Ritual{0}Left", playerDatas[i].playerID + 1)) > 0.9 && Input.GetAxis(string.Format("Ritual{0}Right", playerDatas[i].playerID + 1)) > 0.9;
                    if (charac.CurrentState == PlayerCharacter.CharacterState.Running && dancingInput)
                    {
                        playerDatas[i].ritualCasting = true;
                        charac.StartDancing();
                    }
                    else if(charac.CurrentState == PlayerCharacter.CharacterState.Dancing && !dancingInput)
                    {
                        playerDatas[i].ritualCasting = false;
                        charac.StopDancing();
                    }

                    //Remove outdated objects
                    if (challenge.timeLeftUntilJudgment <= -CHALLENGE_REMOVAL_LIMIT)
                    {
                        data.challenges.Remove(challenge);
                        j--; //Make sure the removal doesn't affect the for loop
                    }
                }
            }
            int casters=0;
            foreach(PlayerData pd in playerDatas)
            {
                if(pd.ritualCasting)
                {
                    casters++;
                }
            }
            if(casters == 4)
            {
                ritualCount++;
                Debug.Log("ALL Praise the Loki " + ritualCount);
            }
        }
	}

    public void InitializeGame(int playerCount = 4)
    {
        this.playerCount = playerCount;
        playerDatas = new List<PlayerData>();
        for(int i = 0; i < playerCount; i++)
        { 
            //Create player
            PlayerData playerData = new PlayerData(i);
            playerData.yPos = i * 4f - 8f;
            GameObject character = (GameObject)Instantiate(playerPrefab, new Vector3(playerData.xPos,playerData.yPos + 1f,0), Quaternion.identity);
            PlayerCharacter charaScript = (PlayerCharacter)character.GetComponent("PlayerCharacter");
            charaScript.playerID = i;
            playerData.character = charaScript;

            //Generate level
            LevelManager lm = FindObjectOfType(typeof(LevelManager)) as LevelManager;
            lm.CreateNewLevel(playerData.yPos);
            foreach(Challenge c in lm.challengeList)
            {
                playerData.challenges.Add(c);
            }
                       
            //Store player data
            playerDatas.Add(playerData);

            //Fix positions
            for (int j = 0; j < playerData.challenges.Count; j++)
            {
                SetChallengePosition(playerData.challenges[j], i);
            }
        }
    }

    public void StartGame()
    {
        running = true;
        Static.Log("Game started!");
    }

    void SetChallengePosition(Challenge challenge, int playerID)
    {
        GameObject go = challenge.attachedGameObject;
        if (go != null)
        {
            float targetX = playerDatas[playerID].xPos + extraDetectionDistance; //At judgment time, its pos is equal to player pos
            targetX += challenge.timeLeftUntilJudgment * movementPerSecond; //Move it away from player depending on judgment time
            targetX -= challenge.xOffset; //Adjust for location on object where challenge occurs
            go.transform.localPosition = new Vector3(targetX, go.transform.localPosition.y, go.transform.localPosition.z);
        }
        else
        {
            Static.WarningOnce("Null object on challenge for player " + playerID, "nullobject");
        }
    }

    bool JudgePlayerState(Challenge challenge, int playerID)
    {
        //playerDatas[i].character._currentState
        PlayerCharacter.CharacterState state = playerDatas[playerID].character.CurrentState;
        switch(challenge.requiredInput)
        {
            case Challenge.InputType.Jump:
                if (state == PlayerCharacter.CharacterState.Jumping) return true;
                break;
            case Challenge.InputType.Duck:
                if (state == PlayerCharacter.CharacterState.Sliding) return true;
                break;
        }
        return false;
    }
}
