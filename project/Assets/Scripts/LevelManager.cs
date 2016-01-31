using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
    public const int levelLength = 100;
	public List<LevelPart> levelParts =	new List<LevelPart>();
    public List<Challenge> challengeList = new List<Challenge>();
    public List<LevelPart> activeLevelParts = new List<LevelPart>();
    public LevelPart emptySection;

    public void CreateNewLevel(float yPos, bool starter = true)
    {
        CreateLevel(levelParts, yPos, starter);
        AddToChallengeList(activeLevelParts);
    }

    private void AddToChallengeList(List<LevelPart> activeLevelParts)
    {
        int totalSize=-5; //Start earlier with the empty sections
        for(int i=0;i<activeLevelParts.Count;i++)
        {
            foreach(Challenge c in activeLevelParts[i].challenge)
            {
                c.attachedGameObject = activeLevelParts[i].gameObject;
                float timer = 0.25f * totalSize;//TODO:zit in deze regel de bug? Opnieuw checken na slape 
                
                c.timeLeftUntilInput += timer;
                c.timeLeftUntilJudgment += timer;
                c.timeLeftUntilObjectGone += timer;
                challengeList.Add(c);
            }
            totalSize += activeLevelParts[i].size;
        }
    }

    private void CreateLevel(List<LevelPart> levelParts, float yPos, bool starter = true)
    {
        activeLevelParts.Clear();
        challengeList.Clear();
        List<LevelPart> temp = new List<LevelPart>();
        int generated=0;
        int count=0;
        foreach(LevelPart lp in levelParts)
        {
            count += lp.chance;
            for(int i=0;i<lp.chance;i++)
            {
                temp.Add(lp);
            }
        }

        //Add starter level parts
        if (starter)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject g = (GameObject)Instantiate(emptySection.gameObject, new Vector3(100, yPos, 0), Quaternion.identity);
                LevelPart newLevelPart = (LevelPart)g.GetComponent("LevelPart");
                generated += newLevelPart.size;
                g.transform.parent = newLevelPart.transform;
                activeLevelParts.Add(newLevelPart);
            }
        }

        //Add random level parts
        while (generated < levelLength)
        {
            int r = 0;
            if (generated > 10)
            {
                r = Random.Range(0, count);
            }         
            GameObject g = (GameObject)Instantiate(temp[r].gameObject, new Vector3(100, yPos, 0), Quaternion.identity);
            LevelPart newLevelPart = (LevelPart)g.GetComponent("LevelPart");
            generated += newLevelPart.size;
            g.transform.parent = newLevelPart.transform;
            activeLevelParts.Add(newLevelPart);
        }
    }
}
