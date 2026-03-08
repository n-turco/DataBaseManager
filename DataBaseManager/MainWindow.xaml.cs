/*  
  FILE          : MainWindow.xaml.cs 
  PROJECT       : PROG2126 - Assignment #2 (AsyncGuess)
  PROGRAMMER    : Nick Turco
  FIRST VERSION : 2026-03-02 
  DESCRIPTION   : Main UI for the Client. Handles interactions between User and databases
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
                if(!DataBaseServices.CopyTableData(ConnectionString.SourceConnection, 
                                                   ConnectionString.DestinationConnection, 
                                                   DataBaseServices.DestinationTableName))
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
                if (!DataBaseServices.CopyTableData(ConnectionString.SourceConnection, 
                                                    ConnectionString.DestinationConnection, 
                                                    DataBaseServices.DestinationTableName))
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