/*  
  FILE          : MainWindow.xaml.cs 
  PROJECT       : PROG3070 - A03 – PROGRAMMING ABSTRACTIONS
  PROGRAMMER    : Nick Turco | 9056530
  FIRST VERSION : 2026-03-03 
  DESCRIPTION   : This class handles the UI interactions from the client, it retrieves the server, database and tables from both the source
                  and destination. It notifies the user on incorrect inputs and successful actions.
*/
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataBaseManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        //Method Name: CheckSource_OnClick
        //Description: This method checks that the source server and database inputs are valid
        //             It gets the user input for the source server and db
        //             Then it creates a connection string with it
        private void CheckSource_OnClick(object sender, RoutedEventArgs e)
        {

            ErrorLbl.Content = "";                      //clear error messages

            if (ConnectionString.ValidateServerName(SServerTxt.Text.Trim()))
            {          //validate inputs
                ErrorLbl.Content = "Invalid Server Name";
                return;
            }
            if (ConnectionString.ValidateDatabaseName(SDatabaseTxt.Text.Trim()))
            {
                ErrorLbl.Content = "Invalid Database name";
                return;
            }
            ConnectionString.BuildSourceConnectionString(SServerTxt.Text.Trim(), SDatabaseTxt.Text.Trim());  //create connection string
            SServerLbl.Content = $"Source Server: {SServerTxt.Text.Trim()}";            //display server  
            SDatabaseLbl.Content = $"Source DB: {SDatabaseTxt.Text.Trim()}";            //display database

            ErrorLbl.Content = "";              //clear labels and text input
            SServerTxt.Text = "";
            SDatabaseTxt.Text = "";
        }
        //Method Name: CheckDestination_OnClick
        //Description: This method checks that the destination server and database inputs are valid
        //             It gets the user input for the destination server and db
        //             Then it creates a connection string with it
        private void CheckDestination_OnClick(object sender, RoutedEventArgs e)
        {
            ErrorLbl.Content = "";           //clear error messages

            if (ConnectionString.ValidateServerName(DServerTxt.Text.Trim()))         //validate inputs
            {
                ErrorLbl.Content = "Invalid Server Name";
                return;
            }
            if (ConnectionString.ValidateDatabaseName(DDatabaseTxt.Text.Trim()))
            {
                ErrorLbl.Content = "Invalid Database name";
                return;
            }
            ConnectionString.BuildDestinationConnectionString(DServerTxt.Text.Trim(), DDatabaseTxt.Text.Trim());  //build destination string
            DServerLbl.Content = $"Destination Server: {DServerTxt.Text.Trim()}\n";
            DDatabaseLbl.Content = $"Destination DB: {DDatabaseTxt.Text.Trim()}";

            ErrorLbl.Content = "";              //clear labels and text input
            DDatabaseTxt.Text = "";
            DServerTxt.Text = "";
        }
        //Method Name: TransferData_OnClick
        //Description: This method intiates the transfer of data between databases
        //             It checks the connection strings and tables are valid
        //             Then it checks if the databases and tables exist
        //             It then gets the schema from the source db which is used to compare to destination db
        //             It checks if the table exists at the destination, if it does it compares the schemas,
        //             if it does not it creates the table with the source table schema
        //             If the schema's don't match the transfer is cancelled and the user notified
        //             If schema's match the data is copied to the table
        private void TransferData_OnClick(object sender, RoutedEventArgs e)
        {

            if (ConnectionString.ValidateTableName(STableTxt.Text.Trim()))      //validate source table
            {
                MessageBox.Show("Specify source table name");
                return;
            }
            else
            {
                DataBaseServices.SourceTableName = STableTxt.Text.Trim();
            }

            if (ConnectionString.ValidateTableName(DTableTxt.Text.Trim()))      //validate destination table
            {
                MessageBox.Show("Specify destination table name");
                return;
            }
            else
            {
                DataBaseServices.DestinationTableName = DTableTxt.Text.Trim();
            }

            if (ConnectionString.SourceConnection == null || ConnectionString.DestinationConnection == null)            //check connection strings exist    
            {
                MessageBox.Show("Invalid connection string.");
                return;
            }

            if (!DataBaseServices.DataBaseExists(ConnectionString.SourceConnection))       //check Source Databases exist
            {
                MessageBox.Show("Source database not found.");
                return;
            }

            if (!DataBaseServices.DatabaseTableExists(DataBaseServices.SourceTableName, 
                                                      ConnectionString.SourceConnection))   //check if table exists 
            {
                MessageBox.Show("Source table not found.");
                return;
            }

            if (!DataBaseServices.GetSchema(DataBaseServices.SourceTableName, 
                                            ConnectionString.SourceConnection, 
                                            out DataTable sTbl))   //get source schema
            {
                MessageBox.Show("Failed to get table schema.");
                return;
            }

            if (!DataBaseServices.DataBaseExists(ConnectionString.DestinationConnection))       //get destination db
            {
                MessageBox.Show("Destination database not found.");
                return;
            }

            if (!DataBaseServices.DatabaseTableExists(DataBaseServices.DestinationTableName, 
                                                      ConnectionString.DestinationConnection))      //check destination table
            {
               if(!DataBaseServices.CreateTableFromSchema(DataBaseServices.DestinationTableName, 
                                                          sTbl, 
                                                          ConnectionString.DestinationConnection))
                {
                    MessageBox.Show("Failed to create table");
                    return;
                }
                if(!DataBaseServices.CopyTableData())
                {
                    MessageBox.Show("Failed to copy table data, roll back invoked.");
                    return;
                }
                MessageBox.Show($"Successfully copied table data to {DataBaseServices.DestinationTableName}");
            }

            if (!DataBaseServices.GetSchema(DataBaseServices.DestinationTableName, 
                                            ConnectionString.DestinationConnection, 
                                            out DataTable dTbl))                    //get dest schema
            {
                MessageBox.Show("Failed to get table schema.");
                return;
            }

            if (!DataBaseServices.CompareSchema(sTbl, dTbl))        //compare source and destination schemas
            {
                MessageBox.Show("Table schema does not match.");
                return;
            }
            else
            {
                if (!DataBaseServices.CopyTableData())
                {
                    MessageBox.Show("Failed to copy table data, roll back invoked.");
                    return;
                }
                MessageBox.Show($"Successfully copied table data to {DataBaseServices.DestinationTableName}");
                return;
            }
        }
    }
}