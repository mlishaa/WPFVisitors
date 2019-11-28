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

    }
}
