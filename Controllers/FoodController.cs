using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using secondapp.DTOs;
using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

namespace secondapp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FoodController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string connString;
        private readonly ILogger<FoodController> _logger;

        public FoodController(IConfiguration configuration, ILogger<FoodController> logger)
        {
            _configuration = configuration;
            _logger = logger;
            var host = _configuration["DBHOST"] ?? "localhost";
            var port = _configuration["DBPORT"] ?? "1433";
            var password = _configuration["MSSQL_SA_PASSWORD"] ?? _configuration.GetConnectionString("MSSQL_SA_PASSWORD");
            var userid = _configuration["MSSQL_USER"] ?? _configuration.GetConnectionString("MSSQL_USER");
            var usersDataBase = _configuration["MSSQL_DATABASE"] ?? _configuration.GetConnectionString("MSSQL_DATABASE");
                        
            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";;
        }
        [HttpGet("GetAllFoods")]
        public async Task<ActionResult<List<FoodDto>>> GetAllFoods()
        {
            var foods = new List<FoodDto>();
            try
            {
                string query = @"SELECT * FROM Foods";
                using (var connection = new SqlConnection(connString))
                {
                    var result = await connection.QueryAsync<FoodDto>(query, CommandType.Text);
                    foods = result.ToList();
                }
                if (foods.Count > 0)
                {
                    return Ok(foods);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }

        [HttpPost("AddNewFood")]
        public async Task<ActionResult<FoodDto>> AddNewFood(FoodDto food)
        {
            var newFood = new FoodDto();
            try
            {
                string query = @"INSERT INTO Foods (Id,Name,Quantity) VALUES (@Id,@Name,@Quantity)";
                var param = new DynamicParameters();
                param.Add("@Id", food.Id);
                param.Add("@Name", food.Name);
                param.Add("@Quantity", food.Quantity);
                using (var connection = new SqlConnection(connString))
                {
                    var result = await connection.ExecuteAsync(query, param, null, null, CommandType.Text);
                    if (result > 0)
                    {
                        newFood = food;
                    }
                }
                if (newFood != null)
                {
                    return Ok(newFood);
                }
                else
                {
                    return BadRequest("Unable To  Food");
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Unable To Process Request");
            }
        }
    }
}