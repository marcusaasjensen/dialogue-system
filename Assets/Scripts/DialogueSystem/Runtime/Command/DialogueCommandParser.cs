//Inspired by Scott Steffes: https://github.com/markv12/VertexTextAnimationDemo/tree/master/Assets/Scripts
//Modified by Marcus Aas Jensen

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using DialogueSystem.Data;
using UnityEngine;
using Utility;

namespace DialogueSystem.Runtime.Command
{
    public abstract class DialogueCommandParser
    {
        // grab the remainder of the text until ">" or end of string
        private const string RemainderRegex = "(.*?((?=>)|(/|$)))";
        
        private const string PauseRegexString = "<p:(?<pause>" + RemainderRegex + ")>";
        private static readonly Regex PauseRegex = new (PauseRegexString);
        private const string SpeedRegexString = "<sp:(?<speed>" + RemainderRegex + ")>";
        private static readonly Regex SpeedRegex = new (SpeedRegexString);
        
        private const string ValueRegexString = "<val:(?<value>" + RemainderRegex + ")>";
        private static readonly Regex ValueRegex = new (ValueRegexString);
        
        private const string EmotionRegexString = "<em:(?<emotion>" + RemainderRegex + ")>";
        private static readonly Regex EmotionRegex = new (EmotionRegexString);
        
        private const string AnimStartRegexString = @"<anim:(?<anim>[^\s>]+)(?:\s+s:(?<s>\d+(?:[\.,]\d+)?)\s*)?(?:\s+a:(?<a>\d+(?:[\.,]\d+)?)\s*)?(?:\s+sync:(?<sync>true|false)\s*)?>";
        private static readonly Regex AnimStartRegex = new (AnimStartRegexString, RegexOptions.IgnoreCase);
        private const string AnimEndRegexString = "</anim>";
        private static readonly Regex AnimEndRegex = new (AnimEndRegexString, RegexOptions.IgnoreCase);
        
        private const string MusicStartRegexString = "<music:(?<music>" + RemainderRegex + ")>";
        private static readonly Regex MusicStartRegex = new (MusicStartRegexString);
        private const string MusicEndRegexString = "</music>";
        private static readonly Regex MusicEndRegex = new (MusicEndRegexString);
        
        private const string SoundRegexString = "<sfx:(?<sfx>" + RemainderRegex + ")>";
        private static readonly Regex SoundRegex = new (SoundRegexString);
        
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
                    {
                        i++;
                    }
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
        
        private static T GetValue<T>(string stringVal, string typeName = "Type")
        {
            T result;
            try
            {
                result = (T)Enum.Parse(typeof(T), stringVal, true);
            }
            catch (ArgumentException)
            {
                LogHandler.Alert($"Invalid {typeName}: {stringVal}");
                result = default;
            }

            return result;
        }

        private static int VisibleCharactersUpToIndex(string message, int index)
        {
            var result = 0;
            for (var i = 0; i < index; i++)
            {
                var insideBrackets = false;
                switch (message[i])
                {
                    case '<':
                        insideBrackets = true;
                        break;
                    case '>':
                        result--;
                        break;
                    default:
                        continue;
                }

                if (!insideBrackets)
                {
                    result++;
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
            processedMessage = HandleMusicStartTags(processedMessage, result);
            processedMessage = HandleMusicEndTags(processedMessage, result);
            processedMessage = HandleSoundTags(processedMessage, result);
            processedMessage = HandleAnimTags(processedMessage, result);

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
                    StartPosition = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    Type = DialogueCommandType.DisplayedEmotion,
                    EmotionValue = GetValue<Emotion>(stringVal, "Emotion"),
                    MustExecute = true
                });
            }

            processedMessage = Regex.Replace(processedMessage, EmotionRegexString, "");
            return processedMessage;
        }

        public static string ReplaceVariableTagsByValue(string processedMessage)
        {
            var valueMatches = ValueRegex.Matches(processedMessage);

            foreach (Match match in valueMatches)
            {
                var variableName = match.Groups["value"].Value;
                var value = DialogueVariableData.Instance.GetValueAsString(variableName);

                if (string.IsNullOrEmpty(value))
                {
                    value = "X";
                }

                processedMessage = Regex.Replace(processedMessage, match.Value, value);
            }

            return processedMessage;
        }

