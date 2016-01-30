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
        if (Once(identifier))
        {
            Log(message);
        }
    }

    public static void LogOnceVerbose(string message, string identifier)
    {
        if (VERBOSE) LogOnce(message, identifier);
    }

    public static void Warning(string message)
    {
        Debug.LogWarning(message);
    }

    public static void WarningOnce(string message, string identifier)
    {
        if (Once(identifier))
        {
            Warning(message);
        }
    }

    static bool Once(string identifier)
    {
        if (alreadyLogged.Contains(identifier)) return false;
        alreadyLogged.Add(identifier);
        return true;
    }
}
