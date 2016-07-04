using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EYPE_LOCAL_SERVER
{
    public class Recipe
    {
        private int _id_recipe; // id recipe
        private string _name; // description
        private float _airTemperature; // air temperature
        private float _waterTemperature; // water temperature
        private int _humidity; // humidity
        private float _pH; // pH solution
        private int _conductivity; // water conductivity
        private string _rgb; // rgb values
        private bool _light; // true=on;false=off
        private string _addressImage; // string contenente l'indirizzo relativo dell'immagine

        // constructor
        public Recipe()
        { }

        // constructor with argument
        public Recipe(string name, float airTemperature, float waterTemperature, int humidity, float pH, int conductivity, string rgb, bool light, string addressImage)
        {
            Name = name;
            AirTemperature = airTemperature;
            WaterTemperature = waterTemperature;
            Humidity = humidity;
            PH = pH;
            Conductivity = conductivity;
            RGB = rgb;
            Light = light;
            AddressImage = addressImage;
        }

        // constructor with argument (+ overload)
        public Recipe(int idRecipe, string name, float airTemperature, float waterTemperature, int humidity, float pH, int conductivity, string rgb, bool light, string addressImage)
        {
            id_recipe = idRecipe;
            Name = name;
            AirTemperature = airTemperature;
            WaterTemperature = waterTemperature;
            Humidity = humidity;
            PH = pH;
            Conductivity = conductivity;
            RGB = rgb;
            Light = light;
            AddressImage = addressImage;
        }

        public int id_recipe
        {
            get
            {
                return _id_recipe;
            }
            set
            {
                _id_recipe = value;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
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

        public string RGB
        {
            get
            {
                return _rgb;
            }
            set
            {
                _rgb = value;
            }
        }

        public bool Light
        {
            get
            {
                return _light;
            }
            set
            {
                _light = value;
            }
        }

        public string AddressImage
        {
            get
            {
                return _addressImage;
            }
            set
            {
                _addressImage = value;
            }
        }
    }
}
