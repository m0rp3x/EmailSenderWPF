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

namespace EmailSenderWPF
{
    public partial class MainWindow : Window
    {
        // Инициализируем само сообщение вне кода дабы область видимости распостранялась на все кнопки
        MailMessage message = new MailMessage();

        public MainWindow()
        {
            InitializeComponent();

        }
        // Кнопка которая отвечает за отправку письма, в ней письмо формируется, а также задаются параметры входа в почту и настраивается сервер
        public void zalupka321_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Здесь сам скелет письма, от кого оно, кому оно идет, тема письма и само письмо
                message.From = new MailAddress("kovalevasvetlana@live.com");
                message.To.Add(new MailAddress(ToTextBox.Text));
                message.Subject = SubjectTextBox.Text;
                message.Body = BodyTextBox.Text;

                //SMTP сервер. Адресс,Порт и данные от почты
                SmtpClient smtpClient = new SmtpClient("smtp-mail.outlook.com");
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Credentials = new NetworkCredential("kovalevasvetlana@live.com", "16092009aB");
                //отправка
                smtpClient.Send(message);
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Multiselect = true;

            if(openFileDialog1.ShowDialog() == true)
            {
                foreach(string file in openFileDialog1.FileNames)
                {
                    Attachment attachment = new Attachment(file);
                    message.Attachments.Add(attachment);
                    MessageBox.Show($"Вы прекрепили {file}");
                }
            }

        }
    }
}

