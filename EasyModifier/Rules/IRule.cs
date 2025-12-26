using System;
using System.Collections.Generic;
using System.Text;

//Developing by Wasiqul Islam at 2nd June, 2013


namespace EasyModifier.Rules
{

    public enum RuleResponse
    {
        Continue,
        End,
        GoBack,
        GoBackAndEnd
    }

    /// <summary>
    /// All rules must inherit this interface
    /// </summary>
    public interface IRule
    {
        string ProcessLine(string singleLine, ref RuleResponse signal);
        bool IsKeyMatched(string key);
        bool IsLineMatched(string singleLine);
        int SortIndex { get; set; }
        string Description { get; }
        void Reset();
    }

}
