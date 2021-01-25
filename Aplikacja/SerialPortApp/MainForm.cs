/**
*   @author Adrian Wojcik
*   @file MainForm.cs
*   @date 02.11.17
*   @brief Main form class.
*   Based on project by Amund Gjersøe (www.codeproject.com/Articles/75770/Basic-serial-port-listening-application)
*/

/*
 * System libraries
 */
using System;
using System.Text;
using System.Windows.Forms;
using SerialPortApp.Serial;
using System.Globalization;
using System.Diagnostics;

namespace SerialPortApp
{
    /*
    * Main form class. Inherit from form Form class.
    * Partial definition -  remider of the class defined in 
    * automatically generated file MainForm.designer.cs
    */
    public partial class MainForm : Form
    {
        //! Default constructor.
        public MainForm()
        {
            InitializeComponent();
            UserInitialization();
        }

        #region Fields

        SerialPortManager _spManager; /** Custom serial port manager class object. */

        #endregion

        #region Event handlers

        /*
         * Main form window closing event handling function.
         * @param sender - contains a reference to the control/object that raised the event.
         * @param e - contains the form closing event data.
         */
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _spManager.Dispose();
        }

        /*
        * New serial port data recived event handlig function. Update of "tbDataReceive" text box.
        * @param sender - contains a reference to the control/object that raised the event.
        * @param e - contains the serial port event data.
        */
        int p = 0;
        void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved), new object[] { sender, e });
                return;
            }

            int maxTextLength = 100; // maximum text length in text box
            if (tbDataReceive.TextLength > maxTextLength)
                tbDataReceive.Text = tbDataReceive.Text.Remove(0, tbDataReceive.TextLength - maxTextLength);

            // Byte array to string
            string str = Encoding.ASCII.GetString(e.Data);

            tbDataReceive.AppendText(str);
            tbDataReceive.ScrollToCaret();
            ValDisp();
            if(_data.Length==3)
            {
                fanChartStart();
                tempChartStart();
            }

        }

        /*
        * Error handling function. Display message in groupBox "Exceptions".
        * @param sender - contains a reference to the control/object that raised the event.
        * @param e - contains the event data.
        */
        private void _spManager_ErrorHandler(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<EventArgs>(_spManager_ErrorHandler), new object[] { sender, e });
                return;
            }
            error_label.Text = ((Exception)sender).Message;
        }

        /*
        * Handles the "Connect"-buttom click event
        * @param sender - contains a reference to the control/object that raised the event.
        * @param e - contains the event data.
        */
        private void btnStart_Click(object sender, EventArgs e)
        {
            Connect();
        }

        /*
        * Handles the "Diconnect"-buttom click event
        * @param sender - contains a reference to the control/object that raised the event.
        * @param e - contains the event data.
        */
        private void btnStop_Click(object sender, EventArgs e)
        {
            Disonnect();
        }

        /*
        * Handles the "Send"-buttom click event
        * @param sender - contains a reference to the control/object that raised the event.
        * @param e - contains the event data.
        */
        private void btnSend_Click(object sender, EventArgs e)
        {
            _spManager.Send(tbDataTransmit.Text);
        }

        /*
        * Handles the "Clear"-buttom click event
        * @param sender - contains a reference to the control/object that raised the event.
        * @param e - contains the event data.
        */
        private void btnClear_Click(object sender, EventArgs e)
        {
            tbDataReceive.Clear();
        }

        /*
         * Receive text box 'Rx Enable' check box click event method.
         * @param sender - contains a reference to the control/object that raised the event.
         * @param e - contains the event data.
         */
        private void rxEnableCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (rxEnableCheckBox.Checked)
                RxTextBoxEnable();
            else
                RxTextBoxDisable();
        }

        #endregion

        #region Methods

        /*
         * User custom initialization.
         */
        private void UserInitialization()
        {
            // New serial port manager
            _spManager = new SerialPortManager();

            // Read current serial port settings 
            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;

            // Bind serial port & user interface data sources with serial port settings
            serialSettingsBindingSource.DataSource = mySerialSettings;
            portNameComboBox.DataSource = mySerialSettings.PortNameCollection;
            baudRateComboBox.DataSource = mySerialSettings.BaudRateCollection;
            dataBitsComboBox.DataSource = mySerialSettings.DataBitsCollection;
            parityComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            stopBitsComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));

            // Add evnet handling functions to serial port manager
            _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
            _spManager.ErrorHandler += new EventHandler<EventArgs>(_spManager_ErrorHandler);

            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

            // Diable "Disconnect" button
            btnStop.Enabled = false;
            textBox1.Text = "25.00";
        }

        /*
        * Connect procedure - open and start listening on COM port.
        */
        private void Connect()
        {
            if (_spManager.StartListening())
            {
                btnStop.Enabled = true;
                btnStart.Enabled = false;
                portNameComboBox.Enabled = false;
                baudRateComboBox.Enabled = false;
                dataBitsComboBox.Enabled = false;
                parityComboBox.Enabled = false;
                stopBitsComboBox.Enabled = false;
            }
        }

        /*
        * Disconnect procedure - close and stop listening on COM port.
        */
        private void Disonnect()
        {
            _spManager.StopListening();
            btnStop.Enabled = false;
            btnStart.Enabled = true;
            portNameComboBox.Enabled = true;
            baudRateComboBox.Enabled = true;
            dataBitsComboBox.Enabled = true;
            parityComboBox.Enabled = true;
            stopBitsComboBox.Enabled = true;
        }
   
        /*
         * Enables receive text box.
         */
        private void RxTextBoxEnable()
        {
            rxEnableCheckBox.Checked = true;
            tbDataReceive.Enabled = true;
            _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
        }

        /*
         * Disables receive text box.
         */
        private void RxTextBoxDisable()
        {
            rxEnableCheckBox.Checked = false;
            tbDataReceive.Enabled = false;
            _spManager.NewSerialDataRecieved -= new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
        }


        #endregion

        UInt16 _dacValue;
        string[] _data;
        double _plotTimeStep = 0.1;
        double _plotTime = 0.0;
        const double _plotTimeMax = 10.0;
        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            TrackBar tb = (TrackBar)sender;
            _dacValue = (UInt16)tb.Value;
            float disp = _dacValue;
            disp = disp / 100;
            textBox1.Text = disp.ToString("0.00");
        }

        private void SetButton_Click(object sender, EventArgs e)
        {
            if(_dacValue<=3500&&_dacValue>=2500)
            {
                _spManager.Send(_dacValue.ToString(""));
            }
        }

        private void ValDisp()
        {
            string str = tbDataReceive.Text;

            if (str.Length > 12)
            {
                string[] tmp = str.Split(';');
                if (tmp.Length > 0)
                {
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        if (tmp[i].Length > 10)
                        {
                            _data = tmp[i].Split(',');
                            if (_data.Length == 3 && _data[0].Length == 4 && _data[1].Length == 4)
                            {
                                _data[1] = _data[1].Replace(" ", String.Empty);
                                TargetTemperatureDisplay(_data[0]);
                                CurrentTemperatureDisplay(_data[1]);
                                FanSpeedDisplay(_data[2]);
                            }
                            break;
                        }
                    }


                }
            }
        }

        private void TargetTemperatureDisplay(string str)
        {
            str = str.Insert(str.Length - 2, ",");
            targetTempBox.Text = str;
        }
        private void CurrentTemperatureDisplay(string str)
        {
            float temp = Convert.ToSingle(str);
            temp = temp / 100;
            currentTempBox.Text = temp.ToString();
        }

        private void FanSpeedDisplay(string str)
        {
            fanSpeedbox.Text = str;
        }

        private void setLog()
        {
            string log;
            string[] logs = new string[3];
            logs[0] = targetTempBox.Text;
            logs[1] = currentTempBox.Text;
            logs[2] = fanSpeedbox.Text;
            if(logs[0] != "" && logs[1] != "" && logs[2] != "" )
            {
                log = "Target Temperature: " + logs[0] + " | Current Temperature: " + logs[1] + " | Fan Speed: " + logs[2] + "\r\n";
                logBox.AppendText(log);
                logBox.ScrollToCaret();
            }
        }

        private void fanChartStart()
        {
                try
                {
                    int fanspeed = Convert.ToInt32(_data[2]);
                if(fanspeed>=0&&fanspeed<=100)
                {
                    fanChart.Series[0].Points.AddXY(_plotTime, fanspeed);
                    _plotTime += _plotTimeStep;
                }
                }
                catch (Exception error)
                {
                    Debug.WriteLine(error.Message);
                }


        }

        private void tempChartStart()
        {
            try
            {
                int tempValue = Convert.ToInt32(_data[0]);
                int currentTemp = Convert.ToInt32(_data[1]);
                if (tempValue >= 2500 && tempValue <= 3500)
                {
                    tempChart.Series[0].Points.AddXY(_plotTime, currentTemp);
                    tempChart.Series[1].Points.AddXY(_plotTime, tempValue);
                  //  _plotTime += _plotTimeStep;
                }
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.Message);
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            setLog();
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void tabControl_Click(object sender, EventArgs e)
        {

        }

        private void tbDataReceive_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void currentTempBox_TextChanged(object sender, EventArgs e)
        {
            setLog();
        }

        private void fanSpeedbox_TextChanged(object sender, EventArgs e)
        {
            setLog();
        }
    }
}
