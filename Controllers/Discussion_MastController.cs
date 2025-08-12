using DocumentFormat.OpenXml.Drawing;
using ECampusDiscussion.EntityData;
using ECampusDiscussion.JwtAuth;
using ECampusDiscussion.Models;
using Job_Vritta.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace QA_Backend.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class Discussion_MastController(MyDbContext db) : ControllerBase
  {
    readonly private MyDbContext db = db;

    [HttpGet]

    [Authorize]

    public IActionResult GetDiscussion_Mast([FromQuery] string? text, [FromQuery] string? lazyParams)
    {
      try
      {
        int first = 0, rows = 10;
        string sortField = "Id";
        int sortOrder = -1;
        if (!string.IsNullOrEmpty(lazyParams))

        {

          var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
          var parsed = System.Text.Json.JsonSerializer.Deserialize<LazyParams>(lazyParams, options);
          if (parsed != null)

          {
            first = parsed.First;
            rows = parsed.Rows;
            sortField = parsed.SortField ?? "Id";
            sortOrder = parsed.SortOrder ?? -1;
          }

        }
        var query = (from d in db.Discussion_Mast
                     join um in db.User_Details on d.Created_By_Id equals um.Login_ID
                     select new
                     {
                       Id=d.Id,
                       Discussion = d.Discussion,
                       Created_At = d.Created_At,
                       Created_By_Id = d.Created_By_Id,
                       createdByName = um.User_Name,
                       Discussion_Title=d.Discussion_Title

                     }).AsQueryable();

       // var query = db.Discussion_Mast.AsQueryable();
        if (!string.IsNullOrEmpty(text))
        {
          query = query.Where(r => r.Discussion.Contains(text));
        }
        var totalRecords = query.Count();
        query = query.OrderByDynamic(sortField, sortOrder == 1);
        var result = query.Skip(first).Take(rows).ToList();
        return Ok(new { discussion = result, count = totalRecords });

      }

      catch (Exception ex)

      {

        return StatusCode(500, new { error = ex.Message });

      }

    }


    [HttpPost]
    [Authorize]
    public IActionResult AddDiscussion_Mast(Discussion_Mast discussion)
    {
      try
      {

        var token = Request.Headers["Authorization"].ToString();
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token.Split(" ")[1]);
        int employeeId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "EmployeeID").Value);

        discussion.Created_By_Id = employeeId;
        discussion.Created_At = DateTime.Now;
        db.Discussion_Mast.Add(discussion);

          db.SaveChanges();
          return Ok(new { discussion });

      }

      catch (Exception ex)

      {

        return BadRequest(ex.Message);

      }

    }


    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
      try
      {
        var discussion = db.Discussion_Mast.SingleOrDefault(r => r.Id == id);
        if (discussion == null)
        {
          return NotFound(new { error = " Category not found" });
        }
        db.Discussion_Mast.Remove(discussion);
        db.SaveChanges();

        return Ok(new { message = " Category deleted successfully" });
      }
      catch (Exception ex)
      {
        return StatusCode(500, new { error = ex.Message });
      }
    }


  }
   
}
