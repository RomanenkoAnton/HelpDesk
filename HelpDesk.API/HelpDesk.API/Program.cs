using AutoMapper;
using FluentValidation;
using HelpDesk.API;
using HelpDesk.API.Data;
using HelpDesk.API.Data.DTO;
using HelpDesk.API.Models;
using HelpDesk.API.Repository;
using HelpDesk.API.Repository.IRepository;
using HelpDesk.API.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(cfg => { }, typeof(Program));
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IValidator<DepartmentCreateDTO>, DepartmentCreateValidation>();
builder.Services.AddScoped<IValidator<DepartmentUpdateDTO>, DepartmentUpdateValidation>();

builder.Services.AddScoped<IValidator<CategoryUpdateDTO>, CategoryUpdateValidation>();
builder.Services.AddScoped<IValidator<CategoryCreateDTO>, CategoryCreateValidation>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapScalarApiReference();



app.MapGet("/api/departments", async (IDepartmentRepository _departRepo, ILogger<Program> _logger) =>
{
    APIResponse response = new APIResponse();

    _logger.Log(LogLevel.Information, "Getting all departments");
    response.Result =await _departRepo.GetAllAsync();
    response.IsSuccess = true;
    response.StatusCode =HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetDepartments").Produces<APIResponse>(200);


app.MapGet("/api/department/{id:int}", async (IDepartmentRepository _departRepo, ILogger< Program > _logger, int id) =>
{
    APIResponse response = new ();
    response.Result = await _departRepo.GetAsync(id);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
    
}).WithName("GetDepartment").Produces<APIResponse>(201).Produces(400);

app.MapPost("/api/department", async  (IDepartmentRepository _departRepo, IMapper _mapper, [FromBody]DepartmentCreateDTO department_C_DTO, IValidator<DepartmentCreateDTO> _validation) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };


    var validationResult = await _validation.ValidateAsync(department_C_DTO);
    if ( !validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }
    if (await _departRepo.GetAsync(department_C_DTO.Title) != null)
    {
        response.ErrorMessages.Add("departments title already Exists");
        return Results.BadRequest(response);
        
    }
    Department department = _mapper.Map<Department>(department_C_DTO);
        
    await _departRepo.CreateAsync(department);
    await _departRepo.SaveAsync();
    DepartmentDTO departmentDTO = _mapper.Map<DepartmentDTO>(department);

    

    response.Result = departmentDTO;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
    //return Results.CreatedAtRoute("GetDepartment", new {id= department.Id }, departmentDTO);
}).WithName("CreateDepartment").Accepts<DepartmentCreateDTO>("application/json").Produces<APIResponse>(201).Produces(400);

app.MapPut("/api/department",  async(IDepartmentRepository _departRepo, IMapper _mapper, [FromBody] DepartmentUpdateDTO department_U_DTO, IValidator<DepartmentUpdateDTO> _validation) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    var validationResult = await _validation.ValidateAsync(department_U_DTO);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }
    await _departRepo.UpdateAsync(_mapper.Map<Department>(department_U_DTO));
    await _departRepo.SaveAsync();

    response.Result = _mapper.Map<DepartmentDTO>(await _departRepo.GetAsync(department_U_DTO.Id));
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);

}).WithName("UpdateDepartment");

app.MapDelete("/api/department/{id:int}", async (IDepartmentRepository _departRepo, int id) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
    Department departmentFromStore = await _departRepo.GetAsync(id);
    if (departmentFromStore != null)
    {
        await _departRepo.RemoveAsync(departmentFromStore);
        await _departRepo.SaveAsync();
        response.IsSuccess = true;
        response.StatusCode = HttpStatusCode.NoContent;
        return Results.Ok(response);
    }
    else
    {
        response.ErrorMessages.Add("Неіснуючий ID");
        return Results.BadRequest(response);
    }


});



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

app.UseHttpsRedirection();
app.Run();


