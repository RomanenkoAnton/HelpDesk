using AutoMapper;
using FluentValidation;
using HelpDesk.API;
using HelpDesk.API.Data;
using HelpDesk.API.Data.DTO;
using HelpDesk.API.Endpoints;
using HelpDesk.API.Models;
using HelpDesk.API.Repository;
using HelpDesk.API.Repository.IRepository;
using HelpDesk.API.Validation;
using HelpDesk.API.Validators;
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
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IValidator<DepartmentCreateDTO>, DepartmentCreateValidation>();
builder.Services.AddScoped<IValidator<DepartmentUpdateDTO>, DepartmentUpdateValidation>();

builder.Services.AddScoped<IValidator<CategoryUpdateDTO>, CategoryUpdateValidation>();
builder.Services.AddScoped<IValidator<CategoryCreateDTO>, CategoryCreateValidation>();

builder.Services.AddScoped<IValidator<UnitCreateDTO>, UnitCreateValidator>();
builder.Services.AddScoped<IValidator<UnitUpdateDTO>, UnitUpdateValidator>();

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IValidator<TicketCreateDTO>, TicketCreateValidator>();
builder.Services.AddScoped<IValidator<TicketUpdateDTO>, TicketUpdateValidator>();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapScalarApiReference();


app.ConfigureDepartmentEndpoints();
app.ConfigureCategoryEndpoints();
app.ConfigureUnitEndpoins();
app.ConfigureTicketEndpoints();


app.UseHttpsRedirection();
app.Run();


