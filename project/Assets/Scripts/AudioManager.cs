using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

    public List<AudioClip> audioClips;
    public static AudioManager instance = null;

	// Use this for initialization
	void Start () {
        AudioManager.instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static bool PlaySoundStatic(string name)
    {
        if(instance == null)
        {
            Static.WarningOnce("AudioManager instance not set.", "noaudiomanager");
            return false;
        }
        return instance.PlaySound(name);
    }

    public bool PlaySound(string name)
    {
        Static.VerboseLog("Playing sound: " + name);
        AudioClip target = null;
        for(int i = 0; i < audioClips.Count; i++)
        {
            if (audioClips[i].name == name) target = audioClips[i];
        }
        if (target == null)
        {
            Static.WarningOnce("Missing sound: " + name, "missingsound" + name);
            return false;
        }

        AudioSource.PlayClipAtPoint(target, Camera.main.transform.position, 1f); //Camera.main.transform.localPosition);

        return true;
    }
}
