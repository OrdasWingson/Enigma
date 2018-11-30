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

        //проверка
        bool check()
        {
            //блок проверки на пустую строку
            if (textBoxPath.Text != "")
            {
                try
                {
                    files = Directory.GetFiles(textBoxPath.Text);
                    return true;
                }
                catch
                {
                    MessageBox.Show("Папка не существует.");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Пустой путь.");
                return false;
            }
        }

        //шифрование
        private void button1_Click(object sender, EventArgs e)
        {
            
            if(!check())
            {
                return;
            }
            Thread thread1 = new Thread(() =>
            {
                foreach (var file in files)
                {
                    extension = file.Substring(file.LastIndexOf(".") + 1);

                    BeginInvoke(new MethodInvoker(delegate
                    {
                        textBoxInfo.Text += file.Substring(file.LastIndexOf(@"\") + 1) + " кодируется." + Environment.NewLine;
                    }));
                    
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

                    BeginInvoke(new MethodInvoker(delegate
                    {
                        textBoxInfo.Text += file.Substring(file.LastIndexOf(@"\") + 1) + " has done" + Environment.NewLine;
                    }));

                    
                }

                BeginInvoke(new MethodInvoker(delegate
                {
                    textBoxInfo.Text += "Кодирование завершено." + Environment.NewLine;
                }));
                
            });
            thread1.Start();
            
                
            
        }

        //дешифровка
        private void button2_Click(object sender, EventArgs e)
        {
            if (!check())
            {
                return;
            }
            Thread thread1 = new Thread(() =>
            {                

                foreach (var file in files)
                {
                    BeginInvoke(new MethodInvoker(delegate
                    {
                        textBoxInfo.Text += file.Substring(file.LastIndexOf(@"\") + 1) + " декодируется." + Environment.NewLine;
                    }));

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
                    else if (extension == "efo")
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
                        File.Move(file, fName[0] + "." + extension[0]);
                    }

                    BeginInvoke(new MethodInvoker(delegate
                    {
                        textBoxInfo.Text += file.Substring(file.LastIndexOf(@"\") + 1) + " has done" + Environment.NewLine;
                    }));
                }

                BeginInvoke(new MethodInvoker(delegate
                {
                    textBoxInfo.Text += "Кодирование завершено." + Environment.NewLine;
                }));
            });
            thread1.Start();
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


