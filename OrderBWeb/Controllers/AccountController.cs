using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OrderBWeb.Controllers
{
    public class AccountController : Controller
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["WebTestconnect"].ConnectionString;

        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session["UserId"] = null;
            return RedirectToAction("Login");
        }

        public string RegisterSubmit(string Username,string account,string password)
        {
            string userid = getuserid();
            if (userid == "")
            {
                return "";
            }
            string sql_text = @"
                                                INSERT INTO UserData
                                                VALUES (@userid, @Username, @account,@password);
                                                ";

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand(sql_text, conn))
                {
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.Parameters.AddWithValue("@Username", Username);
                    cmd.Parameters.AddWithValue("@account", account);
                    cmd.Parameters.AddWithValue("@password", password);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return "ok";
        }

        public string getuserid()
        {
            string userid = "";
            string sql_text = @"SELECT Right('000' + Cast((CONVERT(INT,MAX(UserId))+1) as varchar),3) as UserId  FROM [UserData]";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand(sql_text, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    userid = dr["UserId"].ToString();
                }
            }

            return userid;
        }

        public ActionResult LoginSubmit(string account, string password)
        {
            string sql_text = @"SELECT UserData.UserId
                                            FROM UserData
                                            WHERE account =@account AND password=@password
                                            ";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand(sql_text, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@account", account);
                        da.SelectCommand.Parameters.AddWithValue("@password", password);
                        da.Fill(dt);
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    Session["UserId"] = dr["UserId"].ToString();
                    return RedirectToAction("OrderList", "Order");
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
        }
    }
}