using UnityEngine;
using System.Collections;

public class Challenge : MonoBehaviour {

    public enum InputType
    {
        Jump
    }

    public InputType requiredInput = InputType.Jump; //Button to push/action to take to clear this challenge
    public float timeLeftUntilInput = 9.5f; //Once this hits 0, you can start inputting the required command
    public float timeLeftUntilJudgment = 10f; //Once this hits 0, you will fail the challenge unless it is already cleared
    public bool cleared = false; //Set to true once the user enters the right input

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Clear()
    {

    }

    public void Fail()
    {

    }
}
