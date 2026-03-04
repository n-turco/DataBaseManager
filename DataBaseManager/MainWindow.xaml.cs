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
            if (ConnectionString.ValidateServerName(SServerTxt.Text)) {
                SServerErrLbl.Content = "Invalid Server Name";
                return;
            }
            if(ConnectionString.ValidateDatabaseName(SDatabaseTxt.Text))
            {
                SDatabaseErrLbl.Content = "Invalid Database name";
                return;
            }
            if(ConnectionString.ValidateTableName(SDataTableTxt.Text))
            {
                SDataTableErrLbl.Content = "Invalid table name";
                return;
            }
            ConnectionString.BuildConnectionString(SServerTxt.Text, SDatabaseTxt.Text);

        }

        private void CheckDestination_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void TransferData_OnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}