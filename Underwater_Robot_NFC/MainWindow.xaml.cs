using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
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
using Excel = Microsoft.Office.Interop.Excel;

namespace Underwater_Robot_NFC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort comport;
        System.Windows.Forms.Timer timer;
        public static bool card_tapped = false;
        Byte[] send_buffer = new Byte[] { 0xAA, 0x02, 0x09, 0x04 };
        bool data_coming = false;
        Byte[] recieve_buffer = new Byte[5];
        UInt16 buffer_recieved = 0;
        UInt16 team_id = 0;
        Excel.Application xlApp;
        Excel.Workbook xlWorkbook;
        Excel._Worksheet xlWorksheet;
        Excel.Range xlRange;

        public MainWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                this.comport_list.Items.Add(port);
            }
            xlApp = new Excel.Application();
            xlWorkbook = xlApp.Workbooks.Open(@"C:\Users\mcreng\git\Underwater_Robot_NFC\Underwater_Robot_NFC\bin\Debug\underwater_robot_balance.xlsx");
            xlWorksheet = xlWorkbook.Sheets[1];
            xlRange = xlWorksheet.UsedRange;
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background


            //close and release
            try
            {
                xlWorkbook.Save();
            } catch (InvalidComObjectException exception)
            {
                Console.WriteLine(exception);
            }

            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
            Marshal.FinalReleaseComObject(xlApp);
        }

        private void connect_btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                comport = new SerialPort(this.comport_list.SelectedItem.ToString(), 9600, Parity.None, 8, StopBits.One);
                comport.ReceivedBytesThreshold = 1;
                comport.DataReceived += new SerialDataReceivedEventHandler(comport_data_received);
                if (!comport.IsOpen)
                {
                    comport.Open();
                    System.Windows.MessageBox.Show("Port Opened","Success");
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("No com port selected","Error");
                return;
            }
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 10;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void comport_data_received(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            while(sp.BytesToRead!=0)
            {
                Byte data = (Byte)sp.ReadByte();
                if (data == 0x09 && !data_coming)
                {
                    //timer.Stop();
                    data_coming = true;
                }else if (data_coming)
                {
                    recieve_buffer[buffer_recieved] = data;
                    ++buffer_recieved;
                }else if (data == 0x01)
                {
                    card_tapped = false;
                }
                if (buffer_recieved == 5)
                {
                    buffer_recieved = 0;
                    data_coming = false;
                    decode();
                    card_tapped = true;
                    timer.Start();
                    return;
                }
            }
        }

        private void decode()
        {
            team_id = (UInt16)recieve_buffer[1];
            team_id = 1;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!comport.IsOpen)
                {
                    comport.Open();
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("No com port selected","Error");
                return;
            }
            comport.Write(send_buffer, 0, send_buffer.Length);
            if (team_id > 0&&card_tapped)
            {
                team_name.Text = "Team: " + xlRange.Cells[team_id, 1].Text;
                team_balance.Text = xlRange.Cells[team_id, 2].Text;
            }
            else{
                team_name.Text = "Team: ";
                team_balance.Text = "0";
            }
        }

        private void balance_changed(object sender, KeyEventArgs e)
        {
            /*
            if (!card_tapped)
            {
                System.Windows.MessageBox.Show("Please tap the card!", "Error");
                return;
            }
            */
            if (e.Key == Key.Enter)
            {
                
                Int32 value_reduced = 0;
                if(!Int32.TryParse(((TextBox)sender).Text, out value_reduced))
                {
                    System.Windows.MessageBox.Show("Please type integer!","Error");
                    ((TextBox)sender).Text = "";
                    return;
                }
                if (value_reduced < 0)
                {
                    System.Windows.MessageBox.Show("Please type positive integer!", "Error");
                    ((TextBox)sender).Text = "";
                    return;
                }

                bool flag = false;
                Window1 w = new Window1(ref card_tapped, ref flag);
                w.ShowDialog();
                
                //while (!card_tapped) ;
                //w.Close();
                
                double value = xlRange.Cells[team_id, 2].Value2;
                if (value_reduced > value)
                {
                    System.Windows.MessageBox.Show("Not enough credit!", "Error");
                    ((TextBox)sender).Text = "";
                    return;
                }
                value -= value_reduced;
                xlRange.Cells[team_id, 2].Value2 = value;
                System.Windows.MessageBox.Show("Balance Updated\n" + "New balance: " + value.ToString(), "Success");
                ((TextBox)sender).Text = "";
            }
        }
    }
}
