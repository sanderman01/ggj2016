using UnityEngine;
using System.Collections;

public class Loki : MonoBehaviour {

    public enum Mood
    {
        Neutral,
        Angry,
        Happy
    }

    public SpriteRenderer renderer;
    public Sprite neutralSprite;
    public Sprite angrySprite;
    public Sprite happySprite;

    private float angryTime = 0f;
    private float happyTime = 0f;

    public float angryDuration = 1f;
    public float happyDuration = 1f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        float seconds = Time.deltaTime;

        //By default, Loki is neutral
        Mood newMood = Mood.Neutral;

        //Is Loki still happy?
        if (happyTime > 0)
        {
            happyTime -= seconds;
            if (happyTime > 0) //Yep, still happy
            {
                newMood = Mood.Happy;
            }
        }

        //Is Loki still angry?
        if (angryTime > 0)
        {
            angryTime -= seconds;
            if (angryTime > 0) //Yep, still angry
            {
                newMood = Mood.Angry;
            }
        }

        SetMood(newMood, false);
	}

    public void SetMood(Mood targetMood, bool refresh)
    {
        Sprite newSprite = neutralSprite;
        switch(targetMood)
        {
            case Mood.Angry:
                newSprite = angrySprite;
                if (refresh) angryTime = angryDuration;
                break;
            case Mood.Happy:
                newSprite = happySprite;
                if (refresh) happyTime = happyDuration;
                break;
        }

        renderer.sprite = newSprite;
    }
}
