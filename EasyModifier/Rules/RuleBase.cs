using System;
using System.Collections.Generic;
using System.Text;

namespace EasyModifier.Rules
{
    public class RuleBase
    {
        private string key;
        private string replace;
        private int p;
        
        public static string SplitString { 
            get { 
                return "/---\\";
            }
        }

        public static string NullString
        {
            get
            {
                return "0NULL0";
            }
        }

        protected bool Found(string searchIn, string keyword)
        {
            if (searchIn == null)
            {
                return false;
            }
            if (keyword == null)
            {
                return false;
            }
            if (searchIn.ToLower().IndexOf(keyword.ToLower()) != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool FoundAtFirst(string searchIn, string keyword)
        {
            if (searchIn == null)
            {
                return false;
            }
            if (keyword == null)
            {
                return false;
            }
            if (searchIn.ToLower().StartsWith(keyword.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool FoundAtLast(string searchIn, string keyword)
        {
            if (searchIn == null)
            {
                return false;
            }
            if (keyword == null)
            {
                return false;
            }
            if (searchIn.ToLower().EndsWith(keyword.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
