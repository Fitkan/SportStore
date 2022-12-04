﻿using SportStore.Models;
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
using SportStore.Infrastructure;
using SportStore;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SportStore
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    ///

    public partial class LoginWindow : Window
    {
        bool verify = true;
        int verifyCheck = 0;

        async void disableButton()
        {
            loginButton.IsEnabled = false;
            await Task.Delay(TimeSpan.FromSeconds(10));
            loginButton.IsEnabled = true;
        }


        public LoginWindow()
        {
            InitializeComponent();

            captchaBlock.Visibility = Visibility.Collapsed;
            captchaBox.Visibility = Visibility.Collapsed;

        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            using (SportStoreContext db = new SportStoreContext())
            {
                // проверка, если есть каптча
                if (captchaBlock.Visibility == Visibility.Visible)
                {
                    if (captchaBlock.Text == captchaBox.Text)
                    {
                        verify = true;
                    }
                }

                User user = db.Users.Where(u => u.Login == loginBox.Text && u.Password == passwordBox.Password).Include(u => u.RoleNavigation).FirstOrDefault();

                // admin
                if (user != null && verify)
                {
                    new MainWindow(user).Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неуспешная авторизация");
                    verifyCheck += 1;

                    // captcha view
                    captchaBlock.Visibility = Visibility.Visible;
                    captchaBox.Visibility = Visibility.Visible;

                    captchaBlock.Text = CaptchaBuilder.Refresh();
                    verify = false;

                    if (verifyCheck > 1)
                    {
                        disableButton();
                        captchaBlock.Text = CaptchaBuilder.Refresh();
                    }
                }
            }


        }

        private void guestButtton_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(null).Show();
            this.Close();
        }
           

    }
}
