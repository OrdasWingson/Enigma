using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Enigma
{
    public partial class Form1 : Form
    {
        string[] files;
        int countOfSymbol = 14;
        string extension;//расширение файла


        public Form1()
        {
            InitializeComponent();
            textBoxPath.Text = @"C:\Games\1";


        }

        //шифрование
        private void button1_Click(object sender, EventArgs e)
        {
            //блок проверки на пустую строку
            if (textBoxPath.Text != "")
            {
                try
                {
                    files = Directory.GetFiles(textBoxPath.Text);
                }         
                catch
                {
                    MessageBox.Show("Папка не существует.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Пустой путь.");
                return;
            }

            
            foreach (var file in files)
            {
                extension = file.Substring(file.LastIndexOf(".") + 1);

                if (extension == "efm" || extension == "efo")
                    continue;

                int symb = new int();
                if (extension == "mpg")
                {
                    
                    
                    using (FileStream fs = new FileStream(file, FileMode.Open))
                    {
                        for (var i = 0; i < fs.Length; i++)
                        {
                            symb = fs.ReadByte();
                            symb++;
                            fs.Seek(-1, SeekOrigin.Current);
                            fs.WriteByte((byte)symb);
                        }

                        
                    }

                    string[] fName = new string[2];
                    fName = file.Split('.');
                    File.Move(file, fName[0] + ".efm");

                }
                else
                {
                    using (FileStream fs = new FileStream(file, FileMode.Open))
                    {
                        
                        for (var i = 0; i < countOfSymbol; i++)
                        {
                            symb = fs.ReadByte();
                            symb++;
                            fs.Seek(-1, SeekOrigin.Current);
                            fs.WriteByte((byte)symb);
                        }
                    }

                    string[] fName = new string[2];
                    fName = file.Split('.');
                    File.Move(file, fName[0] + "=" + extension + ".efo");
                }
             }
                textBoxInfo.Text = "\nКодирование завершено.";
                
            
        }

        //дешифровка
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBoxPath.Text != "")
            {
                try
                {
                    files = Directory.GetFiles(textBoxPath.Text);
                }
                catch
                {
                    MessageBox.Show("Папка не существует.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Пустой путь.");
                return;
            }

            textBoxInfo.Text = "Дешифровка начата.";

            foreach (var file in files)
            {
                byte[] array = new byte[countOfSymbol];
                extension = file.Substring(file.LastIndexOf(".") + 1);
                int symb = new int();
                if (extension == "efm")//mpg
                {
                    
                    using (FileStream fs = new FileStream(file, FileMode.Open))
                    {
                        for (var i = 0; i < fs.Length; i++)
                        {
                            symb = fs.ReadByte();
                            symb--;
                            fs.Seek(-1, SeekOrigin.Current);
                            fs.WriteByte((byte)symb);
                        }
                    }

                    string[] fName = new string[2];
                    fName = file.Split('.');
                    File.Move(file, fName[0] + ".mpg");

                }
                else if(extension == "efo")
                {
                    using (FileStream fs = new FileStream(file, FileMode.Open))
                    {
                        for (var i = 0; i < countOfSymbol; i++)
                        {
                            symb = fs.ReadByte();
                            symb--;
                            fs.Seek(-1, SeekOrigin.Current);
                            fs.WriteByte((byte)symb);
                        }
                    }
                    string[] fName = new string[2];
                    fName = file.Split('=');
                    string[] extension = fName[1].Split('.');
                    File.Move(file, fName[0] + "."+ extension[0]);
                }
                
            }
            textBoxInfo.Text = "Декодирование завершено.";
        }
       
        //выбрать путь
        private void buttonPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                textBoxPath.Text = folderBrowser.SelectedPath;
                files = Directory.GetFiles(textBoxPath.Text);

            foreach (var file in files)
                textBoxInfo.Text += "\r\n" + file.ToString() + "\r\n";
            }
        }
    }
}


