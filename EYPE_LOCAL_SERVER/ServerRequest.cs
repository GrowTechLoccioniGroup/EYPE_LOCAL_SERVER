using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace EYPE_LOCAL_SERVER
{
    static class ServerRequest
    {
        static string url;

        static async public void PostRequest(List<Parameters> listParameters)
        {
            using (var client = new HttpClient())
            {
                url = "http://classe-virtuale.loccioni.com:9000/api/values/parameters";

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP POST
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(url, listParameters.ToArray());
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        static async public void PutRequest(Options settings)
        {
            using (var client = new HttpClient())
            {
                url = "http://classe-virtuale.loccioni.com:9000/api/values/options"; // url destinazione

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP PUT
                try
                {
                    HttpResponseMessage response = await client.PutAsJsonAsync(url, settings);
                }
                catch(Exception ex)
                {
                    //throw new Exception(ex.Message);
                }
            }
        }

        static async public void DeleteRequest()
        {
            using (var client = new HttpClient())
            {
                url = "http://classe-virtuale.loccioni.com:9000/api/recipes/delete/";
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP POST
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync(url);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        static async public void PostRequest(List<Recipe> listRecipes)
        {
            using (var client = new HttpClient())
            {
                url = "http://classe-virtuale.loccioni.com:9000/api/recipes/new";

                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP POST
                try
                {
                    HttpResponseMessage response = await client.PostAsJsonAsync(url, listRecipes.ToArray());
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        static async public void DeleteRequest(int idDel)
        {
            using (var client = new HttpClient())
            {
                url = "http://classe-virtuale.loccioni.com:9000/api/recipes/delete/" + idDel;
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // HTTP POST
                try
                {
                    HttpResponseMessage response = await client.DeleteAsync(url);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
    }
}
