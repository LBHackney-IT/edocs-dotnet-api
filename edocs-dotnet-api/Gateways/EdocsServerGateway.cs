namespace edocs_dotnet_api.Gateways
{ 
    using System;
    using PCDCLIENTLib = Hummingbird.DM.Server.Interop.PCDClient;

    namespace edocs_dotnet_api.Gatways
    {
        public class EdocsServerGateway
        {
            public EdocsServerGateway(string username, string password, string library, string serverName)
            {
                this.username = username;
                this.password = password;
                this.library = library;
                this.serverName = serverName;
                this.login();
            }

            public string username { get; private set; }
            public string password { get; private set; }
            public string library { get; private set; }
            public string dst { get; private set; }
            public string serverName { get; private set; }

            public void login()
            {

                int rc = 1;
                PCDCLIENTLib.PCDLogin PCDLogin;

                try
                {
                    PCDLogin = new PCDCLIENTLib.PCDLogin();
                    rc = PCDLogin.AddLogin(0, library, username, password);
                }catch (Exception ex)
                {
                    throw new Exception("Exception while calling PCDLogin.AddLogin() API", ex);
                }

                try
                {
                    if ((null != serverName && (0 < serverName.Trim().Length)))
                        PCDLogin.SetServerName(serverName);

                    rc = PCDLogin.Execute();
                }
                catch(Exception ex)
                {
                    throw new Exception("Exception while calling PCDLogin.Execute() API", ex);
                }

                if (rc != 0)
                {
                    throw new Exception("Login failed : " + PCDLogin.ErrNumber + PCDLogin.ErrDescription);
                }

                this.dst = PCDLogin.GetDST();

            }

            public string getFileType(string docNumber)
            {
                var sql = new PCDCLIENTLib.PCDSQL();
                sql.SetDST(dst);
                int rc = sql.Execute("SELECT PATH FROM DOCSADM.COMPONENTS WHERE DOCNUMBER = " + docNumber);

                if (rc != 0)
                {
                    throw new Exception("Failed to get File Type : " + sql.ErrNumber + sql.ErrDescription);
                }

                sql.SetRow(1);
                var path = sql.GetColumnValue(1);

                var tokens = path.Split('.');
                var fileType = tokens[tokens.Length - 1].ToLower();

                return fileType;
            }

            public string getDocName(string docNumber)
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
                    throw new Exception("Failed to get Document Name : " + obj.ErrNumber + obj.ErrDescription);
                }

                obj.SetRow(1);
                var docname = obj.GetPropertyValue("DOCNAME");
                obj.GetPropertyValue("PATH");
                obj.ReleaseResults();

                return docname;
            }

            public string getVersionId(string docNumber)
            {
                var obj = new PCDCLIENTLib.PCDSearch();
                obj.SetDST(dst);
                obj.AddSearchLib(library);
                obj.SetSearchObject("cyd_cmnversions");
                obj.AddSearchCriteria("DOCNUMBER", docNumber);
                obj.AddOrderByProperty("VERSION", 0);
                obj.AddReturnProperty("VERSION");
                obj.AddReturnProperty("VERSION_ID");

                var rc = obj.Execute();

                if (rc != 0)
                {
                    Console.WriteLine(obj.ErrDescription);
                    throw new SystemException();
                }

                obj.SetRow(1);
                var version = obj.GetPropertyValue("VERSION");
                var versionId = obj.GetPropertyValue("VERSION_ID");
                Console.WriteLine("Version: $version Version ID: " + versionId);
                obj.ReleaseResults();

                //string ver = "" + versionId;
                return versionId;
            }

            public PCDCLIENTLib.PCDGetStream getDocument(string docNumber)
            {

                var docname = this.getDocName(docNumber);
                if (docname == null)
                {
                    return null;
                }

                var fileType = this.getFileType(docNumber);

                string versionId = this.getVersionId(docNumber);

                var getobj = new PCDCLIENTLib.PCDGetDoc();
                getobj.SetDST(dst);
                getobj.AddSearchCriteria("%TARGET_LIBRARY", library);
                getobj.AddSearchCriteria("%VERSION_ID", versionId);
                getobj.AddSearchCriteria("%DOCUMENT_NUMBER", docNumber);
                var rc = getobj.Execute();

                if (rc != 0)
                {
                    throw new Exception(getobj.ErrDescription);
                }

                getobj.NextRow();

                PCDCLIENTLib.PCDGetStream objPCDGetStream = (PCDCLIENTLib.PCDGetStream)getobj.GetPropertyValue("%CONTENT");
                return objPCDGetStream;
            }
        }
    }
}