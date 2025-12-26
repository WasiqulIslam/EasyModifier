using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyModifier.Rules
{
    public class GoBackLineRule : RuleBase, IRule
    {

        private string key;
        private string findFor = null;

        bool isNullFirstTime = true;

        public string ProcessLine(string singleLine, ref RuleResponse signal)
        {
            switch (key)
            {
                case "<----":
                    if (findFor == null)
                    {
                        signal = RuleResponse.GoBackAndEnd;
                    }
                    else
                    {
                        if (singleLine == null)
                        {
                            if (isNullFirstTime)
                            {
                                isNullFirstTime = false;
                                signal = RuleResponse.GoBack;
                            }
                            else
                            {
                                signal = RuleResponse.GoBackAndEnd;
                            }
                        }
                        else
                        {
                            if (Found(singleLine, findFor))
                            {
                                signal = RuleResponse.GoBackAndEnd;
                            }
                            else
                            {
                                signal = RuleResponse.GoBack;
                            }
                        }
                    }
                    return singleLine;
            }
            throw new Exception("Invalid key found");
        }

        public bool IsLineMatched(string singleLine)
        {
            return true;
        }

        public bool IsKeyMatched(string key)
        {
            switch (key)
            {
                case "<----":
                    return true;
                default:
                    return false;
            }
        }

        public int SortIndex
        {
            get
            {
                return 0;
            }
            set
            {

            }
        }

        public string Description
        {
            get
            {
                switch (key)
                {
                    case "<----":
                        if (findFor == null)
                        {
                            return String.Format("Go back one line.");
                        }
                        else
                        {
                            return String.Format("Go back line after line until \"{0}\" is found or at the first line of file.", findFor);
                        }
                    default:
                        return "Unknown";
                }
            }
        }

        public GoBackLineRule(string key, string findFor)
        {
            this.key = key;
            this.findFor = findFor;
        }

        public void Reset()
        {
            isNullFirstTime = true;
        }

    }
}
