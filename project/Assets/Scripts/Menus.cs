using UnityEngine;
using System.Collections;

public class Menus : MonoBehaviour {
    public enum MenuState
    {
        Starting = 0,
        Playing = 1,
        GameOver = 2,
        Victory = 3
    }

    public GameLoopManager gameLoopManager;
    public SpriteRenderer titleSprite;
    public SpriteRenderer gameOverSprite;
    public SpriteRenderer victorySprite;
    public MenuState menuState = MenuState.Starting;

    private static Menus instance;

	// Use this for initialization
	void Start () {
        Menus.instance = this;
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
                else
                {
                    if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Alpha1)) gameLoopManager.InitializeGame(1);
                    if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Alpha2)) gameLoopManager.InitializeGame(2);
                    if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Alpha3)) gameLoopManager.InitializeGame(3);
                    if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Alpha4)) gameLoopManager.InitializeGame(4);
                }
                break;
            case MenuState.GameOver:
                gameOverSprite.enabled = true;
                break;
        }
	}

    public static void Victory()
    {
        instance.victorySprite.enabled = true;
        instance.menuState = MenuState.Victory;
    }
}
