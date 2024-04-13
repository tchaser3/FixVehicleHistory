/* Title:           Main Window Fix Vehicle History
 * Date:            2-18-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is the logon for fixing vehicle history */

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
using NewEventLogDLL;
using NewEmployeeDLL;
using DataValidationDLL;

namespace FixVehicleHistory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();

        public static VerifyLogonDataSet TheVerifyLogonDataSet = new VerifyLogonDataSet();

        int gintNumberOfMisses;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //this will close the program
            TheMessagesClass.CloseTheProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            gintNumberOfMisses = 0;

            pbxPassword.Focus();
        }
        private void LogonFailed()
        {
            gintNumberOfMisses++;

            if(gintNumberOfMisses == 3)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "There Have Been Three Attemps to Log On To Fix Vehicle History");

                TheMessagesClass.ErrorMessage("There Have Been Three Attempts to Sign In\nThe Program Will Closee");

                Application.Current.Shutdown();
            }
            else
            {
                TheMessagesClass.InformationMessage("The Logon Information Is Not Correct");
            }
        }

        private void btnLogon_Click(object sender, RoutedEventArgs e)
        {
            //setting up local variables
            string strValueForValidation;
            int intEmployeeID = 0;
            string strLastName;
            bool blnFatalError = false;
            bool blnThereIsAProblem = false;
            string strErrorMessage = "";
            int intRecordsReturned = 0;

            try
            {
                //data validation
                strValueForValidation = pbxPassword.Password;
                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAProblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage = strErrorMessage + "The Employee ID is not an Integer\n";
                }
                else
                {
                    intEmployeeID = Convert.ToInt32(strValueForValidation);
                }
                strLastName = txtLastName.Text;
                if(strLastName == "")
                {
                    blnFatalError = true;
                    strErrorMessage = strErrorMessage + "The Last Name Was Not Entered";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheVerifyLogonDataSet = TheEmployeeClass.VerifyLogon(intEmployeeID, strLastName);

                intRecordsReturned = TheVerifyLogonDataSet.VerifyLogon.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    LogonFailed();
                }
                else
                {
                    if(TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup != "ADMIN")
                    {
                        LogonFailed();
                    }
                    else
                    {
                        FixVehicleHistoryTables FixVehicleHistoryTables = new FixVehicleHistoryTables();
                        FixVehicleHistoryTables.Show();
                        this.Hide();
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Fix Vehicle History Main Window Logon Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
