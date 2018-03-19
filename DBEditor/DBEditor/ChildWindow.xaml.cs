using System;
using System.Collections.ObjectModel;
using System.Windows;


namespace DBEditor
{
    /// <summary>
    /// Логика взаимодействия для ChildWindow.xaml
    /// </summary>
    public partial class ChildWindow : Window
    {

        Employee TheWorker;
        int index;

        public ChildWindow(ObservableCollection<Department> deps, ObservableCollection<Employee> emps, int _index)
        {
            InitializeComponent();
            index = _index;
            TheWorker = emps[index];                   // узнаём, с каким элементом списка работаем

            comboBoxDep.ItemsSource = deps;            // заполняем окошко данными элемента списка
            textBoxid.Text = TheWorker.Id.ToString();
            textBoxAge.Text = TheWorker.Age.ToString();
            textBoxName.Text = TheWorker.Name;
            textBoxSalary.Text = TheWorker.Salary.ToString();
            comboBoxDep.Text = TheWorker.Department == "Empty" ? deps[0].Name : TheWorker.Department;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            TheWorker.Name = textBoxName.Text;
            TheWorker.Department = comboBoxDep.Text;
            try
            {
                TheWorker.Id = Int32.Parse(textBoxid.Text);
                TheWorker.Age = Int32.Parse(textBoxAge.Text);
                TheWorker.Salary = Double.Parse(textBoxSalary.Text);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            finally
            {
                MainWindow main = this.Owner as MainWindow;
                main.Update(TheWorker, index);
                this.Close();
            }

        }
    }
}

