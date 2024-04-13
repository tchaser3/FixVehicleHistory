/* Title:           Fix Vehicle History Tables
 * Date:            2-21-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to fix the vehicle history tables */

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
using System.Windows.Shapes;
using NewEventLogDLL;
using VehicleHistoryDLL;

namespace FixVehicleHistory
{
    /// <summary>
    /// Interaction logic for FixVehicleHistoryTables.xaml
    /// </summary>
    public partial class FixVehicleHistoryTables : Window
    {
        //setting up the class
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        VehicleHistoryClass TheVehicleHistoryClass = new VehicleHistoryClass();

        VehicleHistoryDataSet TheVehicleHistoryDataSet = new VehicleHistoryDataSet();

        int gintNumberOfRecords;

        public FixVehicleHistoryTables()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TheVehicleHistoryDataSet = TheVehicleHistoryClass.GetVehicleHistoryInfo();

                gintNumberOfRecords = TheVehicleHistoryDataSet.vehiclehistory.Rows.Count - 1;

                dgrResults.ItemsSource = TheVehicleHistoryDataSet.vehiclehistory;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Fix Vehicle History Fix Vehicle History Tables Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //this will close the program
            TheMessagesClass.CloseTheProgram();
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //this will process the transactions
            //setting local variables
            int intCounter;
            PleaseWait PleaseWait = new PleaseWait();
            bool blnUpdateItem;

            PleaseWait.Show();

            try
            {
                //running loop
                for(intCounter = 0; intCounter <= gintNumberOfRecords; intCounter++)
                {
                    blnUpdateItem = false;

                    if(TheVehicleHistoryDataSet.vehiclehistory[intCounter].IsDateNull() == true)
                    {
                        TheVehicleHistoryDataSet.vehiclehistory[intCounter].Date = TheVehicleHistoryDataSet.vehiclehistory[intCounter - 1].Date;
                        blnUpdateItem = true;
                    }
                    if(TheVehicleHistoryDataSet.vehiclehistory[intCounter].IsRemoteVehicleNull() == true)
                    {
                        TheVehicleHistoryDataSet.vehiclehistory[intCounter].RemoteVehicle = "NO";
                        blnUpdateItem = true;
                    }
                    if(blnUpdateItem == true)
                    {
                        TheVehicleHistoryClass.UpdateVehicleHistoryDB(TheVehicleHistoryDataSet);
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Fix Vehicle History Fix Vehicle History Tables Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }
    }
}
