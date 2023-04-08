using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Mail;
using Microsoft.Win32;
using System.IO;
using System.Text.Json;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EmailSenderWPF
{
    public partial class MainWindow : Window
    {
        // Инициализируем само сообщение вне кода дабы область видимости распостранялась на все кнопки, тоже самое и с модальным окном для логина и регистрации
        MailMessage message = new MailMessage();
        Window1 passwordWindow = new Window1();

        public MainWindow()
        {
            InitializeComponent();

        }
        // Кнопка которая отвечает за отправку письма, в ней письмо формируется, а также задаются параметры входа в почту и настраивается сервер
        //теперь этот метод асинхроннен
        public async void zalupka321_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Здесь сам скелет письма, от кого оно, кому оно идет, тема письма и само письмо
                message.From = new MailAddress(passwordWindow.Email.Text);
                message.To.Add(new MailAddress(ToTextBox.Text));
                message.Subject = SubjectTextBox.Text;
                message.Body = BodyTextBox.Text;

                //SMTP сервер. Адресс,Порт и данные от почты
                SmtpClient smtpClient = new SmtpClient("smtp-mail.outlook.com");
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Credentials = new NetworkCredential(passwordWindow.Email.Text, passwordWindow.Password.Text);
                //отправка
                await smtpClient.SendMailAsync(message);
                MessageBox.Show("Письмо отправлено");
            }
            catch (Exception ex)
            {
                //дебагер без дебагера
                MessageBox.Show("Произошла ошибка при отправке письма: " + ex.Message);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        //кнопка отвечающая за прикрепления файлов, использует обычный встроенный метод для открытия проводника, файлов можно крепить сколького угодно
        //теперь кнопка только запускает поток с методом который отвечает за прекрепление файлов FileDrop
        private void Button_Click(object sender, RoutedEventArgs e)
        {


            Thread FileDrops = new Thread(FileDrop);
            FileDrops.Start();

        }

        public void FileDrop()
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Multiselect = true;

            if (openFileDialog1.ShowDialog() == true)
            {
                foreach (string file in openFileDialog1.FileNames)
                {
                    Attachment attachment = new Attachment(file);
                    message.Attachments.Add(attachment);
                    MessageBox.Show($"Вы прекрепили {file}");
                }
            }
        }

        private async void LoginReg_Click(object sender, RoutedEventArgs e)
        {
            

            if (passwordWindow.ShowDialog() == true)
            {

                using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                {
                    AccountJson? person = await JsonSerializer.DeserializeAsync<AccountJson>(fs);


                    if (passwordWindow.Email.Text == person?.Email && passwordWindow.Password.Text == person?.Password)
                    {
                        MessageBox.Show("Успешный вход");

                    }
                    else
                    {
                        MessageBox.Show("Неправильный Email или Пароль");
                    }
                }


            }
        }

         async void Registration_Click(object sender, RoutedEventArgs e)
        {
            Window1 passwordWindow = new Window1();

            

            if (passwordWindow.ShowDialog() == true)
            {

                try
                {


                    await using (FileStream fs = new FileStream("user.json", FileMode.OpenOrCreate))
                    {
                        AccountJson accountJson = new AccountJson(passwordWindow.Email.Text,passwordWindow.Password.Text);
                        await JsonSerializer.SerializeAsync<AccountJson>(fs, accountJson);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.ToString());
                    throw;
                }


            }
        }
    }

}
