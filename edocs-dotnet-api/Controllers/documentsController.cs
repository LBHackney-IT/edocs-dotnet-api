using System;
using System.IO;
using PCDCLIENTLib = Hummingbird.DM.Server.Interop.PCDClient;
using System.Web.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Configuration;
using System.Net;
using edocs_dotnet_api.Gateways.edocs_dotnet_api.Gatways;
using edocs_dotnet_api.infrastructure;

namespace edocs_dotnet_api.Controllers
{
    public class DocumentsController : ApiController
    {

        private string getAuthorizationToken()
        {
            var headers = Request.Headers;
            if (headers.Contains(HttpRequestHeader.Authorization.ToString()))
            {
                return Request.Headers.Authorization.ToString().Substring(7);
            }
            return "";
        }

        private Boolean isAuthenticated()
        {
            if (this.getAuthorizationToken().Equals(APIConfiguration.EDOCS_API_KEY))
            {
                return true;
            }
            return false;
        }

        // GET api/<controller>/id
        public HttpResponseMessage Get(int id)
        {

            if (!isAuthenticated())
            {
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            }

            var docNumber = id.ToString();

            var edocsGatway = new EdocsServerGateway(APIConfiguration.EDOCS_USERNAME, APIConfiguration.EDOCS_PASSWORD, APIConfiguration.EDOCS_LIBRARY, APIConfiguration.EDOCS_SERVERNAME);
            
            PCDCLIENTLib.PCDGetStream objPCDGetStream = edocsGatway.getDocument(docNumber);

            if (objPCDGetStream == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }


            int nbytes = (int)objPCDGetStream.GetPropertyValue("%ISTREAM_STATSTG_CBSIZE_LOWPART");

            string fileType = edocsGatway.getFileType(docNumber);

            var fileName = docNumber + "." + fileType;

            var tempFileName = docNumber + "-" + DateTime.Now.ToString("yyMMddHHmmssfff") + "." + fileType;
            var filenamePath = APIConfiguration.EDOCS_TEMP_FILE_PATH + tempFileName;

            this.saveFileLocally(filenamePath, nbytes, objPCDGetStream);

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
            }
            catch (Exception e)
            {
                throw new Exception("Exception while saving file", e);
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
