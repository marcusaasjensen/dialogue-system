//Inspired by Scott Steffes: https://github.com/markv12/VertexTextAnimationDemo/tree/master/Assets/Scripts
//Modified by Marcus Aas Jensen

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DialogueSystem.Data;
using UnityEngine;
using Utility;

namespace DialogueSystem.Runtime.CommandProcessor
{
    public abstract class DialogueCommandParser
    {
        // grab the remainder of the text until ">" or end of string
        private const string RemainderRegex = "(.*?((?=>)|(/|$)))";
        private const string PauseRegexString = "<p:(?<pause>" + RemainderRegex + ")>";
        private static readonly Regex PauseRegex = new Regex(PauseRegexString);
        private const string SpeedRegexString = "<sp:(?<speed>" + RemainderRegex + ")>";
        private static readonly Regex SpeedRegex = new Regex(SpeedRegexString);
        private const string ValueRegexString = "<val:(?<value>" + RemainderRegex + ")>";
        private static readonly Regex ValueRegex = new Regex(ValueRegexString);
        private const string EmotionRegexString = "<em:(?<emotion>" + RemainderRegex + ")>";
        private static readonly Regex EmotionRegex = new Regex(EmotionRegexString);
        private const string AnimStartRegexString = "<anim:(?<anim>" + RemainderRegex + ")>";
        private static readonly Regex AnimStartRegex = new Regex(AnimStartRegexString);
        private const string AnimEndRegexString = "</anim>";
        private static readonly Regex AnimEndRegex = new Regex(AnimEndRegexString);

        private static readonly Dictionary<string, float> PauseDictionary = new()
        {
            { "tiny", .1f },
            { "short", .25f },
            { "normal", .666f },
            { "long", 1f },
            { "read", 2f },
        };

        public static string RemoveSimpleTextTags(string message)
        {
            var result = string.Empty;
            var i = 0;

            while (i < message.Length)
            {
                if (message[i] == '<')
                {
                    while (i < message.Length && message[i] != '>')
                        i++;
                    i++;
                }
                else
                {
                    result += message[i];
                    i++;
                }
            }

            return result;
        }

        public static List<CommandData> Parse(string message, out string processedMessage)
        {
            var result = new List<CommandData>();
            processedMessage = message;

            processedMessage = ReplaceVariableTagsByValue(processedMessage);
            
            processedMessage = HandlePauseTags(processedMessage, result);
            processedMessage = HandleSpeedTags(processedMessage, result);
            processedMessage = HandleEmotionTags(processedMessage, result);
            processedMessage = HandleAnimStartTags(processedMessage, result);
            processedMessage = HandleAnimEndTags(processedMessage, result);

            return result;
        }

        private static string HandleEmotionTags(string processedMessage, ICollection<CommandData> result)
        {
            var emotionMatches = EmotionRegex.Matches(processedMessage);

            foreach (Match match in emotionMatches)
            {
                var stringVal = match.Groups["emotion"].Value;

                result.Add(new CommandData
                {
                    Position = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    Type = DialogueCommandType.DisplayedEmotion,
                    EmotionValue = GetEmotionValue(stringVal),
                    MustExecute = true
                });
            }

            processedMessage = Regex.Replace(processedMessage, EmotionRegexString, "");
            return processedMessage;
        }

        private static string ReplaceVariableTagsByValue(string processedMessage)
        {
            var valueMatches = ValueRegex.Matches(processedMessage);

            foreach (Match match in valueMatches)
            {
                var variableName = match.Groups["value"].Value;
                var value = DialogueVariables.Instance.GetValue(variableName);

                if (string.IsNullOrEmpty(value))
                    value = "X";

                processedMessage = Regex.Replace(processedMessage, match.Value, value);
            }

            return processedMessage;
        }

        private static string HandleAnimEndTags(string processedMessage, ICollection<CommandData> result)
        {
            var animEndMatches = AnimEndRegex.Matches(processedMessage);
            foreach (Match match in animEndMatches)
            {
                result.Add(new CommandData
                {
                    Position = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    Type = DialogueCommandType.AnimEnd,
                });
            }

            processedMessage = Regex.Replace(processedMessage, AnimEndRegexString, "");
            return processedMessage;
        }

        private static string HandleAnimStartTags(string processedMessage, ICollection<CommandData> result)
        {
            MatchCollection animStartMatches = AnimStartRegex.Matches(processedMessage);
            foreach (Match match in animStartMatches)
            {
                var stringVal = match.Groups["anim"].Value;
                result.Add(new CommandData
                {
                    Position = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    Type = DialogueCommandType.AnimStart,
                    TextAnimValue = GetTextAnimationType(stringVal)
                });
            }

            processedMessage = Regex.Replace(processedMessage, AnimStartRegexString, "");
            return processedMessage;
        }

        private static string HandleSpeedTags(string processedMessage, ICollection<CommandData> result)
        {
            var speedMatches = SpeedRegex.Matches(processedMessage);
            foreach (Match match in speedMatches)
            {
                var stringVal = match.Groups["speed"].Value;

                if (!float.TryParse(stringVal, out var val))
                    val = 0f;

                result.Add(new CommandData
                {
                    Position = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    Type = DialogueCommandType.TextSpeedChange,
                    FloatValue = val
                });
            }

            processedMessage = Regex.Replace(processedMessage, SpeedRegexString, "");
            return processedMessage;
        }

        private static string HandlePauseTags(string processedMessage, ICollection<CommandData> result)
        {
            var pauseMatches = PauseRegex.Matches(processedMessage);
            foreach (Match match in pauseMatches)
            {
                var val = match.Groups["pause"].Value;
                var pauseName = val;
                Debug.Assert(PauseDictionary.ContainsKey(pauseName), "no pause registered for '" + pauseName + "'");
                result.Add(new CommandData
                {
                    Position = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    Type = DialogueCommandType.Pause,
                    FloatValue = PauseDictionary[pauseName]
                });
            }

            processedMessage = Regex.Replace(processedMessage, PauseRegexString, "");
            return processedMessage;
        }

        private static Emotion GetEmotionValue(string stringVal)
        {
            Emotion result;
            try
            {
                result = (Emotion)Enum.Parse(typeof(Emotion), stringVal, true);
            }
            catch (ArgumentException)
            {
                LogHandler.LogError($"Invalid Emotion: {stringVal}");
                result = Emotion.Default;
            }

            return result;
        }

        private static TextAnimationType GetTextAnimationType(string stringVal)
        {
            TextAnimationType result;
            try
            {
                result = (TextAnimationType)Enum.Parse(typeof(TextAnimationType), stringVal, true);
            }
            catch (ArgumentException)
            {
                LogHandler.LogError($"Invalid Text Animation Type: {stringVal}");
                result = TextAnimationType.None;
            }

            return result;
        }

        private static int VisibleCharactersUpToIndex(string message, int index)
        {
            var result = 0;
            var insideBrackets = false;
            for (var i = 0; i < index; i++)
            {
                switch (message[i])
                {
                    case '<':
                        insideBrackets = true;
                        break;
                    case '>':
                        insideBrackets = false;
                        result--;
                        break;
                }

                if (!insideBrackets)
                {
                    result++;
                }
                else if (i + 6 < index && message.Substring(i, 6) == "sprite")
                {
                    result++;
                }
            }

            return result;
        }
    }
}