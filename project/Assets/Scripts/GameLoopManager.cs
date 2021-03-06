﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameLoopManager : MonoBehaviour {

    const float CHALLENGE_REMOVAL_LIMIT = 5f; //Once a challenge has passed for this long, it is destroyed
    const float CHALLENGE_ADDITION_LIMIT = 10f; //There needs to be at least X seconds of challenges; if not, generate new ones
    const float RITUAL_REQUIREMENT = 10f;

    public Loki loki;
    public Text timerText;
    public LokiProgress lokiProgress;

    public int playerCount = 4;
    public GameObject playerPrefab;
    public List<PlayerData> playerDatas;
    public float movementPerSecond = 4f;
    bool running = false;

    private int combo = 0;
    private float ritualTime = 0f;
    private int happyCombo = 15; //The required combo to make Loki happy
    private float extraDetectionDistance = 0.7f; //The distance from the center of the sprite at which judgment should start
    private float timer = 0f;

	// Use this for initialization
	void Start () {
        InitializeGame(4);
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
            timer += seconds;

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
                    challenge.timeLeftUntilObjectGone -= seconds;

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
                                    data.character.StartStumbling();
                                    loki.SetMood(Loki.Mood.Angry, true);
                                    combo = 0;
                                }
                            }
                        }
                    }
                    //Remove outdated objects
                    if (challenge.timeLeftUntilJudgment <= -CHALLENGE_REMOVAL_LIMIT)
                    {
                        DestroyChallenge(challenge, i);
                        j--; //Make sure the removal doesn't affect the for loop
                    }
                }
                //Dancing
                bool dancingInput = Input.GetAxis(string.Format("Ritual{0}Left", i + 1)) > 0.9 && Input.GetAxis(string.Format("Ritual{0}Right", i + 1)) > 0.9;
                if (charac.CurrentState == PlayerCharacter.CharacterState.Running && dancingInput)
                {
                    playerDatas[i].ritualCasting = true;
                    charac.StartDancing();
                }
                else if (charac.CurrentState == PlayerCharacter.CharacterState.Dancing && !dancingInput)
                {
                    playerDatas[i].ritualCasting = false;
                    charac.StopDancing();
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
            if(casters == playerCount)
            {
                ritualTime += seconds;
                Static.VerboseLog("ALL Praise the Loki " + ritualTime);

                VictoryCheck();
            }
            //Loki feedback
            lokiProgress.ShowProgress(ritualTime / RITUAL_REQUIREMENT);

            //Make sure there is enough level left
            for (int i = 0; i < playerCount; i++)
            {
                int challengeIndex = playerDatas[i].challenges.Count - 1;
                Challenge finalChallenge = playerDatas[i].challenges[challengeIndex];
                if (finalChallenge.timeLeftUntilJudgment < CHALLENGE_ADDITION_LIMIT)
                {
                    //Generate more level
                    GenerateLevel(playerDatas[i], finalChallenge.timeLeftUntilObjectGone);
                }
            }
        }

        //Update timer
        float tMinutes = Mathf.Floor(timer / 60);
        float tSeconds = timer - tMinutes * 60;
        tSeconds = (float)System.Math.Round(tSeconds, 2);
        string secondsText = System.Convert.ToString(tSeconds).Length > 1 ? System.Convert.ToString(tSeconds) : System.Convert.ToString(tSeconds) + "0";
        if (tSeconds < 10) secondsText = "0" + secondsText;
        timerText.text = System.Convert.ToString(tMinutes) + ":" + secondsText;

	}

    public void InitializeGame(int playerCount = 4)
    {
        Static.Log("Initializing game for " + playerCount + " players.");

        timer = 0f;
        ritualTime = 0f;

        //Remove existing challenges and objects and shizzle
        if (playerDatas != null)
        {
            for (int i = 0; i < playerDatas.Count; i++)
            {
                if (playerDatas[i].character != null)
                    Destroy(playerDatas[i].character.gameObject);
                for(int j = 0; j < playerDatas[i].challenges.Count; j++)
                {
                    DestroyChallenge(playerDatas[i].challenges[j], playerDatas[i].playerID);
                    j--; //Make sure for loop keeps working despite removal of element
                }
            }
        }

        //Generate new stuff
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
            GenerateLevel(playerData, 0);

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

    void DestroyChallenge(Challenge challenge, int playerID)
    {
        if (playerDatas.Count > playerID)
        {
            PlayerData data = playerDatas[playerID];
            if (data != null)
            {
                data.challenges.Remove(challenge);
            }
        }

        Destroy(challenge.attachedGameObject);
        Destroy(challenge.gameObject);
    }

    void GenerateLevel(PlayerData playerData, float offset)
    {
        LevelManager lm = FindObjectOfType(typeof(LevelManager)) as LevelManager;
        bool starter = true;
        if (offset > 0) starter = false;
        lm.CreateNewLevel(playerData.yPos, starter);
        foreach (Challenge c in lm.challengeList)
        {
            c.timeLeftUntilInput += offset;
            c.timeLeftUntilJudgment += offset;
            c.timeLeftUntilObjectGone += offset;
            playerData.challenges.Add(c);
        }
    }

    void VictoryCheck()
    {
        if (ritualTime > RITUAL_REQUIREMENT && running)
        {
            running = false;
            Menus.Victory();
        }
    }
}
