using UnityEngine;

public static class LogHandler
{
    private const string WhiteColorCode = "#FFFFFF";
    private const string BlueColorCode = "#2CD3E1";
    private const string YellowColorCode = "#FAE392";
    
    public enum Color
    {
        White,
        Blue,
        Yellow
    }

    private static string GetColorCode(Color color)
    {
        return color switch
        {
            Color.White => WhiteColorCode,
            Color.Blue => BlueColorCode,
            Color.Yellow => YellowColorCode,
            _ => WhiteColorCode
        };
    }

    public static void Log(string message, Color color) => Debug.Log($"<color={GetColorCode(color)}>{message}</color>");
    public static void LogError(string message, GameObject go = null) => Debug.LogError(message, go);
    public static void LogWarning(string message, GameObject go = null) => Debug.LogWarning(message, go);
}