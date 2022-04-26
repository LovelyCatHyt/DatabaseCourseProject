using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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
        private ICollection<Student>? _studentDataSource;

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

            // 初始化数据库
            StudentDataBase.Database.Migrate();
            StudentDataBase.Students.Load();

            // 绑定数据源
            _studentDataSource = StudentDataBase.Students.Local.ToObservableCollection();
            studentViewSource.Source = _studentDataSource;
            
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _studentDbContext!.Dispose();
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            StudentDataBase.SaveChanges();
            DataBaseDirty = false;
        }
        
        private void studentsDataGrid_CurrentCellChanged(object sender, System.EventArgs e)
        {
            if (StudentDataBase.ChangeTracker.HasChanges()) DataBaseDirty = true;
        }
    }
}
