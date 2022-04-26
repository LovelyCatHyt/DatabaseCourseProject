using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using StudentManageSystem.DataBase;

namespace StudentManageSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public StudentDataBase StudentDbContext { get; }

        public MainWindow()
        {
            StudentDbContext = new StudentDataBase();
            Debug.WriteLine(StudentDbContext.DbPath);
            StudentDbContext.Database.Migrate();
            InitializeComponent();
        }

        ~MainWindow()
        {
            StudentDbContext.Dispose();
        }
    }
}
