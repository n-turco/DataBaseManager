/*  
  FILE          : MainWindow.xaml.cs 
  PROJECT       : PROG2126 - Assignment #2 (AsyncGuess)
  PROGRAMMER    : Nick Turco
  FIRST VERSION : 2026-03-02 
  DESCRIPTION   : Main UI for the Client. Handles interactions between User and databases
*/
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CheckSource_OnClick(object sender, RoutedEventArgs e)
        {
            //clear error messages
            SServerErrLbl.Content = "";
            SDatabaseErrLbl.Content = "";
            SDataTableErrLbl.Content = "";

            //validate inputs
            if (ConnectionString.ValidateServerName(SServerTxt.Text.Trim())) {
                SServerErrLbl.Content = "Invalid Server Name";
                return;
            }
            if(ConnectionString.ValidateDatabaseName(SDatabaseTxt.Text.Trim()))
            {
                SDatabaseErrLbl.Content = "Invalid Database name";
                return;
            }
            //if(ConnectionString.ValidateTableName(SDataTableTxt.Text))
            //{
            //    SDataTableErrLbl.Content = "Invalid table name";
            //    return;
            //}
            //build the connection string
            ConnectionString.BuildSourceConnectionString(SServerTxt.Text.Trim(), SDatabaseTxt.Text.Trim());

        }

        private void CheckDestination_OnClick(object sender, RoutedEventArgs e)
        {
            //clear error messages
            DServerErrLbl.Content = "";
            DDatabaseErrLbl.Content = "";
            DDataTableErrLbl.Content = "";

            //validate inputs
            if (ConnectionString.ValidateServerName(DServerTxt.Text.Trim()))
            {
                DServerErrLbl.Content = "Invalid Server Name";
                return;
            }
            if (ConnectionString.ValidateDatabaseName(DDatabaseTxt.Text.Trim()))
            {
                DDatabaseErrLbl.Content = "Invalid Database name";
                return;
            }
            //if (ConnectionString.ValidateTableName(DDataTableTxt.Text))
            //{
            //    DDataTableErrLbl.Content = "Invalid table name";
            //    return;
            //}
            //build the connection string
            ConnectionString.BuildDestinationConnectionString(DServerTxt.Text.Trim(), DDatabaseTxt.Text.Trim());
        }

        private void TransferData_OnClick(object sender, RoutedEventArgs e)
        {
            if (ConnectionString.SourceConnection != null) {
                if (DataBaseServices.DataBaseExists(ConnectionString.SourceConnection))
                {
                    GeneralLbl.Content = "DataBase Exists";
                } else
                {
                    MessageBox.Show("Database does not exists");
                }                 
            } else
            {
                MessageBox.Show("Invalid connection string.");
            }
        }
    }
}