using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MySql.Data.MySqlClient;
using MySql.Data.Types;

namespace EYPE_LOCAL_SERVER
{
    static class Database
    {
        static DateTime timeStamp;
        static int idRecipe;
        static int idParameters;
        static string name;
        static float airTemperature;
        static float waterTemperature;
        static int humidity;
        static float pH;
        static int conductivity;
        static string rgb;
        static bool light;
        static string addressImage;
        static List<Parameters> listParameters = new List<Parameters>();
        static Options settings;
        static List<Recipe> listRecipes = new List<Recipe>();
        static string connectionString = "host=localhost; user=eype; password=eype; database=eype_local";
       // static string connectionString = "Server=172.17.82.253;Database=eype_local;Uid=eype;Pwd=eype;";
        // ###PRENDE I PARAMETRI DAL DATABASE###
        static public List<Parameters> getValuesList(string sqlQuery)
        {
            listParameters.Clear();

            try
            {
                MySqlConnection db = new MySqlConnection(connectionString);

              
                db.Open();
                MySqlCommand dbConnection = new MySqlCommand(sqlQuery, db);
                MySqlDataReader reader = dbConnection.ExecuteReader();
                while (reader.Read())
                {
                    idParameters = reader.GetInt32("id_parameters");
                    timeStamp = reader.GetDateTime("TimeStamp");
                    airTemperature = reader.GetFloat("AirTemperature");
                    waterTemperature = reader.GetFloat("WaterTemperature");
                    humidity = reader.GetInt32("Humidity");
                    pH = reader.GetFloat("PH");
                    conductivity = reader.GetInt32("Conductivity");
                    listParameters.Add(new Parameters(idParameters, timeStamp, airTemperature, waterTemperature, humidity, pH, conductivity)); // istanzio un nuovo oggetto Parameters nella lista
                }
                db.Close();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return listParameters;
        }

        // ###PRENDE LE OPZIONI DAL DATABASE###
        static public Options getValuesObject(string sqlQuery)
        {
            try
            {
                MySqlConnection db = new MySqlConnection(connectionString);
                MySqlCommand dbConnection = new MySqlCommand(sqlQuery, db);
                db.Open();

                MySqlDataReader reader = dbConnection.ExecuteReader();
                reader.Read();
                rgb = reader.GetString("RGB");
                light = reader.GetBoolean("Light");
                addressImage = reader.GetString("AddressImage");
                settings = new Options(rgb, light, addressImage);
                db.Close();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return settings;
        }

        // ###PRENDE LE RICETTE DAL DATABASE###
        static public List<Recipe> getRecipesDB(string sqlQuery)
        {
            listRecipes.Clear();

            try
            {
                MySqlConnection db = new MySqlConnection(connectionString);
                MySqlCommand dbConnection = new MySqlCommand(sqlQuery, db);
                db.Open();

                MySqlDataReader reader = dbConnection.ExecuteReader();
                while (reader.Read())
                {
                    idRecipe = reader.GetInt32("id_recipe");
                    name = reader.GetString("Name");
                    airTemperature = reader.GetFloat("AirTemperature");
                    waterTemperature = reader.GetFloat("WaterTemperature");
                    humidity = reader.GetInt32("Humidity");
                    pH = reader.GetFloat("PH");
                    conductivity = reader.GetInt32("Conductivity");
                    rgb = reader.GetString("RGB");
                    light = reader.GetBoolean("Light");
                    addressImage = reader.GetString("AddressImage");
                    listRecipes.Add(new Recipe(idRecipe, name, airTemperature, waterTemperature, humidity, pH, conductivity, rgb, light, addressImage)); // istanzio un nuovo oggetto Parameters nella lista
                }
                db.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return listRecipes;
        }

        // MEMORIZZAZIONE SU DB DEI PARAMETRI
        static public void setValues(string sqlQuery, Parameters obj)
        {
            try
            {
                MySqlConnection db = new MySqlConnection(connectionString);
                MySqlCommand dbConnection = new MySqlCommand(sqlQuery, db);
                dbConnection.CommandText = sqlQuery;
                dbConnection.Parameters.AddWithValue("@timeStamp", obj.TimeStamp);
                dbConnection.Parameters.AddWithValue("@airTemperature", obj.AirTemperature);
                dbConnection.Parameters.AddWithValue("@waterTemperature", obj.WaterTemperature);
                dbConnection.Parameters.AddWithValue("@humidity", obj.Humidity);
                dbConnection.Parameters.AddWithValue("@pH", obj.PH);
                dbConnection.Parameters.AddWithValue("@conductivity", obj.Conductivity);
                db.Open();
                dbConnection.ExecuteNonQuery();
                db.Close();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // MEMORIZZAZIONE SU DB DELLE OPZIONI
        static public void setValues(string sqlQuery, Options jsonObject)
        {
            try
            {
                MySqlConnection db = new MySqlConnection(connectionString);
                MySqlCommand dbConnection = new MySqlCommand(sqlQuery, db);
                dbConnection.CommandText = sqlQuery;
                dbConnection.Parameters.AddWithValue("@rgb", jsonObject.RGB);
                dbConnection.Parameters.AddWithValue("@light", jsonObject.Light);
                dbConnection.Parameters.AddWithValue("@addressImage", jsonObject.AddressImage);
                db.Open();
                dbConnection.ExecuteNonQuery();
                db.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // MEMORIZZAZIONE SU DB DELLE RICETTE
        static public void setValues(string sqlQuery, Recipe obj)
        {
            try
            {
                MySqlConnection db = new MySqlConnection(connectionString);
                MySqlCommand dbConnection = new MySqlCommand(sqlQuery, db);
                dbConnection.CommandText = sqlQuery;
                dbConnection.Parameters.AddWithValue("@name", obj.Name);
                dbConnection.Parameters.AddWithValue("@airTemperature", obj.AirTemperature);
                dbConnection.Parameters.AddWithValue("@waterTemperature", obj.WaterTemperature);
                dbConnection.Parameters.AddWithValue("@humidity", obj.Humidity);
                dbConnection.Parameters.AddWithValue("@pH", obj.PH);
                dbConnection.Parameters.AddWithValue("@conductivity", obj.Conductivity);
                dbConnection.Parameters.AddWithValue("@rgb", obj.RGB);
                dbConnection.Parameters.AddWithValue("@light", obj.Light);
                dbConnection.Parameters.AddWithValue("@addressImage", obj.AddressImage);
                db.Open();
                dbConnection.ExecuteNonQuery();
                db.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // ELIMINAZIONE DI UNA RICETTA DAL DB
        static public void setValues(string sqlQuery, int obj)
        {
            try
            {
                MySqlConnection db = new MySqlConnection(connectionString);
                MySqlCommand dbConnection = new MySqlCommand(sqlQuery, db);
                dbConnection.CommandText = sqlQuery;
                dbConnection.Parameters.AddWithValue("@idRecipe", obj);
                db.Open();
                dbConnection.ExecuteNonQuery();
                db.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // ELIMINAZIONE DEI PARAMETRI DAL DATABASE
        static public void setValues(string sqlQuery)
        {
            try
            {
                MySqlConnection db = new MySqlConnection(connectionString);
                MySqlCommand dbConnection = new MySqlCommand(sqlQuery, db);
                dbConnection.CommandText = sqlQuery;
                db.Open();
                dbConnection.ExecuteNonQuery();
                db.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
