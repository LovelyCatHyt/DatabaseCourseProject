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

        public NewClassWindow(DbContext dbContext, int newClassId)
        {
            InitializeComponent();
            _dbContext = dbContext;
            _newClass = new NaturalClass
            {
                ClassId = newClassId,
                DepartmentId = dbContext.Set<Department>().First().DepartmentId
            };
            _newClass.Init(dbContext);
            DataContext = _newClass;
            var departmentNames = dbContext.Set<Department>().Select(d => d.Name).ToArray();
            ClassDepartmentNameCbBox.ItemsSource = departmentNames;
            ClassDepartmentNameCbBox.Focus();
            ClassDepartmentNameCbBox.SelectedValue = departmentNames[0];

            ClassMajorNameCbBox.ItemsSource = _newClass.MajorNames;
            ClassMajorNameCbBox.SelectedValue = ClassMajorNameCbBox.ItemsSource.OfType<object>().First();
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

        private void DepartmentChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var names = _newClass.MajorNames;
            ClassMajorNameCbBox.ItemsSource = names;
            ClassMajorNameCbBox.SelectedValue = names[0];
        }
    }
}
