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
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DBDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // string to tell us where is the database
       private string dbConnection = ConfigurationManager.ConnectionStrings["DBDemo.Properties.Settings.ConferenceDBConnectionString"].ConnectionString;
        public MainWindow()
        {
            InitializeComponent();
            this.SizeToContent = SizeToContent.Height;
            PopulateCountries();
        }

        private void BtnLoadAllData_Click(object sender, RoutedEventArgs e)
        {
            QueryVisitorTable(@"SELECT * from Visitors");
        }




        private void QueryVisitorTable(string command)
        {
            // Command : SQL
            
            // DataSet : class allows me to save data extracted from DB
            DataSet ds = new DataSet();
            // DataTable to save the dataset
            DataTable dt;
            try
            {
                using (SqlConnection con = new SqlConnection(dbConnection))
                {                   
                    SqlCommand cmd = new SqlCommand(command, con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    sda.Fill(ds, "Visitors");
                    //sda.Fill(ds, "Mike");
                    // dt = ds.Tables["Visitors"];
                    gridDbData.ItemsSource = ds.Tables["Visitors"].DefaultView;
                    gridDbData.Items.Refresh();

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error Accessing the Database "+ex.Message,"DB ERROR",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void BtnSearchByName_Click(object sender, RoutedEventArgs e)
        {
            QueryVisitorTable(@"SELECT * from Visitors WHERE FullName LIKE '%"+txtName.Text+@"%'");
        }


        // populating the countries combo box

        private void PopulateCountries()
        {
            try
            {
                string command = @"SELECT Name from Countries";
                DataSet ds=new DataSet();
                using(SqlConnection con =new SqlConnection(dbConnection))
                {
                    SqlCommand cmd = new SqlCommand(command, con);
                    SqlDataAdapter sad = new SqlDataAdapter(cmd);
                   sad.Fill(ds, "Countries");
                    cmbCountries.ItemsSource = ds.Tables["Countries"].Rows;

                    cmbCountries.SelectedValuePath = ".[Name]";
                    cmbCountries.DisplayMemberPath = ".[Name]";
                    cmbCountries.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void CmbCountries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QueryVisitorTable(@"SELECT * from Visitors WHERE  lower(Country) = '" + cmbCountries.SelectedValue.ToString().ToLower() + @"'");
        }

        private void ChbxSpeaker_Click(object sender, RoutedEventArgs e)
        {
            if(chbxSpeaker.IsChecked.Value == true) { 
            QueryVisitorTable(@"SELECT * from Visitors WHERE  Speaker =1");
            }
            else
            {
                QueryVisitorTable(@"SELECT * from Visitors WHERE  Speaker =0");
            }
        }

        private void BtnSearchDate_Click(object sender, RoutedEventArgs e)
        {
            if(txtDate.SelectedDate.HasValue && txtEndDate.SelectedDate.HasValue)
            QueryVisitorTable(@"SELECT * from Visitors WHERE  CheckInDate between '" +txtDate.SelectedDate.Value +@"' AND '"+txtEndDate.SelectedDate.Value + @"'");
          else
            MessageBox.Show("Error Please enter the date first ", "DATE ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnSearchMajor_Click(object sender, RoutedEventArgs e)
        {
            QueryVisitorTable(@"SELECT * from Visitors WHERE Major LIKE  '%" + txtMajor.Text + @"%'");
        }
    }
}
