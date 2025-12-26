using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using EasyModifier.Rules;
using System.Linq;

//Developing by Wasiqul Islam at 7th June, 2013

namespace EasyModifier.Utils
{
    public class RuleManager
    {

        public static bool ruleAdded = false;
        private static List<RuleGroup> allGroups;

        public static string RULE_GROUP_START_TAG = "----->>>>>";

        public static List<RuleGroup> AllGroups
        {
            get
            {
                return allGroups;
            }
        }
        
        public static void AddRulesFromSettings()
        {

            if (ruleAdded)
            {
                throw new Exception("Rules are already added.");
            }

            if (!File.Exists("settings.txt"))
            {
                throw new Exception("The file settings.txt is not found.");
            }

            allGroups = new List<RuleGroup>();

            RuleGroup ruleGroup = null;

            StreamReader reader = new StreamReader("settings.txt",Encoding.Default);
            String singleLine = "";
            int lineNo = 1;

            try
            {

                IRule currentRule = null;
                while (true)
                {

                    singleLine = reader.ReadLine();
                    if (singleLine == null )
                    {
                        break;
                    }
                    if (singleLine.Length < 5)
                    {
                        continue;
                    }

                    if (singleLine == RULE_GROUP_START_TAG)
                    {
                        if (ruleGroup != null)
                        {
                            allGroups.Add(ruleGroup);
                        }
                        ruleGroup = null;
                        continue;
                    }

                    string key = singleLine.Substring(0, 5);
                    string allData = singleLine.Substring(5);
                    if (new FindReplaceRule("?????", "", "", "").IsKeyMatched(key))
                    {
                        string[] splitted = allData.Split(new string[]{RuleBase.SplitString}, StringSplitOptions.None);
                        if( splitted.Length != 3 )
                        {
                            throw new Exception("Invalid find replace rule syntext at line:" + lineNo);
                        }
                        string find1 = null, find2 = null, replace = null;
                        if (splitted[0] != RuleBase.NullString)
                        {
                            find1 = splitted[0];
                        }
                        if (splitted[1] != RuleBase.NullString)
                        {
                            find2 = splitted[1];
                        }
                        if (splitted[2] != RuleBase.NullString)
                        {
                            replace = splitted[2];
                        }
                        currentRule = (IRule)(new FindReplaceRule(key, find1, find2, replace));                   
                    }
                    else if (new LineRule(">>>>>", "").IsKeyMatched(key))
                    {
                        string replace = null;
                        string[] splitted = allData.Split(new string[] { RuleBase.SplitString }, StringSplitOptions.None);
                        if (splitted.Length != 1 && splitted.Length != 3 )
                        {
                            throw new Exception("Invalid line rule syntext at line:" + lineNo);
                        }
                        int characterCount = 0;
                        string replaceCancelIf = null;
                        if (splitted.Length == 3)
                        {
                            if (splitted[1] != RuleBase.NullString)
                            {
                                replaceCancelIf = splitted[1];
                            }
                            characterCount = Convert.ToInt32(splitted[2]);
                        }
                        else
                        {
                            characterCount = 0;
                        }
                        if (splitted[0] != RuleBase.NullString)
                        {
                            replace = splitted[0];
                        }
                        currentRule = (IRule)(new LineRule(key, replace, replaceCancelIf, characterCount));
                        
                    }
                    else if (new GoBackLineRule("<----", "").IsKeyMatched(key))
                    {
                        string findFor = null;
                        if (allData != "")
                        {
                            if (allData != RuleBase.NullString)
                            {
                                findFor = allData;
                            }
                        }
                        currentRule = (IRule)(new GoBackLineRule(key, findFor));
                    }
                    else
                    {
                        throw new Exception("Rule is not a match at line:" + lineNo);
                    }

                    if (ruleGroup == null)
                    {
                        ruleGroup = new RuleGroup(currentRule);
                    }
                    else
                    {
                        ruleGroup.AddRule(currentRule);
                    } 

                    lineNo++;
                }

                if (ruleGroup != null)
                {
                    allGroups.Add(ruleGroup);
                }

            }
            finally
            {
                reader.Close();
            }

            //sorting the rules so that some exceptions will overcome
            allGroups = allGroups.OrderByDescending(x => x.SortIndex).ToList();

            ruleAdded = true;

        }

        public static string GetSettingsDescription()
        {
            if (!File.Exists("settings.txt"))
            {
                throw new Exception("The file settings.txt is not found.");
            }

            StringBuilder description = new StringBuilder("");
            description.Append( Environment.NewLine + Environment.NewLine + Environment.NewLine + "Current Settings Description:" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            foreach( RuleGroup ruleGroup in RuleManager.allGroups )
            {
                description.Append(Environment.NewLine + Environment.NewLine + Environment.NewLine + "New Rule Group:" + Environment.NewLine + Environment.NewLine);
                foreach (IRule rule in ruleGroup.Rules)
                {
                    description.Append(rule.Description + Environment.NewLine);
                }
            }
            return description.ToString();
        }

    }
}
