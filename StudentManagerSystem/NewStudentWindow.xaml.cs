using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
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

        public NewStudentWindow(DbContext dbContext)
        {
            _dbCtx = dbContext;
            InitializeComponent();
            // 设置列属性
            studentGender.ItemsSource = new[] { "男", "女" };
            studentClass.ItemsSource = dbContext.Set<NaturalClass>().Select(x => x.ClassId).ToArray();
            // 设置数据源
            newStudentSource = (CollectionViewSource)FindResource(nameof(newStudentSource));
            newStudentSource.Source = _newStudentList = new ObservableCollection<Student>();
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

        public Student GenNewStudent(Student? student)
        {
            // TODO: 生成一个符合规则的学号
            student ??= new Student();
            var maxId = new[] { _dbCtx.Set<Student>().Max(s => s.Id)!, _newStudentList.Max(s => s.Id) ?? "0" }.Max();
            // 只要数据库里起码有一个学生, 这里就不会是 null
            student.Id = (long.Parse(maxId!) + 1).ToString();
            var now = DateTime.Now;
            student.Birth = new DateTime(now.Year - 18, 1, 1);
            student.ClassId = _dbCtx.Set<NaturalClass>().First().ClassId;
            return student;
        }

        private void toAddStudentGrid_AddingNewItem(object sender, System.Windows.Controls.AddingNewItemEventArgs e)
        {
            var s = (Student) e.NewItem;
            e.NewItem = GenNewStudent(s);
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Commit();
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
