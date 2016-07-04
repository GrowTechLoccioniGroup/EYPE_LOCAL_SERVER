using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Timers;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace EYPE_LOCAL_SERVER
{
    static class SerialConfig
    {
        const int maxModules = 2;
        static string portName = "COM4";
        static string packet;
        static SerialPort serialPort;
        static List<SerialPort> porte = getSerialPorts();
        public static event EventHandler<string> dataReceived;
        static public void Inizialize()
        {
            try
            {
                serialPort = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
                serialPort.Handshake = Handshake.None;
                //serialPort.RtsEnable = true;
                serialPort.ReceivedBytesThreshold = 1;
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            packet += sp.ReadExisting();
            if (packet.Contains("$"))
            {
                var tmp = packet.Substring(0, packet.IndexOf("$"));
                packet = packet.Substring(packet.IndexOf("$")+1);
                dataReceived?.Invoke(null, tmp);
            }
        }

        static public void CloseConnection()
        {
            serialPort.Close();
        }

        static public void sendData(string buffer)
        {
            try
            {
                serialPort.Write(buffer);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            //circuito.Write(buffer);
        }

        static public string ReceivedData
        {
            get { return packet; }
            set { packet = value; }
        }

        static public List<SerialPort> getSerialPorts()
        {
            string[] ports = new string[2];
            List<SerialPort> modules = new List<SerialPort>();
            ports = SerialPort.GetPortNames(); //check available portnames
            foreach (string port in ports)
            {
                SerialPort sTemp = new SerialPort(port);
                modules.Add(sTemp);
            }
            return modules;
        }
    }

    //public class SerialModules
    //{
    //    const double tick = 3600000; // every hour

    //    public List<SerialPort> modules;

    //    public SerialModules(List<SerialPort> modules)
    //    {
    //        this.modules = modules;
    //    }

    //    public void startRead()
    //    {
    //        Timer timer = new Timer(tick);
    //        timer.Start();
    //        timer.Elapsed += readParameters();
    //    }

    //    private ElapsedEventHandler readParameters()
    //    {
    //        //lettura della riga dalla seriale e inserimento nel database
    //        for (int i = 0; i < modules.Count; i++)
    //        {


    //        }


    //        throw new NotImplementedException();
    //    }
    //}
}
