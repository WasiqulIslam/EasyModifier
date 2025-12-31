using System;
using System.Collections.Generic;
using System.Text;

//developing by Wasiqul Islam at 7th June, 2013

namespace EasyModifier.Rules
{
    public class FindReplaceRule : RuleBase, IRule
    {

        private string key;
        private string find1;
        private string find2;
        private string replace;

        public string Description
        {
            get
            {
                switch (key)
                {
                    case "????>":
                        return String.Format("Find \"{0}\" in line and halt if found. Then find \"{1}\" and replace all text after the end of found word with \"{2}\".", find1, find2, replace);
                    case "<????":
                        return String.Format("Find \"{0}\" in line and halt if found. Then find \"{1}\" and replace all text before the beginning of found word with \"{2}\".", find1, find2, replace);
                    case "+????":
                        return String.Format("Find \"{0}\" in line and halt if found. Then find \"{1}\" and append \"{2}\" at the beginning of the found word.", find1, find2, replace);
                    case "????+":
                        return String.Format("Find \"{0}\" in line and halt if found. Then find \"{1}\" and append \"{2}\" at the end of found word.\"{2}\".", find1, find2, replace);
                    case "?????":
                        return String.Format("Find \"{0}\" in line and halt if found. Then find \"{1}\" and replace with \"{2}\".", find1, find2, replace);
                    default:
                        return "Unknown";
                }
            }
        }

        public string ProcessLine(string singleLine, ref RuleResponse signal)
        {
            return Finder(singleLine, ref signal);
        }

        public string Finder(string singleLine, ref RuleResponse signal)
        {
            if( find1 == null || Found(singleLine, find1) ) //if find1 is null then assume that its found already
            {
                if (find2 != null && Found(singleLine, find2))
                {
                    signal = RuleResponse.End;
                    switch (key)
                    {
                        case "????>":
                            return ReplaceAtEnd(singleLine, ref signal);
                        case "<????":
                            return ReplaceAtBeginning(singleLine, ref signal);
                        case "+????":
                            return AppendAtBeginning(singleLine, ref signal);
                        case "????+":
                            return AppendAtEnd(singleLine, ref signal);
                        case "?????":
                            return ReplaceExact(singleLine, ref signal);
                    }
                    throw new Exception("Invalid key found");
                }
                else
                {
                    signal = RuleResponse.End;
                    return singleLine;
                }
            }
            else
            {
                signal = RuleResponse.Continue;
                return singleLine;
            }
        }

        private string ReplaceAtEnd(string singleLine, ref RuleResponse signal)
        {
            int index = singleLine.IndexOf(find2);
            string firstPart = singleLine.Substring(0, index + find2.Length);
            string lastPart = singleLine.Substring(index + find2.Length);
            if (FoundAtFirst(lastPart, replace))
            {
                return singleLine;
            }
            else
            {
                return firstPart + replace;
            }
        }

        private string ReplaceAtBeginning(string singleLine, ref RuleResponse signal)
        {
            int index = singleLine.IndexOf(find2);
            string firstPart = singleLine.Substring(0, index);
            string lastPart = singleLine.Substring(index);
            if (FoundAtLast(firstPart, replace))
            {
                return singleLine;
            }
            else
            {
                return replace + lastPart;
            }
        }

        private string AppendAtEnd(string singleLine, ref RuleResponse signal)
        {
            int index = singleLine.IndexOf(find2);
            string firstPart = singleLine.Substring(0, index + find2.Length);
            string lastPart = singleLine.Substring(index + find2.Length);
            if (FoundAtFirst(lastPart, replace))
            {
                return singleLine;
            }
            else
            {
                return firstPart + replace + lastPart;
            }
        }

        private string AppendAtBeginning(string singleLine, ref RuleResponse signal)
        {
            int index = singleLine.IndexOf(find2);
            string firstPart = singleLine.Substring(0, index);
            string lastPart = singleLine.Substring(index);
            if (FoundAtLast(firstPart, replace))
            {
                return singleLine;
            }
            else
            {
                return firstPart + replace + lastPart;
            }
        }

        private string ReplaceExact(string singleLine, ref RuleResponse signal)
        {
            return singleLine.Replace(find2, replace);
        }

        public bool IsKeyMatched(string key)
        {
            switch (key)
            {
                case "????>":
                case "<????":
                case "+????":
                case "????+":
                case "?????":
                    return true;
                default:
                    return false;
            }
        }

        public bool IsLineMatched(string singleLine)
        {
            if (find1 == null || Found(singleLine, find1)) //if find1 is null then assume that its found already
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public FindReplaceRule(string key, string find1, string find2, string replace)
        {
            if (!IsKeyMatched(key))
            {
                throw new Exception("Key is invalid");
            }
            this.key = key;
            this.find1 = find1;
            this.find2 = find2;
            this.replace = replace;
            this.SortIndex = find1.Length;
        }

        public int SortIndex { get; set; }

        public void Reset()
        {
            //do nothing
        }

    }
}
