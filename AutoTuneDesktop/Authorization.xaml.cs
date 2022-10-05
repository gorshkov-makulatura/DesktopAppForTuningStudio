using AutoTuneDesktop.Classes;
using AutoTuneDesktop.Db;
using AutoTuneDesktop.Windows;
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

namespace AutoTuneDesktop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class Authorization: Window
    {
        private static Random random = new Random();
        AutoTuneEntities db = new AutoTuneEntities();
        public int tryCount = 0;
        public Authorization()
        {
            InitializeComponent();
            FastAuth();
        }
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async void loginButtonClick(object sender, RoutedEventArgs e)
        {
            if(emailBox.Text != "" || passwordBox.Password != "")
            {
                if (tryCount >= 3)
                {
                    if (captchaBlock.Text != captchaBox.Text)
                    {
                        captchaBlock.Text = RandomString(8);
                        captchaBox.Text = "";
                        return;
                    }
                }
                (Users user, bool result) = await Auth(emailBox.Text, passwordBox.Password);
                if (result == true)
                {
                        Admin admin = new Admin(user);
                        this.Close();
                        admin.ShowDialog();
                }
                else
                {
                    captchaBlock.Text = RandomString(8);
                    tryCount++;
                }
                if (tryCount == 3)
                {
                    captchaGeneral.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Messages.ShowError("Пожалуйста, убедитесь в том что все поля заполнены");
            }
            
        }
        public async Task<(Users, bool)> Auth(string Email,string Password)
        {
            return await Task.Run(() =>
            {
                if (!db.Users.Any(x => x.Email == Email))
                    return (null, false);
                var User = db.Users.Single(x => x.Email == Email);
                if (User.Password == Password)
                {
                    return (User,true);
                }
                return (null, false);
            });
            
        }
        public void FastAuth()
        {
            emailBox.Text = "sobakka@gmail.com";
            passwordBox.Password = "XARAKTER";
        }
    }
}
