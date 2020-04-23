using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace edocs_dotnet_api.infrastructure
{
    public class APIConfiguration
    {
        public static string EDOCS_API_KEY
        {
            get { return ConfigurationManager.AppSettings["EDOCSAPIKEY"]; }
        }

        public static string EDOCS_LIBRARY
        {
            get { return ConfigurationManager.AppSettings["EDOCSLIBRARY"]; }
        }

        public static string EDOCS_TEMP_FILE_PATH
        {
            get { return ConfigurationManager.AppSettings["EDOCSTEMPFILEPATH"]; }
        }

        public static string EDOCS_USERNAME
        {
            get { return ConfigurationManager.AppSettings["EDOCSUSERNAME"]; }
        }

        public static string EDOCS_PASSWORD
        {
            get { return ConfigurationManager.AppSettings["EDOCSPASSWORD"]; }
        }

        public static string EDOCS_SERVERNAME
        {
            get { return ConfigurationManager.AppSettings["EDOCSSERVERNAME"]; }
        }

    }
}