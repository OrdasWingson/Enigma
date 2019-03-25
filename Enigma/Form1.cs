using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace Enigma
{
    public partial class Form1 : Form
    {
        string[] files; //файлы в папке
        int countOfSymbol = 14; // количество первых кодируемых байт
        string extension;//расширение файла
        int step = 100; //шаг
        
        public Form1()
        {
            InitializeComponent();
            textBoxPath.Text = @"C:\Games\1";
            
            listBoxFuller();
        }

        void listBoxFuller()//заполнение листбокас с проверкой
        {
            if (check())
            {
                listBoxPath.Items.Clear();                      
                foreach (var file in files)
                {
                    extension = file.Substring(file.LastIndexOf(".") + 1);//получаем расширение
                    if (extension == "efm" || extension == "efo")//помечает кодированные файлы красным некодированны зеленым
                    {
                        listBoxPath.Items.Add(new MyListBoxItem(Color.Red, file.Substring(file.LastIndexOf(@"\") + 1)));
                        


                    }
                    else
                    {
                       listBoxPath.Items.Add(new MyListBoxItem(Color.Green, file.Substring(file.LastIndexOf(@"\") + 1)));
                        
                    }
                }
            }
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

            textBoxInfo.Text = "";
            button1.Enabled = false;
            button2.Enabled = false;
            Thread threadCod = new Thread(() =>
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

                    if (extension == "mpg" || extension == "MPG")
                    {
                        using (FileStream fs = new FileStream(file, FileMode.Open))
                        {
                            //int procces=0;
                            //double size = (double)fs.Length;
                            for (var i = 0; i < fs.Length; i += step)
                            {
                                fs.Seek(i, SeekOrigin.Begin);//устанавливает каретку на i байт
                                symb = fs.ReadByte(); // считывает байт в память и переводит каретку на байт вперед
                                symb++; // кодирует байт
                                fs.Seek(i, SeekOrigin.Begin); //возвращает каретку на i байт
                                fs.WriteByte((byte)symb); // записывае байт

                                
                            }

                           
                        }
                        //три строки меняю расширение файла
                        string[] fName = new string[2]; 
                        fName = file.Split('.');
                        File.Move(file, fName[0] + ".efm");

                    }
                    else //если расширение не соответствует mpeg
                    {
                        using (FileStream fs = new FileStream(file, FileMode.Open))
                        {

                            for (var i = 0; i < countOfSymbol; i++)
                            {                                
                                fs.Seek(i, SeekOrigin.Begin);//устанавливает каретку на i байт
                                symb = fs.ReadByte(); // считывает байт в память и переводит каретку на байт вперед
                                symb++; // кодирует байт
                                fs.Seek(i, SeekOrigin.Begin); //возвращает каретку на i байт
                                fs.WriteByte((byte)symb); // записывае байт

                            }
                        }
                        //три строки меняю расширение файла
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
                    listBoxFuller();
                    textBoxInfo.Text = "Кодирование завершено." + Environment.NewLine;
                    button1.Enabled = true;
                    button2.Enabled = true;
                }));

            });
            threadCod.Start();
                
            
        }



        //дешифровка
        private void button2_Click(object sender, EventArgs e)
        {

            if (!check())
            {
                return;
            }

            textBoxInfo.Text = "";
            button1.Enabled = false;
            button2.Enabled = false;
            Thread threadDec = new Thread(() =>
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
                            for (var i = 0; i < fs.Length; i += step)
                            {
                                fs.Seek(i, SeekOrigin.Begin);//устанавливает каретку на i байт
                                symb = fs.ReadByte(); // считывает байт в память и переводит каретку на байт вперед
                                symb--; // кодирует байт
                                fs.Seek(i, SeekOrigin.Begin); //возвращает каретку на i байт
                                fs.WriteByte((byte)symb); // записывае байт

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
                                fs.Seek(i, SeekOrigin.Begin);//устанавливает каретку на i байт
                                symb = fs.ReadByte(); // считывает байт в память и переводит каретку на байт вперед
                                symb--; // кодирует байт
                                fs.Seek(i, SeekOrigin.Begin); //возвращает каретку на i байт
                                fs.WriteByte((byte)symb); // записывае байт
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
                    listBoxFuller();
                    textBoxInfo.Text = "Декодирование завершено." + Environment.NewLine;
                    button1.Enabled = true;
                    button2.Enabled = true;
                }));

            });
            threadDec.Start();
        }
       
        //выбрать путь
        private void buttonPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();

            folderBrowser.SelectedPath = textBoxPath.Text;

            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                textBoxPath.Text = folderBrowser.SelectedPath;
                files = Directory.GetFiles(textBoxPath.Text);
                listBoxFuller();
            }
        }

        private void listBoxPath_DrawItem(object sender, DrawItemEventArgs e)
        {
            MyListBoxItem item = listBoxPath.Items[e.Index] as MyListBoxItem; // Get the current item and cast it to MyListBoxItem
            e.DrawBackground();

            if (item != null)
            {
                e.Graphics.DrawString(item.Message, e.Font, new SolidBrush(item.ItemColor), e.Bounds, StringFormat.GenericDefault);                
                e.DrawFocusRectangle();
            }
            else
            {
                // The item isn't a MyListBoxItem, do something about it
            }
        }
    }

    public class MyListBoxItem
    {
        public MyListBoxItem(Color c, string m)
        {
            ItemColor = c;
            Message = m;
        }
        public Color ItemColor { get; set; }
        public string Message { get; set; }
    }
}


/*procces = (int)(((double)i/size)*100);
                                                               
                                if ((procces % 5) == 0)
                                {                                   
                                   BeginInvoke(new MethodInvoker(delegate
                                  {
                                      textBoxInfo.Text += file.Substring(file.LastIndexOf(@"\") + 1) + " декодируется. " + procces + "%" + Environment.NewLine;
                                  }));
                                }*/
