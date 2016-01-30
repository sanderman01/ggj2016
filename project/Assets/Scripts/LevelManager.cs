using UnityEngine;
using System.Collections.Generic;
using System;

public class LevelManager : MonoBehaviour {
	public Dictionary<GameObject, int> LevelPart =	new Dictionary<GameObject, int>();
    public List<Challenge> ChallengeList = new List<Challenge>();

    void Start()
    {
        AddToChallengeList(LevelPart);
    }

    private void AddToChallengeList(Dictionary<GameObject, int> levelPart)
    {
        throw new NotImplementedException();
    }
}
