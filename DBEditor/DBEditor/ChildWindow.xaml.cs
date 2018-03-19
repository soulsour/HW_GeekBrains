using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;


namespace DBEditor
{
    /// <summary>
    /// Логика взаимодействия для ChildWindow.xaml
    /// </summary>
    public partial class ChildWindow : Window
    {

     
        public DataRow resultRow { get; set; }
        public ChildWindow(DataRow dataRow)
        {
            InitializeComponent();
            resultRow = dataRow;
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            resultRow["ID"] = textBoxid.Text;
            resultRow["Name"] = textBoxName.Text;
            resultRow["Age"] = textBoxAge.Text;
            resultRow["Salary"] = textBoxSalary.Text;
            this.DialogResult = true;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
            textBoxName.Text = resultRow["Name"].ToString();
            textBoxAge.Text = resultRow["Age"].ToString();
            textBoxSalary.Text = resultRow["Salary"].ToString();

        }

       
    }
}

