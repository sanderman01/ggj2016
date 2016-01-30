using UnityEngine;
using System.Collections;

public class FloorHider : MonoBehaviour {

	// Use this for initialization
	void Start () {
        SpriteRenderer sr = (SpriteRenderer)this.gameObject.GetComponent("SpriteRenderer");
	    if (!Static.SHOW_FLOORS)
        {
            sr.enabled = false;
        }
        else
        {
            Object test = Resources.Load("redbox2");
            Debug.Log(test);
            Sprite spr = Resources.Load<Sprite>("redbox2");
            sr.sprite = spr;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
