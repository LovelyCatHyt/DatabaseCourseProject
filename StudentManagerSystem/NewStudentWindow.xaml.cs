using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
    /// NewStudentWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewStudentWindow : Window
    {
        public CollectionViewSource newStudentSource;

        private readonly DbContext _dbCtx;
        private readonly ObservableCollection<Student> _newStudentList;
        private readonly StudentDbValidator _dbValidator;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">数据库上下文</param>
        public NewStudentWindow(DbContext dbContext)
        {
            _dbCtx = dbContext;
            _dbValidator = new StudentDbValidator(_dbCtx);
            InitializeComponent();
            // 设置 UI 元素
            studentGender.ItemsSource = new[] { "男", "女" };
            studentClass.ItemsSource = dbContext.Set<NaturalClass>().Select(x => x.ClassId).ToArray();
            ErrorMsg.Content = "";
            // 设置数据源
            newStudentSource = (CollectionViewSource)FindResource(nameof(newStudentSource));
            newStudentSource.Source = _newStudentList = new ObservableCollection<Student>();
            _newStudentList.Add(GenNewStudent());
            
            // _newStudentList.Add(new Student());
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Cancel();
        }

        public void Cancel()
        {
            _newStudentList.Clear();
        }

        public void Commit()
        {
            foreach (var student in _newStudentList)
            {
                _dbCtx.Add(student);
            }
        }

        public Student GenNewStudent(Student? student = null)
        {
            // TODO: 生成一个符合规则的学号
            student ??= new Student();
            var maxId = new[] { _dbCtx.Set<Student>().Max(s => s.Id)!, _newStudentList.Max(s => s.Id) ?? "0" }.Max();
            // 只要数据库里起码有一个学生, 这里就不会是 null
            student.Id = (long.Parse(maxId!) + 1).ToString();
            student.Name = "姓名";
            student.IsMale = true;
            var now = DateTime.Now;
            student.Birth = new DateTime(now.Year - 18, 1, 1);
            student.ClassId = _dbCtx.Set<NaturalClass>().First().ClassId;
            return student;
        }

        public bool CheckDataValid()
        {
            var valid = _dbValidator.Validate(_newStudentList, out var msg);
            ErrorMsg.Content = valid ? "" : msg;
            saveButton.IsEnabled = valid;
            return valid;
        }

        private void toAddStudentGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {
            var s = (Student)e.NewItem;
            e.NewItem = GenNewStudent(s);
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckDataValid()) return;
            Commit();
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void toAddStudentGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            CheckDataValid();
        }

        private void ResetRowButton_OnClick(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow row)
                {
                    var index = row.GetIndex();
                    if (_newStudentList.Count > index)
                    {
                        var temp = GenNewStudent(_newStudentList[index]);
                        _newStudentList[index] = new Student();
                        _newStudentList[index] = temp;
                    }
                    else
                    {
                        _newStudentList.Add(GenNewStudent());
                    }
                    break;
                }
            }

            CheckDataValid();
        }

        private void DeleteRowButton_OnClick(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow row)
                {
                    var index = row.GetIndex();
                    _newStudentList.RemoveAt(index);
                    break;
                }
            }

            CheckDataValid();
        }
    }
}
