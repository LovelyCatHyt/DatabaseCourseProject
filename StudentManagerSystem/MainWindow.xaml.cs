using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
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
        public StudentDataBase StudentDataBase => _studentDbContext ??= new StudentDataBase();
        public bool DataBaseDirty
        {
            get => _dataBaseDirty;
            set
            {
                _dataBaseDirty = value;
                Title = _dataBaseDirty ? $"{BasicTitle}*" : BasicTitle;
            }
        }

        /// <summary>
        /// 学生视图表
        /// </summary>
        public CollectionViewSource studentViewSource;

        private bool _dataBaseDirty;
        private StudentDataBase? _studentDbContext;
        private ObservableCollection<Student>? _studentDataSource;
        private NewStudentWindow? _newStudentWindow;

        public MainWindow()
        {
            Debug.WriteLine("/---------------------------------/\n" +
                            "|StudentManageSystem\n" +
                            "|State: Before MainWindow.Ctor\n" +
                            "|Detail: component not initialized.\n" +
                            "/---------------------------------/");

            InitializeComponent();
            // 获取或设置XAML资源
            studentViewSource = (CollectionViewSource)FindResource(nameof(studentViewSource));
            DataBaseDirty = false;
            studentGender.ItemsSource = new[] { "男", "女" };
            var classIdsQueryable = StudentDataBase.Set<NaturalClass>().Select(x => x.ClassId);
            // Debug.WriteLine($"Query class id: \n{classIdsQueryable.ToQueryString()}");
            studentClass.ItemsSource = classIdsQueryable.ToArray();

            // 初始化数据库
            StudentDataBase.Database.Migrate();
            StudentDataBase.Students.Load();

            // 绑定数据源
            _studentDataSource = StudentDataBase.Students.Local.ToObservableCollection();
            studentViewSource.Source = _studentDataSource;

        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _newStudentWindow?.Close();
            _studentDbContext!.Dispose();
        }
        
        private void studentsDataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (StudentDataBase.ChangeTracker.HasChanges()) DataBaseDirty = true;
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

        private void RefreshDataGrid()
        {
            studentViewSource.Source = _studentDataSource = StudentDataBase.Students.Local.ToObservableCollection();
            studentsDataGrid.ItemsSource = _studentDataSource;
        }

        private void studentsDataGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (StudentDataBase.ChangeTracker.HasChanges()) DataBaseDirty = true;
        }

        private void RevertAllButton_Click(object sender, RoutedEventArgs e)
        {
            var reverted = RollBack(StudentDataBase);
            Debug.WriteLine($"{reverted} Changes removed.");
            DataBaseDirty = false;
            RefreshDataGrid();
        }

        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            _newStudentWindow ??= new NewStudentWindow(StudentDataBase);
            _newStudentWindow.Closed += (_, _) => _newStudentWindow = new NewStudentWindow(StudentDataBase);
            _newStudentWindow.Show();
            _newStudentWindow.Activate();
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            StudentDataBase.SaveChanges();
            DataBaseDirty = false;
        }

        private void RemoveStudentButton_Click(object sender, RoutedEventArgs e)
        {
            if (studentsDataGrid.CurrentItem != null)
            {
                studentsDataGrid.Items.Remove(studentsDataGrid.SelectedItem);
            }
        }

        
    }
}
