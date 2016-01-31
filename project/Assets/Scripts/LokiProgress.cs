using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LokiProgress : MonoBehaviour {

    public Text textField;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ShowProgress(float percentage)
    {
        string toWrite = "I am waiting...";
        if (percentage < 0.01f) toWrite = "Dance for me, mortals!";
        else if (percentage < 0.1f) toWrite = "Dance harder! I don't have all day!";
        else if (percentage < 0.2f) toWrite = "Keep dancing!";
        else if (percentage < 0.3f) toWrite = "Yes, this is more like it.";
        else if (percentage < 0.4f) toWrite = "Go on. Continue dancing, mortals!";
        else if (percentage < 0.5f) toWrite = "Yes, yes! Keep dancing!";
        else if (percentage < 0.6f) toWrite = "Good. I can enjoy this ritual.";
        else if (percentage < 0.7f) toWrite = "More glory to me! Keep it up!";
        else if (percentage < 0.8f) toWrite = "Great! A dance befitting of my glory!";
        else if (percentage < 0.9f) toWrite = "YES! I enjoy this greatly!";
        else if (percentage < 1.0f) toWrite = "Wonderful! I am almost satisfied!";
        else if (percentage >= 1.0f) toWrite = "EXCELLENT! You mortals have amused me!";
        textField.text = toWrite;
    }
}
