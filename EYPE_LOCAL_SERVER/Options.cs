using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EYPE_LOCAL_SERVER
{
    public class Options
    {
        private int _id_options; // options primary key
        private string _rgb; // rgb values
        private bool _light; // true=on;false=off
        private string _addressImage; // string contenente l'indirizzo relativo dell'immagine

        // constructor
        public Options()
        {
            RGB = "";
            Light = true;
            AddressImage = "";
        }

        // constructor with arguments
        public Options(string rgb, bool light, string addressImage)
        {
            RGB = rgb;
            Light = light;
            AddressImage = addressImage;
        }

        // constructor with arguments (+ overload)
        public Options(int idOptions, string rgb, bool light)
        {
            id_options = idOptions;
            RGB = rgb;
            Light = light;
        }

        // constructor with arguments (+2 overload)
        public Options(string rgb, bool light)
        {
            RGB = rgb;
            Light = light;
        }

        public int id_options
        {
            get
            {
                return _id_options;
            }
            set
            {
                _id_options = value;
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
