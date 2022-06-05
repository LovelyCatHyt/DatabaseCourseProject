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

namespace StudentManageSystem
{
    /// <summary>
    /// NewCourseWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewCourseWindow : Window
    {

        private readonly DbContext _dbContext;
        private readonly Course _newCourse;

        public NewCourseWindow(DbContext context, int newId)
        {
            InitializeComponent();
            _dbContext = context;
            DataContext = _newCourse = new Course{Id = newId, Name = ""};
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            _dbContext.Add(_newCourse);
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
