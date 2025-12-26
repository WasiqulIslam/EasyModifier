using System;
using System.Collections.Generic;
using System.Text;
using EasyModifier.Rules;

//Developing by Wasiqul Islam at 22nd May, 2012
//Updating at 6th June, 2013

namespace EasyModifier.Utils
{
    public class RuleGroup
    {

        private List<IRule> rules;

        private int currentIndex = 0;

        public int SortIndex { get; set; }

        public void Reset()
        {
            this.currentIndex = 0;
            for (int i = 0; i < rules.Count; i++)
            {
                rules[i].Reset();
            }
        }

        public string ProcessLine(string singleLine, ref RuleResponse signal)
        {
            IRule currentRule = rules[currentIndex];
            string processedLine = currentRule.ProcessLine(singleLine, ref signal);
            if (signal == RuleResponse.End || signal == RuleResponse.GoBackAndEnd)
            {
                if (frmMain.logDetailMessages)
                {
                    frmMain.LogIt(Environment.NewLine + "Successfully ended rule: " + Environment.NewLine + Rules[currentIndex].Description);
                }
                rules[currentIndex].Reset();
                currentIndex++;
            }
            if (currentIndex > rules.Count - 1)
            {
                Reset();
                if (signal == RuleResponse.GoBack || signal == RuleResponse.GoBackAndEnd)
                {
                    signal = RuleResponse.GoBackAndEnd; 
                }
                else
                {
                    signal = RuleResponse.End;
                }
            }
            else
            {
                if (signal == RuleResponse.GoBack || signal == RuleResponse.GoBackAndEnd)
                {
                    signal = RuleResponse.GoBack; 
                }
                else
                {
                    signal = RuleResponse.Continue;
                }
            }
            return processedLine;
        }

        public bool isFirstRuleMatches(string singleLine)
        {
            return rules[0].IsLineMatched(singleLine);
        }

        public RuleGroup(IRule rule)
        {
            if (rule == null)
            {
                throw new Exception("First rule is null");
            }
            if (!(rule is FindReplaceRule))
            {
                throw new Exception("First rule must be a Find-Replace rule in a rule group");
            }
            this.SortIndex = (rule as FindReplaceRule).SortIndex;
            AddRule(rule);
        }

        public List<IRule> Rules
        {
            get
            {
                return rules;
            }
        }

        public void AddRule( IRule rule )
        {
            if (rules == null)
            {
                rules = new List<IRule>();
            }
            rules.Add(rule);
        }

    }
}
