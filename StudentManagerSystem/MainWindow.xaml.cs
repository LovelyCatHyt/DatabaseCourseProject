using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using StudentManageSystem.DataBase;
using StudentManageSystem.Entities;

namespace StudentManageSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public const string BasicTitle = "学生管理系统";

        /// <summary>
        /// 学生数据库
        /// </summary>
        public StudentDataBase studentDataBase;
        public bool DataBaseDirty
        {
            get => _dataBaseDirty;
            set
            {
                _dataBaseDirty = value;
                Title = _dataBaseDirty ? $"{BasicTitle}*" : BasicTitle;
                if (value) CheckDataValid();
            }
        }

        /// <summary>
        /// 学生视图表
        /// </summary>
        public CollectionViewSource studentViewSource;
        /// <summary>
        /// 班级视图表
        /// </summary>
        public CollectionViewSource classViewSource;
        /// <summary>
        /// 院系视图表
        /// </summary>
        public CollectionViewSource departmentViewSource;
        /// <summary>
        /// 专业视图表
        /// </summary>
        public CollectionViewSource majorViewSource;

        private bool _dataBaseDirty;
        private readonly StudentDbValidator _studentDbValidator;
        private ObservableCollection<Student> _studentDataSource;
        private NewStudentWindow? _newStudentWindow;

        private ObservableCollection<NaturalClass> _classDataSource;
        private NewClassWindow? _newClassWindow;

        private ObservableCollection<Department> _departmentDataSource;
        private NewDepartmentWindow? _newDepartmentWindow;

        private ObservableCollection<Major> _majorDataSource;
        private NewMajorWindow? _newMajorWindow;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            Debug.WriteLine("/---------------------------------/\n" +
                            "|StudentManageSystem\n" +
                            "|State: Before MainWindow.Ctor\n" +
                            "|Detail: component not initialized.\n" +
                            "/---------------------------------/");

            InitializeComponent();

            // 初始化数据库
            studentDataBase = new StudentDataBase();
            studentDataBase.Database.Migrate();
            studentDataBase.Students.Load();
            _studentDbValidator = new StudentDbValidator(studentDataBase);
            studentDataBase.Classes.Load();
            studentDataBase.Departments.Load();
            studentDataBase.Majors.Load();

            // 获取或设置XAML资源
            studentViewSource = (CollectionViewSource)FindResource(nameof(studentViewSource));
            classViewSource = (CollectionViewSource) FindResource(nameof(classViewSource));
            departmentViewSource = (CollectionViewSource) FindResource(nameof(departmentViewSource));
            majorViewSource = (CollectionViewSource)FindResource(nameof(majorViewSource));
            DataBaseDirty = false;
            studentGender.ItemsSource = new[] { "男", "女" };
            studentClass.ItemsSource = studentDataBase.Set<NaturalClass>().Select(x => x.ClassId).ToArray();
            ClassDepartmentName.ItemsSource = studentDataBase.Departments.Select(d => d.Name).ToArray();
            ClassMajorName.ItemsSource = studentDataBase.Majors.Select(m => m.MajorName).ToArray();
            // MajorDepartmentName.ItemsSource = studentDataBase.Departments.Select(d => d.Name).ToArray();
            // TODO: 逐行设置 ItemSource
            // ClassMajorName.ItemsSource = ?


            // 绑定数据源
            _studentDataSource = studentDataBase.Students.Local.ToObservableCollection();
            // InitVisitors(_studentDataSource);
            studentViewSource.Source = _studentDataSource;
            _classDataSource = studentDataBase.Classes.Local.ToObservableCollection();
            InitVisitors(_classDataSource);
            classViewSource.Source = _classDataSource;
            _departmentDataSource = studentDataBase.Departments.Local.ToObservableCollection();
            // InitVisitors(_departmentDataSource);
            departmentViewSource.Source = _departmentDataSource;
            _majorDataSource = studentDataBase.Majors.Local.ToObservableCollection();
            InitVisitors(_majorDataSource);
            majorViewSource.Source = _majorDataSource;

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _newStudentWindow?.Close();
            _newClassWindow?.Close();
            _newDepartmentWindow?.Close();
            _newMajorWindow?.Close();
            studentDataBase.Dispose();
        }

        private void InitVisitors(IEnumerable<IDbVisitor> visitors)
        {
            foreach (var dbVisitor in visitors)
            {
                dbVisitor.Init(studentDataBase);
            }
        }

        private void DataGridCellChanged(object sender, EventArgs e)
        {
            if (studentDataBase.ChangeTracker.HasChanges()) DataBaseDirty = true;
        }

        private int RollBack(DbContext ctx)
        {
            var reverted = 0;
            foreach (var entityEntry in ctx.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged))
            {
                switch (entityEntry.State)
                {
                    case EntityState.Deleted:
                        entityEntry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Modified:
                        entityEntry.CurrentValues.SetValues(entityEntry.OriginalValues);
                        entityEntry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entityEntry.State = EntityState.Detached;
                        break;
                }

                reverted++;
            }

            return reverted;
        }

        private bool CheckDataValid()
        {
            var valid = _studentDbValidator.ValidateData(_studentDataSource, out var msg);
            ErrorMsg.Text = valid ? "" : msg;
            return valid;
        }

        private void RefreshDataGrid()
        {
            studentViewSource.Source = _studentDataSource = studentDataBase.Students.Local.ToObservableCollection();
            studentsDataGrid.ItemsSource = _studentDataSource;
            classViewSource.Source = _classDataSource = studentDataBase.Classes.Local.ToObservableCollection();
            classDataGrid.ItemsSource = _classDataSource;
            departmentViewSource.Source = _departmentDataSource = studentDataBase.Departments.Local.ToObservableCollection();
            departmentDataGrid.ItemsSource = _departmentDataSource;
            CheckDataValid();
        }

        private void RevertAllButton_Click(object sender, RoutedEventArgs e)
        {
            var reverted = RollBack(studentDataBase);
            Debug.WriteLine($"{reverted} Changes removed.");
            DataBaseDirty = false;
            RefreshDataGrid();
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            studentDataBase.SaveChanges();
            DataBaseDirty = false;
        }

        private void DataGridMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (studentDataBase.ChangeTracker.HasChanges()) DataBaseDirty = true;
        }
        
        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            _newStudentWindow ??= new NewStudentWindow(studentDataBase);
            _newStudentWindow.Closed += (_, _) => _newStudentWindow = null;
            _newStudentWindow.Show();
            _newStudentWindow.Activate();
        }

        private void RemoveStudentButton_Click(object sender, RoutedEventArgs e)
        {
            var students = studentsDataGrid.SelectedItems.OfType<Student>().ToArray();
            foreach (var student in students)
            {
                _studentDataSource.Remove(student);
            }
        }

        private void DatePicker_OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e)
        {
            //if (e.RemovedItems.Count > 0 && e.AddedItems.Count > 0) Debug.WriteLine($"perv: {e.RemovedItems[0]}, new: {e.AddedItems[0]}");
            //Debug.WriteLine($"Origin source: {e.OriginalSource}");
            //var temp = (Visual?)sender;
            //while (temp != null && temp is not DataGridRow) temp = VisualTreeHelper.GetParent(temp) as Visual;
            //if (temp == null) return;
            //var row = (DataGridRow) temp;
            //row.DataContext
            var temp = DataBaseDirty;
            if (studentDataBase.ChangeTracker.HasChanges()) DataBaseDirty = true;
            if (temp != DataBaseDirty)
                Debug.WriteLine("Detect change in DatePicker_OnSelectedDateChanged");
        }

        private void AddClassButton_Click(object sender, RoutedEventArgs e)
        {
            _newClassWindow ??= new NewClassWindow(studentDataBase);
            _newClassWindow.Closed += (_, _) => _newClassWindow = null;
            _newClassWindow.Show();
            _newClassWindow.Activate();
        }

        private void RemoveClassButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void AddDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            _newDepartmentWindow ??= new NewDepartmentWindow(studentDataBase);
            _newDepartmentWindow.Closed += (_, _) => _newDepartmentWindow = null;
            _newDepartmentWindow.Show();
            _newDepartmentWindow.Activate();
        }

        private void RemoveDepartmentButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void AddMajor_Click(object sender, RoutedEventArgs e)
        {
            _newMajorWindow ??= new NewMajorWindow(studentDataBase);
            _newMajorWindow.Closed += (_, _) => _newMajorWindow = null;
            _newMajorWindow.Show();
            _newMajorWindow.Activate();
        }

        private void RemoveMajor_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }
    }
}
