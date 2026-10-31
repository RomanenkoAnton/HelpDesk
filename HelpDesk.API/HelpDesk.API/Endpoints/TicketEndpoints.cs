using AutoMapper;
using FluentValidation;
using HelpDesk.API.Data.DTO;
using HelpDesk.API.Models;
using HelpDesk.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HelpDesk.API.Endpoints
{
    public static class TicketEndpoints
    {
        public static void ConfigureTicketEndpoints(this WebApplication app)
        {
            app.MapGet("/api/tickets", async (ITicketRepository _ticketRepo, IMapper _mapper, ILogger<Program> _logger) =>
            {
                APIResponse response = new();

                _logger.Log(LogLevel.Information, "Getting all tickets");
                response.Result = _mapper.Map<List<TicketDTO>>(await _ticketRepo.GetAllAsync());
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }).WithName("GetTickets").Produces<APIResponse>(200);

            app.MapGet("/api/ticket/{id:int}", async (ITicketRepository _ticketRepo, IMapper _mapper, int id) =>
            {
                APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.NotFound };

                var ticket = await _ticketRepo.GetAsync(id);
                if (ticket == null)
                {
                    response.ErrorMessages.Add("Заявку не найдено");
                    return Results.NotFound(response);
                }

                response.Result = _mapper.Map<TicketDTO>(ticket);
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }).WithName("GetTicket").Produces<APIResponse>(200).Produces<APIResponse>(404);

            app.MapPost("/api/ticket", async (ITicketRepository _ticketRepo, IDepartmentRepository _departRepo,
                IMapper _mapper, [FromBody] TicketCreateDTO ticket_C_DTO, IValidator<TicketCreateDTO> _validation) =>
            {
                APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

                var validationResult = await _validation.ValidateAsync(ticket_C_DTO);
                if (!validationResult.IsValid)
                {
                    response.ErrorMessages.Add(validationResult.Errors.First().ErrorMessage);
                    return Results.BadRequest(response);
                }

                if (await _departRepo.GetAsync(ticket_C_DTO.DepartmentId) == null)
                {
                    response.ErrorMessages.Add("Подразделение не найдено");
                    return Results.BadRequest(response);
                }

                Ticket ticket = _mapper.Map<Ticket>(ticket_C_DTO);
                await _ticketRepo.CreateAsync(ticket);
                await _ticketRepo.SaveAsync();

                response.Result = _mapper.Map<TicketDTO>(await _ticketRepo.GetAsync(ticket.Id));
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.Created;
                return Results.Created($"/api/ticket/{ticket.Id}", response);
            }).WithName("CreateTicket").Accepts<TicketCreateDTO>("application/json")
              .Produces<APIResponse>(201).Produces<APIResponse>(400);

            app.MapPut("/api/ticket", async (ITicketRepository _ticketRepo, IDepartmentRepository _departRepo,
                IMapper _mapper, [FromBody] TicketUpdateDTO ticket_U_DTO, IValidator<TicketUpdateDTO> _validation) =>
            {
                APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

                var validationResult = await _validation.ValidateAsync(ticket_U_DTO);
                if (!validationResult.IsValid)
                {
                    response.ErrorMessages.Add(validationResult.Errors.First().ErrorMessage);
                    return Results.BadRequest(response);
                }

                if (!await _ticketRepo.ExistsAsync(ticket_U_DTO.Id))
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.ErrorMessages.Add("Заявку не найдено");
                    return Results.NotFound(response);
                }

                if (await _departRepo.GetAsync(ticket_U_DTO.DepartmentId) == null)
                {
                    response.ErrorMessages.Add("Подразделение не найдено");
                    return Results.BadRequest(response);
                }

                await _ticketRepo.UpdateAsync(_mapper.Map<Ticket>(ticket_U_DTO));
                await _ticketRepo.SaveAsync();

                response.Result = _mapper.Map<TicketDTO>(await _ticketRepo.GetAsync(ticket_U_DTO.Id));
                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.OK;
                return Results.Ok(response);
            }).WithName("UpdateTicket").Produces<APIResponse>(200)
              .Produces<APIResponse>(400).Produces<APIResponse>(404);

            app.MapDelete("/api/ticket/{id:int}", async (ITicketRepository _ticketRepo, int id) =>
            {
                APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.NotFound };

                var ticket = await _ticketRepo.GetAsync(id);
                if (ticket == null)
                {
                    response.ErrorMessages.Add("Заявку не найдено");
                    return Results.NotFound(response);
                }

                await _ticketRepo.RemoveAsync(ticket);
                await _ticketRepo.SaveAsync();

                response.IsSuccess = true;
                response.StatusCode = HttpStatusCode.NoContent;
                return Results.Ok(response);
            }).WithName("DeleteTicket").Produces<APIResponse>(200).Produces<APIResponse>(404);
        }
    }
}