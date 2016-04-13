/*  GNU General Public License
  
    Forskning med 1000 ord is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    Forskning med 1000 ord is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with Forskning med 1000 ord.  If not, see <http://www.gnu.org/licenses/>.
*/
/*  Author:
    Jakob Berg Johansen
    Research Assistant at the Technical University of Denmark
    jajoh@byg.dtu.dk
    Date: 07-04-2016, Lyngby, Denmark
 
    Copyright (c) 2016 Jakob Berg Johansen 
    Released under GNU GPL v3
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Forskning_med_1000_ord
{
    public partial class ForskningMed1000Ord : Form
    {
        // Declare variables and arrays

        string[] words;
        string[] stringArray;
        int[] signArray;
        List<int> signList = new List<int>();
        List<string> stringList = new List<string>();
        List<string> individualWords = new List<string>();
        double totalWords = 0;
        double matchedWords = 0;
        double nonMatchedWords = 0;
        double percentMatch = 0;
        

        // Initialize main window
        public ForskningMed1000Ord()
        {
            InitializeComponent();
        }



        // Exit botton on menuestrip
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Show about window
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        // New botton on menuestrip resets the textbox
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textToAnalyse.Text = "";
        }

        // Open botton on menuestrip starts file dialog and prints file content to textbox
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(ofd.FileName, System.Text.Encoding.Default);
                textToAnalyse.Text = sr.ReadToEnd();
                sr.Close();
            }
        }

        // Save botton on menuestrip opens file dialog and writes textbox content to file
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files | *.txt";
            sfd.DefaultExt = "txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName);
                sw.Write(textToAnalyse.Text);
                sw.Close();
            }
        }



        //Run botton starts the comparison of words in the text to the 1000 words list
        private void buttonRun_Click(object sender, EventArgs e)
        {
            //Making string variable to hold words and punctuations
            string indexString = "";

            //Starting list of punctuations (signList) with zero
            signList.Add(0);

            //Finding the last character in the text
            int lastLetter_int = textToAnalyse.Text.Length - 1;
            string lastLetter_str = textToAnalyse.Text.Substring(lastLetter_int, 1);

            //If the last character is a letter then a space is added
            if (Char.IsLetter(lastLetter_str[0]))
            {
                textToAnalyse.Text = textToAnalyse.Text + " ";
            }

            //Searching for any punctuations in the text and their index
            foreach (Match m in Regex.Matches(textToAnalyse.Text, @"\W"))
            {
                signList.Add(m.Index);
            }

            //Making an array of the punctuation indexes
            signArray = signList.ToArray();
            signList.Clear();

            //Making a list of strings from the text delimited by punctuation indexes
            for (int i = 1; i < signArray.Length; i++)
            {
                //Handeling the first character if it is a punctuation
                if (signArray[i - 1] - signArray[i] == 1)
                {
                    indexString = textToAnalyse.Text.Substring(signArray[i], 1);
                    stringList.Add(indexString);
                }
                
                //If the first character is a letter
                else
                {
                    //Handeling the first word
                    if (i == 1)
                    {
                        indexString = textToAnalyse.Text.Substring(0, signArray[i]);
                        stringList.Add(indexString);

                        indexString = textToAnalyse.Text.Substring(signArray[i], 1);
                        stringList.Add(indexString);
                    }
                    
                    //Handeling subsequent words and punctuations
                    else
                    {
                        indexString = textToAnalyse.Text.Substring(signArray[i - 1] + 1, signArray[i] - signArray[i - 1] - 1);
                        stringList.Add(indexString);

                        indexString = textToAnalyse.Text.Substring(signArray[i], 1);
                        stringList.Add(indexString);
                    }
                }
            }

            //Making a string array of words and punctuations
            stringArray = stringList.ToArray();
            stringList.Clear();

            
            //Getting directory path of program
            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            
            //Checking if WordList.txt exists in the directory
            bool fileExists = System.IO.File.Exists(path + @"\WordList.txt");
            
            //If WordList exists then do analysis
            if (fileExists)
            {
                //Getting word list
                words = System.IO.File.ReadAllLines(path + @"\WordList.txt");

                //Splitting the WordList (tap delimited) into list of individual words 
                for (int h = 0; h < words.Length; h++)
                {
                    string tempWordString = words[h];
                    string[] split = tempWordString.Split('\t');

                    for (int i = 0; i < split.Length; i++)
                    {
                        string tempIndividualWord = split[i];

                        if (tempIndividualWord == "")
                        {
                            //Do nothing if the string is empty
                        }
                        else
                        {
                            individualWords.Add(tempIndividualWord);
                        }
                    }
                }
                
                //Make word list array with individual words
                words = individualWords.ToArray();
                individualWords.Clear();

                //Clearing the text box
                textToAnalyse.Text = "";

                //Comparing words in the text with the words in the word list
                for (int h = 0; h < stringArray.Length; h++)
                {
                    string stringTemp = stringArray[h];
                    int lengthOfText = textToAnalyse.TextLength;
                    int tempTextLength = textToAnalyse.TextLength;

                    //Set the check for doubles to zero
                    int doublesCheck = 0;

                    //Comparing the individual strings in stringArray (from the text) to the word list
                    for (int k = 0; k < words.Length; k++)
                    {
                        string tempWord = words[k];
                        
                        //If it is equal print it in green color
                        if (stringTemp.Equals(tempWord, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (doublesCheck == 0)
                            {
                                int stringLength = stringTemp.Length;
                                textToAnalyse.AppendText(stringTemp);

                                lengthOfText = textToAnalyse.TextLength;

                                textToAnalyse.SelectionStart = lengthOfText - stringLength;
                                textToAnalyse.SelectionLength = stringLength;
                                textToAnalyse.SelectionColor = Color.Green;

                                totalWords = totalWords + 1;
                                matchedWords = matchedWords + 1;
                                doublesCheck = 1;
                            }
                            else
                            {
                                //Do nothing if word has already been found (doublesCheck > 0)
                            }
                            
                        }

                    }

                    //If no new characters have been printed the string is either empty, has no match or is a punctuation
                    if (tempTextLength == lengthOfText)
                    {
                        //If the string is empty
                        if (stringTemp == "")
                        {
                            //Do nothing if string is empty
                        }
                        
                        //If the string is not empty
                        else
                        {
                            //If the string has a letter as its first character print the string red
                            if (Char.IsLetter(stringTemp[0]))
                            {
                                int stringLength = stringTemp.Length;
                                textToAnalyse.AppendText(stringTemp);

                                lengthOfText = textToAnalyse.TextLength;

                                textToAnalyse.SelectionStart = lengthOfText - stringLength;
                                textToAnalyse.SelectionLength = stringLength;
                                textToAnalyse.SelectionColor = Color.Red;

                                totalWords = totalWords + 1;
                                nonMatchedWords = nonMatchedWords + 1;
                            }

                            //If the string does not have a letter as its first character print the string black
                            else
                            {
                                int stringLength = stringTemp.Length;
                                textToAnalyse.AppendText(stringTemp);

                                lengthOfText = textToAnalyse.TextLength;

                                textToAnalyse.SelectionStart = lengthOfText - stringLength;
                                textToAnalyse.SelectionLength = stringLength;
                                textToAnalyse.SelectionColor = Color.Black;
                            }
                        }
                    }
                }

                //Write number of words in text box
                string totalWords_str = totalWords.ToString();
                this.textBoxTotalWords.Text = totalWords_str;
                

                //Write number of matched words
                string matchedWords_str = matchedWords.ToString();
                this.textBoxAntalRigtigeOrd.Text = matchedWords_str;
                

                //Write number of non-matched words
                string nonMatchedWords_str = nonMatchedWords.ToString();
                this.textBoxAntalForkerteOrd.Text = nonMatchedWords_str;
                

                //Percent of matched words
                percentMatch = Math.Truncate((matchedWords / totalWords) * 10000) / 100;
                string percentMatch_str = percentMatch.ToString();
                this.textBoxProcent.Text = percentMatch_str + " %";
                
                                
                totalWords = 0;
                matchedWords = 0;
                nonMatchedWords = 0;
                percentMatch = 0;
                                
            }
            
            //If WordList does not exist
            else
            {
                WordListDoesNotExist wordListDoesNotExist = new WordListDoesNotExist();
                wordListDoesNotExist.ShowDialog();
            }
        }           
    }
}
