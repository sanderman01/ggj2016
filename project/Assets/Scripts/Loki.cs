using UnityEngine;

public class Loki : MonoBehaviour {

    public enum Mood
    {
        Neutral,
        Angry,
        Happy
    }

    public SpriteRenderer lokiRenderer;
    public SpriteRenderer textRenderer;
    public Sprite neutralSprite;
    public Sprite angrySprite;
    public Sprite happySprite;
    public Sprite text;
    public Sprite angryText;

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
        Sprite newText = text;
        switch(targetMood)
        {
            case Mood.Angry:
                newSprite = angrySprite;
                newText = angryText;
                if (refresh) angryTime = angryDuration;
                break;
            case Mood.Happy:
                newSprite = happySprite;
                if (refresh) happyTime = happyDuration;
                break;
        }

        lokiRenderer.sprite = newSprite;
        textRenderer.sprite = newText;
    }
}