        private static string HandleAnimTags(string processedMessage, ICollection<CommandData> result)
        {
            var animStartMatches = AnimStartRegex.Matches(processedMessage);
            var animEndMatches = AnimEndRegex.Matches(processedMessage);
            var matchesToIgnore = new List<Match>();
            
            foreach (Match match in animStartMatches)
            {
                var stringVal = match.Groups["anim"].Value;
                var speedVal = match.Groups["s"].Value;
                var amountVal = match.Groups["a"].Value;
                var syncVal = match.Groups["sync"].Value;
                
                var endIndex = -1;
                
                foreach (Match endMatch in animEndMatches)
                {
                    if (endMatch.Index <= match.Index && matchesToIgnore.Contains(endMatch))
                    {
                        continue;
                    }
                    endIndex = endMatch.Index;
                    matchesToIgnore.Add(endMatch);
                    break;
                }
                
                var floatValues = string.IsNullOrEmpty(speedVal) || string.IsNullOrEmpty(amountVal)
                    ? null
                    : new[] {
                        float.Parse(speedVal.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture),
                        float.Parse(amountVal.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture)
                    };

                result.Add(new CommandData
                {
                    StartPosition = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    EndPosition = VisibleCharactersUpToIndex(processedMessage, endIndex),
                    Type = DialogueCommandType.Animation,
                    TextAnimValue = GetValue<TextAnimationType>(stringVal, "Text Animation Type"),
                    MustExecute = true,
                    FloatValues = floatValues,
                    BoolValues = new []{bool.Parse(syncVal)}
                });
            }

            processedMessage = Regex.Replace(processedMessage, AnimStartRegexString, "");
            processedMessage = Regex.Replace(processedMessage, AnimEndRegexString, "");
            return processedMessage;
        }
        
        private static string HandleMusicStartTags(string processedMessage, ICollection<CommandData> result)
        {
            MatchCollection musicStartMatches = MusicStartRegex.Matches(processedMessage);
            foreach (Match match in musicStartMatches)
            {
                var stringVal = match.Groups["music"].Value;
                result.Add(new CommandData
                {
                    StartPosition = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    Type = DialogueCommandType.MusicStart,
                    StringValue = stringVal,
                    MustExecute = true
                });
            }

            processedMessage = Regex.Replace(processedMessage, MusicStartRegexString, "");
            return processedMessage;
        }

        private static string HandleSoundTags(string processedMessage, ICollection<CommandData> result)
        {
            MatchCollection musicStartMatches = SoundRegex.Matches(processedMessage);
            foreach (Match match in musicStartMatches)
            {
                var stringVal = match.Groups["sfx"].Value;
                result.Add(new CommandData
                {
                    StartPosition = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    Type = DialogueCommandType.SoundEffect,
                    StringValue = stringVal,
                    MustExecute = true
                });
            }

            processedMessage = Regex.Replace(processedMessage, SoundRegexString, "");
            return processedMessage;
        }
        
        private static string HandleMusicEndTags(string processedMessage, ICollection<CommandData> result)
        {
            MatchCollection musicEndMatches = MusicEndRegex.Matches(processedMessage);
            foreach (Match match in musicEndMatches)
            {
                result.Add(new CommandData
                {
                    StartPosition = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    Type = DialogueCommandType.MusicEnd
                });
            }

            processedMessage = Regex.Replace(processedMessage, MusicEndRegexString, "");
            return processedMessage;
        }

        private static string HandleSpeedTags(string processedMessage, ICollection<CommandData> result)
        {
            var speedMatches = SpeedRegex.Matches(processedMessage);
            foreach (Match match in speedMatches)
            {
                var stringVal = match.Groups["speed"].Value;

                if (!float.TryParse(stringVal, out var val))
                {
                    val = 0f;
                }

                result.Add(new CommandData
                {
                    StartPosition = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    Type = DialogueCommandType.Speed,
                    FloatValues = new [] { val }
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
                Debug.Assert(PauseDictionary.ContainsKey(val), "no pause registered for '" + val + "'");
                result.Add(new CommandData
                {
                    StartPosition = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    Type = DialogueCommandType.Pause,
                    FloatValues = new []{ PauseDictionary[val] }
                });
            }

            processedMessage = Regex.Replace(processedMessage, PauseRegexString, "");
            return processedMessage;
        }
    }
}