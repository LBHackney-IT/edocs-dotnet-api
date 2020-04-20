using System;
using System.IO;
using PCDCLIENTLib = Hummingbird.DM.Server.Interop.PCDClient;
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using edocs_dotnet_api.Gatways;
using System.Web;
using Microsoft.Win32.SafeHandles;
using System.Net;

namespace edocs_dotnet_api.Controllers
{
    public class documentsController : ApiController
    {

        string apiKey = ConfigurationManager.AppSettings["EDOCSAPIKEY"];
        string library = ConfigurationManager.AppSettings["EDOCSLIBRARY"];
        string tempFilePath = ConfigurationManager.AppSettings["EDOCSTEMPFILEPATH"];

        string username = ConfigurationManager.AppSettings["EDOCSUSERNAME"];
        string password = ConfigurationManager.AppSettings["EDOCSPASSWORD"];

        private string getAuthorizationToken()
        {
            var headers = Request.Headers;
            if (headers.Contains("Authorization"))
            {
                return Request.Headers.Authorization.ToString().Substring(7);
            }
            return "";
        }

        // GET api/<controller>/id
        public HttpResponseMessage Get(int id)
        {
            if (!this.getAuthorizationToken().Equals(apiKey))
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }
            
            var docNumber = id.ToString();            

            var edocsGatway = new EdocsServerGateway(username,password,library);
            PCDCLIENTLib.PCDGetStream objPCDGetStream = edocsGatway.getDocument(docNumber);
            int nbytes = (int)objPCDGetStream.GetPropertyValue("%ISTREAM_STATSTG_CBSIZE_LOWPART");

            string fileType = edocsGatway.getFileType(docNumber);

            var fileName = docNumber + "." + fileType;

            var tempFileName = docNumber + "-" + DateTime.Now.ToString("yyMMddHHmmssfff") + "." + fileType;
            var filenamePath = tempFilePath + tempFileName;

            using (Stream to = new FileStream(filenamePath, FileMode.OpenOrCreate))
            {
                int readCount;
                byte[] buffer = new byte[nbytes];
                buffer = (byte[])objPCDGetStream.Read(nbytes, out readCount);
                to.Write(buffer, 0, readCount);
            }

            var stream = new FileStream(filenamePath, FileMode.Open, FileAccess.Read);
            
            var response = new FileHttpResponseMessage(filenamePath);
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(System.Web.MimeMapping.GetMimeMapping(fileName));
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = fileName
                };
            return response;
                
        }

    }
}