using dataposting.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace dataposting.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IHttpActionResult Get()
        {
            string dbConnection = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
            SqlCommand cmd = new SqlCommand("Sp_get_personName");
            cmd.CommandType = CommandType.StoredProcedure;
            SqlConnection connec = new SqlConnection(dbConnection);

            cmd.Connection = connec;
            DataTable table = new DataTable();
            if (table == null)
            {

            }
            else
            {
                try
                {
                    using (SqlDataAdapter a=new SqlDataAdapter(cmd))
                    {
                        a.Fill(table);
                    }
                }
                catch(Exception e)
                {

                }
            }
            return Ok(table);
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
       public IHttpActionResult Post([FromBody] List<Person> persons)
        {

            string dbConnection = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
            SqlCommand cmd = new SqlCommand("Sp_Ins_PersonData");
            var person = persons[0];
            foreach (var data in persons)
            {
                
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", data.Name);
                cmd.Parameters.AddWithValue("@Age", data.Age);
                SqlConnection connec = new SqlConnection(dbConnection);

                cmd.Connection = connec;

                connec.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {

                }
                finally
                {
                    connec.Close();
                    cmd.Parameters.Clear();
                }
            }
            return Ok(1);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
