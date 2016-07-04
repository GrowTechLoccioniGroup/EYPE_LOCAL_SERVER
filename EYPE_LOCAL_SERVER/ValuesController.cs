using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Controllers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Globalization;

namespace EYPE_LOCAL_SERVER
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api")]
    public class ValuesController : ApiController
    {

        static object locker = new object();
        static bool token = true;
        static List<Parameters> listParameters = new List<Parameters>();
        static Options settings = new Options();
        static List<Recipe> listRecipes = new List<Recipe>();
        static string sqlQuery; // query per database
        static bool result;
        static bool curtains = false; // curtains state: false=down;true=up

        // GET api/shutdown ###SPEGNIMENTO SERRA###
        [Route("shutdown")]
        [HttpGet]
        public void shutdownPC()
        {
            /* Abbassamento tende */
            /* Invio dati ai circuiti elettronici */
            if(curtains)
            {
                string packet = "#G0;$"; // creo il pacchetto per le opzioni da inviare
                try
                {
                    result = sendOnSerial(packet);
                    Console.WriteLine("Inviato: " + packet);
                }
                catch (Exception ex)
                {
                    //throw new Exception(ex.Message);
                }
            }

            /* Chiusura connessione seriale */
            SerialConfig.CloseConnection();

            /* Spegnimento computer */
            Process.Start("shutdown", "/f /s /t 5"); // shutdown pc
        }

        // GET api/values/getparameters ###RICHIESTA PARAMETRI DAL CLIENT LOCALE###
        [Route("values/getparameters")]
        [HttpGet]
        public List<Parameters> getParameters()
        {
            return ValuesController.getParameterFromDB();
        }

        public static List<Parameters> getParameterFromDB()
        {
            var sqlQuery = "SELECT * from Parameters ORDER BY TimeStamp DESC";
            return Database.getValuesList(sqlQuery);
        }

        // GET api/values/getoptions ###RICHIESTA OPZIONI DAL CLIENT LOCALE###
        [Route("values/getoptions")]
        [HttpGet]
        public Options getOptions()
        {
            return getOptionsFromDB();
        }

        public static Options getOptionsFromDB()
        {
            var sqlQuery = "SELECT * from Options";
            return Database.getValuesObject(sqlQuery);
        }

        // GET api/recipes ###RICHIESTA RICETTE###
        [Route("recipes")]
        [HttpGet]
        public List<Recipe> getRecipes()
        {
            sqlQuery = "SELECT * from Recipes";
            listRecipes = Database.getRecipesDB(sqlQuery);
            return listRecipes;
        }


        Parameters jsonObjectForParameters = null;
        private void internalSetParameter(Parameters jsonObject)
        {
            jsonObjectForParameters = jsonObject;
            string packet = "#A" + jsonObject.AirTemperature + ";C" + jsonObject.Humidity + ";D" + jsonObject.PH + ";$"; // creo il pacchetto per le opzioni da inviare
      
                try
                {
                SerialConfig.dataReceived += serialReceived_setparameter;
                    result = sendOnSerial(packet);
                Console.WriteLine("Inviato: " + packet);
            }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }               

        }

        private void serialReceived_setparameter(object sender, string e)
        {
            SerialConfig.dataReceived -= serialReceived_setparameter;
            Console.WriteLine("Ricevuto: " + e);
            if (e.Contains("Z0"))
            {
                internalSetParameter(jsonObjectForParameters);
            }
        }

        // PUT api/values/setparameters ###MODIFICA PARAMETRI DA PARTE DEL CLIENT LOCALE###
        [Route("values/setparameters")]
        [HttpPut]
        public bool setParameters([FromBody]Parameters jsonObject)
        {
            /* Invio dati ai circuiti elettronici */

            internalSetParameter(jsonObject);
            return result;
        }

        Options receivedOptionFromClient = null;

        private void internalSetOption(Options jsonObject)
        {
            receivedOptionFromClient = jsonObject;
            /* Invio dati ai circuiti elettronici */
            string packet = "#F" + jsonObject.RGB + ";H" + (jsonObject.Light ? 1 : 0) + ";$"; // creo il pacchetto per le opzioni da inviare

            SerialConfig.dataReceived += serialReceived_options;
            try
            {
                result = sendOnSerial(packet);
                Console.WriteLine("Inviato: " + packet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {

            }


            /* Memorizzazione dati ricevuti su Database */

        }

        // PUT api/values/setoptions ###MODIFICA OPZIONI DA PARTE DEL CLIENT LOCALE###
        [Route("values/setoptions")]
        [HttpPut]
        public bool setOptions([FromBody]Options jsonObject)
        {
            internalSetOption(jsonObject);
            return result;
        }

        private void serialReceived_options(object sender, string e)
        { SerialConfig.dataReceived -= serialReceived_options;
            Console.WriteLine("Ricevuto: " + e);
            if (e.Contains("Z0"))
            {
                internalSetOption(receivedOptionFromClient);
            }
            else
            {
                try
                {
                    sqlQuery = "UPDATE Options SET RGB = @rgb, Light = @light, AddressImage = @addressImage LIMIT 1";
                    Database.setValues(sqlQuery, receivedOptionFromClient);
                }
                catch (Exception ex)
                {
                    //throw new Exception(ex.Message);
                }
                finally
                {

                }

                /* Invio dati al server web */
                try
                {
                    ServerRequest.PutRequest(receivedOptionFromClient);
                }
                catch (Exception ex)
                {
                    //throw new Exception(ex.Message);
                }
            }
        }

        // POST api/recipes/new ###CREAZIONE DI UNA NUOVA RICETTA###
        [Route("recipes/new")]
        [HttpPost]
        public bool newRecipe([FromBody]Recipe jsonObject)
        {
            listRecipes.Clear();
            listRecipes.Add(jsonObject);
            result = true;

            /* Memorizzazione dati ricevuti su Database */
            try
            {
                sqlQuery = "INSERT INTO Recipes (Name, AirTemperature, WaterTemperature, Humidity, PH, Conductivity, RGB, Light, AddressImage) VALUES (@name, @airTemperature, @waterTemperature, @humidity, @pH, @conductivity, @rgb, @light, @addressImage)";
                Database.setValues(sqlQuery, jsonObject);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
            }

            /* Invio dati al server web */
            try
            {
                ServerRequest.PostRequest(listRecipes);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
            }

            return result;
        }

        // PUT api/recipes/apply ###APPLICAZIONE DI UNA RICETTA###
        [Route("recipes/apply")]
        [HttpPut]
        public bool applyRecipe([FromBody]Recipe jsonObject)
        {
            result = true;
            /* Invio parametri ai circuiti elettronici */
            Parameters param = new Parameters(DateTime.Now, jsonObject.AirTemperature, jsonObject.WaterTemperature, jsonObject.Humidity, jsonObject.PH, jsonObject.Conductivity);
            internalSetParameter(param);

            /* Invio opzioni ai circuiti elettronici */
            Options opt = new Options(jsonObject.RGB, jsonObject.Light);
            internalSetOption(opt);

            /* Eliminazione dei parametri vecchi */
            sqlQuery = "DELETE FROM Parameters WHERE id_parameters>0";
            Database.setValues(sqlQuery);
            /* Creazione primo record */
            DateTime timeStamp = DateTime.Now;
            Parameters obj = new Parameters(timeStamp, 0.0f, 0.0f, 0, 7, 0);
            sqlQuery = "INSERT INTO Parameters (TimeStamp, AirTemperature, WaterTemperature, Humidity, PH, Conductivity) VALUES (@timeStamp, @airTemperature, @waterTemperature, @humidity, @pH, @conductivity)";
            Database.setValues(sqlQuery, obj);

            /* Memorizzazione dati ricevuti su Database */
            Options modified = new Options(jsonObject.RGB, jsonObject.Light, jsonObject.AddressImage);
            try
            {
                sqlQuery = "UPDATE Options SET RGB = @rgb, Light = @light, AddressImage = @addressImage LIMIT 1";
                Database.setValues(sqlQuery, modified);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
            }

            /* Invio dati al server web */
            try
            {
                ServerRequest.DeleteRequest();
                ServerRequest.PutRequest(modified);
                listRecipes.Clear();
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
            }

            return result;
        }

        // PUT api/values/setcurtains ###DAL CLIENT SI ALZANO O ABBASSANO LE TENDE###
        [Route("values/setcurtains/{val}")]
        [HttpPut]
        public bool setCurtains(int val)
        {
            /* Invio dati ai circuiti elettronici */
            string packet = "#G" + (val == 1 ? 1 : 0) + ";$"; // creo il pacchetto per le opzioni da inviare
            bool app = (val == 1 ? true : false);

            if(curtains!=app)
            {
                try
                {
                    SerialConfig.dataReceived += serialReceived_curtains;
                    result = sendOnSerial(packet);
                    curtains = app;
                    Console.WriteLine("Inviato: " + packet);
                }
                catch (Exception ex)
                {
                    //throw new Exception(ex.Message);
                }
            }

            return result;
        }

        private void serialReceived_curtains(object sender, string e)
        {
            SerialConfig.dataReceived -= serialReceived_curtains;
            //throw new NotImplementedException();
            Console.WriteLine("CURTAINS Ricevuto:" + e);
            Console.WriteLine(e);
           
        }

        // ###INVIO DATI ALL'ELETTRONICA SULLA SERIALE###
        public static bool sendOnSerial(string packet)
        {
            /* Invio dati su seriale */
            try
            {
                SerialConfig.sendData(packet);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }
        static int  indexGggggg = 0;
        // ###RICEZIONE DATI SULLA SERIALE DALL'ELETTRONICA###
        public static void  receiveBySerial()
        {
            
            string packet; // data packet received from circuits
            

            /* Richiesta dati all'elettronica */
            packet = "#R;$"; // packet for requesting data
            SerialConfig.dataReceived += serialReceived_parameter;
            result = sendOnSerial(packet); // request data from serial
            // Console.Clear();
            Console.WriteLine("------------------");
            Console.WriteLine("Data:" + DateTime.Now);
            Console.WriteLine("Giro numero " + (indexGggggg++));
            Console.WriteLine("Inviato:" + packet);
            packet = "";
        }


          

        private static void serialReceived_parameter(object sender, string e)
        {
            SerialConfig.dataReceived -= serialReceived_parameter;
            Console.WriteLine("Ricevuto: " + e);

            Random rnd = new Random();

            Options jsonObject = null;
            DateTime timeStamp;
            float airTemperature = 26.5f;
            float waterTemperature = 23.5f;
            int humidity = 70;
            float pH = 6.5f;
            int conductivity = 1500;
            string rgb = "213-000-043";
            bool light = true;
            bool dataReceived = true; // true if received useful data from circuits
            string packet; // data packet received from circuits


            try
            {
                packet = e;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            Console.WriteLine("Ricevuto:" + packet);

            if (packet.Length > 10 && packet[0] == '#')
            {
                string app = ""; // stringa d'appoggio
                // eseguo il ciclo fino ad arrivare a fine stringa o fino al carattere terminatore
                string toParse = packet.Substring(1, packet.Length - 2);
                Console.WriteLine("To parse " + toParse);

                string[] tmpArray = toParse.Split(';');
                foreach (string value in tmpArray)
                {
                    switch (value[0])
                    {
                        case 'A':
                            airTemperature = Convert.ToSingle(value.Substring(1).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                            Console.WriteLine("airTemperature " + airTemperature);
                            break;

                        case 'B':
                            waterTemperature = Convert.ToSingle(value.Substring(1).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                            Console.WriteLine("waterTemperature " + waterTemperature);
                            break;

                        case 'C':
                            humidity = (int)Math.Round(Convert.ToSingle(value.Substring(1).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)));
                            Console.WriteLine("humidity " + humidity);
                            break;

                        case 'D':
                            pH = Convert.ToSingle(value.Substring(1).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                            Console.WriteLine("pH " + pH);
                            break;

                        case 'E':
                            conductivity = (int)Math.Round(Convert.ToSingle(value.Substring(1).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)));
                            Console.WriteLine("conductivity " + conductivity);
                            break;

                        case 'F':
                            rgb = value.Substring(1);
                            Console.WriteLine("rgb " + rgb);
                            break;

                        case 'H':
                            light = (value[1] == '1' ? true : false);
                            break;
                    }
                }




                /* Generazione random del dato della temperatura dell'acqua */
                waterTemperature = Convert.ToSingle(rnd.NextDouble());
                waterTemperature = (float)Math.Round(waterTemperature, 1);
                waterTemperature = airTemperature - (3 + waterTemperature);

                /* Generazione random del dato della conducibilità */
                conductivity = ValuesController.getParameterFromDB()[0].Conductivity;

                // do this only if received something useful
                if (dataReceived)
                {
                    // creazione di un nuovo nodo nella lista
                    timeStamp = DateTime.Now;
                    listParameters.Add(new Parameters(timeStamp, airTemperature, waterTemperature, humidity, pH, conductivity));

                    /* Memorizzazione dati ricevuti su Database */
                    try
                    {
                        Parameters obj = new Parameters(timeStamp, airTemperature, waterTemperature, humidity, pH, conductivity);
                        sqlQuery = "INSERT INTO Parameters (TimeStamp, AirTemperature, WaterTemperature, Humidity, PH, Conductivity) VALUES (@timeStamp, @airTemperature, @waterTemperature, @humidity, @pH, @conductivity)";
                        Database.setValues(sqlQuery, obj);
                    }
                    catch (Exception ex)
                    {
                        //throw new Exception(ex.Message);
                    }

                    try
                    {
                        var sqlQuery = "SELECT * from Options";
                        string address = Database.getValuesObject(sqlQuery).AddressImage;
                        jsonObject = new Options(rgb, light, address);
                        sqlQuery = "UPDATE Options SET RGB = @rgb, Light = @light, AddressImage = @addressImage LIMIT 1";
                        Database.setValues(sqlQuery, jsonObject);
                    }
                    catch (Exception ex)
                    {
                        //throw new Exception(ex.Message);
                    }

                    /* Invio dati al server web */
                    // POST dei parametri
                    try
                    {
                        ServerRequest.PostRequest(listParameters);
                        listParameters.Clear();
                    }
                    catch (Exception ex)
                    {
                        //throw new Exception(ex.Message);
                    }
                    // PUT delle opzioni
                    try
                    {
                        ServerRequest.PutRequest(jsonObject);
                    }
                    catch (Exception ex)
                    {
                        //throw new Exception(ex.Message);
                    }
                }
            }            
        }

         

        // DELETE api/recipes/delete/{id} ###ELIMINAZIONE DI UNA RICETTA###
        [Route("recipes/delete/{id}")]
        [HttpDelete]
        public bool delRecipe(int id)
        {
            /* Modifica sul database (eliminazione della ricetta) */
            try
            {
                sqlQuery = "DELETE from Recipes where id_recipe = @idRecipe";
                Database.setValues(sqlQuery, id);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            /* Invio richiesta delete al server web */
            try
            {
                ServerRequest.DeleteRequest(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return true;
        }

        public static void  GreenhouseInizialization() // ###OPERAZIONI DA COMPIERE ALL'ACCENSIONE###
        {
            /* Ottenimento parametri e opzioni dal database */
            var listParameters = ValuesController.getParameterFromDB();
            var settings = ValuesController.getOptionsFromDB();

            ///* Invio parametri ai circuiti elettronici */
            //internalSetParameter(listParameters[0]);

            Parameters jsonObject = listParameters[0];
            string packet = "#A" + jsonObject.AirTemperature + ";C" + jsonObject.Humidity + ";D" + jsonObject.PH + ";$"; // creo il pacchetto per le opzioni da inviare
            
            try
            {
               // SerialConfig.dataReceived += serialReceived_setparameter;
                result = sendOnSerial(packet);
                Console.WriteLine("Inviato: " + packet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            System.Threading.Thread.Sleep(500);

            packet = "";
            /* Invio opzioni ai circuiti elettronici */
            packet = "#F" + settings.RGB + ";H" + (settings.Light ? 1 : 0) + ";$"; // creo il pacchetto per le opzioni da inviare
            try
            {
                result = sendOnSerial(packet);
                Console.WriteLine("Inviato: " + packet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {

            }
        }
    }
}
