using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using StudentManageSystem.Entities;

namespace StudentManageSystem
{
    /// <summary>
    /// NewClassWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewClassWindow : Window
    {
        private readonly NaturalClass _newClass;
        private readonly DbContext _dbContext;

        public NewClassWindow(DbContext dbContext)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _newClass = new NaturalClass
            {
                ClassId = dbContext.Set<NaturalClass>().Max(c => c.ClassId) + 1,
                DepartmentId = dbContext.Set<Department>().First().DepartmentId
            };
            DataContext = _newClass;
            ClassDepartmentIdCbBox.ItemsSource = dbContext.Set<Department>().Select(d => d.DepartmentId).ToArray();
            ClassDepartmentIdCbBox.Focus();
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            _dbContext.Add(_newClass);
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
