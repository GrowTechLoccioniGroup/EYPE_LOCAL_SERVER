using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json.Serialization;
using Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Tracing;

namespace EYPE_LOCAL_SERVER
{
    public class Startup
    {
        IDisposable webServer;
        List<Assembly> assembliesWithControllers;
        string ethInterface;
        int tcpPort;
        string webAppFolder;
        private const string BASE_URL_FOR_REST = "/api";
        private const string BASE_URL_FOR_STATIC = "";

        //private List<string> assembliesToResolveForControllers = new List<string>();
        /// <summary>
        /// Adds an assembly file name where the server look for controllers.
        /// </summary>
        /// <param name = "controllersAsssemblyFileName" > Name of the controllers asssembly file.</param>
        //public void AddAssemblyFileNameForControllers(string controllersAsssemblyFileName)
        //{
        //    assembliesToResolveForControllers.Add(controllersAsssemblyFileName);
        //}

        /// <summary>
        /// Adds assembly file names where the server look for controllers.
        /// </summary>
        /// <param name="controllersAsssemblyFileNames">The controllers asssembly file names.</param>
        //public void AddAssemblyFileNamesForControllers(List<string> controllersAsssemblyFileNames)
        //{
        //    assembliesToResolveForControllers.AddRange(controllersAsssemblyFileNames);
        //}

        /// <summary>
        ///// Adds an assembly where the server look for controllers.
        ///// </summary>
        ///// <param name="controllerAssembly">The controller assembly.</param>
        //public void AddAssemblyForControllers(Assembly controllerAssembly)
        //{
        //    assembliesToResolveForControllers.Add(controllerAssembly.GetFileName());
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="Server"/> class.
        /// </summary>
        /// <param name="webAppFolder">The web application folder.</param>
        /// <param name="ethInterface">The eth interface.</param>
        /// <param name="tcpPort">The TCP port.</param>
        public Startup(string webAppFolder = "wwwroot", string ethInterface = "*", int tcpPort = 9000)
        {
            this.tcpPort = 9000;
            this.ethInterface = "*";
            this.webAppFolder = "wwwroot";
        }



        public void Configuration(IAppBuilder appBuilder)
        {
            appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            //appBuilder.Map(BASE_URL_FOR_REST, map =>
            //{
            HttpConfiguration config = new HttpConfiguration();
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            config.MapHttpAttributeRoutes();


            appBuilder.UseWebApi(config);
            //});

            // file system web hosting


            webAppFolder = System.IO.Path.Combine(Environment.CurrentDirectory, webAppFolder);
            if (webAppFolder != null && webAppFolder != "" && IsAValidPath(webAppFolder))
            {
                if (!System.IO.Directory.Exists(webAppFolder))
                    System.IO.Directory.CreateDirectory(webAppFolder);

                var fileServerOptions = new FileServerOptions();
                fileServerOptions.EnableDefaultFiles = true;
                fileServerOptions.FileSystem = new PhysicalFileSystem(webAppFolder);
                fileServerOptions.RequestPath = new Microsoft.Owin.PathString(BASE_URL_FOR_STATIC);
                fileServerOptions.StaticFileOptions.ServeUnknownFileTypes = true;
                fileServerOptions.StaticFileOptions.DefaultContentType = "text/plain";
                appBuilder.UseFileServer(fileServerOptions);
            }

            // Configure Web API for self-host. 
            //HttpConfiguration config = new HttpConfiguration();
            //appBuilder.UseWebApi(config);
            //config.MapHttpAttributeRoutes();


            //appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            ////appBuilder.UseCors()
            //const string rootFolder = "./wwwroot";
            //var fileSystem = new PhysicalFileSystem(rootFolder);
            //var options = new FileServerOptions
            //{
            //    EnableDefaultFiles = true,
            //    FileSystem = fileSystem

            //};

            //appBuilder.UseFileServer(options);
        }

        // This code configures Web API. 
        //public virtual void Configuration(IAppBuilder appBuilder)
        //{
        //    appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        //    appBuilder.Map(BASE_URL_FOR_REST, map =>
        //    {
        //        HttpConfiguration config = new HttpConfiguration();
        //        var cors = new EnableCorsAttribute("*", "*", "*"); 
        //        config.EnableCors(cors);

        //        config.MapHttpAttributeRoutes();


        //        map.UseWebApi(config);
        //    });

        //    // file system web hosting
        //    webAppFolder = "wwwroot";

        //    webAppFolder = System.IO.Path.Combine(Environment.CurrentDirectory, webAppFolder);
        //    if (webAppFolder != null && webAppFolder != "" && IsAValidPath(webAppFolder))
        //    {
        //        if (!System.IO.Directory.Exists(webAppFolder))
        //            System.IO.Directory.CreateDirectory(webAppFolder);

        //        var fileServerOptions = new FileServerOptions();
        //        fileServerOptions.EnableDefaultFiles = true;
        //        fileServerOptions.FileSystem = new PhysicalFileSystem(webAppFolder);
        //        fileServerOptions.RequestPath = new Microsoft.Owin.PathString(BASE_URL_FOR_STATIC);
        //        fileServerOptions.StaticFileOptions.ServeUnknownFileTypes = true;
        //        fileServerOptions.StaticFileOptions.DefaultContentType = "text/plain";
        //        appBuilder.UseFileServer(fileServerOptions);
        //    }
        //}

        public bool IsAValidPath(string path)
        {
            return (webAppFolder.IndexOfAny(System.IO.Path.GetInvalidPathChars()) == -1);
        }

        public string BaseAddress { get; private set; }

        public string WebServerMainPage { get; private set; }



        public void Start()
        {
            //log.Debug("Web interface initialization ...");

            // assembliesWithControllers = LoadAssemblies(assembliesToResolveForControllers);

            BaseAddress = $"http://{ethInterface}:{tcpPort}/";
            WebServerMainPage = $"http://localhost:{tcpPort}{BASE_URL_FOR_STATIC}";
            try
            {
                webServer = WebApp.Start(BaseAddress, this.Configuration);
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                webServer = null;
                return;
            }
            //log.Debug("WebServer ready on address: " + BaseAddress);
            //log.Debug("WebServer main page: " + WebServerMainPage);

            //EnableDebugTracing = false; // by default, after creation, log level of web server return to warn
        }

        public void Stop()
        {
            //EnableDebugTracing = true; // by default, during destruction, log level of web server return to debug
            // log.Debug("Web server dispose");
            if (webServer != null) webServer.Dispose();
            webServer = null;
            // EnableDebugTracing = false;
        }

        public bool IsActive
        {
            get
            {
                return webServer != null;
            }
        }


    }
}



//using Microsoft.Owin.FileSystems;
//using Microsoft.Owin.StaticFiles;
//using Owin;
//using System.Web.Http;


//namespace EYPE_LOCAL_SERVER
//{
//    public class Startup
//    {
//        // This code configures Web API. The Startup class is specified as a type
//        // parameter in the WebApp.Start method.
//        public void Configuration(IAppBuilder appBuilder)
//        {
//            appBuilder.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
//            // Configure Web API for self-host. 
//            HttpConfiguration config = new HttpConfiguration();
//            appBuilder.UseWebApi(config);
//            config.MapHttpAttributeRoutes();




//            //appBuilder.UseCors()
//            const string rootFolder = "./wwwroot";
//            var fileSystem = new PhysicalFileSystem(rootFolder);
//            var options = new FileServerOptions
//            {
//                EnableDefaultFiles = true,
//                FileSystem = fileSystem

//            };

//            appBuilder.UseFileServer(options);
//        }
//    }
//}