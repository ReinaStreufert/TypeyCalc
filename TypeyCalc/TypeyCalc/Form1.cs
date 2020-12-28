using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

namespace TypeyCalc
{
    public partial class Form1 : Form
    {
        Timer Timer = new Timer();
        bool PinnedAnswer = false;
        string PinnedAnswerText;
        string Ans;
        public Form1()
        {
            InitializeComponent();
            Timer.Interval = 500;
            Timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            label1.ForeColor = Color.DarkRed;
            //label1.Text = "";
            Timer.Stop();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            object Answer;
            try
            {
                Answer = Evaluate(RemoveComments(textBox1.Text.ToLower().Replace('x', '*').Replace("e+", "*10^").Replace("e", "*10^")));
            } catch
            {
                Answer = null;
            }
            if (Answer != null)
            {
                Timer.Stop();
                label1.ForeColor = SystemColors.ControlText;
                if (!PinnedAnswer)
                {
                    label1.Text = Answer.ToString();
                } else
                {
                    label1.Text = PinnedAnswerText + " // " + Answer.ToString();
                }
                Ans = Answer.ToString();
            } else
            {
                Timer.Start();
            }
            if (textBox1.Text.ToLower().Replace(" ", "") == "typeycalc")
            {
                Timer.Stop();
                label1.ForeColor = SystemColors.ControlText;
                label1.Text = "Amazing";
                Ans = "Amazing";
            }
            if (textBox1.Text.ToLower().Replace(" ", "") == "logan")
            {
                Timer.Stop();
                label1.ForeColor = SystemColors.ControlText;
                label1.Text = "Idiot";
                Ans = "Idiot";
            }
            if (textBox1.Text.ToLower().Replace(" ", "") == "typeycalc=amazing")
            {
                Timer.Stop();
                label1.ForeColor = SystemColors.ControlText;
                label1.Text = "True";
                Ans = "True";
            }
            if (textBox1.Text.ToLower().Replace(" ", "") == "typeycalc=bad")
            {
                Timer.Stop();
                label1.ForeColor = SystemColors.ControlText;
                label1.Text = "False";
                Ans = "False";
            }
            if (textBox1.Text.ToLower().Replace(" ", "") == "logan=idiot")
            {
                Timer.Stop();
                label1.ForeColor = SystemColors.ControlText;
                label1.Text = "True";
                Ans = "True";
            }
            if (textBox1.Text.ToLower().Replace(" ", "") == "logan=notidiot")
            {
                Timer.Stop();
                label1.ForeColor = SystemColors.ControlText;
                label1.Text = "False";
                Ans = "False";
            }
        }

        private string RemoveComments(string Input)
        {
            string Output = "";
            bool InComment = false;
            foreach (Char c in Input)
            {
                if (c == '\\')
                {
                    if (!InComment)
                    {
                        InComment = true;
                    } else
                    {
                        InComment = false;
                    }
                }
                if (!InComment && c != '\\')
                {
                    Output = Output + c;
                }
            }
            return Output;
        }

