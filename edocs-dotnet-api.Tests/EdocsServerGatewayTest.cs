using System;
using edocs_dotnet_api.Gatways;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using PCDCLIENTLib = Hummingbird.DM.Server.Interop.PCDClient;
using Hummingbird.DM.Server.Interop.PCDClient;

namespace edocs_dotnet_api.Tests
{
    [TestClass]
    
    public class EdocsServerGatewayTest
    {
        public class PCDLoginSpy : PCDCLIENTLib.IPCDLogin
        {
            public int AddLogin(short iNetworkType, string zUnitName, string zUserName, string zPassword)
            {
                this.AddLoginWasCalled = true;
                return 0;
            }

            public int SetDST(string zDST)
            {
                throw new NotImplementedException();
            }

            public int Execute()
            {
                throw new NotImplementedException();
            }

            public PCDNetAliasList GetAliasList()
            {
                throw new NotImplementedException();
            }

            public string GetDOCSUserName()
            {
                throw new NotImplementedException();
            }

            public string GetDST()
            {
                throw new NotImplementedException();
            }

            public PCDNetAliasList GetFailedLoginList()
            {
                throw new NotImplementedException();
            }

            public string GetLoginLibrary()
            {
                throw new NotImplementedException();
            }

            public string GetPrimaryGroup()
            {
                throw new NotImplementedException();
            }

            public string ErrDescription => throw new NotImplementedException();

            public int ErrNumber => throw new NotImplementedException();

            public bool AddLoginWasCalled { get; private set; }
        }


        [TestMethod]
        public void CanInitialiseNewGateway()
        {
            //const string username = "Someuser";
            //const string password = "Somepass";
            //PCDLoginSpy login = new PCDLoginSpy();

            //var edocsGatway = new EdocsServerGateway(username: username, password: password, pcdLogin: login);

            //edocsGatway.username.Should().Be("Someuser");
            //edocsGatway.password.Should().Be("Somepass");

            //login.AddLoginWasCalled.Should().BeTrue();
        }
    }
}
