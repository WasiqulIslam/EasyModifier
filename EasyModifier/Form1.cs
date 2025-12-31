using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using EasyModifier.Rules;
using EasyModifier.Utils;

//Developing by Wasiqul Islam at 22nd May, 2012
//Updating at 31st July 2013

//Note drag-drop is pending

namespace EasyModifier
{
    public partial class frmMain : Form
    {

        private string sourceFolder = "";
        private string destinationFolder = "";
        private FolderMonitor monitor;
        private bool overriteFileIfExists = false;

        public delegate void LogMessageDelegate(String message);
        private LogMessageDelegate logMessage;

        public static LogMessageDelegate LogIt;

        private string applicationName = "EasyModifier";
        private string applicationLabel = "Easy Modifier";
        private string applicationVersion = "";

        private bool programStoppedDueToError = false;
        public static bool logDetailMessages = false;

        int delay = 2000;
        
        private string helpText = "";

        public frmMain()
        {
            InitializeComponent();
        }

        private void lblHelp_Click(object sender, EventArgs e)
        {
            LogMessage(helpText);
        }

        private void grpDrag_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
            //if (e.Data.GetDataPresent(DataFormats.UnicodeText) || e.Data.GetDataPresent(DataFormats.Text))
            //{
            //    e.Effect = DragDropEffects.Link;
            //}
            //else
            //{
            //    e.Effect = DragDropEffects.None;
            //}
        }

        private void grpDrag_DragDrop(object sender, DragEventArgs e)
        {
            MessageBox.Show( e.Data.GetData(DataFormats.Text).ToString() );
        }


        private void frmMain_Load(object sender, EventArgs e)
        {

            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;

            this.Text = applicationLabel + " " + applicationVersion;
            applicationVersion = "v" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            AddHelpText();

            try
            {

                logMessage = LogMessageMethod; //attach the delegate
                LogIt = new LogMessageDelegate(LogMessage);  //note: LogIt is static

                RuleManager.AddRulesFromSettings();
                sourceFolder = ConfigurationManager.AppSettings["sourceFolder"];
                destinationFolder = ConfigurationManager.AppSettings["outputFolder"];
                if (!Directory.Exists(sourceFolder))
                {
                    LogMessage("Error: Source Folder not found at:" + sourceFolder);
                    SpeciyThatThisProgramIsNotWorking();
                    return;
                }
                if (!Directory.Exists(destinationFolder))
                {
                    try
                    {
                        Directory.CreateDirectory(destinationFolder);
                    }
                    catch
                    {
                        throw new Exception("Unable to create output folder");
                    }
                }
                DirectoryInfo sourceInfo = new DirectoryInfo(sourceFolder);
                DirectoryInfo destinationInfo = new DirectoryInfo(destinationFolder);
                if (sourceInfo.Root.FullName == destinationInfo.Root.FullName && sourceInfo.Name == destinationInfo.Name)
                {
                    LogMessage("Error: Source and destination folder is same.");
                    SpeciyThatThisProgramIsNotWorking();
                    return;
                }
                if (ConfigurationManager.AppSettings["overwriteFileIfExists"].ToLower() == "yes")
                {
                    overriteFileIfExists = true;
                }
                else
                {
                    overriteFileIfExists = false;
                }
                bool monitorCreation = false;
                if (ConfigurationManager.AppSettings["moniotorCreation"].ToLower() == "yes")
                {
                    monitorCreation = true;
                }
                bool monitorModification = false;
                if (ConfigurationManager.AppSettings["monitorModification"].ToLower() == "yes")
                {
                    monitorModification = true;
                }
                bool monitorRename = false;
                if (ConfigurationManager.AppSettings["monitorRename"].ToLower() == "yes")
                {
                    monitorRename = true;
                }
                monitor = new FolderMonitor(sourceFolder, monitorCreation, monitorModification, monitorRename);
                monitor.NewFileCreated += new NewFileDelegate(NewFileCreated);

                logDetailMessages = false; //logDetailMessages
                if (ConfigurationManager.AppSettings["logDetailMessages"].ToLower() == "yes")
                {
                    logDetailMessages = true;
                }

                try
                {
                    delay = (int)(Convert.ToDouble(ConfigurationManager.AppSettings["waitSecondsBeforeFileProcessing"]) * 1000);
                }
                catch
                {}
                
                LogMessage("Monitoring folder: " + sourceFolder);
                LogMessage("Modified files will be saved at: " + destinationFolder);

            }
            catch (Exception exc)
            {
                LogMessage(GetErrorString(exc));
                SpeciyThatThisProgramIsNotWorking();
                return;
            }

        }

