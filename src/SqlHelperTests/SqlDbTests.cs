using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlHelper.Tests
{
    [TestClass()]
    public class SqlDbTests
    {
        [TestMethod()]
        public void SqlDbTest()
        {
            try
            {
                SqlDb db = null; //Your connection data
                Assert.IsTrue(db.IsConnected);
            }
            catch (Exception ex)
            {
                Assert.Fail();
            }
        }
    }
}