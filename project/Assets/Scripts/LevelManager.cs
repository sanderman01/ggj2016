using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Model;
using System;

public class LevelManager : MonoBehaviour {
    public const int levelLength = 100;
	public List<LevelPart> levelParts =	new List<LevelPart>();
    public List<Challenge> challengeList = new List<Challenge>();
    public List<LevelPart> activeLevelParts = new List<LevelPart>();

    void Start()
    {
        CreateLevel(levelParts);
        AddToChallengeList(activeLevelParts);
    }

    private void AddToChallengeList(List<LevelPart> activeLevelParts)
    {
        throw new NotImplementedException();
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
            int r = UnityEngine.Random.Range(0, count);
            LevelPart newLevelPart = temp[r];
            generated += newLevelPart.size;
            activeLevelParts.Add(newLevelPart);
        }
    }
}
