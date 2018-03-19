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
        SqlDataAdapter adapter, adapter2;
        DataTable dt;
        DataSet dc;
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
                SqlCommand command = new SqlCommand("SELECT emp.id, emp.name, emp.age, emp.salary, dep.name as depname, emp.idDep FROM Employee emp LEFT JOIN Department dep on emp.idDep = dep.id", connection);
                adapter.SelectCommand = command;

           
            //insert
            command = new SqlCommand(@"INSERT INTO Employee ( id,name,age,salary,idDep) 
                          VALUES (@id, @name,@age,@salary,@idDep);",
                          connection);
            command.Parameters.Add("@ID", SqlDbType.NVarChar, -1, "id");
            command.Parameters.Add("@name", SqlDbType.NVarChar, -1, "name");
            command.Parameters.Add("@age", SqlDbType.Int, 100, "age");
            command.Parameters.Add("@salary", SqlDbType.Int, -1, "salary");
            command.Parameters.Add("@idDep", SqlDbType.Int, -1, "idDep");

            SqlParameter param; //= command.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");

           // param.Direction = ParameterDirection.Output;
            adapter.InsertCommand = command;


            // update
            command = new SqlCommand(@"UPDATE Employee SET name = @name,
age = @age, salary = @salary, idDep = @idDep WHERE ID = @ID", connection);

            command.Parameters.Add("@name", SqlDbType.NVarChar, -1, "name");
            command.Parameters.Add("@age", SqlDbType.Int, 100, "age");
            command.Parameters.Add("@salary", SqlDbType.Int, -1, "salary");
            command.Parameters.Add("@idDep", SqlDbType.Int, -1, "idDep");
            param = command.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");

            param.SourceVersion = DataRowVersion.Original;

            adapter.UpdateCommand = command;
            //delete
            command = new SqlCommand("DELETE FROM Employee WHERE ID = @ID", connection);
            param = command.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");
            param.SourceVersion = DataRowVersion.Original;
            adapter.DeleteCommand = command;

            dt = new DataTable();
                adapter.Fill(dt);
                peopleDataGrid.DataContext = dt.DefaultView;

            adapter2 = new SqlDataAdapter();
            SqlCommand command2 = new SqlCommand("SELECT id, name FROM Department", connection);
            adapter2.SelectCommand = command2;
            dc = new DataSet();
            adapter2.Fill(dc,"Department");
            }




        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            DataSet _dc = dc.Copy();
            DataRow newRow = dt.NewRow();
            ChildWindow childWindow = new ChildWindow(newRow,_dc);
            childWindow.ShowDialog();

            if (childWindow.DialogResult.Value)
            {
                dt.Rows.Add(childWindow.resultRow);
                adapter.Update(dt);
            }
        }
        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            DataSet _dc = dc.Copy();
            if (peopleDataGrid.SelectedItem != null)
            {


                DataRowView newRow = (DataRowView)peopleDataGrid.SelectedItem;

                newRow.BeginEdit();
                ChildWindow childWindow = new ChildWindow(newRow.Row, _dc);
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
            else MessageBox.Show("Выберите строку");
        }
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView newRow = (DataRowView)peopleDataGrid.SelectedItem;

            newRow.Row.Delete();
            adapter.Update(dt);
        }
    }

    
}

