using System;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
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
        public static bool processed_flag = false; 
        Byte[] send_buffer = new Byte[] { 0xAA, 0x02, 0x09, 0x04 };
        bool data_coming = false;
        Byte[] recieve_buffer = new Byte[5];
        UInt16 buffer_recieved = 0;
        UInt16 team_id = 0;
        Excel.Application xlApp;
        Excel.Workbook xlWorkbook;
        Excel._Worksheet xlWorksheet, xlLog;
        Excel.Range xlRange;
        Excel.PivotTables pivotTables;

        /// <summary>
        /// product cost
        /// </summary>
        private Int32 costTotal = 0;

        /// <summary>
        /// weight
        /// </summary>
        private Int32 weight = 0;

        /// <summary>
        /// cost per gram
        /// </summary>
        private Int32 unitPrice = 0;

        public MainWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                comport_list.Items.Add(port);
            }

            try
            {
                xlApp = (Excel.Application)Marshal.GetActiveObject("Excel.Application");
            }
            catch (Exception)
            {
                xlApp = new Excel.Application();
            }

            try
            {
                xlWorkbook = xlApp.Workbooks.Open(System.Windows.Forms.Application.StartupPath + "\\NFC.xlsx");
                xlWorksheet = xlWorkbook.Sheets[1];
                xlLog = xlWorkbook.Sheets[2];
                xlRange = xlWorksheet.UsedRange;
                pivotTables = (Excel.PivotTables)xlLog.PivotTables(Type.Missing);
            }
            catch (COMException e)
            {
                MessageBox.Show("Please close and start \"NFC.xlsx\" and try again!");
                Log(e.ToString());
            }

        }

        private void Log(string log_message)
        {
            using(StreamWriter log_text = File.AppendText("log.txt")){
                log_text.Write("\r\nLog Entry : ");
                log_text.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
                log_text.WriteLine("  {0}", log_message);
                log_text.WriteLine("-------------------------------");
            }
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  e: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background


            //close and release
            xlWorkbook.Close();

            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);
            Marshal.FinalReleaseComObject(xlApp);
        }

        private void ConnectBtnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (comport == null)
                {
                    comport = new SerialPort(this.comport_list.SelectedItem.ToString(), 9600, Parity.None, 8, StopBits.One)
                    {
                        ReceivedBytesThreshold = 1
                    };
                    comport.DataReceived += new SerialDataReceivedEventHandler(ComportDataReceived);
                    if (!comport.IsOpen)
                    {
                        comport.Open();
                        connect_btn.Content = "Disconnect";
                    }
                    timer = new System.Windows.Forms.Timer
                    {
                        Interval = 10
                    };
                    timer.Tick += new EventHandler(TimerTick);
                    timer.Start();
                    Log("Success: port opened");
                }
                else if (!comport.IsOpen)
                {
                    comport.Open();
                    connect_btn.Content = "Disconnect";
                    timer.Start();
                    Log("Success: port opened");
                }
                else
                {
                    comport.Close();
                    timer.Stop();
                    connect_btn.Content = "Connect";
                    Log("Success: port closed");
                }
            }
            catch
            {
                MessageBox.Show("No com port selected","Error");
                Log("Error: cannot open port");
                return;
            }
        }

        private void ComportDataReceived(object sender, SerialDataReceivedEventArgs e)
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
                    Decode();
                    card_tapped = true;
                    timer.Start();
                    return;
                }
            }
        }

        private void Decode()
        {
            team_id = (UInt16)recieve_buffer[1];
        }

        private void TimerTick(object sender, EventArgs e)
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
                MessageBox.Show("No com port selected","Error");
                return;
            }
            comport.Write(send_buffer, 0, send_buffer.Length);
            if (team_id > 0 && card_tapped)
            {
                TextBlockTeamID.Text = team_id.ToString();
                TextBlockTeamName.Text = xlRange.Cells[team_id+1, 2].Text;
                TextBlockBalance.Text = xlRange.Cells[team_id+1, 3].Text;
            }
            else{
                TextBlockTeamID.Text = "";
                TextBlockTeamName.Text = "";
                TextBlockBalance.Text = "";
            }
        }

        private void CheckOut()
        {
            try
            {
                if (!comport.IsOpen)
                {
                    MessageBox.Show("No com port selected", "Error");
                    Log("Error: port not opened");
                    return;
                }
                if (!Int32.TryParse(TextBoxWeight.Text, out weight))
                {
                    MessageBox.Show("Please input integer!", "Error");
                    TextBoxWeight.Text = "";
                    return;
                }
                if (weight < 0)
                {
                    MessageBox.Show("Please input positive integer!", "Error");
                    TextBoxWeight.Text = "";
                    return;
                }
                if (!Int32.TryParse(TextBoxUnitPrice.Text, out unitPrice))
                {
                    MessageBox.Show("Please input integer in unit price!", "Error");
                    TextBoxUnitPrice.Text = "";
                    return;
                }
                if (unitPrice < 0)
                {
                    MessageBox.Show("Please input positive integer in unit price!", "Error");
                    TextBoxWeight.Text = "";
                    return;
                }
                UpdateSum();
                bool flag = false;
                Window1 w = new Window1(ref card_tapped, ref flag);
                processed_flag = false;
                w.ShowDialog();

                if (!processed_flag)
                {
                    weight = 0;
                    UpdateSum();
                    return;
                }
                double value = xlRange.Cells[team_id+1, 3].Value2;
                if (costTotal > value)
                {
                    MessageBox.Show("Not enough credits!", "Error");
                    weight = 0;
                    UpdateSum();
                    Log("Error: " + xlRange.Cells[team_id+1, 2].Text + " remains " + value.ToString() + " credits, requests " + costTotal.ToString() + " credits, not enough credits.");
                    return;
                }

                String name = xlWorksheet.Cells[team_id + 1, 2].Text;
                String inf = "Confirm trade with Team " + name + " for payment of $" + costTotal.ToString() + "?";

                if (MessageBox.Show(inf, "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    MessageBox.Show("Cancelled.", "");
                    return;
                }

                value -= costTotal;
                //xlRange.Cells[team_id, 3].Value2 = value;
                int ptr = (int)xlLog.Cells[3, 14].Value2;
                xlLog.Cells[ptr, 1].Value2 = team_id;
                xlLog.Cells[ptr, 2].Value2 = -costTotal;
                xlLog.Cells[ptr, 3] = DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString();
                xlLog.Cells[3, 14] = ptr+1;
                MessageBox.Show("Balance Updated\n" + "New balance: " + value.ToString(), "Success");

                weight = 0;
                UpdateSum();

                Log("Success: " + xlRange.Cells[team_id+1, 2].Text + " remains " + value.ToString() + " credits, requests " + costTotal.ToString() + " credits, new balance " + value.ToString() + " credits.");
                pivotTables.Item(1).RefreshTable();
                xlWorkbook.Save();
            }
            catch (Exception e)
            {
                MessageBox.Show("No com port selected", "Error");
                Log("Error: cannot open port" + e.ToString());
                return;
            }

        }

        private void TextBoxWeightQuickCheckOut(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CheckOut();
            }
        }

        private void PriceAddOneClick(object sender, RoutedEventArgs e)
        {
            if (!Int32.TryParse(TextBoxWeight.Text, out weight) || weight < 0)
                weight = 1;
            else
                weight += 1;
            UpdateSum();
        }

        private void PriceAddTenClick(object sender, RoutedEventArgs e)
        {
            if (!Int32.TryParse(TextBoxWeight.Text, out weight) || weight < 0)
                weight = 10;
            else
                weight += 10;
            UpdateSum();
        }

        private void PriceAddFiveClick(object sender, RoutedEventArgs e)
        {
            if (!Int32.TryParse(TextBoxWeight.Text, out weight) || weight < 0)
                weight = 5;
            else
                weight += 5;
            UpdateSum();
        }

        private void PriceSubOneClick(object sender, RoutedEventArgs e)
        {
            if (!Int32.TryParse(TextBoxWeight.Text, out weight) || weight < 1)
                weight = 0;
            else
                weight -= 1;
            UpdateSum();
        }

        private void PriceSubFiveClick(object sender, RoutedEventArgs e)
        {
            if (!Int32.TryParse(TextBoxWeight.Text, out weight) || weight < 5)
                weight = 0;
            else
                weight -= 5;
            UpdateSum();
        }

        private void PriceSubTenClick(object sender, RoutedEventArgs e)
        {
            if (!Int32.TryParse(TextBoxWeight.Text, out weight) || weight < 10)
                weight = 0;
            else
                weight -= 10;
            UpdateSum();
        }

        private void CheckoutClick(object sender, RoutedEventArgs e)
        {
            CheckOut();
        }

        private void ClrBtnClick(object sender, RoutedEventArgs e)
        {
            costTotal = 0;
            weight = 0;
            UpdateSum();
        }

        private void BalanceChanged(object sender, KeyEventArgs e)
        {
            Int32.TryParse(TextBoxWeight.Text, out weight);
            Int32.TryParse(TextBoxUnitPrice.Text, out unitPrice);
            UpdateSum();
        }

        private void UpdateSum()
        {
            costTotal = weight * unitPrice;
            TextBlockSum.Text = costTotal.ToString();
            TextBoxWeight.Text = weight.ToString();
        }
    }
}
