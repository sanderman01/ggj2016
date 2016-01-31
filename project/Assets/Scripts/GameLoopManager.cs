using UnityEngine;
using System.Collections.Generic;

public class GameLoopManager : MonoBehaviour {

    const float CHALLENGE_REMOVAL_LIMIT = 1.5f;

    public Loki loki;

    public int playerCount = 4;
    public GameObject playerPrefab;
    public List<PlayerData> playerDatas;
    public float movementPerSecond = 4f;
    bool running = false;

    private int combo = 0;
    private int happyCombo = 15; //The required combo to make Loki happy
    private float extraDetectionDistance = 0.7f; //The distance from the center of the sprite at which judgment should start

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

                    //Remove outdated objects
                    if (challenge.timeLeftUntilJudgment <= -CHALLENGE_REMOVAL_LIMIT)
                    {
                        DestroyChallenge(challenge, i);
                        j--; //Make sure the removal doesn't affect the for loop
                    }
                }
            }
        }
	}

    public void InitializeGame(int playerCount = 4)
    {
        Static.Log("Initializing game for " + playerCount + " players.");
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
}
