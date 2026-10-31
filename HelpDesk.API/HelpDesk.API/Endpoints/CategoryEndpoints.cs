using AutoMapper;
using FluentValidation;
using HelpDesk.API.Data.DTO;
using HelpDesk.API.Models;
using HelpDesk.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HelpDesk.API.Endpoints
{
    public static class CategoryEndpoints
    {
        public static void ConfigureCategoryEndpoints(this WebApplication app)
        {
            app.MapGet("/api/categories", async (ICategoryRepository _catRepo, ILogger<Program> _logger) =>
            {
                APIResponse response = new APIResponse();

                _logger.Log(LogLevel.Information, "Getting all departments");
                response.Result = await _catRepo.GetAllAsync();
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }).WithName("GetCategoties").Produces<APIResponse>(200);



            app.MapGet("/api/category/{id:int}", async (ICategoryRepository _catRepo, ILogger<Program> _logger, int id) =>
            {
                APIResponse response = new();
                response.Result = await _catRepo.GetAsync(id);
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);

            }).WithName("Getcategory").Produces<APIResponse>(201).Produces(400);



            app.MapPost("/api/category", async (ICategoryRepository _catRepo, IMapper _mapper, [FromBody] CategoryCreateDTO category_C_DTO, IValidator<CategoryCreateDTO> _validation) =>
            {
                APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };


                var validationResult = await _validation.ValidateAsync(category_C_DTO);
                if (!validationResult.IsValid)
                {
                    response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
                    return Results.BadRequest(response);
                }
                if (await _catRepo.GetAsync(category_C_DTO.Title) != null)
                {
                    response.ErrorMessages.Add("Category title already Exists");
                    return Results.BadRequest(response);

                }
                Category category = _mapper.Map<Category>(category_C_DTO);

                await _catRepo.CreateAsync(category);
                await _catRepo.SaveAsync();
                CategoryDTO categoryDTO = _mapper.Map<CategoryDTO>(category);



                response.Result = categoryDTO;
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);

            }).WithName("CreateCategory").Accepts<CategoryCreateDTO>("application/json").Produces<APIResponse>(201).Produces(400);

        }
    }
}
