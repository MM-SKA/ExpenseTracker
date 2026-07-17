using Finance.Api.Application.DTOs.Categories;
using Finance.Api.Domain.Entities;
using Finance.Api.Infrastructure.Data;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Finance.Api.Application.Interfaces;

namespace Finance.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly FinanceDbContext _context;
    private readonly ICategoryService _categoryService;

    public CategoriesController(FinanceDbContext context, ICategoryService categoryService)
    {
        _context = context;
        _categoryService = categoryService;
    }

    //------------------------------------------------------------------------------------------------------------------------------------
    //create category endpoint

    [HttpPost("create")]
    public async Task<IActionResult> CreateCategoryAsync(CreateCategoryDto request)
    {
        var userId = User.GetUserId();
        return CreatedAtAction(nameof(CreateCategoryAsync), _categoryService.CreateCategoryAsync(userId, request));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //get all categories for the user

    [HttpGet("get")]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        var userId = User.GetUserId();
        return Ok(_categoryService.GetCategoriesAsync(userId));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //update category endpoint

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCategoryAsync(int id, UpdateCategoryDto request)
    {
        var userId = User.GetUserId();
        return Ok(_categoryService.UpdateCategoryAsync(userId, id, request));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //delete category endpoint

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteCategoryAsync(int id)
    {
        var userId = User.GetUserId();
        return Ok(_categoryService.DeleteCategoryAsync(userId, id));
    }
}
