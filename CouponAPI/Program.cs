using AutoMapper;
using CouponAPI;
using CouponAPI.Data;
using CouponAPI.Models;
using CouponAPI.Models.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("api/coupon", (ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "Getting all Coupons");

    APIResponse response = new()
    {
        Result = CouponStore.couponList,
        Success = true,
        StatusCode = HttpStatusCode.OK
    };

    return Results.Ok(response);
}).WithName("GetCoupons").Produces<APIResponse>(200);

app.MapGet("api/coupon/{id:int}", (int id) =>
{
    APIResponse response = new() { Success = false, StatusCode = HttpStatusCode.NotFound };

    var c = CouponStore.couponList.FirstOrDefault(c => c.Id == id);
    if (c == null)
    {
        response.ErrorMessages.Add("Coupon not found");
        return Results.BadRequest(response);
    }

    response.Result = c;
    response.Success = true;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("GetCoupon").Produces<APIResponse>(200);

app.MapPost("api/coupon", async (IMapper _mapper, IValidator<CouponCreateDTO> _validator, [FromBody] CouponCreateDTO couponCreateDTO) =>
{
    APIResponse response = new() { Success = false, StatusCode = HttpStatusCode.BadRequest };

    var validationResult = await _validator.ValidateAsync(couponCreateDTO);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    if (CouponStore.couponList.FirstOrDefault(c => c.Name.Equals(couponCreateDTO.Name, StringComparison.OrdinalIgnoreCase)) != null)
    {
        response.ErrorMessages.Add("Coupon Name already exists");
        return Results.BadRequest(response);
    }

    Coupon coupon = _mapper.Map<Coupon>(couponCreateDTO);
    coupon.Created = DateTime.Now;

    if (CouponStore.couponList.Any())
    {
        coupon.Id = CouponStore.couponList.OrderByDescending(c => c.Id).FirstOrDefault().Id + 1;
    }
    else
    {
        coupon.Id = 1;
    }

    CouponStore.couponList.Add(coupon);

    CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

    response.Result = couponDTO;
    response.Success = true;
    response.StatusCode = HttpStatusCode.Created;
    return Results.Ok(response);

    //return Results.CreatedAtRoute("GetCoupon", new { id = coupon.Id }, couponDTO);
}).WithName("CreateCoupon").Accepts<CouponDTO>("application/json").Produces<APIResponse>(201).Produces(400);

app.MapPut("api/coupon", async (IMapper _mapper, IValidator<CouponUpdateDTO> _validator, [FromBody] CouponUpdateDTO couponUpdateDTO) =>
{
    APIResponse response = new() { Success = false, StatusCode = HttpStatusCode.BadRequest };

    var validationResult = await _validator.ValidateAsync(couponUpdateDTO);
    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    var couponToBeUpdated = CouponStore.couponList.FirstOrDefault(c => c.Id == couponUpdateDTO.Id);
    if (couponToBeUpdated == null)
    {
        response.ErrorMessages.Add("Coupon Not Found!");
        return Results.BadRequest(response);
    }

    couponToBeUpdated.Name = couponUpdateDTO.Name;
    couponToBeUpdated.Percent = couponUpdateDTO.Percent;
    couponToBeUpdated.IsActive = couponUpdateDTO.IsActive;
    couponToBeUpdated.LastUpdated = DateTime.Now;

    CouponDTO couponDTO = _mapper.Map<CouponDTO>(couponToBeUpdated);

    response.Result = couponDTO;
    response.Success = true;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("UpdateCoupon").Accepts<CouponDTO>("application/json").Produces<APIResponse>(200).Produces(400);

app.MapDelete("api/coupon/{id:int}", (int id) =>
{
    APIResponse response = new() { Success = false, StatusCode = HttpStatusCode.NotFound };

    var couponToBeDeleted = CouponStore.couponList.FirstOrDefault(c => c.Id == id);
    if (!CouponStore.couponList.Remove(couponToBeDeleted))
    {
        response.ErrorMessages.Add("Coupon not found");
        return Results.BadRequest(response);
    }

    response.Success = true;
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("DeleteCoupon").Produces<APIResponse>(200).Produces(400);

app.UseHttpsRedirection();

app.Run();
