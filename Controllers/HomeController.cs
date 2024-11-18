using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CalculateSumMVC.Models;
using System.Data.SqlClient;
using System.Web;
//using System.Web.UI;

namespace CalculateSumMVC.Controllers;

public class HomeController : Controller
{
    public List<string> queryList = new List<string>();
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger){
        _logger = logger;
    }

    public IActionResult Index(){
        return View();
    }

    public IActionResult Privacy(){
        return View();
    }
    public IActionResult addition(){
            return View();
    }
    public IActionResult Info(){
            return View();
    }

    [HttpPost]
    public IActionResult add(){
        //get the sum
        int number1 = Convert.ToInt32(HttpContext.Request.Form["txtFirstNum"].ToString());
        int number2 = Convert.ToInt32(HttpContext.Request.Form["txtSecondNum"].ToString());
        int res = number1 + number2;
        ViewBag.Result = res.ToString();

        //set connection
        string connstring = "Data Source = localhost;Initial Catalog = TutorialDB; Integrated Security = true;";
        SqlConnection con = new SqlConnection(connstring);
        con.Open();

        //execute stored procedure for a new sum
        String query = "EXEC AddAddedValue @val = @inputVal";
        SqlParameter numParam = new SqlParameter("@inputVal", System.Data.SqlDbType.Int, 0);
        numParam.Value = res;
        SqlCommand cmd = new SqlCommand(query, con);
        cmd.Parameters.Add(numParam);
        cmd.Prepare();
        cmd.ExecuteNonQuery();
        
        //load all addition results
        query = "Select * from Addition";
        cmd = new SqlCommand(query, con);
        SqlDataReader reader = cmd.ExecuteReader();
            
        while(reader.Read()){
            queryList.Add("Output = " + reader.GetValue(0).ToString());
        }
        ViewBag.QueryResult = queryList;
        return View("addition");
    }
    [HttpPost]
    public IActionResult infoQuery(){
        string connstring = "Data Source = localhost;Initial Catalog = TutorialDB; Integrated Security = true;";
        SqlConnection con = new SqlConnection(connstring);
        con.Open();
        string query = "Select COUNT(*) from Customers";
        SqlCommand cmd = new SqlCommand(query, con);
        int  index = (int)cmd.ExecuteScalar() + 1;

            
        query = "INSERT INTO Customers VALUES (@id, @name, @country, @email)";
        SqlParameter numParam = new SqlParameter("@id", System.Data.SqlDbType.Int, 0);
        numParam.Value = index.ToString();
        SqlParameter nameParam = new SqlParameter("@name", System.Data.SqlDbType.NVarChar, 30);
        nameParam.Value = HttpContext.Request.Form["txtName"].ToString();
        SqlParameter countryParam = new SqlParameter("@country", System.Data.SqlDbType.NVarChar, 30);
        countryParam.Value = HttpContext.Request.Form["txtCountry"].ToString();
        SqlParameter emailParam = new SqlParameter("@email", System.Data.SqlDbType.NVarChar, 30);
        emailParam.Value = HttpContext.Request.Form["txtEmail"].ToString();

        cmd = new SqlCommand(query, con);
        cmd.Parameters.Add(numParam);
        cmd.Parameters.Add(nameParam);
        cmd.Parameters.Add(countryParam);
        cmd.Parameters.Add(emailParam);
        cmd.Prepare();
        cmd.ExecuteNonQuery();


        query = "Select * from Customers";
        cmd = new SqlCommand(query, con);
        SqlDataReader reader = cmd.ExecuteReader();
            
        while(reader.Read()){
            queryList.Add(string.Format("Output = {0:D}-{1:D}-{2:D}-{3:D}", reader.GetValue(0), reader.GetValue(1) , reader.GetValue(2) , reader.GetValue(3)));
        }
        ViewBag.QueryResult = queryList;
        return View("Info");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
