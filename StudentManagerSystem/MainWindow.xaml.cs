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

        private bool _dataBaseDirty;
        private StudentDbValidator _studentDbValidator;
        private ObservableCollection<Student> _studentDataSource;
        private NewStudentWindow? _newStudentWindow;


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

            // 获取或设置XAML资源
            studentViewSource = (CollectionViewSource)FindResource(nameof(studentViewSource));
            DataBaseDirty = false;
            studentGender.ItemsSource = new[] { "男", "女" };
            var classIdsQueryable = studentDataBase.Set<NaturalClass>().Select(x => x.ClassId);
            studentClass.ItemsSource = classIdsQueryable.ToArray();

            // 绑定数据源
            _studentDataSource = studentDataBase.Students.Local.ToObservableCollection();
            studentViewSource.Source = _studentDataSource;

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _newStudentWindow?.Close();
            studentDataBase.Dispose();
        }

        private void studentsDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            // TODO: remove debug
            var temp = DataBaseDirty;
            if (studentDataBase.ChangeTracker.HasChanges()) DataBaseDirty = true;
            if(temp != DataBaseDirty)
                Debug.WriteLine("Detect change in CurrentCellChanged");
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
            CheckDataValid();
        }

        private void studentsDataGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var temp = DataBaseDirty;
            if (studentDataBase.ChangeTracker.HasChanges()) DataBaseDirty = true;
            if(temp!= DataBaseDirty)
                Debug.WriteLine("Detect change in MouseDown");
        }

        private void RevertAllButton_Click(object sender, RoutedEventArgs e)
        {
            var reverted = RollBack(studentDataBase);
            Debug.WriteLine($"{reverted} Changes removed.");
            DataBaseDirty = false;
            RefreshDataGrid();
        }

        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            _newStudentWindow ??= new NewStudentWindow(studentDataBase);
            _newStudentWindow.Closed += (_, _) => _newStudentWindow = new NewStudentWindow(studentDataBase);
            _newStudentWindow.Show();
            _newStudentWindow.Activate();
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            studentDataBase.SaveChanges();
            DataBaseDirty = false;
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
    }
}
