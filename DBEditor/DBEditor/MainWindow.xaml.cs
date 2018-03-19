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
                    DataSource = @"wpl26.hosting.reg.ru",
                    InitialCatalog = "u0483520_GeekBrainsHW",
                    UserID  = "u0483520_admin",
                    Password= "2rb0medi2",
                };
                connection = new SqlConnection(connectionStringBuilder.ConnectionString);
                adapter = new SqlDataAdapter();
                SqlCommand command = new SqlCommand("SELECT emp.id, emp.name, emp.age, emp.salary, dep.name as depname FROM Employee emp LEFT JOIN Department dep on emp.idDep = dep.id", connection);
                adapter.SelectCommand = command;

            //insert
            command = new SqlCommand(@"INSERT INTO Employee ( name,age,salary,idDep) 
                          VALUES ( @name,@age,@salary,@idDep); SET @ID = @@IDENTITY;",
                          connection);

            //command.Parameters.Add("@id", SqlDbType.Int, -1, "id");
            command.Parameters.Add("@name", SqlDbType.NVarChar, -1, "name");
            command.Parameters.Add("@age", SqlDbType.Int, 100, "age");
            command.Parameters.Add("@salary", SqlDbType.Int, -1, "salary");
            command.Parameters.Add("@idDep", SqlDbType.Int, -1, "idDep");

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




        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            // добавим новую строку
            DataRow newRow = dt.NewRow();
            ChildWindow childWindow = new ChildWindow(newRow);
            childWindow.ShowDialog();

            if (childWindow.DialogResult.Value)
            {
                dt.Rows.Add(childWindow.resultRow);
                adapter.Update(dt);
            }
        }
        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView newRow = (DataRowView)peopleDataGrid.SelectedItem;
            newRow.BeginEdit();
            ChildWindow childWindow = new ChildWindow(newRow.Row);
            childWindow.ShowDialog();
            if (childWindow.DialogResult.HasValue && childWindow.DialogResult.Value)
            {
                newRow.EndEdit();
                adapter.Update(dt);
            }
            else
            {
                newRow.CancelEdit();
            }
        }
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView newRow = (DataRowView)peopleDataGrid.SelectedItem;

            newRow.Row.Delete();
            adapter.Update(dt);
        }
    }

    
}