        private void NewFileCreated(string filePath)
        {
                FileInfo info = new FileInfo(filePath);
                if (!info.Exists)  //if for some cases file does not exists then do nothing
                {
                    return;
                }
                if (info.Length > 1000000)
                {
                    LogMessage("File " + filePath + " is too big.");
                    return;
                }
                string filePath2 = Path.Combine(destinationFolder, info.Name);

                if (File.Exists(filePath2))
                {
                    if (!overriteFileIfExists)
                    {
                        LogMessage("Warning: File " + filePath2 + " already exists, so it is not modified.");
                        return;
                    }
                }
                try
                {
                    Thread.Sleep(2000); //wait for 2 second before deleting/modifying
                    ModifyFiles(filePath, filePath2);
                    File.Delete(filePath);
                    LogMessage("Success. File edited as: " + filePath2);
                }
                catch (Exception exception)
                {
                    LogMessage("Error in processing file" + filePath + ": " + exception.Message);
                }

        }

        public string GetErrorString(Exception e)
        {
            return "An error has occured:" + e.Message + ".";
        }
        
        public void ModifyFiles(string sourceFile, string outputFile)
        {
            
            Processor processor = new Processor();
            processor.ModifyFiles(sourceFile, outputFile);

        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure that you want to clear all logs?", "Confirmation", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                txtLog.Text = "";
            }
        }

        private void LogMessageMethod(string message)
        {
            txtLog.Text += message + Environment.NewLine;
        }

        private void LogMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(logMessage, message);
            }
            else
            {
                logMessage(message);
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!programStoppedDueToError)
            {
                if (!(MessageBox.Show("Are you sure that you want to close this application?", "Confirmation", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes))
                {
                    e.Cancel = true;
                    return;
                }
            }
            if (monitor != null)
            {
                monitor.StopMonitoring();
            }
        }

        private void SpeciyThatThisProgramIsNotWorking()
        {
            programStoppedDueToError = true;
            LogMessage("This program has stopped working for some errors. Please fix those and try to run this program again.");
        }

        private void lblDescribeSettings_Click(object sender, EventArgs e)
        {
            try
            {
                LogMessage(RuleManager.GetSettingsDescription());
            }
            catch (Exception exc)
            {
                LogMessage(GetErrorString(exc));
                return;
            }
        }

        private void AddHelpText()
        {

            this.Text = String.Format("{0} {1}", applicationLabel, applicationVersion);

            helpText = String.Format(@"
----------------------------------------------------------------------
Help/About:
{2} {1}
This software is developed by Wasiqul Islam
E-mail: islam.wasiqul@gmail.com
Country: Bangladesh
        
Short Notes:
1. Modify the {0}.exe.config xml file and change the ""sourceFolder"" and ""outputFolder"" key's value as needed. Make sure that those are different directory.
2. A setting.txt file must be there on the same directory of this application from which settings are loaded. This application will work according to the settings specified.

This program go through each line one after another and modifies the line as described in the settings.
Settings file contains some group of rules. A block of rule is called Rule-Group. In a Rule-Group some rules can be there within which first rule must be of search type.

When a new file is being processed this program go through each line and checks which RuleGroup matches
with this line. A rule group is matched with a line if the first rule of the rule group is matched with
that line. When a rule group is matched this program sets that rule group as active rule group and
matches all the rules of that rule group sequentially. When all rules are completed in that rule
group a new rule group is selected again as described previously. Continues until all line of that 
file ends. For a new file this process starts from the beginning.

Settings file modification instructions:

First line must contain text/characters: {3}. This indicates that a Rule-Group starts here.
Following should be some rules until the rule group ends.
To begin a new rule group add a new line that contains {3} again.
Then add some rules for this rule group.
Now, currently there are 2 types of rules: 1. Find and Replace Rule 2. Line Rule.
Find and replace rule go to each next line and finds for a match with some given characters.
Line Rule works only for the next line.
{4} is used as seperators in a line.
Example of a Find and Replace rule line:
?????searchText/---\searchWord/---\replaceWithWord
This rule will go through each line until it finds searchText in a line.
It stops at that line if it is found then it finds searchWord in that line and replace it with replaceWithWord.
However if replaceWithWord already exists in that line then it will do nothing.
When a Find and Replace Rule finds the desired line the rule ends and next rule of that Rule Group is being executed.
Example of a Line Rule:
>>>>>sample text
A Line Rule does not search anything. But it works for the next line. This line rule tells that at the end of next line append ""sample text""
A special character 0NULL0 is used to indicate a blank(null) value.
For example the follwing Find and Replace rule has null in searchWord and null in replace word:
?????searchText/---\0NULL0/---\0NULL0
This rule will go to next line that has searchText text. As as null is provided, it finds nothing and hence replaces nothing.
After this line this rule ends and next rule is being executed if any.

Types of Find and Replace Rule(includes symbol):
1. Simple Find and Replace(begins with ?????)
2. Find and Replace all text after the end of found word with replace value (begins with ????>)
3. Find and Append the replace value at the end of found word (begins with ????+)
4. Find and Replace all text before the beginning of found word with replace value (begins with <????)
5. Find and Append the replace value at the beginning of the found word (begins with +????)

Example of Find and Replace Rule:
1. 
?????searchText/---\searchWord/---\replaceWithWord
2. 
????>searchText/---\searchWord/---\replaceWithWord
3. 
????+searchText/---\searchWord/---\replaceWithWord
4. 
<????searchText/---\searchWord/---\replaceWithWord
5. 
+????searchText/---\searchWord/---\replaceWithWord

Types of Line Rule( includes symbol):
1. Add to end of line
2. Add to beginning of line
3. Add to end of line using a specified padding. However don't modify anything if a specific text is found in that line.
4. Add to beginning of line using a specified padding. However don't modify anything if a specific text is found in that line.

Example of Line Rule:
1.
>>>>>sample text
2.
<<<<<sample text
3.
>>>>>sample text/---\sample/---\40
4.
<<<<<sample text/---\sample/---\40
Here 40 is the padding value that follows the {4} seperator.
Here for 3 and 4 if ""sample"" is found in line then the line will be kept as it is.

Example of a sample setting file:
----->>>>>
?????ZIA/---\ZIA/---\BONDHU
----->>>>>
?????Model: ChannelStream/---\ChannelStream/---\GFLO-VC
>>>>> Body Style: Globe/---\Body/---\65
????>Face to face:/---\Face to face:/---\ ISA
????+Bonnet:/---\Bonnet: Std./---\ ,body matl
?????Clamps,gld flg:/---\Standard/---\Hdware SS/SS Bolts
?????______________________________/---\0NULL0/---\0NULL0
<<<<<Notes:  
<<<<<0NULL0
<<<<<1.  Design Pressure:
<<<<<2.  Design Temperature:
?????/---\/---\
?????dummyStarts/---\0NULL0/---\0NULL0
>>>>>right text1/---\right text/---\40
>>>>>right text2/---\right text/---\40
>>>>>right text3/---\right text/---\40
>>>>>right text4/---\right text/---\50
>>>>>right text5/---\right text/---\50
>>>>>right text6/---\right text/---\50
<<<<<left  text1/---\right text/---\40
<<<<<left  text2/---\right text/---\40
<<<<<left  text3/---\right text/---\40
<<<<<left  text4/---\right text/---\50
<<<<<left  text5/---\right text/---\50
<<<<<left  text6/---\right text/---\50
>>>>>right text7
<<<<<left  text7


----------------------------------------------------------------------

From v1.3.0.0 -> 2 new rules are added
Generally file is read line by line and never goes back.
This rule will go to previous lines as specified.

<-----

will go back to one line back

<-----findText
will go back and back until ""findText"" is found. If nowhere found it will go to the first line of the file.

[N.B. These rules are very efficient. To apply two rules(line or find replace rule) for a same line going back is needed.]

        ", applicationName, applicationVersion, applicationLabel, RuleManager.RULE_GROUP_START_TAG, RuleBase.SplitString);

        }

    }
}
