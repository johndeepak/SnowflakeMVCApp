using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SnowflakeMVCApp.Models;
using System.Web.UI.WebControls;
using Snowflake.Data.Client;
using System.Data;
using Newtonsoft.Json;

namespace SnowflakeMVCApp.Controllers
{
    public class HomeController : Controller
    {
        //string connectionString = "scheme=https;host=jo69808.us-central1.gcp.snowflakecomputing.com;ROLE=sysadmin;WAREHOUSE=John;user=johns;password=Water123;DB=JOHN;account=jo69808.us-central1.gcp.*;";

        string connectionString = "scheme=https;host=xo08461.europe-west2.gcp.snowflakecomputing.com;ROLE=sysadmin;user=johns1;password=Water123;DB=JOHN;account=xo08461.europe-west2.gcp.*;";
        public ActionResult Index()
        {
            List<Employee> Employeedata;

            Employeedata = ShowData();
            return View(Employeedata);
        }
        public List<Employee> ShowData()
        {
            List<Employee> EmployeeData = new List<Employee>();
            DataTable dt = new DataTable();
            SnowflakeDbConnection con = new SnowflakeDbConnection();
            con.ConnectionString = connectionString;
            con.Open();
            SnowflakeDbDataAdapter adapt = new SnowflakeDbDataAdapter("CALL Employee_Grid()", con);
            adapt.Fill(dt);


            if (dt.Rows.Count > 0)
            {
                string data = dt.Rows[0]["Employee_Grid"].ToString();

                EmployeeData = JsonConvert.DeserializeObject<List<Employee>>(data);

            }
            return EmployeeData;
        }
        public ActionResult AddOREdit(string id = "")
        {
            if (id == "")
            {
                return View();
            }
            else
            {
                Employee Employees = new Employee();
                List<Employee> response = GetData(id);
                foreach(var item in response)
                {
                    Employees.Firstname = item.Firstname;
                    Employees.Lastname = item.Lastname;
                    Employees.Address = item.Address;
                    Employees.Id = item.Id;
                }
                return View(Employees);
            }
        }
        public List<Employee> GetData(string id)
        {
            List<Employee> EmployeeData = new List<Employee>();
            // string connectionString1 = "scheme=https;host=jo69808.us-central1.gcp.snowflakecomputing.com;ROLE=sysadmin;WAREHOUSE=John;user=johns;password=Water123;DB=JOHN;account=jo69808.us-central1.gcp.*;";
            DataTable dt = new DataTable();
            SnowflakeDbConnection con = new SnowflakeDbConnection();
            con.ConnectionString = connectionString;
            con.Open();
            SnowflakeDbDataAdapter adapt = new SnowflakeDbDataAdapter("CALL Employee_Select('" + id + "')", con);
            adapt.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                string data = dt.Rows[0]["Employee_Select"].ToString();
               string Jsondata = data.Replace('\n'.ToString(), "");
                EmployeeData = JsonConvert.DeserializeObject<List<Employee>>(Jsondata);
            
            }
            return EmployeeData;
        }
        [HttpPost]
        public ActionResult AddOREdit(Employee mod)
        {
            string IDs = "";
            using (var conn = new SnowflakeDbConnection())
            {
                conn.ConnectionString = connectionString;
                conn.Open();
                var cmd = conn.CreateCommand();
                if (mod.Id == null)
                {
                    IDs = System.Guid.NewGuid().ToString();
                }
                else
                {
                    IDs = mod.Id;
                }
                cmd.CommandText = "call Employeesave('" + mod.Firstname + "','" + mod.Lastname + "','" + mod.Address + "','" + IDs + "')";
                var reader = cmd.ExecuteReader();

                conn.Close();
                ShowData();

                return RedirectToAction("Index");

            }
        }
        public ActionResult Delete(int id)
        {
            using (var conn = new SnowflakeDbConnection())
            {
                conn.ConnectionString = connectionString;
                conn.Open();
                var cmd = conn.CreateCommand();

                cmd.CommandText = "call Employee_Delete('" + id + "')";
                var reader = cmd.ExecuteReader();
                //Save.Text = "Save";

                conn.Close();
                ShowData();
            }
            return RedirectToAction("Index");

        }

    }

}
