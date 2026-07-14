using Finance.Api.Application.DTOs.Category;
using Finance.Api.Models;
using Finance.Api.Data;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Finance.Api.Application.Interfaces;

namespace Finance.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly FinanceDbContext _context;
    private readonly ICategoryService _categoryService;

    public CategoriesController(FinanceDbContext context , ICategoryService categoryService)
    {
        _context = context;
        _categoryService=categoryService;
    }

    //------------------------------------------------------------------------------------------------------------------------------------
    //create category endpoint

    [HttpPost("create")]
    public async Task<IActionResult> CreateCategory(CreateCategoryDto request)
    {
        var userId = User.GetUserId();
        return CreatedAtAction(nameof(CreateCategory), _categoryService.CreateCategoryAsync(userId,request));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //get all categories for the user

    [HttpGet("get")]
    public async Task<IActionResult> GetCategories()
    {
        var userId = User.GetUserId();
        return Ok(_categoryService.GetCategoriesAsync(userId));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //update category endpoint

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryDto request)
    {
        var userId = User.GetUserId();
        return Ok(_categoryService.UpdateCategoryAsync(userId,id,request));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //delete category endpoint

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var userId = User.GetUserId();
        return Ok(_categoryService.DeleteCategoryAsync(userId,id));
    }
}