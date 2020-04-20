using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace edocs_dotnet_api.Controllers
{
    public class FileHttpResponseMessage : HttpResponseMessage
    {
        private string filePath;
        public FileHttpResponseMessage(string filePath)
        {
            this.filePath = filePath;
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Content.Dispose();
            File.Delete(filePath);
        }
    }
}