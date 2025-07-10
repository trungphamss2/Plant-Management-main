using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PlantManagement.Helpers
{
    public static class UserSession
    {
        public static string Username
        {
            get => Application.Current.Resources["username"] as string;
            set => Application.Current.Resources["username"] = value;
        }
    }

}