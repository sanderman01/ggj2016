﻿using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
    public const int levelLength = 100;
	public List<LevelPart> levelParts =	new List<LevelPart>();
    public List<Challenge> challengeList = new List<Challenge>();
    public List<LevelPart> activeLevelParts = new List<LevelPart>();

    public void CreateNewLevel()
    {
        CreateLevel(levelParts);
        AddToChallengeList(activeLevelParts);
    }

    private void AddToChallengeList(List<LevelPart> activeLevelParts)
    {
        int totalSize=0;
        for(int i=0;i<activeLevelParts.Count;i++)
        {
            foreach(Challenge c in activeLevelParts[i].challenge)
            {
                c.attachedGameObject = activeLevelParts[i].levelObject;
                float timer = 0.25f * totalSize;
                c.timeLeftUntilInput += timer;
                c.timeLeftUntilJudgment += timer;
                challengeList.Add(c);
            }
            totalSize += activeLevelParts[i].size;
        }
    }

    private void CreateLevel(List<LevelPart> levelParts)
    {
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
        while(generated <levelLength)
        {
            int r = Random.Range(0, count);
            LevelPart newLevelPart = temp[r];
            generated += newLevelPart.size;
            GameObject g = (GameObject) Instantiate(newLevelPart.gameObject, new Vector3(100, 100, 100), Quaternion.identity);
            g.transform.parent = gameObject.transform;
            activeLevelParts.Add(newLevelPart);
        }
    }
}
