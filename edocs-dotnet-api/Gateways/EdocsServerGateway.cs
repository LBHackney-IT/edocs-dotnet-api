﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PCDCLIENTLib = Hummingbird.DM.Server.Interop.PCDClient;

namespace edocs_dotnet_api.Gatways
{
    public class EdocsServerGateway
    {
        public EdocsServerGateway(string username, string password, string library)
        {
            this.username = username;
            this.password = password;
            this.library = library;
            this.login();
        }

        public string username { get; private set; }
        public string password { get; private set; }
        public string library { get; private set; }
        public string dst { get; private set; }

        public string getFileType(string docNumber)
        {
            var sql = new PCDCLIENTLib.PCDSQL();
            sql.SetDST(dst);
            int rc = sql.Execute("SELECT PATH FROM DOCSADM.COMPONENTS WHERE DOCNUMBER = " + docNumber);

            if (rc != 0)
            {
                Console.WriteLine(sql.ErrDescription);
                throw new SystemException();
            }

            sql.SetRow(1);
            var path = sql.GetColumnValue(1);
            
            var tokens = path.Split('.');
            var fileType = tokens[tokens.Length - 1].ToLower();
            
            return fileType;
        }

        public void login()
        {
            var PCDLogin = new PCDCLIENTLib.PCDLogin();
            var rc = PCDLogin.AddLogin(0, library, username, password);

            rc = PCDLogin.Execute();

            if (rc != 0)
            {
                throw new SystemException();
            }

            this.dst = PCDLogin.GetDST();
            
        }

        public PCDCLIENTLib.PCDGetStream getDocument(string docNumber)
        {

            var obj = new PCDCLIENTLib.PCDSearch();
            obj.SetDST(dst);
            obj.AddSearchLib(library);
            obj.SetSearchObject("DEF_PROF");
            obj.AddSearchCriteria("DOCNUMBER", docNumber);
            obj.AddReturnProperty("PATH");
            obj.AddReturnProperty("DOCNAME");
            
        
            var rc = obj.Execute();
            if (rc != 0)
            {
                Console.WriteLine(obj.ErrDescription);
                throw new SystemException();
            }

            obj.SetRow(1);


            var docname = obj.GetPropertyValue("DOCNAME");
            
            obj.GetPropertyValue("PATH");
            obj.ReleaseResults();


            var sql = new PCDCLIENTLib.PCDSQL();
            sql.SetDST(dst);
            rc = sql.Execute("SELECT PATH FROM DOCSADM.COMPONENTS WHERE DOCNUMBER = " + docNumber);

            if (rc != 0)
            {
                Console.WriteLine(sql.ErrDescription);
                throw new SystemException();
            }

            sql.SetRow(1);
            var path = sql.GetColumnValue(1);
            Console.WriteLine("The path is:" + path);
            var tokens = path.Split('.');

            var fileType = tokens[tokens.Length - 1].ToLower();
            Console.WriteLine("File type:" + fileType);

            obj = new PCDCLIENTLib.PCDSearch();
            obj.SetDST(dst);
            obj.AddSearchLib(library);
            obj.SetSearchObject("cyd_cmnversions");
            obj.AddSearchCriteria("DOCNUMBER", docNumber);
            obj.AddOrderByProperty("VERSION", 0);
            obj.AddReturnProperty("VERSION");
            obj.AddReturnProperty("VERSION_ID");

            rc = obj.Execute();


            if (rc != 0)
            {
                Console.WriteLine(sql.ErrDescription);
                throw new SystemException();
            }

            obj.SetRow(1);
            var version = obj.GetPropertyValue("VERSION");
            var versionId = obj.GetPropertyValue("VERSION_ID");
            Console.WriteLine("Version: $version Version ID: " + versionId);
            obj.ReleaseResults();

            string ver = "" + versionId;

            var getobj = new PCDCLIENTLib.PCDGetDoc();
            getobj.SetDST(dst);
            getobj.AddSearchCriteria("%TARGET_LIBRARY", library);
            getobj.AddSearchCriteria("%VERSION_ID", ver);
            getobj.AddSearchCriteria("%DOCUMENT_NUMBER", docNumber);
            rc = getobj.Execute();

            if (rc != 0)
            {
                Console.WriteLine(getobj.ErrDescription);
                throw new SystemException();
            }

            getobj.NextRow();

            PCDCLIENTLib.PCDGetStream objPCDGetStream = (PCDCLIENTLib.PCDGetStream)getobj.GetPropertyValue("%CONTENT");
            return objPCDGetStream;
        }
    }
}