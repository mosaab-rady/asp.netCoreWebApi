// using MongoDB.Bson;
// using MongoDB.Bson.Serialization;
// using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WebApi.Api.configuration;
using WebApi.Api.Repositories;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);


		// Add services to the container.
		builder.Services.AddControllers();
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		var _MongoDbSettings = builder.Configuration.GetSection("MongoDb");

		builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));
		builder.Services.AddSingleton<IItemsRepository, MongoDbItemsRepository>();


		// inject the repository from InMemItemsRepository  to the constructor of ItemsController once
		// builder.Services.AddSingleton<IItemsRepository, InMemItemsRepository>();




		builder.Services.AddHealthChecks().AddMongoDb(_MongoDbSettings.Get<MongoDbSettings>().ConnectionString, name: "mongodb", timeout: TimeSpan.FromSeconds(3), tags: new[] { "ready" });


		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
			app.UseHttpsRedirection();
		}


		app.UseAuthorization();

		app.MapControllers();

		app.MapHealthChecks("/health/ready", new HealthCheckOptions
		{
			Predicate = (check) => check.Tags.Contains("ready"),
			ResponseWriter = async (context, report) =>
			{
				var result = JsonSerializer.Serialize(
					new
					{
						status = report.Status.ToString(),
						checks = report.Entries.Select(entry => new
						{
							name = entry.Key,
							status = entry.Value.Status.ToString(),
							exception = entry.Value.Exception != null ? entry.Value.Exception.Message.ToString() : "none",
							duration = entry.Value.Duration.ToString()
						})
					}
				);
				context.Response.ContentType = MediaTypeNames.Application.Json;
				await context.Response.WriteAsync(result);
			}
		});

		app.MapHealthChecks("/health/live", new HealthCheckOptions
		{
			Predicate = (_) => false
		});

		app.Run();
	}
}