using AutoMapper;
using FluentValidation;
using HelpDesk.API.Data.DTO;
using HelpDesk.API.Models;
using HelpDesk.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HelpDesk.API.Endpoints
{
    public static class DepartmentEndpoints
    {
        public static void ConfigureDepartmentEndpoints(this WebApplication app)
        {
            app.MapGet("/api/departments", async (IDepartmentRepository _departRepo, ILogger<Program> _logger) =>
            {
                APIResponse response = new APIResponse();

                _logger.Log(LogLevel.Information, "Getting all departments");
                response.Result = await _departRepo.GetAllAsync();
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }).WithName("GetDepartments").Produces<APIResponse>(200);


            app.MapGet("/api/department/{id:int}", async (IDepartmentRepository _departRepo, ILogger<Program> _logger, int id) =>
            {
                APIResponse response = new();
                response.Result = await _departRepo.GetAsync(id);
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);

            }).WithName("GetDepartment").Produces<APIResponse>(201).Produces(400);

            app.MapPost("/api/department", async (IDepartmentRepository _departRepo, IMapper _mapper, [FromBody] DepartmentCreateDTO department_C_DTO, IValidator<DepartmentCreateDTO> _validation) =>
            {
                APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };


                var validationResult = await _validation.ValidateAsync(department_C_DTO);
                if (!validationResult.IsValid)
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

            app.MapPut("/api/department", async (IDepartmentRepository _departRepo, IMapper _mapper, [FromBody] DepartmentUpdateDTO department_U_DTO, IValidator<DepartmentUpdateDTO> _validation) =>
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



        }
    }
}
