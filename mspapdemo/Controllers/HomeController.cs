using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using mspapdemo.Models;
using System.Data;
using System.Diagnostics;

namespace mspapdemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _configuration;

       

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost("submitOrder")]
        public IActionResult SubmitOrder([FromBody] OrderModel order)
        {
            // Connect to the Azure SQL Server
            string connStr = "Server=tcp:mspap-server.database.windows.net,1433;Initial Catalog=mspap;Persist Security Info=False;User ID=mspapadmin;Password=P@$$w0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            //using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                connection.Open();

                // Insert the order into the database
                using (SqlCommand command = new SqlCommand("INSERT INTO Orders (Size, Topping, CustomerName) VALUES (@Size, @Topping, @CustomerName)", connection))
                {
                    command.Parameters.AddWithValue("@Size", order.Size);
                    command.Parameters.AddWithValue("@Topping", order.Topping);
                    command.Parameters.AddWithValue("@CustomerName", order.CustomerName);

                    command.ExecuteNonQuery();
                }
            }
            
            return Ok();
        }


      //  [HttpGet("getReport")]
        public IActionResult Report()
        {

           // return View("Report");
            // Connect to the Azure SQL Server
            string connStr = "Server=tcp:mspap-server.database.windows.net,1433;Initial Catalog=mspap;Persist Security Info=False;User ID=mspapadmin;Password=P@$$w0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            DataTable dtOrders = new DataTable();
            List<OrderModel> Orders = new List<OrderModel>();
            OrderModel item = null;
            //using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            using (SqlConnection connection = new SqlConnection(connStr))
            {
                connection.Open();
                SqlDataAdapter adp = new SqlDataAdapter("select * from Orders", connStr);
                adp.Fill(dtOrders);
                if (dtOrders.Rows.Count > 0)
                {
                    // loop through datatable and populate orders model
                    foreach (DataRow row in dtOrders.Rows)
                    {
                        item = new OrderModel();
                        item.CustomerName = row["CustomerName"].ToString();
                        item.Topping = row["Topping"].ToString();
                        item.Size = row["Size"].ToString();
                        Orders.Add(item);

                    }
                }
            }

            return View("Report",Orders);
        }
    }

}

