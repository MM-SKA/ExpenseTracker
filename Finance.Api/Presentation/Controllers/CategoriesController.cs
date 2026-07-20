using Finance.Api.Application.DTOs.Category;
using Finance.Api.Domain.Entities;
using Finance.Api.Infrastructure.Data;
using Finance.Api.Application.DTOs.Common;
using Finance.Api.Presentation.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Finance.Api.Application.Interfaces;

namespace Finance.Api.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController(ICategoryService categoryService) : ControllerBase
{

    //------------------------------------------------------------------------------------------------------------------------------------
    //create category endpoint

    [HttpPost("create")]
    public async Task<IActionResult> CreateCategoryAsync(CreateCategoryDto request)
    {
        var userId = User.GetUserId();
        return CreatedAtAction(nameof(CreateCategoryAsync), categoryService.CreateCategoryAsync(userId, request));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //get all categories for the user

    [HttpGet("get")]
    public async Task<IActionResult> GetCategoriesAsync()
    {
        var userId = User.GetUserId();
        return Ok(categoryService.GetCategoriesAsync(userId));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //update category endpoint

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCategoryAsync(int id, UpdateCategoryDto request)
    {
        var userId = User.GetUserId();
        return Ok(categoryService.UpdateCategoryAsync(userId, id, request));
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //delete category endpoint

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteCategoryAsync(int id)
    {
        var userId = User.GetUserId();
        return Ok(categoryService.DeleteCategoryAsync(userId, id));
    }
}
