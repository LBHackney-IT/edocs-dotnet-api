﻿using System.IO;
using System.Net.Http;

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