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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

namespace DBEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Employee> workers = new ObservableCollection<Employee>();
        ObservableCollection<Department> deps = new ObservableCollection<Department>();
        ObservableCollection<Employee> deleted = new ObservableCollection<Employee>();
        public Employee TheWorker;

        SqlConnection connection;
        SqlDataAdapter adapter;
        DataTable dt;
        public MainWindow()
        {

            InitializeComponent();
            

        }

    
            private void Window_Loaded(object sender, RoutedEventArgs e)
            {
                var connectionStringBuilder = new SqlConnectionStringBuilder
                {
                    DataSource = @"(localdb)\MSSQLLocalDB",
                    InitialCatalog = "demo1603",
                };
                connection = new SqlConnection(connectionStringBuilder.ConnectionString);
                adapter = new SqlDataAdapter();
                SqlCommand command = new SqlCommand("SELECT ID, FIO, Birthday, Email, Phone FROM People", connection);
                adapter.SelectCommand = command;

                //insert
                command = new SqlCommand(@"INSERT INTO People (FIO, Birthday, Email, Phone) 
                          VALUES (@FIO, @Birthday, @Email, @Phone); SET @ID = @@IDENTITY;",
                              connection);

                command.Parameters.Add("@FIO", SqlDbType.NVarChar, -1, "FIO");
                command.Parameters.Add("@Birthday", SqlDbType.NVarChar, -1, "Birthday");
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 100, "Email");
                command.Parameters.Add("@Phone", SqlDbType.NVarChar, -1, "Phone");

                SqlParameter param = command.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");

                param.Direction = ParameterDirection.Output;
                adapter.InsertCommand = command;


                // update
                command = new SqlCommand(@"UPDATE People SET FIO = @FIO,
Birthday = @Birthday, Email = @Email, Phone = @Phone WHERE ID = @ID", connection);

                command.Parameters.Add("@FIO", SqlDbType.NVarChar, -1, "FIO");
                command.Parameters.Add("@Birthday", SqlDbType.NVarChar, -1, "Birthday");
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 100, "Email");
                command.Parameters.Add("@Phone", SqlDbType.NVarChar, -1, "Phone");
                param = command.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");

                param.SourceVersion = DataRowVersion.Original;

                adapter.UpdateCommand = command;
                //delete
                command = new SqlCommand("DELETE FROM People WHERE ID = @ID", connection);
                param = command.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");
                param.SourceVersion = DataRowVersion.Original;
                adapter.DeleteCommand = command;

                dt = new DataTable();
                adapter.Fill(dt);
                peopleDataGrid.DataContext = dt.DefaultView;
            }
        

        private void Button_Click(object sender, RoutedEventArgs e)   // кнопка добавления нового работника
        {
            TheWorker = new Employee() { Department = "Empty" };
            workers.Add(TheWorker);
            CorrectWorker(TheWorker, workers.Count - 1, "Новый работник");
        }

        public void CorrectWorker(Employee _worker, int index, string title)  // вызов редактора
        {
            ChildWindow childWindow = new ChildWindow(this.deps, this.workers, index);
            childWindow.Owner = this;
            childWindow.Title = title;
            childWindow.Show();
        }
        private void lvEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e) // обработчик касания списка
        {
            int currentindex = lvEmployee.SelectedIndex;
            if (currentindex >= 0)                                           // проверка на всякий случай, выбран ли элемент
                if ((bool)cbDel.IsChecked) Delete(currentindex);                // удалить элемент
                else CorrectWorker(TheWorker, currentindex, "Редактирование");  // редактировать элемент
        }
        public void Update(Employee w, int i)               // открытый метод при закрытом члене класса
        {
            workers[i] = w;                                 // вызывается внешним окном
        }
        public void Delete(int i)
        {
            deleted.Add(workers[i]);                       // предусмотрим возможность обратить удаления вспять
            workers.RemoveAt(i);
        }


        private void Window_Closing(object sender, CancelEventArgs e)   // запрос на восстановление удалённых записей
        {
            if (deleted.Count != 0)
            {
                MessageBoxResult result =
                  MessageBox.Show(
                    "Имеются удалённые записи. Восстановить?",
                    "Data App",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    foreach (Employee d in deleted) workers.Add(d);
                    deleted.Clear();
                    e.Cancel = true;
                }
            }

        }
    }

    public class Employee : INotifyPropertyChanged
    {
        static int count = 0;
        int id = 0;
        string name = "?";
        int age = 0;
        double salary = 0;

        public Department dep;
        public Employee()
        {
            Id = ++count;
            dep = new Department() { Name = "Empty" };
            NotifyPropertyChanged("Department");
        }
        public Employee(string _name, int _age, int _salary) : this()
        {
            Name = _name;
            Age = _age;
            Salary = _salary;
        }
        public int Id
        {
            get { return id; }
            set { if (id != value) { id = value; NotifyPropertyChanged("Id"); } }
        }
        public string Name
        {
            get { return name; }
            set { if (name != value) { name = value; NotifyPropertyChanged("Name"); } }
        }
        public int Age
        {
            get { return age; }
            set { if (age != value) { age = value; NotifyPropertyChanged("Age"); } }
        }
        public double Salary
        {
            get { return salary; }
            set { if (salary != value) { salary = value; NotifyPropertyChanged("Salary"); } }
        }
        public string Department
        {
            get { return dep.Name; }
            set { if (dep.Name != value) { dep.Name = value; NotifyPropertyChanged("Department"); } }
        }


        public override string ToString()
        {
            return $"{Id}\t{Name}\t{Age}\t{Salary}\t{Department}";
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
    public class Department : INotifyPropertyChanged
    {
        string name;
        public Department()
        {
        }
        public Department(string _name)
        {
            name = _name;
            NotifyPropertyChanged("Name");
        }
        public string Name
        {
            get { return name; }
            set { if (name != value) { name = value; NotifyPropertyChanged("Name"); } }
        }
        public override string ToString()
        {
            return $"{Name}";
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
}

