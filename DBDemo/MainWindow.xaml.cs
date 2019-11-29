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
        private int conferenceID;
        private DataSet ds = new DataSet();
        
        public MainWindow(int _conferenceID,string conf)
        {
            InitializeComponent();
            this.SizeToContent = SizeToContent.Height;
            conferenceID = _conferenceID;
            PopulateCountries();
            LoadAllData();
            this.Title = conf;
        }

        private void BtnLoadAllData_Click(object sender, RoutedEventArgs e)
        {
            QueryVisitorTable(@"SELECT * from Visitors");
        }

        private void LoadAllData()
        {
            // Command : SQL
          //  FilterVisitorTable("");
            // DataSet : class allows me to save data extracted from DB
         //   DataSet ds = new DataSet(); this will be moved to the class level
            // DataTable to save the dataset
           
            string command = @"SELECT * FROM Visitors WHERE ConferenceID ="+ conferenceID;
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
                    if (ds.Tables["Visitors"].Rows.Count == 0)
                        MessageBox.Show("No Data Available.,","Alert",MessageBoxButton.OK, MessageBoxImage.Exclamation);

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Error Accessing the Database " + ex.Message, "DB ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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
            // QueryVisitorTable(@"SELECT * from Visitors WHERE FullName LIKE '%"+txtName.Text+@"%'");
            FilterVisitorTable(@"FullName LIKE '%" + txtName.Text + @"%'");

        }







        private void FilterVisitorTable(string filter)
        {
            try { 
           ds.Tables["Visitors"].DefaultView.RowFilter = "";
            ds.Tables["Visitors"].DefaultView.RowFilter = filter ;
            gridDbData.Items.Refresh();
            }catch(Exception ex)
            {
                MessageBox.Show("ERROR " + ex.Message);
            }
        }



        // populating the countries combo box

        private void PopulateCountries()
        {
            try
            {
                string command = @"SELECT Name from Countries";
              //  DataSet ds=new DataSet();
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
            //  QueryVisitorTable(@"SELECT * from Visitors WHERE  lower(Country) = '" + cmbCountries.SelectedValue.ToString().ToLower() + @"'");

            FilterVisitorTable(@"Country LIKE '" + cmbCountries.SelectedValue.ToString().ToLower() + @"'");
        }

        private void ChbxSpeaker_Click(object sender, RoutedEventArgs e)
        {
            //if(chbxSpeaker.IsChecked.Value == true) { 
            //QueryVisitorTable(@"SELECT * from Visitors WHERE  Speaker =1");
            //}
            //else
            //{
            //    QueryVisitorTable(@"SELECT * from Visitors WHERE  Speaker =0");
            //}
            FilterVisitorTable(@"Speaker ='" + chbxSpeaker.IsChecked.Value + @"'");  
}

        private void BtnSearchDate_Click(object sender, RoutedEventArgs e)
        {
            if(txtDate.SelectedDate.HasValue && txtEndDate.SelectedDate.HasValue)
           // QueryVisitorTable(@"SELECT * from Visitors WHERE  CheckInDate between '" +txtDate.SelectedDate.Value +@"' AND '"+txtEndDate.SelectedDate.Value + @"'");
            FilterVisitorTable(@"CheckInDate >= #" + txtDate.SelectedDate.Value + "# AND CheckInDate <= #" + txtEndDate.SelectedDate.Value + "#");
          else
            MessageBox.Show("Error Please enter the date first ", "DATE ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void BtnSearchMajor_Click(object sender, RoutedEventArgs e)
        {
            QueryVisitorTable(@"SELECT * from Visitors WHERE Major LIKE  '%" + txtMajor.Text + @"%'");
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            FilterVisitorTable("");
        }

        private void BtnAddNewVisitor_Click(object sender, RoutedEventArgs e)
        {
            VisitorWindow newVW = new VisitorWindow(ds.Tables["Countries"].Rows);
            if (newVW.ShowDialog().Value)
            {

                // add the database
                Visitor v = newVW.visitoInfo;
                try
                {
                    using (SqlConnection con = new SqlConnection(dbConnection))
                    {
                        string command = "INSERT INTO Visitors" +
                            "(FullName,Major,Country,Status,Speaker,CheckInDate,ConferenceID) VALUES(" +
                            @"'" + v.FullName + @"'," +
                            @"'" + v.Major + @"'," +
                            @"'" + v.Country + @"'," +
                            @"'" + v.VisitorStatus.ToString() + @"'," +
                            @"'" + v.IsSpeaker + @"'," +
                            @"'" + v.CheckInDate + @"'," +
                            conferenceID + ")";
                        SqlCommand cmd = new SqlCommand(command, con);
                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        // update my grid
                        ds.Tables["Visitors"].Clear();
                        LoadAllData();
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private void BtnEditVisitor_Click(object sender, RoutedEventArgs e)
        {
            try { 
            if((gridDbData.SelectedIndex > 0) && (gridDbData.SelectedIndex != gridDbData.Items.Count))
            {
                DataRowView r = gridDbData.SelectedItem as DataRowView;
                Visitor existing = new Visitor()
                {
                    FullName = r["FullName"].ToString(),
                    Major = r["Major"].ToString(),
                    Country = r["Country"].ToString(),
                    VisitorStatus = r["Status"].ToString() == Status.Teacher.ToString() ? Status.Teacher :
                                      r["Status"].ToString() == Status.Student.ToString() ? Status.Student : Status.Proffessional,
                    IsSpeaker = bool.Parse(r["Speaker"].ToString()),
                    CheckInDate = DateTime.Parse(r["CheckIndate"].ToString())

                };

                VisitorWindow modifyVisitor = new VisitorWindow(ds.Tables["Countries"].Rows, existing);
                if (modifyVisitor.ShowDialog().Value)
                {
                        try
                        {
                            Visitor v = modifyVisitor.visitoInfo;
                            using(SqlConnection con=new SqlConnection(dbConnection))
                            {
                                string command =
                                    @"UPDATE Visitors set " +
                                    @"FullName ='" + v.FullName + @"'," +
                                    @"Major ='" + v.Major + @"'," +
                                    @"Country ='" + v.Country + @"'," +
                                    @"Status ='" + v.VisitorStatus.ToString() + @"'," +
                                    @"Speaker ='" + v.IsSpeaker + @"'," +
                                    @"CheckIndate ='" + v.CheckInDate + @"' WHERE Id=" + r["Id"];
                                      SqlCommand cmd = new SqlCommand(command, con);
                                       con.Open();
                                      cmd.ExecuteNonQuery();
                                      con.Close();
                                         ds.Tables["Visitors"].Clear();
                                         LoadAllData();

                            }
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show("ERROR " + ex.Message);
                        }
                    // update the database
                    MessageBox.Show("Update the database");

                }
            }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
