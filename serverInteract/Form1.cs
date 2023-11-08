using serverInteract;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;




namespace serverInteract
{
    public partial class Form1 : Form
    {
        private TcpListener tcpListener;
        private List<TcpClient> connectedClients;
        private Thread serverThread;
        private bool isServerRunning = false;

        public Form1()
        {
            tcpListener = new TcpListener(IPAddress.Any, 12345); // Choose a port number
            connectedClients = new List<TcpClient>();
            serverThread = new Thread(ListenForClients);
            serverThread.Start();
            isServerRunning = true;
            UpdateStatus("Server is running.");
            InitializeComponent();
            UTLnv.Text = UTLdisp.Text;
            LTLnv.Text = LTLdisp.Text;
            TTnv.Text = TTdisp.Text;
            UALnv.Text = UALdisp.Text;
            LALnv.Text = LTLnv.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double UTL = (float)Convert.ToDecimal(torqueValueFormatter(UTLnv.Text));
            double LTL = (float)Convert.ToDecimal(torqueValueFormatter(LTLnv.Text));
            double TT = (float)Convert.ToDecimal(torqueValueFormatter(TTnv.Text));
            if (UTL<LTL)
            {
                UTL=0;
                LTL = 0;
                Console.WriteLine("invalid Torque Ranges entered");
            }
            if (TT == 0.00)
            {
                UTL = 0.00;
                LTL = 0.00;
                Console.WriteLine("trigger torque is 0");

            }
            if (TT > UTL || TT < 5.00)
            {
                TT = 0.00;
                Console.WriteLine("invalid Torque Ranges entered");
            }

            string dataToSend = $"SetTorqueLimit,UTorque={torqueValueFormatter(UTL.ToString())},LTorque={torqueValueFormatter(LTL.ToString())},TTorque={torqueValueFormatter(TT.ToString())}\r\n";
            byte[] sendData = Encoding.ASCII.GetBytes(dataToSend);

            foreach (TcpClient client in connectedClients)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(sendData, 0, sendData.Length);
            }
        }
        
        private string torqueValueFormatter(string input)
        {

            float torque;
            if (input == null || input=="")
            {
                torque = 0;
                return string.Format("{0:00.00}", torque);
            }
            else
            {
                torque = (float)Convert.ToDecimal(input);
                if (torque >= 100.00 || torque<0.0)
                {
                    torque = 0;
                    Console.WriteLine("invalid Torque value");
                }
                return string.Format("{0:00.00}", torque);

            }
        }
        private void ListenForClients()
        {
            tcpListener.Start();

            while (isServerRunning)
            {
                try
                {
                    TcpClient client = tcpListener.AcceptTcpClient();
                    connectedClients.Add(client);
                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.Start();
                }
                catch (SocketException)
                {
                    // Handle exceptions as needed
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024]; // Adjust the buffer size as needed
            int bytesRead;

            while (isServerRunning)
            {
                try
                {
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        break; // Connection closed
                    }

                    string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    AddReceivedDataToTextBox(receivedData);
                }
                catch (Exception)
                {
                    // Handle exceptions as needed
                    break;
                }
            }

            connectedClients.Remove(client);
            client.Close();
        }

        private void AddReceivedDataToTextBox(string data)
        {
            // Update the form's UI with received data
            /*   if (textBoxReceived.InvokeRequired)
               {
                   textBoxReceived.Invoke(new Action(() => AddReceivedDataToTextBox(data)));
               }
               else
               {
                   textBoxReceived.AppendText(data + Environment.NewLine);
               }*/
            Console.WriteLine(data);
            string[] inputlist = data.Split(',');
            foreach (string item in inputlist)
            {
                Console.WriteLine(item);
                if (item != "AllSetting")
                {
                    string[] tmpitem = item.Split('=');
                    Console.WriteLine(tmpitem[0].ToString());
                    try
                    {
                        //Some code here.
                        if (tmpitem[0].ToString() == "UTorque")
                    {
                        Console.WriteLine("chk1");
                        //UTLdisp.Text = tmpitem[1].ToString();
                        if (UTLdisp.InvokeRequired)
                            {
                                UTLdisp.Invoke(new Action(() => AddReceivedDataToTextBox(data)));
                            }
                        else
                            {
                                UTLdisp.Text = tmpitem[1].ToString();
                            }
                        }
                    else if (tmpitem[0].ToString() == "LTorque")
                    {
                        //LTLdisp.Text = tmpitem[1].ToString();
                            if (LTLdisp.InvokeRequired)
                            {
                                LTLdisp.Invoke(new Action(() => AddReceivedDataToTextBox(data)));
                            }
                            else
                            {
                                LTLdisp.Text = tmpitem[1].ToString();
                            }
                        }
                    else if (tmpitem[0].ToString() == "TTorque")
                    {
                        //TTdisp.Text = tmpitem[1].ToString();
                            if (TTdisp.InvokeRequired)
                            {
                                TTdisp.Invoke(new Action(() => AddReceivedDataToTextBox(data)));
                            }
                            else
                            {
                                TTdisp.Text = tmpitem[1].ToString();
                            }
                        }
                    else if (tmpitem[0].ToString() == "UAngle")
                    {
                        //UALdisp.Text = tmpitem[1].ToString();
                            if (UALdisp.InvokeRequired)
                            {
                                UALdisp.Invoke(new Action(() => AddReceivedDataToTextBox(data)));
                            }
                            else
                            {
                                UALdisp.Text = tmpitem[1].ToString();
                            }
                        }
                    else if (tmpitem[0].ToString() == "LAngle")
                    {
                        //LALdisp.Text = tmpitem[1].ToString();
                            if (LALdisp.InvokeRequired)
                            {
                                LALdisp.Invoke(new Action(() => AddReceivedDataToTextBox(data)));
                            }
                            else
                            {
                                LALdisp.Text = tmpitem[1].ToString();
                            }
                        }
                        //Also, set your breakpoints here.
                        
                    }

                    catch (InvalidOperationException exc)
                    {
                        Console.WriteLine(exc.ToString());
                    }

                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                    
                }
            }
        }

        private void UpdateStatus(string status)
        {
           /* if (labelStatus.InvokeRequired)
            {
                labelStatus.Invoke(new Action(() => UpdateStatus(status)));
            }
            else
            {
                labelStatus.Text = status;
            }*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int TT = 0;
            if (TTnv.Text == null || TTnv.Text.Trim() == "")
            {
               TT = Convert.ToInt16(TTdisp.Text);
            }
            else
            {
                TT = Convert.ToInt16(TTnv.Text);
            }
            int UAL = Convert.ToInt16(angleValueFormatter(UALnv.Text));
            int LAL = Convert.ToInt16(angleValueFormatter(LALnv.Text));

            if (TT == 0)
            {
                LAL = 0;
                UAL = 0;
            }
            if (LAL > UAL)
            {
                LAL = 0;
                UAL = 0;
            }
            string dataToSend = $"SetAngleLimit,UAngle={UAL.ToString().PadLeft(3, '0')},LAngle={LAL.ToString().PadLeft(3, '0')}\r\n";
            byte[] sendData = Encoding.ASCII.GetBytes(dataToSend);

            foreach (TcpClient client in connectedClients)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(sendData, 0, sendData.Length);
            }

        }

        private string angleValueFormatter(string text)
        {
            int angle = Convert.ToInt16(text);
            if(angle < 0 || angle > 999)
            {
                angle = 0;
                Console.WriteLine("angle out of range");
            }
            return $"{angle:000}";

        }
    }
}
