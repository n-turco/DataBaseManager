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

            if (ConnectionString.ValidateServerName(SServerTxt.Text.Trim())) {          //validate inputs
                ErrorLbl.Content = "Invalid Server Name";
                return;
            }
            if(ConnectionString.ValidateDatabaseName(SDatabaseTxt.Text.Trim()))
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
            ConnectionString.BuildDestinationConnectionString(DServerTxt.Text.Trim(), DDatabaseTxt.Text.Trim());
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
            } else
            {
                DataBaseServices.SourceTableName = STableTxt.Text.Trim();
            }
            
            if (ConnectionString.SourceConnection != null)                //check Source Connection strings exist
                {           
                if (DataBaseServices.DataBaseExists(ConnectionString.SourceConnection))       //check Source Databases exist
                {    
                    if (DataBaseServices.DatabaseTableExists(DataBaseServices.SourceTableName, ConnectionString.SourceConnection))   //check if table exists and get schema
                    {
                       if(DataBaseServices.GetSchema(DataBaseServices.SourceTableName, ConnectionString.SourceConnection, out DataTable sTbl))
                        {
                            //compare source schema to destination schema
                            //if it is the same, copy data
                            //if it is not abort
                            //if table does not exist create one with source schema
                        }
                        else
                        {
                            MessageBox.Show("Failed to get table schema.");
                            return;
                        }
                    } 
                    else
                    {
                        MessageBox.Show("Source table does not exist.");
                        return;
                    }
                    
                    //copy table data, if data fails to copy, rollback
                } 
                else
                {
                    MessageBox.Show("Database does not exists");
                    return;
                }                 
            } 
            else
            {
                MessageBox.Show("Invalid connection string.");
                return;
            }
            if (ConnectionString.ValidateTableName(DTableTxt.Text.Trim()))      //validate destination table
            {
                MessageBox.Show("Specify destination table name");      
                return;
            }

            if (ConnectionString.DestinationConnection != null)         //check DestinationConnection string exists
            {
                if (DataBaseServices.DataBaseExists(ConnectionString.DestinationConnection))    //check Destination Database exists
                {
                    //check if table exists

                    //if table does not exist, create table in destination database
                    //if table exists check schema, if it does not match notify user 
                    //if table exists and schema matches source, copy data, if data fails to copy, rollback

                }
                else 
                {
                    MessageBox.Show("Database does not exists");
                    return;
                }
            }
        }
    }
}