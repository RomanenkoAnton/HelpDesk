using Scalar.AspNetCore;
using HelpDesk.API.Data;
using Microsoft.AspNetCore.Mvc;
using HelpDesk.API.Models;
using HelpDesk.API.Data.DTO;
using HelpDesk.API;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(cfg => { }, typeof(Program));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapScalarApiReference();






app.MapGet("/api/departments", (ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "Getting all departments");
    return Results.Ok(Store.departmentsList);
}).WithName("GetDepartments").Produces<IEnumerable<Department>>(200);


app.MapGet("/api/department/{id:int}", (int id) =>
{
    return Results.Ok(Store.departmentsList.FirstOrDefault(u => u.Id == id));
}).WithName("GetDepartment").Produces<Department>(201).Produces(400);

app.MapPost("/api/departments", (IMapper _mapper,[FromBody]DepartmentCreateDTO department_C_DTO) =>
{
    if ( string.IsNullOrEmpty(department_C_DTO.Title))
    {
        return Results.BadRequest("Invalid id or department Title");
    }
    if (Store.departmentsList.FirstOrDefault(u => u.Title.ToLower() == department_C_DTO.Title.ToLower()) != null)
    {
        return Results.BadRequest("departments title already Exists");
    }
    Department department = _mapper.Map<Department>(department_C_DTO);


    department.Id = Store.departmentsList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
    Store.departmentsList.Add(department);

    DepartmentDTO departmentDTO = _mapper.Map<DepartmentDTO>(department);

    return Results.CreatedAtRoute("GetDepartment", new {id= department.Id }, departmentDTO);
}).WithName("CreateDepartment").Accepts<DepartmentCreateDTO>("application/json").Produces<DepartmentDTO>(201).Produces(400);

app.MapPut("/api/departments", () =>
{

});
app.MapDelete("/api/departments/{id:int}", (int id) =>
{

});



app.UseHttpsRedirection();
app.Run();