        private object Evaluate(string Input)
        {
            string withoutSpaces = Input.Replace(" ", "");
            if (withoutSpaces == "")
            {
                return (double)0;
            }
            List<object> SeperateTerms = new List<object>();
            string Temp = "";
            int ParenthaseLevel = 0;
            foreach (char c in withoutSpaces)
            {
                if (c == '(')
                {
                    ParenthaseLevel++;
                    if (SeperateTerms.Count == 0)
                    {
                        SeperateTerms.Add("");
                    }
                    if (SeperateTerms.Last().GetType() != typeof(string))
                    {
                        SeperateTerms.Add("");
                    }
                    if (ParenthaseLevel == 1)
                    {
                        continue;
                    }
                }
                if (c == ')')
                {
                    ParenthaseLevel--;
                    if (ParenthaseLevel < 0)
                    {
                        throw new FormatException();
                    }
                    if (ParenthaseLevel == 0)
                    {
                        //SeperateTerms[SeperateTerms.Count - 1] = (string)SeperateTerms.Last() + c;
                        continue;
                    }
                }
                if (ParenthaseLevel == 0)
                {
                    if (c == '0' || c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9' || c == '.')
                    {
                        Temp = Temp + c;
                    }
                    else
                    {
                        if (Temp != "")
                        {
                            double Out;
                            if (double.TryParse(Temp, out Out))
                            {
                                SeperateTerms.Add(Out);
                            }
                            else
                            {
                                throw new FormatException();
                            }
                            Temp = "";
                        }
                        if (c == '+' || c == '-' || c == '*' || c == '/' || c == '^' || c == '=')
                        {
                            SeperateTerms.Add(c);
                        }
                        else
                        {
                            throw new FormatException();
                        }
                    }
                }
                else
                {
                    SeperateTerms[SeperateTerms.Count - 1] = (string)SeperateTerms.Last() + c;
                }
            }
            if (Temp != "")
            {
                double Out;
                if (double.TryParse(Temp, out Out))
                {
                    SeperateTerms.Add(Out);
                }
                Temp = "";
            }
            //Negatives
            for (int i = 0; i < SeperateTerms.Count; i++)
            {
                if (i == 0 && SeperateTerms[i].GetType() == typeof(char) && (char)SeperateTerms[i] == '-')
                {
                    if (SeperateTerms.Count - 1 == i || SeperateTerms[i + 1].GetType() != typeof(double))
                    {
                        throw new FormatException();
                    }
                    SeperateTerms[i] = "0 - " + SeperateTerms[i + 1].ToString();
                    SeperateTerms.RemoveAt(i + 1);
                    i = -1;
                }
                if (i > 0 && SeperateTerms.Count - 1 != i && SeperateTerms[i].GetType() == typeof(char) && (char)SeperateTerms[i] == '-' && (SeperateTerms[i - 1].GetType() != typeof(double) && SeperateTerms[i - 1].GetType() != typeof(string)) && SeperateTerms[i + 1].GetType() == typeof(double))
                {
                    SeperateTerms[i] = "0 - " + SeperateTerms[i + 1].ToString();
                    SeperateTerms.RemoveAt(i + 1);
                    i = -1;
                }
            }
            /*foreach (Object o in SeperateTerms)
            {
                Console.WriteLine(o);
            }
            Console.WriteLine("------------------------------------");*/

            //Parenthases
            for (int i = 0; i < SeperateTerms.Count; i++)
            {
                if (SeperateTerms[i].GetType() == typeof(string))
                {
                    object toReplace = Evaluate((string)SeperateTerms[i]);
                    SeperateTerms[i] = toReplace;
                    i = -1;
                    continue;
                }
            }
            /*foreach (Object o in SeperateTerms)
            {
                Console.WriteLine(o);
            }
            Console.WriteLine("------------------------------------");*/

            //Exponents
            for (int i = 0; i < SeperateTerms.Count; i++)
            {
                if (SeperateTerms[i].GetType() == typeof(char) && (char)SeperateTerms[i] == '^')
                {
                    if (!(i > 0 && i < SeperateTerms.Count - 1 && SeperateTerms[i - 1].GetType() == typeof(double) && SeperateTerms[i + 1].GetType() == typeof(double)))
                    {
                        throw new FormatException();
                    }
                    else
                    {
                        double toReplace = Math.Pow((double)SeperateTerms[i - 1], (double)SeperateTerms[i + 1]);
                        SeperateTerms.RemoveAt(i - 1);
                        SeperateTerms.RemoveAt(i - 1);
                        SeperateTerms[i - 1] = toReplace;
                        i = -1;
                        continue;
                    }
                }
            }
            /*foreach (Object o in SeperateTerms)
            {
                Console.WriteLine(o);
            }
            Console.WriteLine("------------------------------------");*/

            //Multiplication & Division
            for (int i = 0; i < SeperateTerms.Count; i++)
            {
                if (SeperateTerms[i].GetType() == typeof(char) && (char)SeperateTerms[i] == '*')
                {
                    if (!(i > 0 && i < SeperateTerms.Count - 1 && SeperateTerms[i - 1].GetType() == typeof(double) && SeperateTerms[i + 1].GetType() == typeof(double)))
                    {
                        throw new FormatException();
                    }
                    else
                    {
                        double toReplace = (double)SeperateTerms[i - 1] * (double)SeperateTerms[i + 1];
                        SeperateTerms.RemoveAt(i - 1);
                        SeperateTerms.RemoveAt(i - 1);
                        SeperateTerms[i - 1] = toReplace;
                        i = -1;
                        continue;
                    }
                }
                if (SeperateTerms[i].GetType() == typeof(char) && (char)SeperateTerms[i] == '/')
                {
                    if (!(i > 0 && i < SeperateTerms.Count - 1 && SeperateTerms[i - 1].GetType() == typeof(double) && SeperateTerms[i + 1].GetType() == typeof(double)))
                    {
                        throw new FormatException();
                    }
                    else
                    {
                        double toReplace = (double)SeperateTerms[i - 1] / (double)SeperateTerms[i + 1];
                        SeperateTerms.RemoveAt(i - 1);
                        SeperateTerms.RemoveAt(i - 1);
                        SeperateTerms[i - 1] = toReplace;
                        i = -1;
                        continue;
                    }
                }
            }
            /*foreach (Object o in SeperateTerms)
            {
                Console.WriteLine(o);
            }
            Console.WriteLine("------------------------------------");*/

            //Addition & Subtraction
            for (int i = 0; i < SeperateTerms.Count; i++)
            {
                if (SeperateTerms[i].GetType() == typeof(char) && (char)SeperateTerms[i] == '+')
                {
                    if (!(i > 0 && i < SeperateTerms.Count - 1 && SeperateTerms[i - 1].GetType() == typeof(double) && SeperateTerms[i + 1].GetType() == typeof(double)))
                    {
                        throw new FormatException();
                    }
                    else
                    {
                        double toReplace = (double)SeperateTerms[i - 1] + (double)SeperateTerms[i + 1];
                        SeperateTerms.RemoveAt(i - 1);
                        SeperateTerms.RemoveAt(i - 1);
                        SeperateTerms[i - 1] = toReplace;
                        i = -1;
                        continue;
                    }
                }
                if (SeperateTerms[i].GetType() == typeof(char) && (char)SeperateTerms[i] == '-')
                {
                    if (!(i > 0 && i < SeperateTerms.Count - 1 && SeperateTerms[i - 1].GetType() == typeof(double) && SeperateTerms[i + 1].GetType() == typeof(double)))
                    {
                        throw new FormatException();
                    }
                    else
                    {
                        double toReplace = (double)SeperateTerms[i - 1] - (double)SeperateTerms[i + 1];
                        SeperateTerms.RemoveAt(i - 1);
                        SeperateTerms.RemoveAt(i - 1);
                        SeperateTerms[i - 1] = toReplace;
                        i = -1;
                        continue;
                    }
                }
            }
            /*foreach (Object o in SeperateTerms)
            {
                Console.WriteLine(o);
            }
            Console.WriteLine("------------------------------------");*/

            //Equal to
            for (int i = 0; i < SeperateTerms.Count; i++)
            {
                if (SeperateTerms[i].GetType() == typeof(char) && (char)SeperateTerms[i] == '=')
                {
                    if (!(i > 0 && i < SeperateTerms.Count - 1))
                    {
                        throw new FormatException();
                    }
                    else
                    {
                        bool toReplace = (SeperateTerms[i - 1].Equals(SeperateTerms[i + 1]));
                        SeperateTerms.RemoveAt(i - 1);
                        SeperateTerms.RemoveAt(i - 1);
                        SeperateTerms[i - 1] = toReplace;
                        i = -1;
                        continue;
                    }
                }
            }

            if (SeperateTerms.Count == 0)
            {
                throw new Exception();
            }
            if (SeperateTerms[0].GetType() == typeof(char) || SeperateTerms[0].GetType() == typeof(string))
            {
                throw new Exception();
            }
            if (SeperateTerms.Count != 1)
            {
                throw new Exception();
            }
            else
            {
                return SeperateTerms[0];
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(label1.Text);
        }

        private void pinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!PinnedAnswer)
            {
                PinnedAnswerText = Ans;
                PinnedAnswer = true;
                pinToolStripMenuItem.Text = "Unpin";
                copyToolStripMenuItem.Text = "Copy All";
                copyJustAnswerToolStripMenuItem.Visible = true;
                copyJustPinnedToolStripMenuItem.Visible = true;
                replacePinToolStripMenuItem.Visible = true;
                textBox1_TextChanged(null, null);
            } else
            {
                PinnedAnswer = false;
                pinToolStripMenuItem.Text = "Pin";
                copyToolStripMenuItem.Text = "Copy";
                copyJustAnswerToolStripMenuItem.Visible = false;
                copyJustPinnedToolStripMenuItem.Visible = false;
                replacePinToolStripMenuItem.Visible = false;
                textBox1_TextChanged(null, null);
            }
        }

        private void copyJustAnswerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(Ans);
        }

        private void copyJustPinnedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(PinnedAnswerText);
        }

        private void replacePinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PinnedAnswerText = Ans;
            textBox1_TextChanged(null, null);
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            textBox1.Width = this.Width - 40;
            label1.Width = this.Width - 40;
            textBox1.Height = this.Height - 134;
            label1.Location = new Point(label1.Location.X, textBox1.Height - 10);
        }
    }
}
