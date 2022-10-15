using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Rename_v1._0
{
        
    public partial class Form1 : Form
    {
        bool subfolder = false, subfolder2 = false, subfolder3 = false;
        bool prefixfolder = false; bool subfixfolder = false;
        bool except = false;
        int fail = 0;
        int success = 0;
        string path = "", path2 = "", path3 = "";
        string fail_message = "";
       

        void FindReplace(string f, string r, List<String> file)
        { 
            for (int i = 0; i < file.Count; i++)
            {
                string oldName = Path.GetFileNameWithoutExtension(file[i]);
                string extension = Path.GetExtension(file[i]);
                string newName = oldName.Replace(f, r);
                string newPath = Path.GetDirectoryName(file[i]) + "\\" + newName + extension;
                if (File.Exists(newPath) || newName == "")
                {
                        fail_message = fail_message + file[i] + "\n";
                        fail++;
                }
                else
                {
                        File.Move(file[i], newPath);
                        success++;
                }
                
            }
            return;
        }

        void PreSubFix(string pre0, string pre1, string sub0, string sub1, List<String> file)
        {
            for (int i = 0; i < file.Count; i++)
            {
                string oldName = Path.GetFileNameWithoutExtension(file[i]);
                string extension = Path.GetExtension(file[i]);
                string newName;
                string pre = pre0, sub = sub0;
                if (prefixfolder) pre = pre0 + Path.GetFileName(Path.GetDirectoryName(file[i])) + pre1;
                if (subfixfolder) sub = sub0 + Path.GetFileName(Path.GetDirectoryName(file[i])) + sub1;
                newName = pre + oldName + sub;
                string newPath = Path.GetDirectoryName(file[i]) + "\\" + newName + extension;
                if (File.Exists(newPath))
                {
                    fail_message = fail_message + file[i] + "\n";
                    fail++;
                }
                else
                {
                    File.Move(file[i], newPath);
                    success++;
                }

            }
            return;
        }

        void Capital(List<String> file)
        {
            for (int i = 0; i < file.Count; i++)
            {
                string oldName = Path.GetFileNameWithoutExtension(file[i]);
                string extension = Path.GetExtension(file[i]);
                string newName = "";
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                if (wordcap.Checked)
                {
                    newName = ti.ToTitleCase(oldName);
                    if (except)
                    {
                        string[] words = exceptstring.Text.Split(' ');
                        foreach (string w in words)
                        {
                            string ww = ti.ToTitleCase(w);
                            newName = newName.Replace(ww, ti.ToLower(ww));
                        }
                    }
                }
                else if (lettercap.Checked) newName = ti.ToUpper(oldName);
                else if (nocap.Checked) newName = ti.ToLower(oldName);
                string newPath = Path.GetDirectoryName(file[i]) + "\\" + newName + extension;
                File.Move(file[i], newPath);
                success++;
            }
        }
        
        private List<String> DirSearch(string path, string find, string filetype)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(path))
                {
                    string name = Path.GetFileNameWithoutExtension(f);
                    string extension = Path.GetExtension(f).Replace(".", "");
                    if (name.Contains(find) && (Regex.IsMatch(filetype, @"(^|\s)" + extension + "(\\s|$)", RegexOptions.IgnoreCase) || string.IsNullOrWhiteSpace(filetype) == true))
                        files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(path))
                {
                    files.AddRange(DirSearch(d, find, filetype));
                }
            }
            catch (System.Exception excpt)
            {
                MessageBox.Show(excpt.Message);
            }
            return files;
        }

        private List<String> DirSearch2(string path, string find, string filetype)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(path))
                {
                    string name = Path.GetFileNameWithoutExtension(f);
                    string extension = Path.GetExtension(f).Replace(".", "");
                    if (name.Contains(find) && (Regex.IsMatch(filetype, @"(^|\s)" + extension + "(\\s|$)", RegexOptions.IgnoreCase) || string.IsNullOrWhiteSpace(filetype) == true))
                        files.Add(f);
                }
            }
            catch (System.Exception excpt)
            {
                MessageBox.Show(excpt.Message);
            }
            return files;
        }


        public Form1()
        {
            InitializeComponent();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This program is made by me. blah blah blah to be edited.\n Gen is bald.");
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Find and replace
        private void folderPath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.userInput.Text = folderBrowserDialog1.SelectedPath;
                path = this.userInput.Text;
                pathempty.Text = findempty.Text = invalidfilename.Text = "";
                showfailed1.Enabled = false;
                fail_message = "";
                fail = 0;
                success = 0;
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            subfolder = checkBox1.Checked;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            pathempty.Text = findempty.Text = invalidfilename.Text = "";
            showfailed1.Enabled = false;
            fail_message = "";
            fail = 0;
            success = 0;
            if (path == "")
            {
                pathempty.Text = "Please choose a directory.";
                return;
            }
            else if (findtxt.Text == "")
            {
                findempty.Text = "Can't be empty.";
                return;
            }
            else
            {
                string find = findtxt.Text;
                string replace = replacetxt.Text;
                string filetype = fileType.Text;
                if (find.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || replace.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    invalidfilename.Text = "File name can't contain \\ / : * ? \" < > | . ";
                    return;
                }
                
                if (subfolder)
                {
                    List<String> file = DirSearch(path, find, filetype);
                    var confirm = MessageBox.Show(file.Count + " file(s) will be renamed. Continue?","Confirmation", MessageBoxButtons.OKCancel);
                    if (confirm == DialogResult.Cancel)
                    {
                        file.Clear();
                        return;
                    }
                    FindReplace(find, replace, file);
                    Success1.Text = "Renamed: " + (success) + "/" + (success+fail) + " file(s)";
                    Failure1.Text = "Failed: " + fail + " file(s)";
                    if (fail > 0)
                    {
                        showfailed1.Enabled = true;
                    }
                    file.Clear();
                    MessageBox.Show("Done.");
                    return;
                }
                else
                {
                    List<String> file = DirSearch2(path, find, filetype);
                    var confirm = MessageBox.Show(file.Count + " file(s) will be renamed. Continue?", "Confirmation", MessageBoxButtons.OKCancel);
                    if (confirm == DialogResult.Cancel)
                    {
                        file.Clear();
                        return;
                    }
                    FindReplace(find, replace, file);
                    Success1.Text = "Renamed: " + (success) + "/" + (success+fail) + " file(s)";
                    Failure1.Text = "Failed: " + fail + " file(s)";
                    if (fail > 0)
                    {
                        showfailed1.Enabled = true;
                    }
                    file.Clear();
                    MessageBox.Show("Done.");
                    return;
                }
            }
        }
        private void showfailed1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("File(s) failed:\n" + fail_message + "\nRenamed file(s) already exist.\nFile names can't be empty.", "Failed Files");
        }
     
        // Prefix subfix
        private void folderPath2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                this.userInput2.Text = folderBrowserDialog2.SelectedPath;
                path2 = this.userInput2.Text;
                pathempty2.Text = invalidfilename2.Text = "";
                showfailed2.Enabled = false;
                fail_message = "";
                fail = 0;
                success = 0;
            }
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            subfolder2 = checkBox2.Checked;
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            prefixfolder = checkBox3.Checked;
            if (prefixfolder)
            {
                prefixtxt.Enabled = false;
                prefolder0.Enabled = true;
                prefolder1.Enabled = true;
                foldername1.Enabled = true;
            }
            else
            {
                prefixtxt.Enabled = true;
                prefolder0.Enabled = false;
                prefolder1.Enabled = false;
                foldername1.Enabled = false;
            }
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            subfixfolder = checkBox4.Checked;
            if (subfixfolder)
            {
                subfixtxt.Enabled = false;
                subfolder0.Enabled = true;
                subfolder1.Enabled = true;
                foldername2.Enabled = true;
            }
            else
            {
                subfixtxt.Enabled = true;
                subfolder0.Enabled = false;
                subfolder1.Enabled = false;
                foldername2.Enabled = false;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            pathempty2.Text = invalidfilename2.Text = "";
            showfailed2.Enabled = false;
            fail_message = "";
            fail = 0;
            success = 0;
            if (path2 == "")
            {
                pathempty2.Text = "Please choose a directory.";
                return;
            }
            else
            {
                string filetype2 = fileType2.Text;
                if ((subfixtxt.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 && !subfixfolder) || (prefixtxt.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 && !prefixfolder))
                {
                    invalidfilename2.Text = "File name can't contain \\ / : * ? \" < > | . ";
                    return;
                }

                if (prefolder0.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || prefolder1.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || subfolder0.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || subfolder1.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                {
                    invalidfilename2.Text = "File name can't contain \\ / : * ? \" < > | . ";
                    return;
                }
                List<String> file = null;
                if (subfolder2) file = DirSearch(path2, "", filetype2);
                else file = DirSearch2(path2, "", filetype2);
                    var confirm = MessageBox.Show(file.Count + " file(s) will be renamed. Continue?", "Confirmation", MessageBoxButtons.OKCancel);
                    if (confirm == DialogResult.Cancel)
                    {
                        file.Clear();
                        return;
                    }
                    string prefix0 = "", prefix1 = "", subfix0 = "", subfix1 = "";
                    if (prefixfolder)
                    {
                        prefix0 = prefolder0.Text;
                        prefix1 = prefolder1.Text;
                    }
                    else prefix0 = prefix1 = prefixtxt.Text;
                    if (subfixfolder)
                    {
                        subfix0 = subfolder0.Text;
                        subfix1 = subfolder1.Text;
                    }
                    else subfix0 = subfix1 = subfixtxt.Text;
                    PreSubFix(prefix0, prefix1, subfix0, subfix1, file);
                    Success2.Text = "Renamed: " + (success) + "/" + (success + fail) + " file(s)";
                    Failure2.Text = "Failed: " + fail + " file(s)";
                    if (fail > 0)
                    {
                        showfailed2.Enabled = true;
                    }
                    file.Clear();
                    MessageBox.Show("Done.");
                    return;
            }
        }
        private void showfailed2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("File(s) failed:\n" + fail_message, "Failed Files");
        }
  
        //Capitalization
        private void folderPath3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog3.ShowDialog() == DialogResult.OK)
            {
                this.userInput3.Text = folderBrowserDialog3.SelectedPath;
                path3 = this.userInput3.Text;
                pathempty.Text = findempty.Text = invalidfilename.Text = "";
                showfailed3.Enabled = false;
                fail_message = "";
                fail = 0;
                success = 0;
            }
        }
        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            subfolder3 = checkBox5.Checked;
        }
        private void capexcept_CheckedChanged(object sender, EventArgs e)
        {
            except = capexcept.Checked;
            if (except)
            {
                exceptstring.Enabled = true;
            }
            else exceptstring.Enabled = false;
        }
        private void capitalize_Click(object sender, EventArgs e)
        {
            pathempty3.Text = "";
            showfailed3.Enabled = false;
            fail_message = "";
            fail = 0;
            success = 0;
            if (path3 == "")
            {
                pathempty3.Text = "Please choose a directory.";
                return;
            }
            string filetype3 = fileType3.Text;
            List<String> file = null;
            if (subfolder3) file = DirSearch(path3, "", filetype3);
            else file = DirSearch2(path3, "", filetype3);
            var confirm = MessageBox.Show(file.Count + " file(s) will be renamed. Continue?", "Confirmation", MessageBoxButtons.OKCancel);
            if (confirm == DialogResult.Cancel)
            {
                file.Clear();
                return;
            }
            Capital(file);
            Success3.Text = "Renamed: " + (success) + "/" + (success + fail) + " file(s)";
            Failure3.Text = "Failed: " + fail + " file(s)";
            if (fail > 0)
            {
                showfailed3.Enabled = true;
            }
            file.Clear();
            MessageBox.Show("Done.");
            return;
        }
        private void showfailed3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("File(s) failed:\n" + fail_message, "Failed Files");
        }    
    }
}
