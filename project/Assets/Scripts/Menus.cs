using UnityEngine;
using System.Collections;

public class Menus : MonoBehaviour {
    public enum MenuState
    {
        Starting = 0,
        Playing = 1,
        GameOver = 2
    }

    public GameLoopManager gameLoopManager;
    public SpriteRenderer titleSprite;
    public SpriteRenderer gameOverSprite;
    public MenuState menuState = MenuState.Starting;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        titleSprite.enabled = false;
        gameOverSprite.enabled = false;
	    switch(menuState)
        {
            case MenuState.Starting:
                titleSprite.enabled = true;
                if (Input.GetButton("Confirm"))
                {
                    gameLoopManager.StartGame();
                    menuState = MenuState.Playing;
                }
                break;
            case MenuState.GameOver:
                gameOverSprite.enabled = true;
                break;
        }
	}
}
