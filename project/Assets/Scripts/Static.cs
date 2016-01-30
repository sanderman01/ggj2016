using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Static {

    static bool VERBOSE = true;

    static List<string> alreadyLogged = new List<string>();
    

    public static void Log(string message)
    {
        Debug.Log(message);
    }

    public static void VerboseLog(string message)
    {
        if (VERBOSE) Log(message);
    }

    public static void LogOnce(string message, string identifier)
    {
        if (!alreadyLogged.Contains(identifier))
        {
            Log(message);
            alreadyLogged.Add(identifier);
        }
    }

    public static void LogOnceVerbose(string message, string identifier)
    {
        if (VERBOSE) LogOnce(message, identifier);
    }
}
