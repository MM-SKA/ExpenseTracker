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
    public async Task<IActionResult> CreateCategoryAsync(CreateCategoryDto request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await categoryService.CreateCategoryAsync(userId, request, cancellationToken);
        return Ok(result);
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //get all categories for the user

    [HttpGet("get")]
    public async Task<IActionResult> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var categories = await categoryService.GetCategoriesAsync(userId, cancellationToken).ConfigureAwait(false);
        return Ok(categories);
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //update category endpoint

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCategoryAsync(string id, UpdateCategoryDto request, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await categoryService.UpdateCategoryAsync(userId, id, request, cancellationToken);
        return Ok(result);
    }
    //------------------------------------------------------------------------------------------------------------------------------------
    //delete category endpoint

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteCategoryAsync(string id, CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var result = await categoryService.DeleteCategoryAsync(userId, id, cancellationToken);
        return Ok(result);
    }
}
