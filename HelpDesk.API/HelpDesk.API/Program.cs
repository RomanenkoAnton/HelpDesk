using AutoMapper;
using FluentValidation;
using HelpDesk.API;
using HelpDesk.API.Data;
using HelpDesk.API.Data.DTO;
using HelpDesk.API.Models;
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
builder.Services.AddScoped<IValidator<DepartmentCreateDTO>, DepartmentCreateValidation>();
builder.Services.AddScoped<IValidator<DepartmentUpdateDTO>, DepartmentUpdateValidation>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapScalarApiReference();






app.MapGet("/api/departments", (ILogger<Program> _logger) =>
{
    APIResponse response = new APIResponse();

    _logger.Log(LogLevel.Information, "Getting all departments");
    response.Result = Store.departmentsList;
    response.IsSuccess = true;
    response.StatusCode =HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetDepartments").Produces<APIResponse>(200);


app.MapGet("/api/department/{id:int}", (ILogger < Program > _logger, int id) =>
{
    APIResponse response = new ();
    response.Result = Store.departmentsList.FirstOrDefault(u => u.Id == id);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
    
}).WithName("GetDepartment").Produces<APIResponse>(201).Produces(400);

app.MapPost("/api/department", async  (IMapper _mapper, [FromBody]DepartmentCreateDTO department_C_DTO, IValidator<DepartmentCreateDTO> _validation) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };


    var validationResult = await _validation.ValidateAsync(department_C_DTO);
    if ( !validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }
    if (Store.departmentsList.FirstOrDefault(u => u.Title.ToLower() == department_C_DTO.Title.ToLower()) != null)
    {
        response.ErrorMessages.Add("departments title already Exists");
        return Results.BadRequest(response);
        
    }
    Department department = _mapper.Map<Department>(department_C_DTO);


    department.Id = Store.departmentsList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
    Store.departmentsList.Add(department);
    DepartmentDTO departmentDTO = _mapper.Map<DepartmentDTO>(department);
    

    response.Result = departmentDTO;
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
    //return Results.CreatedAtRoute("GetDepartment", new {id= department.Id }, departmentDTO);
}).WithName("CreateDepartment").Accepts<DepartmentCreateDTO>("application/json").Produces<APIResponse>(201).Produces(400);

app.MapPut("/api/department",  async(IMapper _mapper, [FromBody] DepartmentUpdateDTO department_U_DTO, IValidator<DepartmentUpdateDTO> _validation) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
    var validationResult = await _validation.ValidateAsync(department_U_DTO);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }
    Department departmentFromStore = Store.departmentsList.FirstOrDefault(u => u.Id == department_U_DTO.Id);
    departmentFromStore.Title = department_U_DTO.Title;
    response.Result = _mapper.Map<DepartmentDTO>(departmentFromStore);
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);

}).WithName("UpdateCoupon");

app.MapDelete("/api/department/{id:int}", (int id) =>
{
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
    Department departmentFromStore = Store.departmentsList.FirstOrDefault(u => u.Id == id);
    if (departmentFromStore != null)
    {
        Store.departmentsList.Remove(departmentFromStore);
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



app.UseHttpsRedirection();
app.Run();


