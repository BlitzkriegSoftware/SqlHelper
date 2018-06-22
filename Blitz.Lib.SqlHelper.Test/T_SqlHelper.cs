namespace Blitz.Lib.SqlHelper.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Blitz.Lib.SqlHelper;
    using System.Data.SqlClient;
    using System.Collections.Generic;
    using System.Data;

    [TestClass]
    public class T_SqlHelper
    {
        #region "BoilerPlate"
        public TestContext TestContext { get; set; }
        #endregion

        #region "Helpers"

        private string MakeConnectionString()
        {
            var cs =  string.Format(@"Server=.\SQLExpress;AttachDbFilename={0}\Zoo.mdf;Database=Zoo;Trusted_Connection = Yes; ", System.IO.Directory.GetCurrentDirectory());
            return cs;
        }

        #endregion

        #region "SP Tests"

        [TestMethod]
        public void T_ExecuteStoredProcedureWithDataTable_1()
        {
            SqlParameter p = SqlHelper.ParameterBuilder("@atLeastThisMany", System.Data.SqlDbType.Int, 10);

            var paras = new List<SqlParameter>()
            {
                p
            };

            paras = SqlHelper.CleanParameters(paras);

            var dt = SqlHelper.ExecuteStoredProcedureWithDataTable(MakeConnectionString(), "pAnimalEnclosures", paras);

            Assert.IsTrue(SqlHelper.HasRows(dt));
        }

        [TestMethod]
        public void T_ExecuteStoredProcedureWithNoReturn_1()
        {
            var paras = new List<SqlParameter>();
            SqlHelper.ExecuteStoredProcedureWithNoReturn(MakeConnectionString(), "pAnimalCount", paras);
        }

        [TestMethod]
        public void T_ExecuteStoredProcedureWithParametersToScaler_1()
        {
            var paras = new List<SqlParameter>();
            var ct = SqlHelper.ExecuteStoredProcedureWithParametersToScaler<int>(MakeConnectionString(), "pAnimalCount", paras);
            Assert.IsTrue((ct > 0));
        }

        #endregion

        #region "SQL Tests"

        [TestMethod]
        public void T_ExecuteSqlWithParametersToScaler_1()
        {
            var paras = new List<SqlParameter>();
            var sql = "select count(1) from [Animal]";
            var data = SqlHelper.ExecuteSqlWithParametersToScaler<int>(MakeConnectionString(), sql, paras);
            Assert.IsNotNull(data);
        }

        [TestMethod]
        public void T_ExecuteSqlWithParametersToDataTable_1()
        {
            var paras = new List<SqlParameter>();
            var sql = "select * from [Animal]";
            var dt = SqlHelper.ExecuteSqlWithParametersToDataTable(MakeConnectionString(), sql, paras);
            Assert.IsTrue(SqlHelper.HasRows(dt));
        }

        [TestMethod]
        public void T_ExecuteSqlWithParametersNoReturn_1()
        {
            var paras = new List<SqlParameter>();
            var sql = "select count(1) from [Animal]";
            SqlHelper.ExecuteSqlWithParametersNoReturn(MakeConnectionString(), sql, paras);
        }

        #endregion

        #region "Misc."

        [TestMethod]
        public void T_FixPara_1()
        {
            var inText = "Foo, Bar, Moo,";
            var expected = "\"Foo\",\"Bar\",\"Moo\"";
            var outText = SqlHelper.FixSqlInText(inText, string.Empty, '"');
            Assert.AreEqual(expected, outText);
        }

        [TestMethod]
        public void T_FixPara_2()
        {
            var inText = "Foo, Bar, Moo,";
            var expected = "\"Foo\",\"Bar\",\"Moo\"";
            var outText = SqlHelper.FixSqlInText(string.Empty, inText, '"');
            Assert.AreEqual(expected, outText);
        }

        [TestMethod]
        public void T_CleanParameters_1()
        {
            SqlParameter p = SqlHelper.ParameterBuilder("@textPara", System.Data.SqlDbType.NVarChar, "Pete's");

            var paras = new List<SqlParameter>()
            {
                p
            };

            paras = SqlHelper.CleanParameters(paras);

            var p2 = paras[0];

            Assert.AreEqual(p2.Value, "Pete''s");
        }

        [TestMethod]
        public void T_HasTables_1()
        {
            var dt = new DataTable();
            var ds = new DataSet();
            ds.Tables.Add(dt);
            Assert.IsTrue(SqlHelper.HasTables(ds));
        }

        #endregion

    }
}
