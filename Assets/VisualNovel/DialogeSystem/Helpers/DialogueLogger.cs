﻿using UnityEngine;

namespace CC.DialogueSystem
{
    public class DialogueLogger
    {
        private const string LogPrefix = "[DialogueSystem] ";

        public enum LogLevel
        {
            DEBUG,
            WARNING,
            ERROR,
            DISABLE
        }

        /**
         * This gets sets in the DialogueController via the inspector then passed here in the Awake().
         * Not too keen on the implementation, but it's the most central object to the dialogue system, and it's the first place I'd look.
         * Maybe if there's enough things that need setting we should make a settings class, the DialogueController works right now
         **/
        public static int CurrentLogLevel;

        public static void Log(string message)
        {
            if (CurrentLogLevel > 0)
                return;

            Debug.Log(LogPrefix + message);
        }

        public static void LogWarning(string message)
        {
            if (CurrentLogLevel > 1)
                return;

            Debug.LogWarning(LogPrefix + message);
        }

        public static void LogError(string message)
        {
            if (CurrentLogLevel > 2)
                return;
            
            Debug.LogError(LogPrefix + message);
        }
    }
}