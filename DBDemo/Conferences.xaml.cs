using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Shapes;

namespace DBDemo
{
    /// <summary>
    /// Interaction logic for Conferences.xaml
    /// </summary>
    public partial class Conferences : Window
    {
        private string dbConnection = ConfigurationManager.ConnectionStrings["DBDemo.Properties.Settings.ConferenceDBConnectionString"].ConnectionString;

        public Conferences()
        {
            InitializeComponent();
            PopulateConferences();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
           // this.SizeToContent = SizeToContent.Height;
        }

        private void PopulateConferences()
        {
            try
            {
                string command = @"SELECT * from Conferences";
                DataSet ds = new DataSet();
                using (SqlConnection con = new SqlConnection(dbConnection))
                {
                    SqlCommand cmd = new SqlCommand(command, con);
                    SqlDataAdapter sad = new SqlDataAdapter(cmd);
                    sad.Fill(ds, "Conferences");
                    cmbLoadVisitorForm.ItemsSource = ds.Tables["Conferences"].Rows;
                    // the value here has to be and id to pass to aother form
                    cmbLoadVisitorForm.SelectedValuePath = ".[Id]";
                    cmbLoadVisitorForm.DisplayMemberPath = ".[Name]";
                    cmbLoadVisitorForm.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void BtnAddConference_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtConferenceName.Text))
            {
                MessageBox.Show("ERROR" + "please enter the conference name");
            }
            else { 
            try
            {
                using (SqlConnection con = new SqlConnection(dbConnection))
                {
                    string command = @"INSERT INTO Conferences (Name) VALUES ("+@"'"+txtConferenceName.Text+@"')";
                    SqlCommand cmd = new SqlCommand(command, con);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :" + ex.Message);

            }
            // repopulate my conference list
            PopulateConferences();
            txtConferenceName.Text = "";
            }
        }

        private void CmbLoadVisitorForm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // load all visitors form(Main Window)
            // the argument has to be h=changed later
            string conferenceName = (cmbLoadVisitorForm.SelectedItem as DataRow)["Name"].ToString();
            MessageBox.Show(conferenceName);
            MainWindow allVisitorsForm = new MainWindow(int.Parse(cmbLoadVisitorForm.SelectedValue.ToString()),conferenceName);
            allVisitorsForm.Show();
        }
    }
}
