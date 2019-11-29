using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for VisitorWindow.xaml
    /// </summary>
    public partial class VisitorWindow : Window
    {
        private bool isModification;
        public Visitor visitoInfo { get; }
        // new visitor scenario
        public VisitorWindow(DataRowCollection countries)
        {
            InitializeComponent();
           
            SetUpWindow(countries);
            visitoInfo = new Visitor();
            
        }

        // modified visitor scenario
        public VisitorWindow(DataRowCollection countries,Visitor existingVisitor)
        {
            InitializeComponent();

            SetUpWindow(countries);
            visitoInfo =existingVisitor;
            // change the title ,change the button content
            this.Title = "Modifiy Visitor Info";
            btnSave.Content = "Modify";
            btnClear.IsEnabled = false;
            LoadVisitorInfo();

        }

        private void LoadVisitorInfo()
        {
            txtName.Text = visitoInfo.FullName;
           txtMajor.Text= visitoInfo.Major;
            cmbCountries.SelectedValue = visitoInfo.Country;
            dpCheckIn.SelectedDate = visitoInfo.CheckInDate;
            chkbSpeaker.IsChecked = visitoInfo.IsSpeaker;
            if (visitoInfo.VisitorStatus == Status.Teacher)
                rbtnTeacher.IsChecked = true;
            if (visitoInfo.VisitorStatus == Status.Student)
                rbtnStudent.IsChecked = true;
            if (visitoInfo.VisitorStatus == Status.Proffessional)
                rbtnProf.IsChecked = true;
        }

        private void SetUpWindow(DataRowCollection countries)
        {
            this.SizeToContent = SizeToContent.Height;
            this.SizeToContent = SizeToContent.Width;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            cmbCountries.ItemsSource = countries;
            cmbCountries.SelectedValuePath = ".[Name]";
            cmbCountries.DisplayMemberPath = ".[Name]";
            cmbCountries.Items.Refresh();
        }


        private bool CheckAllInput()
        {
            StringBuilder msg = new StringBuilder();

            //Name
            if (string.IsNullOrEmpty(txtName.Text))
                msg.AppendLine("Name is a required field");

            //Major
            if (string.IsNullOrEmpty(txtMajor.Text))
                msg.AppendLine("Major is a required field");

            //Country
            if (cmbCountries.SelectedValue == null)
                msg.AppendLine("No Country Selected");

            //Status
            if (!(rbtnTeacher.IsChecked.Value || rbtnStudent.IsChecked.Value || rbtnProf.IsChecked.Value))
                msg.AppendLine("Status is not chosen");

            //Check in date
            if (!dpCheckIn.SelectedDate.HasValue)
                msg.AppendLine("Date is not selected");

            if (!string.IsNullOrEmpty(msg.ToString()))
            {
                MessageBox.Show(msg.ToString(), "Form Missing Data", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }


       

        private void ClearForm()
        {
            txtName.Clear();
            txtMajor.Clear();
            cmbCountries.SelectedIndex = -1;
            rbtnProf.IsChecked = false;
            rbtnTeacher.IsChecked = false;
            rbtnStudent.IsChecked = false;
            chkbSpeaker.IsChecked = false;
            dpCheckIn.SelectedDate = null;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            bool update = true;
            if (isModification)
               update= compareInfo();

            if (CheckAllInput())
            {
                visitoInfo.FullName = txtName.Text;
                visitoInfo.Major = txtMajor.Text;
                visitoInfo.Country = cmbCountries.SelectedValue.ToString();
                visitoInfo.IsSpeaker = chkbSpeaker.IsChecked.Value;
                visitoInfo.CheckInDate = dpCheckIn.SelectedDate.Value;
                visitoInfo.VisitorStatus = rbtnTeacher.IsChecked.Value ? Status.Teacher :
                                rbtnStudent.IsChecked.Value ? Status.Student : Status.Proffessional;
                ClearForm();
                this.DialogResult = true;
                this.Close();
            }
        }

        // compare info
        private bool compareInfo()
        {
            if (visitoInfo.FullName != txtName.Text)
                return true;
            if (visitoInfo.Major != txtMajor.Text)
                return true;
            if (visitoInfo.Country != cmbCountries.SelectedValue.ToString())
                return true;
            if (visitoInfo.IsSpeaker != chkbSpeaker.IsChecked.Value)
                return true;
            if (visitoInfo.CheckInDate != dpCheckIn.SelectedDate.Value)
                return true;

            string currentStatus = rbtnTeacher.IsChecked.Value ? Status.Teacher.ToString() :
                                     rbtnTeacher.IsChecked.Value ? Status.Teacher.ToString() : Status.Proffessional.ToString();
            if (currentStatus != visitoInfo.VisitorStatus.ToString())
                return true;

            return false;
        }


        private void BtnCanel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

     

    }
}
