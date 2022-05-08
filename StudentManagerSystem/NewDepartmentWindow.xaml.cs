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
    /// NewDepartmentWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewDepartmentWindow : Window
    {
        private readonly DbContext _dbContext;
        private readonly Department _newDepartment;

        public NewDepartmentWindow(DbContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            var set = _dbContext.Set<Department>();
            _newDepartment = new Department
            {
                DepartmentId = set.Max(d => d.DepartmentId) + 1,
                DepartmentType = "工科",
                Name = ""
            };
            DataContext = _newDepartment;
            DepartmentTypeCbBox.ItemsSource = new[] { "哲学", "经济学", "法学", "教育", "文学", "历史", "理科", "工科", "农科", "医科", "军事", "管理", "艺术" };
            DepartmentNameTxt.Focus();
        }
        
        private void Confirm(object sender, RoutedEventArgs e)
        {
            _dbContext.Add(_newDepartment);
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
