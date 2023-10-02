using UnityEngine;

public static class LogHandler
{
    public static void Log(string message, string color = "#FFFFFF")
    {
        Debug.Log($"<color={color}>{message}</color>");
    }
}