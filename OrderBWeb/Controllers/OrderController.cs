using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OrderBWeb.ActionFilter;

namespace BookOrderWeb.Controllers
{
    public class OrderController : Controller
    {
        public string ConnectionString = ConfigurationManager.ConnectionStrings["WebTestconnect"].ConnectionString;

        // GET: Order
        [OrderBWeb.ActionFilter.LoginStatusCheck]
        public ActionResult OrderList()
        {
            var id = Session["UserId"];
            if (id == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string userid = Session["UserId"].ToString();
            //userid = "001"; //尚未做登入先預設001
            string sql_text = @"SELECT OrderList.OrderId,OrderList.OrderItem,ItemData.Itemname, 
                                                            OrderList.Price,OrderList.Cost,StatusData.StatusName
                                            FROM OrderList
                                            LEFT JOIN ItemData ON ItemData.ItemId = OrderList.OrderItem
                                            LEFT JOIN StatusData ON StatusData.Status = OrderList.Status
                                            WHERE OrderList.UserId =@userid
                                            ";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand(sql_text, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@userid", userid);
                        da.Fill(dt);
                    }
                }
                ViewBag.OrderList = dt;
            }
            return View();
        }

        public ActionResult Detail(string OrderItem)
        {
            string sql_text = @"SELECT *
                                            FROM ItemData
                                            WHERE ItemData.ItemId =@ItemId
                                            ";
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                DataTable dt = new DataTable();
                using (SqlCommand cmd = new SqlCommand(sql_text, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@ItemId", OrderItem);
                        da.Fill(dt);
                    }
                }
                ViewBag.ItemData = dt;
            }
            return View();
        }

        [HttpPost]
        public string Confirm(string[] OrderId)
        {
            if (!IsNullOrEmpty(OrderId))
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < OrderId.Length; i++)
                {
                    sb.Append("'" + OrderId[i] + "',");
                }
                string A = sb.ToString().TrimEnd(',');
                string sql_text = string.Format("UPDATE OrderList SET Status='C' WHERE OrderId IN ({0}) ", A);

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
                }
                return "ok";
            }
            else
            {
                Response.StatusCode = 400;
                return "";
            }
        }

        public ActionResult ItemData()
        {
            string sql_text = @"SELECT *
                                            FROM ItemData
                                            ";
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
                ViewBag.ItemData = dt;
            }
            return View();
        }

        public bool IsNullOrEmpty(string[] myStringArray)
        {
            return myStringArray == null || myStringArray.Length < 1;
        }
    }
}