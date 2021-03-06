using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        /// <summary>
        /// 课程视图表
        /// </summary>
        public CollectionViewSource courseViewSource;
        /// <summary>
        /// 选课视图表
        /// </summary>
        public CollectionViewSource courseSelectionViewSource;

        private bool _dataBaseDirty;
        private readonly StudentDbValidator _studentDbValidator;
        private ObservableCollection<Student> _studentDataSource;
        private NewStudentWindow? _newStudentWindow;
        private StudentDetailWindow? _studentDetailWindow;

        private readonly ClassDbValidator _classValidator;
        private ObservableCollection<NaturalClass> _classDataSource;
        private NewClassWindow? _newClassWindow;

        private readonly DepartmentDbValidator _departmentValidator;
        private ObservableCollection<Department> _departmentDataSource;
        private NewDepartmentWindow? _newDepartmentWindow;

        private readonly MajorDbValidator _majorValidator;
        private ObservableCollection<Major> _majorDataSource;
        private NewMajorWindow? _newMajorWindow;
        
        private readonly CourseDbValidator _courseDbValidator;
        private ObservableCollection<Course> _courseDataSource;
        private NewCourseWindow? _newCourseWindow;

        private readonly CourseSelectionDbValidator _courseSelectionDbValidator;
        private ObservableCollection<CourseSelection> _courseSelectionDataSource;
        
        private IQueryable<Student>? _lastQuery;

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
            _classValidator = new ClassDbValidator(studentDataBase);

            studentDataBase.Departments.Load();
            _departmentValidator = new DepartmentDbValidator(studentDataBase);

            studentDataBase.Majors.Load();
            _majorValidator = new MajorDbValidator(studentDataBase);

            studentDataBase.Courses.Load();
            _courseDbValidator = new CourseDbValidator(studentDataBase);

            studentDataBase.Selections.Load();
            _courseSelectionDbValidator = new CourseSelectionDbValidator(studentDataBase);

            // 获取或设置XAML资源
            studentViewSource = (CollectionViewSource)FindResource(nameof(studentViewSource));
            classViewSource = (CollectionViewSource)FindResource(nameof(classViewSource));
            departmentViewSource = (CollectionViewSource)FindResource(nameof(departmentViewSource));
            majorViewSource = (CollectionViewSource)FindResource(nameof(majorViewSource));
            courseViewSource = (CollectionViewSource)FindResource(nameof(courseViewSource));
            courseSelectionViewSource = (CollectionViewSource)FindResource(nameof(courseSelectionViewSource));
            DataBaseDirty = false;
            studentGender.ItemsSource = new[] { "男", "女" };
            SqlViewer.OnSqlUpdated += () => sqlList.ItemsSource = SqlViewer.SqlStatements;
            var intQueryable = studentDataBase.Set<NaturalClass>().Select(x => x.ClassId);
            SqlViewer.Add(intQueryable);
            studentClass.ItemsSource = intQueryable.ToArray();
            var strQueryable = studentDataBase.Departments.Select(d => d.Name);
            SqlViewer.Add(strQueryable);
            classDepartmentName.ItemsSource = strQueryable.ToArray();
            queryGrid.Visibility = Visibility.Collapsed;

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

            _courseDataSource = studentDataBase.Courses.Local.ToObservableCollection();
            courseViewSource.Source = _courseDataSource;

            _courseSelectionDataSource = studentDataBase.Selections.Local.ToObservableCollection();
            InitVisitors(_courseSelectionDataSource);
            courseSelectionViewSource.Source = _courseSelectionDataSource;

        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (DataBaseDirty)
            {
                switch (MessageBox.Show("数据变更未保存, 是否保存?", "未保存退出提示", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning))
                {
                    case MessageBoxResult.None:         // 直接关掉?
                    case MessageBoxResult.Cancel:       // 取消等同于直接关掉
                        e.Cancel = true;
                        return;
                    case MessageBoxResult.Yes:          // 保存并退出
                        studentDataBase.SaveChanges();
                        CloseAllWindow();
                        studentDataBase.Dispose();
                        return;
                    case MessageBoxResult.No:           // 不保存退出
                        CloseAllWindow();
                        studentDataBase.Dispose();
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            }
            else
            {
                CloseAllWindow();
                studentDataBase.Dispose();
            }
        }

        private void CloseAllWindow()
        {
            _newStudentWindow?.Close();
            _newClassWindow?.Close();
            _newDepartmentWindow?.Close();
            _newMajorWindow?.Close();
        }

        private void InitVisitors(IEnumerable<IDbVisitor> visitors)
        {
            foreach (var dbVisitor in visitors)
            {
                dbVisitor.Init(studentDataBase);
            }
        }

        private void DataGridCellChanged(object sender, EventArgs e) => DetectChange();

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
            var info = _studentDbValidator.ValidateRange(_studentDataSource);
            info.AddOrConcat(_classValidator.ValidateRange(_classDataSource));
            info.AddOrConcat(_majorValidator.ValidateRange(_majorDataSource));
            info.AddOrConcat(_departmentValidator.ValidateRange(_departmentDataSource));
            info.AddOrConcat(_courseDbValidator.ValidateRange(_courseDataSource));
            info.AddOrConcat(_courseSelectionDbValidator.ValidateRange(_courseSelectionDataSource));
            ErrorMsg.Text = info;
            return info;
        }

        private void DetectChange()
        {
            if (studentDataBase.ChangeTracker.HasChanges())
            {
                DataBaseDirty = true;
                //// var departmentColumn = classDataGrid.Columns.First(c => (string) c.Header == "院系");
                //var majorColumn = classDataGrid.Columns.First(c => (string) c.Header == "专业") as DataGridComboBoxColumn;
                //for (var index = 0; index < _classDataSource.Count; index++)
                //{
                //    var cls = _classDataSource[index];

                //}
            }
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

        private void RevertAll(object sender, RoutedEventArgs e)
        {
            var reverted = RollBack(studentDataBase);
            Debug.WriteLine($"{reverted} Changes removed.");
            DataBaseDirty = false;
            RefreshDataGrid();
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            studentDataBase.SaveChanges();
            DataBaseDirty = false;
        }

        private void DataGridMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) => DetectChange();

        private void DatePicker_OnSelectedDateChanged(object? sender, SelectionChangedEventArgs e) => DetectChange();

        private void AddStudent(object sender, RoutedEventArgs e)
        {
            _newStudentWindow ??= new NewStudentWindow(studentDataBase);
            _newStudentWindow.Closed += (_, _) =>
            {
                DetectChange();
                _newStudentWindow = null;
            };
            _newStudentWindow.Show();
            _newStudentWindow.Activate();
        }

        private void RemoveStudent(object sender, RoutedEventArgs e)
        {
            foreach (var student in studentsDataGrid.SelectedItems.OfType<Student>().ToArray())
            {
                _studentDataSource.Remove(student);
            }
            DetectChange();
        }

        private void AddClass(object sender, RoutedEventArgs e)
        {
            _newClassWindow ??= new NewClassWindow(studentDataBase, _classDataSource.Max(c => c.ClassId) + 1);
            _newClassWindow.Closed += (_, _) =>
            {
                _newClassWindow = null;
                DetectChange();
            };
            _newClassWindow.Show();
            _newClassWindow.Activate();
        }

        private void RemoveClass(object sender, RoutedEventArgs e)
        {
            foreach (var naturalClass in classDataGrid.SelectedItems.OfType<NaturalClass>().ToArray())
            {
                _classDataSource.Remove(naturalClass);
            }
            DetectChange();
        }

        private void AddDepartment(object sender, RoutedEventArgs e)
        {
            _newDepartmentWindow ??= new NewDepartmentWindow(studentDataBase);
            _newDepartmentWindow.Closed += (_, _) =>
            {
                DetectChange();
                _newDepartmentWindow = null;
            };
            _newDepartmentWindow.Show();
            _newDepartmentWindow.Activate();
        }

        private void RemoveDepartment(object sender, RoutedEventArgs e)
        {
            foreach (var department in departmentDataGrid.SelectedItems.OfType<Department>().ToArray())
            {
                _departmentDataSource.Remove(department);
            }
            DetectChange();
        }

        private void AddMajor(object sender, RoutedEventArgs e)
        {
            _newMajorWindow ??= new NewMajorWindow(studentDataBase);
            _newMajorWindow.Closed += (_, _) =>
            {
                DetectChange();
                _newMajorWindow = null;
            };
            _newMajorWindow.Show();
            _newMajorWindow.Activate();
        }

        private void RemoveMajor(object sender, RoutedEventArgs e)
        {
            foreach (var major in majorGrid.SelectedItems.OfType<Major>().ToArray())
            {
                _majorDataSource.Remove(major);
            }
            DetectChange();
        }

        private void AddCourse(object sender, RoutedEventArgs e)
        {
            _newCourseWindow ??= new NewCourseWindow(studentDataBase, _courseDataSource.Any() ? _courseDataSource.Max(c => c.Id) + 1 : 1);
            _newCourseWindow.Closed += (_, _) =>
            {
                DetectChange();
                _newCourseWindow = null;
            };
            _newCourseWindow.Show();
            _newCourseWindow.Activate();
        }

        private void RemoveCourse(object sender, RoutedEventArgs e)
        {
            foreach (var course in courseDataGrid.SelectedItems.OfType<Course>().ToArray())
            {
                _courseDataSource.Remove(course);
            }
            DetectChange();
        }

        private void AddCourseSelection(object sender, RoutedEventArgs e)
        {
            if (studentsDataGrid.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择一个学生再选课!", "学生管理系统-信息不足提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var newId = _courseSelectionDataSource.Any()? _courseSelectionDataSource.Max(cs => cs.Id) + 1 : 1;
            var selectedStudent = studentsDataGrid.SelectedItems[0] as Student;
            var newCourseSelection = new CourseSelection
            {
                Course = null!, CourseId = 0, Id = newId, Student = selectedStudent!, StudentId = selectedStudent!.Id
            };
            newCourseSelection.Init(studentDataBase);
            _courseSelectionDataSource.Add(newCourseSelection);
        }

        private void RemoveCourseSelection(object sender, RoutedEventArgs e)
        {
            foreach (var courseSelection in courseSelectionGrid.SelectedItems.OfType<CourseSelection>().ToArray())
            {
                _courseSelectionDataSource.Remove(courseSelection);
            }
            DetectChange();
        }

        private void ToggleQuery(object sender, RoutedEventArgs e)
        {
            queryGrid.Visibility = queryGrid.Visibility switch
            {
                Visibility.Visible => Visibility.Collapsed,
                Visibility.Hidden => Visibility.Visible,
                Visibility.Collapsed => Visibility.Visible,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void RunQuery(object sender, RoutedEventArgs e)
        {
            // TODO: Query from _lastQuery
            _lastQuery = studentDataBase.Students.AsQueryable();
            SqlViewer.Remove(_lastQuery);
            if (studentNameChkBox.IsChecked ?? false)
            {
                _lastQuery = _lastQuery.Where(s => s.Name.Contains(studentNameQueryTxt.Text.Trim()));
            }
            if (studentIdChkBox.IsChecked ?? false)
            {
                _lastQuery = _lastQuery.Where(s => s.Id.Contains(studentIdQueryTxt.Text.Trim()));
            }
            if (studentMajorChkBox.IsChecked ?? false)
            {
                _lastQuery = _lastQuery.Where(s => s.Class.MajorName.Contains(studentMajorQueryTxt.Text.Trim()));
            }
            if (studentDepartmentChkBox.IsChecked ?? false)
            {
                _lastQuery = _lastQuery.Where(s =>
                    s.Class.Department!.Name.Contains(studentDepartmentQueryTxt.Text.Trim()));
            }
            SqlViewer.Add(_lastQuery);
            studentQueryResult.ItemsSource = _lastQuery.ToArray();
        }

        private void ClearSqlViewer(object sender, RoutedEventArgs e)
        {
            SqlViewer.Clear();
        }

        private void ShowDetail(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow row)
                {
                    var index = row.GetIndex();
                    _studentDetailWindow ??=
                        new StudentDetailWindow(_studentDataSource[index], studentDataBase);
                    _studentDetailWindow.Closed += (_, _) =>
                    {
                        DetectChange();
                        _studentDetailWindow = null;
                    };
                    _studentDetailWindow.Show();
                    _studentDetailWindow.Activate();
                    break;
                }
            }
        }
    }
}
