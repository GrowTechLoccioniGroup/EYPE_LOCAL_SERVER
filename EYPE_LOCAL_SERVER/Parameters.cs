using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EYPE_LOCAL_SERVER
{
    public class Parameters
    {
        private int _id_parameters; // primary key
        private DateTime _timeStamp; // date and time
        private float _airTemperature; // air temperature
        private float _waterTemperature; // water temperature
        private int _humidity; // humidity
        private float _pH; // pH solution
        private int _conductivity; // water conductivity

        // constructor
        public Parameters()
        {
            TimeStamp = Convert.ToDateTime("01/01/2000 00:00:00");
            AirTemperature = 0.0f;
            WaterTemperature = 0.0f;
            Humidity = 0;
            PH = 0.0f;
            Conductivity = 0;
        }

        // constructor with arguments
        public Parameters(DateTime timeStamp, float airTemperature, float waterTemperature, int humidity, float pH, int conductivity)
        {
            TimeStamp = timeStamp;
            AirTemperature = airTemperature;
            WaterTemperature = waterTemperature;
            Humidity = humidity;
            PH = pH;
            Conductivity = conductivity;
        }

        // constructor with arguments (+ overload)
        public Parameters(int idParameters, DateTime timeStamp, float airTemperature, float waterTemperature, int humidity, float pH, int conductivity)
        {
            id_parameters = idParameters;
            TimeStamp = timeStamp;
            AirTemperature = airTemperature;
            WaterTemperature = waterTemperature;
            Humidity = humidity;
            PH = pH;
            Conductivity = conductivity;
        }

        public int id_parameters
        {
            get
            {
                return _id_parameters;
            }
            set
            {
                _id_parameters = value;
            }
        }

        public DateTime TimeStamp
        {
            get
            {
                return _timeStamp;
            }
            set
            {
                _timeStamp = value;
            }
        }

        public float AirTemperature
        {
            get
            {
                return _airTemperature;
            }
            set
            {
                _airTemperature = value;
            }
        }

        public float WaterTemperature
        {
            get
            {
                return _waterTemperature;
            }
            set
            {
                _waterTemperature = value;
            }
        }

        public int Humidity
        {
            get
            {
                return _humidity;
            }
            set
            {
                _humidity = value;
            }
        }

        public float PH
        {
            get
            {
                return _pH;
            }
            set
            {
                _pH = value;
            }
        }

        public int Conductivity
        {
            get
            {
                return _conductivity;
            }
            set
            {
                _conductivity = value;
            }
        }
    }
}
