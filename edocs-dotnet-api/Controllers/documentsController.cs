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

        string serverName = ConfigurationManager.AppSettings["EDOCSSERVERNAME"];

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

            var edocsGatway = new EdocsServerGateway(username,password,library,serverName);
            PCDCLIENTLib.PCDGetStream objPCDGetStream = edocsGatway.getDocument(docNumber);
            int nbytes = (int)objPCDGetStream.GetPropertyValue("%ISTREAM_STATSTG_CBSIZE_LOWPART");

            string fileType = edocsGatway.getFileType(docNumber);

            var fileName = docNumber + "." + fileType;

            var tempFileName = docNumber + "-" + DateTime.Now.ToString("yyMMddHHmmssfff") + "." + fileType;
            var filenamePath = tempFilePath + tempFileName;

            this.saveFileLocally(filenamePath,nbytes,objPCDGetStream);

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

        private Boolean saveFileLocally(string filename, int nbytes, PCDCLIENTLib.PCDGetStream objPCDGetStream)
        {
            Boolean status = false;
            try
            {
                using (Stream to = new FileStream(filename, FileMode.OpenOrCreate))
                {
                    int readCount;
                    byte[] buffer = new byte[nbytes];
                    buffer = (byte[])objPCDGetStream.Read(nbytes, out readCount);
                    to.Write(buffer, 0, readCount);
                }
                status = true;
            }catch (Exception e)
            {
                throw new Exception("Exception while saving file",e);
            }
            return status;
        }

    }
}

//private Boolean saveFileLocally(string filename, int nbytes, PCDCLIENTLib.PCDGetStream objPCDGetStream)
//{
//    Boolean status = false;
//    try
//    {
//        using (Stream to = new FileStream(filename, FileMode.OpenOrCreate))
//        {

//            int bytesRead;
//            objPCDGetStream.Read(1024, out bytesRead);

//            while (objPCDGetStream.ErrNumber == 0 && (bytesRead > 0))
//            {
//                byte[] buffer = objPCDGetStream.Read(1024, out bytesRead);
//                to.Write(buffer, 0, bytesRead);
//            }
//            objPCDGetStream.SetComplete();
//        }
//        status = true;
//    }
//    catch (Exception e)
//    {
//        throw new Exception("Exception while saving file", e);
//    }
//    return status;
//}