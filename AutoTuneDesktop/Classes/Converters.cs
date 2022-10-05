using AutoTuneDesktop.Db;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AutoTuneDesktop.Classes
{
    public class FIOConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Users u = value as Users;
            if(u != null)
            {
                if (u.LastName != null)
                    return ($"{u.SecondName} {u.FirstName[0]}.{u.LastName[0]}.");
                else
                    return ($"{u.SecondName} {u.FirstName[0]}");
            }
            else
            {
                return "";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
