﻿using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace FoodReserve.API.Extensions
{
    public static class ExceptionExtensions
    {
        public static void UseExceptionConfig(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    IExceptionHandlerFeature? exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (exceptionHandlerFeature != null)
                    {
                        Exception exception = exceptionHandlerFeature.Error;

                        ProblemDetailsFactory problemDetailsFactory = context.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                        ProblemDetails problemDetails = problemDetailsFactory.CreateProblemDetails(
                            context,
                            title: exception.Message
                        );

                        context.Response.StatusCode = problemDetails.Status.Value;
                        context.Response.ContentType = "application/problem+json";

                        await context.Response.WriteAsJsonAsync(problemDetails);
                    }
                });
            });
        }
    }
}
