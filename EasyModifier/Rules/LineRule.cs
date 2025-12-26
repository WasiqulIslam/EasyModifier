using System;
using System.Collections.Generic;
using System.Text;

//developing by Wasiqul Islam at 7th June, 2013

namespace EasyModifier.Rules
{
    public class LineRule:RuleBase,IRule
    {

        private string key;
        private string replace;
        private string replaceCancelIf;
        private int characterCount = 0;

        public string ProcessLine(string singleLine, ref RuleResponse signal)
        {
            signal = RuleResponse.End;
            switch (key)
            {
                case ">>>>>":
                    if (!FoundAtLast(singleLine, replace))
                    {
                        if (characterCount <= 0)
                        {
                            return singleLine + replace;
                        }
                        else
                        {
                            return AdjustAtEnd(singleLine, ref signal);
                        }
                    }
                    else
                    {
                        return singleLine;
                    }
                case "<<<<<":
                    if( !FoundAtFirst( singleLine, replace ))
                    {
                        if (characterCount <= 0)
                        {
                            return replace + singleLine;
                        }
                        else
                        {
                            return AdjustAtBeginning(singleLine, ref signal);
                        }
                    }
                    else
                    {
                        return singleLine;
                    }
            }
            throw new Exception( "Invalid key found");
        }

        private string AdjustAtEnd(string singleLine, ref RuleResponse signal)
        {
            if (replaceCancelIf != null && Found(singleLine, replaceCancelIf))
            {
                return singleLine;
            }
            int index = singleLine.Length - 1;
            for (int i = singleLine.Length - 1; i >= 0; i--)
            {
                if (singleLine[i] == ' ' || singleLine[i] == '\t')
                {
                    index = i;
                    continue;
                }
                else
                {
                    break;
                }
            }
            string leftPart = singleLine.Substring(0, index + 1);
            int spaceCount = 0;
            if (leftPart.Length < (characterCount - replace.Length))
            {
                spaceCount = (characterCount - replace.Length) - leftPart.Length;
            }
            StringBuilder spaces = new StringBuilder("");
            for (int i = 0; i < spaceCount; i++)
            {
                spaces.Append(" ");
            }
            return leftPart + spaces.ToString() + replace;
        }

        private string AdjustAtBeginning(string singleLine, ref RuleResponse signal)
        {
            if (replaceCancelIf != null && Found( singleLine, replaceCancelIf ))
            {
                return singleLine;
            }
            int index = 0;
            for (int i = 0; i <= singleLine.Length - 1; i++)
            {
                if (singleLine[i] == ' ' || singleLine[i] == '\t')
                {
                    index = i;
                    continue;
                }
                else
                {
                    break;
                }
            }
            string rightPart = singleLine.Substring(index);
            int spaceCount = 0;
            if (rightPart.Length < (characterCount - replace.Length))
            {
                spaceCount = (characterCount - replace.Length) - rightPart.Length;
            }
            StringBuilder spaces = new StringBuilder("");
            for (int i = 0; i < spaceCount; i++)
            {
                spaces.Append(" ");
            }
            return replace + spaces.ToString() + rightPart;
        }

        public bool IsKeyMatched(string key)
        {
            switch (key)
            {
                case ">>>>>":
                case "<<<<<":
                    return true;
                default:
                    return false;
            }
        }

        public bool IsLineMatched(string singleLine)
        {
            return true;
        }

        public LineRule(string key, string replace, int characterCount)
        {
            if (!IsKeyMatched(key))
            {
                throw new Exception("Key is invalid");
            }
            this.key = key;
            this.replace = replace;
            this.characterCount = characterCount;
            this.replaceCancelIf = null;
        }

        public LineRule(string key, string replace, string replaceCancelf, int characterCount)
        {
            if (!IsKeyMatched(key))
            {
                throw new Exception("Key is invalid");
            }
            this.key = key;
            this.replace = replace;
            this.characterCount = characterCount;
            this.replaceCancelIf = replaceCancelf;
        }

        public LineRule(string key, string replace) : this(key, replace, null, 0 )
        {
        }

        public int SortIndex { 
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
                    case ">>>>>":
                        if (characterCount <= 0)
                        {
                            return String.Format("Go to next line. Add \"{0}\" to the end of line.", replace, characterCount);
                        }
                        else
                        {
                            string text = String.Format("Go to next line. Add \"{0}\" to the end of line using padding value \"{1}\".", replace, characterCount);
                            if (replaceCancelIf != null)
                            {
                                text += String.Format(" However don't do anything if \"{0}\" is found in that line.", replaceCancelIf);
                            }
                            return text;
                        }
                    case "<<<<<":
                        if (characterCount <= 0)
                        {
                            return String.Format("Go to next line. Add \"{0}\" to the beginning of line.", replace, characterCount);
                        }
                        else
                        {
                            string text = String.Format("Go to next line. Add \"{0}\" to the beginning of line using padding value \"{1}\".", replace, characterCount);
                            if (replaceCancelIf != null)
                            {
                                text += String.Format(" However don't do anything if \"{0}\" is found in that line.", replaceCancelIf);
                            }
                            return text;
                        }
                    default:
                        return "Unknown";
                }
            }
        }

        public void Reset()
        {
            //do nothing
        }

    }
}
