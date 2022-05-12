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
    /// NewMajorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewMajorWindow : Window
    {
        private DbContext _context;

        private Major _newMajor;

        public NewMajorWindow(DbContext context)
        {
            InitializeComponent();
            _context = context;
            var departments = _context.Set<Department>().Select(d => d.Name).ToArray();
            DataContext = _newMajor = new Major();
            _newMajor.Init(_context);
            // TODO: 无院系时可能会越界
            _newMajor.DepartmentName = departments[0];
            DepartmentNameTxtCbBox.ItemsSource = departments;
            DepartmentNameTxtCbBox.SelectedValue = departments[0];
            MajorNameTxt.Focus();
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            _context.Add(_newMajor);
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
