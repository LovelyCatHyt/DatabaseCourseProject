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
using Microsoft.EntityFrameworkCore;
using StudentManageSystem.Entities;
using StudentManageSystem.ViewModels;

namespace StudentManageSystem
{
    /// <summary>
    /// StudentDetailWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StudentDetailWindow : Window
    {
        public DbContext _context;
        public StudentDetail _student;

        public StudentDetailWindow(Student target, DbContext context)
        {
            InitializeComponent();
            _context = context;
            DataContext = _student = new StudentDetail(target, context);
            
            // XAML 属性
            GenderCbBox.ItemsSource = new[] {"男", "女"};
        }
    }
}
