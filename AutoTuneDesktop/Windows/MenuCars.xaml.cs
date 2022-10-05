using AutoTuneDesktop.Db;
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
using System.Windows.Shapes;

namespace AutoTuneDesktop.Windows
{
    /// <summary>
    /// Логика взаимодействия для MenuCars.xaml
    /// </summary>
    public partial class MenuCars : Window
    {
        public AutoTuneEntities db = new AutoTuneEntities();
        public int idToUpdate;
        public MenuCars(int ID)
        {
            InitializeComponent();
            idToUpdate = ID;
            DataContext = this;
            UpdateCars();
            UpdateMarks();
           
        }
        public void UpdateCars()
        {
            carsGrid.ItemsSource = db.Cars.Where(x => x.UserID == idToUpdate).ToList();
            carsGrid.SelectedIndex = 0;
        }
        public void UpdateMarks()
        {
            marksGrid.ItemsSource = db.Marks.ToList();
            marksGrid.SelectedItem = 0;
        }

        private void addCarClick(object sender, RoutedEventArgs e)
        {
            AddEditCar adc = new AddEditCar(0,idToUpdate);
            adc.ShowDialog();
            db = new AutoTuneEntities();
            UpdateCars();
        }

        private void editClick(object sender, RoutedEventArgs e)
        {
            Cars car = (Cars)carsGrid.SelectedItem;
            AddEditCar adc = new AddEditCar(car.ID,idToUpdate);
            adc.ShowDialog();
            db = new AutoTuneEntities();
            UpdateCars();
        }

        private void addMarkClick(object sender, RoutedEventArgs e)
        {
            
            AddEditMark adm = new AddEditMark(0);
            adm.ShowDialog();
            db = new AutoTuneEntities();
            UpdateMarks();
        }

        private void close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void editMarkClick(object sender, RoutedEventArgs e)
        {
            Cars sentCar = (Cars)carsGrid.SelectedItem;
            AddEditMark adm = new AddEditMark(sentCar.ID);
            adm.ShowDialog();
            db = new AutoTuneEntities();
            UpdateMarks();
        }
    }
}
