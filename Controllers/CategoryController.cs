using DotnetStockAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using StockAPI.Models;

namespace DotnetStockAPI.Controllers;

// [Authorize(Roles = UserRolesModel.Admin + "," + UserRolesModel.Manager)]
[Authorize]
[ApiController]
[Route("api/[controller]")]
[EnableCors("MultipleOrigins")]
public class CategoryController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<Category> GetCategories()
    {
        var category = _context.categories.ToList();

        return Ok(category);
    }

    [HttpGet("{id}")]
    public ActionResult<Category> GetCategories(int id)
    {
        var category = _context.categories.Find(id);

        if (category == null)
        {
            return StatusCode(
               StatusCodes.Status500InternalServerError,
               new ResponseModel
               {
                   Status = "Error",
                   Message = "ไอ้ความไม่มีไอดีนี้"
               }
           );
        }
        return Ok(category);
    }


    [HttpPost]

    public ActionResult<Category> AddCategory([FromBody] Category category)
    {
        _context.categories.Add(category);
        _context.SaveChanges();
        return Ok(category);
    }


    [HttpPut("{id}")]
    public ActionResult<Category> UpdateCategory(int id, [FromBody] Category category)
    {
        var cat = _context.categories.Find(id);
        if (cat == null)
        {
            return StatusCode(
               StatusCodes.Status500InternalServerError,
               new ResponseModel
               {
                   Status = "Error",
                   Message = "มึงเป็นเหี้ยอะไร ไอห่าาา อย่าให้กูเจอนะ ไอ้ควาย"
               }
           );
        }
        cat.categoryname = category.categoryname;
        cat.categorystatus = category.categorystatus;

        _context.SaveChanges();

        return Ok(cat);


    }


    [HttpDelete("{id}")]
    public ActionResult<Category> UpdateCategory(int id)
    {
        var cat = _context.categories.Find(id);
        if (cat == null)
        {
            return StatusCode(
               StatusCodes.Status500InternalServerError,
               new ResponseModel
               {
                   Status = "Error",
                   Message = "มึงเป็นเหี้ยอะไร ไอห่าาา อย่าให้กูเจอนะ ไอ้ควาย"
               }
           );
        }
        _context.categories.Remove(cat);
        _context.SaveChanges();

        return Ok(cat);


    }


}