using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Diagnostics;
using System.Timers;
using Microsoft.Owin.Hosting;
using System.Net.Http;
using EYPE_LOCAL_SERVER;

namespace ConsoleApplication1
{

    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://*:9000/";
            try
            {
                SerialConfig.Inizialize();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //var config = new HttpSelfHostConfiguration("http://*:9000");

            /* Inizializzazione serra e timer */
            try
            {
                ValuesController.GreenhouseInizialization(); // Greenhouse inizialization from ValuesController class
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            /* Set timer per acquisizione dati */
            Timer tmr = new Timer(); // timer per ricezione dati dai sensori

            tmr.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            tmr.Interval = 1800000; // set timer interval to 1800 seconds (30 minutes)
            //tmr.Interval = 10000; // set timer interval to 10 seconds
            tmr.Enabled = true;

            /* Set timer per invio comando per nutrienti alla pianta */
            Timer tmr2 = new Timer(); // timer per l'invio del comando di rifornimento nutrienti per pianta

            tmr2.Elapsed += new ElapsedEventHandler(OnTimedEvent2);
            tmr2.Interval = 2592000000; // set timer interval to 2592000 seconds (30 day)
            //tmr2.Enabled = true;

            /* Inizializzazione server */

            // Start OWIN host 
            Startup tmp = new Startup();
            tmp.Start();
            //using (WebApp.Start<Startup>(url: baseAddress))
            {
                // Create HttpCient and make a request to api/values 
                Console.WriteLine("Premi return per chiudere l'applicazione...");
                Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", "-kiosk -fullscreen " + "localhost:9000");
                Console.ReadLine();
            }
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                ValuesController.receiveBySerial();
            }
            catch (Exception ex)
            {
                throw new Exception("C'è stato un errore con l'acquisizione dei dati nella serra!");
            }
        }

        private static void OnTimedEvent2(object source, ElapsedEventArgs e)
        {
            string packet = "#I1;$";
            do
            {
                try
                {
                    SerialConfig.sendData(packet);
                }
                catch (Exception ex)
                {
                    throw new Exception("C'è stato un errore con l'acquisizione dei dati nella serra!");
                }

                packet = SerialConfig.ReceivedData;
            } while (packet != "#Z1;$");
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Http;
//using System.Web.Http.SelfHost;
//using System.Diagnostics;
//using System.Timers;
//using Microsoft.Owin.Hosting;
//using System.Net.Http;
//using EYPE_LOCAL_SERVER;

//namespace ConsoleApplication1
//{

//    class Program
//    {
//        static Timer tmr = new Timer(); // timer per ricezione dati dai sensori
//        static Timer tmr2 = new Timer(); // timer per l'invio del comando di rifornimento nutrienti per pianta

//        static void Main(string[] args)
//        {
//            string baseAddress = "http://*:9000/";
//            try
//            {
//                SerialConfig.Inizialize();
//            }
//            catch(Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }

//            //var config = new HttpSelfHostConfiguration("http://*:9000");

//            /* Inizializzazione serra e timer */
//            //ValuesController.GreenhouseInizialization(); // Greenhouse inizialization from ValuesController class

//            /* Set timer per acquisizione dati */

//            tmr.Elapsed += new ElapsedEventHandler(OnTimedEvent);
//            tmr.Interval = 1800000; // set timer interval to 1800 seconds (30 minutes)
//            //tmr.Interval = 50; // set timer interval to 1800 seconds (30 minutes)
//            //tmr.Enabled = true;

//            /* Set timer per invio comando per nutrienti alla pianta */

//            tmr2.Elapsed += new ElapsedEventHandler(OnTimedEvent2);
//            //tmr2.Interval = 2592000000; // set timer interval to 2592000 seconds (30 day)
//            tmr2.Interval = 2000; // set timer interval to 2592000 seconds (30 day)
//            //tmr2.Enabled = true;

//            /* Inizializzazione server */

//            // Start OWIN host 
//            using (WebApp.Start<Startup>(url: baseAddress))
//            {
//                // Create HttpCient and make a request to api/values 
//                Console.WriteLine("Premi return per chiudere l'applicazione...");
//                Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", "-kiosk -fullscreen " + "localhost:9000");
//                Console.ReadLine();
//            }
//        }

//        private static void OnTimedEvent(object source, ElapsedEventArgs e)
//        {
//            tmr.Enabled = false;
//            try
//            {
//                ValuesController.receiveBySerial();
//            }
//            catch(Exception ex)
//            {
//                throw new Exception("C'è stato un errore con l'acquisizione dei dati nella serra!");
//            }
//            tmr.Enabled = true;
//        }

//        private static void OnTimedEvent2(object source, ElapsedEventArgs e)
//        {
//            string packet = "#I1;$";
//            do
//            {
//                try
//                {
//                    SerialConfig.sendData(packet);
//                }
//                catch (Exception ex)
//                {
//                    throw new Exception("C'è stato un errore con l'acquisizione dei dati nella serra!");
//                }

//                packet = SerialConfig.ReceivedData;
//            } while (packet != "#Z1;$");
//        }
//    }
//}
