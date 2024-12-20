﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace CC.DialogueSystem
{
    public class Conversation
    {
        public enum Types
        {
            DEFAULT,
            BACKGROUND
        }

        [JsonIgnore] public Types ConversationType;
        public string Type; // Type from JSON
        [JsonProperty("conversation")] public List<Dialogue> Dialogues = new();
        public List<DialogueAction> Actions = new();

        // Setup anything we need to do here before validating
        public void PreValidation()
        {
            // Set the conversation type
            switch (Type?.ToLower())
            {
                case "background":
                    ConversationType = Types.BACKGROUND;
                    break;
            }

            // Set the defaults for each type. Only using an extra switch in case an added conversation in the future has a lot of settings to change
            switch (ConversationType)
            {
                case Types.BACKGROUND:
                    // Stop validation warning
                    foreach (var diag in Dialogues)
                        diag.AutoProceed = true;
                    break;
            }
        }

        // Let child classes know we've finished parsing and do any casts/prep needed
        public void FinishedParsing()
        {
            foreach (var diag in Dialogues)
                diag.FinishedParsing();
        }
    }

    #region Dialogue

    public class Dialogue
    {
        public int Id;
        public int NextId = -1;
        public string SpeakersName;
        public string CharacterSpritesName;
        public string StartingSprite;
        public string Theme;
        public bool AutoProceed;
        public bool CanBeUsedAsStartingPoint = true;
        public string AnchorObject; // The optional path to the object that's talking in the background.

        public List<Condition> StartConditions = new();
        public List<string> Sentences = new();
        public List<Option> Options = new();
        [JsonProperty("OnFinishActions")] public List<string> OnFinishedActionNames = new();

        // Let the conditions know they're ok to precast anything that needs it
        public void FinishedParsing()
        {
            foreach (var con in StartConditions)
                con.FinishedParsing();
        }

        // Evalues the starting conditions and return whether they all passed or one or more failed
        public bool EvaluateStartingConditions()
        {
            foreach (var con in StartConditions)
                if (!con.Evaluate())
                    return false;

            return true;
        }
    }

    public class Option
    {
        public int NextId = -1;
        public string Text;

        [JsonProperty("selectedActions")] public List<string> SelectedActionNames = new();
    }

    #endregion

    #region Actions

    public class DialogueAction
    {
        public enum Types
        {
            LOG,
            LOG_WARNING,
            LOG_ERROR,
            CLOSE_CONVERSATION,
            SEND_MESSAGE,
            CHANGE_THEME,
            CLOSE_BG_CONVERSATIONS,
            START_BG_CONVERSATION
        }

        public string Name;
        public string Type; // Type from JSON

        // Used like the value in the custom tags. e.g. for the logging actions, this is the message, and for the sendmessage, this is the message to send
        public string Message;
        public string Target;

        [JsonIgnore] public Types ActionType;

        // Parse type. Done this was instead of using JSONConverter to spit out an error that's a bit more helpful
        public bool GetActionType()
        {
            // Figure type
            switch (Type.ToLower())
            {
                case "log":
                    ActionType = Types.LOG;
                    break;

                case "log_warning":
                case "log warning":
                case "logwarning":
                    ActionType = Types.LOG_WARNING;
                    break;

                case "log_error":
                case "log error":
                case "logerror":
                    ActionType = Types.LOG_ERROR;
                    break;


                case "close_conversation":
                case "close conversation":
                case "closeconversation":
                    ActionType = Types.CLOSE_CONVERSATION;
                    break;

                case "send_message":
                case "send message":
                case "sendmessage":
                    ActionType = Types.SEND_MESSAGE;
                    break;

                case "change_theme":
                case "change theme":
                case "changetheme":
                    ActionType = Types.CHANGE_THEME;
                    break;

                case "close_bg_conversations":
                case "close bg conversations":
                case "closebgconversations":
                    ActionType = Types.CLOSE_BG_CONVERSATIONS;
                    break;

                case "start_bg_conversation":
                case "start bg conversation":
                case "startbgconversation":
                    ActionType = Types.START_BG_CONVERSATION;
                    break;

                default:
                    DialogueLogger.LogError($"Unsupported action type {Type} found in action with the name {Name}.");
                    break;
            }

            return true;
        }
    }

    #endregion

    #region Conditions

    public class Condition
    {
        public List<Variable> Variables = new();
        public string Comparison;

        private IComparison _comparer;

        // Cast any varaibles needed and get the comparer
        public void FinishedParsing()
        {
            getComparer();
            castVariables();
        }

        // Tell the all of the conditions to pre cast the values if not in the repo
        private void castVariables()
        {
            // Cast variables
            foreach (var variale in Variables)
                variale.Cast();
        }

        // Get the right implementation of IComparison from the Comparison variable
        private void getComparer()
        {
            switch (Comparison)
            {
                case ">":
                    _comparer = new GreaterThan();
                    break;
                case "<":
                    _comparer = new LessThan();
                    break;
                case ">=":
                    _comparer = new GreateOrEqualTo();
                    break;
                case "<=":
                    _comparer = new LessToEqualTo();
                    break;
                case "==":
                    _comparer = new EqualTo();
                    break;
                case "!=":
                    _comparer = new NotEqualTo();
                    break;
                default:
                    DialogueLogger.LogError($"Unsupported comparison operator {Comparison} used");
                    break;
            }
        }

        // Test the variables according to the comparison string
        public bool Evaluate()
        {
            // Check the comparer
            if (_comparer == null)
            {
                DialogueLogger.LogError("Trying to evaluate a condition, but the comparison operator is null");
                return false;
            }

            // Get values
            var var1 = Variables[0].GetValue();
            var var2 = Variables[1].GetValue();

            if (var1 == null || var2 == null)
            {
                DialogueLogger.LogWarning($"Trying to evaluate a condition but one or more of the variables are null");
                return false;
            }

            // Evaluate
            // We're using dynamics here, so be careful. We'll have to validate types on the conversation load
            return _comparer.Execute(var1, var2);
        }
    }

    public class Variable
    {
        public bool FromRepo = false;
        public string Name;
        public string Value;
        public string Type;

        private object _castValue;

        // Retrieve from the repo or return the precast value - if we know the type
        public T GetValue<T>()
        {
            if (FromRepo)
                return VariableRepo.Instance.Retrieve<T>(Name);

            if (_castValue == null)
                DialogueLogger.LogError("Trying to retrieve a variable value for comparison that is null");

            return (T)_castValue;
        }

        // Do we need to know the type when retrieving for evaluation?
        public object GetValue()
        {
            if (FromRepo)
                return VariableRepo.Instance.Retrieve(Name);

            if (_castValue == null)
                DialogueLogger.LogError("Trying to retrieve a variable value for comparison that is null");

            return _castValue;
        }

        // Pre cast the variables for quicker retrieval
        public void Cast()
        {
            if (!FromRepo)
                switch (Type.ToLower())
                {
                    case "short":
                        _castValue = short.Parse(Value);
                        break;
                    case "int":
                        _castValue = int.Parse(Value);
                        break;
                    case "long":
                        _castValue = long.Parse(Value);
                        break;
                    case "float":
                        _castValue = float.Parse(Value);
                        break;
                    case "bool":
                        _castValue = bool.Parse(Value);
                        break;
                    case "string":
                        _castValue = Value;
                        break;
                    default:
                        DialogueLogger.LogError($"Unsupported type {Type} using in variable");
                        break;
                }
        }
    }

    #endregion

    // TODO: Refactor?

    #region Condition Comparison Implementations

    public interface IComparison
    {
        bool Execute(dynamic var1, dynamic var2);
    }

    // >
    public class GreaterThan : IComparison
    {
        public bool Execute(dynamic var1, dynamic var2)
        {
            return var1 > var2;
        }
    }

    // <
    public class LessThan : IComparison
    {
        public bool Execute(dynamic var1, dynamic var2)
        {
            return var1 < var2;
        }
    }

    // >=
    public class GreateOrEqualTo : IComparison
    {
        public bool Execute(dynamic var1, dynamic var2)
        {
            return var1 >= var2;
        }
    }

    // <=
    public class LessToEqualTo : IComparison
    {
        public bool Execute(dynamic var1, dynamic var2)
        {
            return var1 <= var2;
        }
    }

    // ==
    public class EqualTo : IComparison
    {
        public bool Execute(dynamic var1, dynamic var2)
        {
            return var1 == var2;
        }
    }

    // != 
    public class NotEqualTo : IComparison
    {
        public bool Execute(dynamic var1, dynamic var2)
        {
            return var1 != var2;
        }
    }

    #endregion
}