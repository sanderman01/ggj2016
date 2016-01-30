using System.Collections.Generic;
using UnityEngine;

public class LevelPart : MonoBehaviour { 
    private GameObject levelObject;
    public int chance;
    public int size;
    public List<Challenge> challenge = new List<Challenge>();

    void start()
    {
        levelObject = gameObject;
    }
}
