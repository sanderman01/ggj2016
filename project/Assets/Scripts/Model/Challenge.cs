using UnityEngine;

public class Challenge : MonoBehaviour {

    public enum InputType
    {
        None = 0,
        Jump = 1,
        Duck = 2
    }

    public GameObject attachedGameObject;
    public float xOffset = 0; //X coordinate offset from start of gameObject that the challenge is judged at

    public InputType requiredInput = InputType.None; //Button to push/action to take to clear this challenge
    public float timeLeftUntilInput = 0f; //Once this hits 0, you can start inputting the required command
    public float timeLeftUntilJudgment = 0.5f; //Once this hits 0, you will fail the challenge unless it is already cleared
    public float timeLeftUntilObjectGone = 1.25f; //hax
    public bool cleared = false; //Set to true once the user enters the right input
    public bool failed = false; //Set to true once the judgment timing expires and it's not cleared yet

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Clear()
    {
        cleared = true;
        AudioManager.PlaySoundStatic("success");
        Static.VerboseLog("Challenge cleared!");
    }

    public void Fail()
    {
        failed = true;
        AudioManager.PlaySoundStatic("fail");
        Static.VerboseLog("Challenge failed!");
    }
}
