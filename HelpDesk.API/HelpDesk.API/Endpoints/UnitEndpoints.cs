using AutoMapper;
using FluentValidation;
using HelpDesk.API.Data.DTO;
using HelpDesk.API.Models;
using HelpDesk.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HelpDesk.API.Endpoints
{
    public static class UnitEndpoints
    {
        public static void ConfigureUnitEndpoins(this WebApplication app)
        {
                        app.MapGet("/api/units", async(IUnitRepository _unitRepo, IMapper _mapper, ILogger<Program> _logger) =>
            {
                APIResponse response = new();

                        _logger.Log(LogLevel.Information, "Getting all units");
                response.Result = _mapper.Map<List<UnitDTO>>(await _unitRepo.GetAllAsync());
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }).WithName("GetUnits").Produces<APIResponse>(200);

            app.MapGet("/api/unit/{id:int}", async(IUnitRepository _unitRepo, IMapper _mapper, int id) =>
            {
                APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.NotFound };

                    var unit = await _unitRepo.GetAsync(id);
                if (unit == null)
                {
                    response.ErrorMessages.Add("ТМЦ не найдено");
                    return Results.NotFound(response);
                }

                response.Result = _mapper.Map<UnitDTO>(unit);
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }).WithName("GetUnit").Produces<APIResponse>(200).Produces<APIResponse>(404);

            app.MapPost("/api/unit", async (IUnitRepository _unitRepo, IMapper _mapper,
                [FromBody] UnitCreateDTO unit_C_DTO, IValidator<UnitCreateDTO> _validation) =>
            {
                APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

                var validationResult = await _validation.ValidateAsync(unit_C_DTO);
                if (!validationResult.IsValid)
                {
                    response.ErrorMessages.Add(validationResult.Errors.First().ErrorMessage);
                    return Results.BadRequest(response);
                }

                var unit = _mapper.Map<Unit>(unit_C_DTO);
                await _unitRepo.CreateAsync(unit);
                await _unitRepo.SaveAsync();

                response.Result = _mapper.Map<UnitDTO>(await _unitRepo.GetAsync(unit.Id));
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.Created;
                return Results.Created($"/api/unit/{unit.Id}", response);
            }).WithName("CreateUnit").Produces<APIResponse>(201).Produces<APIResponse>(400);

            app.MapPut("/api/unit", async (IUnitRepository _unitRepo, IMapper _mapper,
                [FromBody] UnitUpdateDTO unit_U_DTO, IValidator<UnitUpdateDTO> _validation) =>
            {
                APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

                var validationResult = await _validation.ValidateAsync(unit_U_DTO);
                if (!validationResult.IsValid)
                {
                    response.ErrorMessages.Add(validationResult.Errors.First().ErrorMessage);
                    return Results.BadRequest(response);
                }

                if (!await _unitRepo.ExistsAsync(unit_U_DTO.Id))
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("ТМЦ не найдено");
                    return Results.NotFound(response);
                }

                await _unitRepo.UpdateAsync(_mapper.Map<Unit>(unit_U_DTO));
                await _unitRepo.SaveAsync();

                response.Result = _mapper.Map<UnitDTO>(await _unitRepo.GetAsync(unit_U_DTO.Id));
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }).WithName("UpdateUnit").Produces<APIResponse>(200).Produces<APIResponse>(400).Produces<APIResponse>(404);

            app.MapDelete("/api/unit/{id:int}", async (IUnitRepository _unitRepo, int id) =>
            {
                APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.NotFound };

                var unit = await _unitRepo.GetAsync(id);
                if (unit == null)
                {
                    response.ErrorMessages.Add("ТМЦ не найдено");
                    return Results.NotFound(response);
                }

                await _unitRepo.RemoveAsync(unit);
                await _unitRepo.SaveAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.NoContent;
                return Results.Ok(response);
            }).WithName("DeleteUnit").Produces<APIResponse>(200).Produces<APIResponse>(404);
                    }
    }
}
