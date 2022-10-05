using AutoTuneDesktop.Classes;
using AutoTuneDesktop.Db;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;

namespace AutoTuneDesktop.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddEditUser.xaml
    /// </summary>
    public partial class AddEditUser : Window
    {
        public Users bindingUser { get; set; }
        AutoTuneEntities db = new AutoTuneEntities();
        public AddEditUser(int userID,int activeUserRolesID)
        {
            InitializeComponent();

            DataContext = this;
            if(activeUserRolesID == 3)
                roleBox.IsEnabled = false;
            if (userID == 0)
            {
                bindingUser = new Users();
                bindingUser.IsActive = true;
                bindingUser.Roles = db.Roles.Where(x => x.Name == "Клиент").SingleOrDefault();
            }
            else
                bindingUser = db.Users.Where(x => x.ID == userID).FirstOrDefault();
            loadComboBoxes();
        }
        public void loadComboBoxes()
        {
            genderBox.ItemsSource = db.Genders.ToList();
            roleBox.ItemsSource = db.Roles.ToList();
        }
        private void changePhotoClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Выберите изображение";
            op.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";
            if (op.ShowDialog() == true)
            {
                bindingUser.Photo = File.ReadAllBytes(op.FileName);
                DataContext = null;
                DataContext = this;
            }
        }

        private void editClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void saveClick(object sender, RoutedEventArgs e)
        {
            if (bindingUser.ID == 0)
            {
                if (HasEmptyBoxes() == false)
                {
                    try
                    {
                        db.Users.Add(bindingUser);
                        db.SaveChanges();
                        Messages.ShowInfo("Успешно!");
                        this.Close();
                    }
                    catch(Exception)
                    {
                        Messages.ShowError("Не удалось добавить пользователя");
                    }
                }
                else
                    Messages.ShowError("Не удалось сохранить изменения, проверьте все ли поля заполнены");

            }
            else
            {
                if (db.ChangeTracker.HasChanges() == true)
                {
                    if (HasEmptyBoxes() == false)
                    {
                        if(passwordNameBox.Text.Length < 8)
                        {
                            Messages.ShowError("Длина пароля не может быть меньше 8 символов");
                        }
                        else
                        {
                            try
                            {
                                db.SaveChanges();
                                Messages.ShowInfo("Успешно!");
                                this.Close();
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                                Messages.ShowError("Не удалось сохранить изменения");
                            }
                        }
                    }
                    else
                        Messages.ShowError("Не удалось сохранить изменения, проверьте все ли поля заполнены");
                }
                else
                {
                    Messages.ShowError("Вы не внесли какие-либо изменения");
                }
            }
           
        }
        public bool HasEmptyBoxes()
        {
            if (secondNameBox.Text == "" || firstNameBox.Text == "" || phoneNameBox.Text.Contains("_")|| passwordNameBox.Text == "" || 
                genderBox.SelectedItem == null || roleBox.SelectedIndex == -1 || dateBox.SelectedDate == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
