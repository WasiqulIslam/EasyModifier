using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EasyModifier.Rules;

//Developing by Wasiqul Islam at 22nd May, 2012
//Updating at 7th June 2013
//Updating at 17th June 2013

namespace EasyModifier.Utils
{
    public class Processor
    {

        RuleGroup currentRuleGroup = null;

        public Processor()
        {
            foreach (RuleGroup group in RuleManager.AllGroups)
            {
                group.Reset();
            }
        }

        public void ModifyFiles(string sourceFile, string outputFile)
        {

            StreamReader reader = null;
            StreamWriter writter = null;
            
            try
            {

                reader = new StreamReader(sourceFile, Encoding.Default);
                List<string> allLines = new List<string>();

                //save full file text into array
                string singleLine = "";
                while (true)
                {
                    singleLine = reader.ReadLine();
                    if (singleLine == null)
                    {
                        break;
                    }
                    else
                    {
                        allLines.Add(singleLine);
                    }
                }

                //Now process lines as specified
                RuleResponse signal = RuleResponse.Continue;
                singleLine = null;
                string outputLine = null;
                for (int i = 0; i <= allLines.Count; ) //here i is currentLine
                {
                    if (i < 0 || i >= allLines.Count)
                    {
                        singleLine = null;
                    }
                    else
                    {
                        singleLine = allLines[i];
                    }
                    outputLine = ProcessLine(singleLine, ref signal);
                    if (!(i < 0 || i >= allLines.Count))
                    {
                        allLines[i] = outputLine;
                    }
                    if (signal == RuleResponse.GoBack || signal == RuleResponse.GoBackAndEnd)
                    {
                        i--;
                    }
                    else
                    {
                        i++;
                    }
                }

                //finally write to output file
                writter = new StreamWriter(outputFile, false, Encoding.Default);
                for (int i = 0; i < allLines.Count;i++ ) //here i is currentLine
                {
                    writter.WriteLine(allLines[i]);
                }
                
            }
            catch (Exception exc)
            {
                frmMain.LogIt("Error modifying/processing file: " + sourceFile);
                frmMain.LogIt("Error details: " + exc.ToString());
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
                if (writter != null)
                {
                    writter.Close();
                }
            }

        }

        public string ProcessLine(string singleLine, ref RuleResponse signal)
        {

            if (currentRuleGroup == null)
            {
                foreach (RuleGroup group in RuleManager.AllGroups)
                {
                    if (group.isFirstRuleMatches(singleLine))
                    {
                        if (frmMain.logDetailMessages)
                        {
                            frmMain.LogIt(Environment.NewLine + "Activated Group with first rule: " + Environment.NewLine + group.Rules[0].Description);
                        }
                        currentRuleGroup = group;
                        break;
                    }
                }
            }

            if (currentRuleGroup == null)
            {
                return singleLine;
            }
            else
            {
                signal = RuleResponse.Continue;
                string processedLine = currentRuleGroup.ProcessLine(singleLine, ref signal);
                if (signal == RuleResponse.End || signal == RuleResponse.GoBackAndEnd)
                {
                    currentRuleGroup = null;
                }
                return processedLine;
            }

        }

    }
}
