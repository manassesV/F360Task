// ─────────────────────────────
// ASP.NET Core
// ─────────────────────────────
global using Microsoft.AspNetCore.Mvc;         // Base para controllers, atributos HTTP, etc.

// ─────────────────────────────
// MediatR / CQRS
// ─────────────────────────────
global using MediatR;                          // MediatR para comandos e consultas (CQRS)
global using F360Task.Report.Application.Commands;

// ─────────────────────────────
// Controllers / API Layer
// ─────────────────────────────
global using F360Task.API.Report.Swagger.Models;

// ─────────────────────────────
// MongoDB
// ─────────────────────────────
global using MongoDB.Driver;                   // Driver oficial do MongoDB para acesso a dados
global using Microsoft.OpenApi.Models;
global using Swashbuckle.AspNetCore.SwaggerGen;
global using Microsoft.Extensions.Options;
global using F360Task.API.Report.Swagger.Models;
global using System.Reflection;
global using Microsoft.AspNetCore.Mvc.ApiExplorer;
global using System.Text.Json;
global using F360Task.API.Report.ApiVersionSupport;
global using F360Task.API.Report.Swagger;
global using F360Task.Report.Application.Extensions;
global using F360Task.EventBusRabbitMQ.Extensions;
global using F360Task.Infrastructure;
global using System.Text.Json.Serialization;
global using F360Task.API.Report.Extensions;
global using FluentResults;
global using MongoFramework.Linq;
