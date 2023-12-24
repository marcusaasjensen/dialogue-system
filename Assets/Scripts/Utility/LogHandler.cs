﻿using UnityEngine;

namespace Utility
{
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

        public static void Log<T>(T message) => Debug.Log($"<color={GetColorCode(Color.White)}>{message}</color>");
        public static void Log<T>(T message, Color color) => Debug.Log($"<color={GetColorCode(color)}>{message}</color>");
        public static void Alert(string message, GameObject go) => Debug.LogError(message, go);
        public static void Alert(string message) => Debug.LogError(message);
        public static void Warn(string message, GameObject go) => Debug.LogWarning(message, go);
        public static void Warn(string message) => Debug.LogWarning(message);
    }
}