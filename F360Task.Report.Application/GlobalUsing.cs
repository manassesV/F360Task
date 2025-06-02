// ─────────────────────────────
// Microsoft Extensions
// ─────────────────────────────
global using Microsoft.Extensions.Logging;     // Logging genérico (ILogger)

// ─────────────────────────────
// System
// ─────────────────────────────
global using System.Text.Json;                 // Serialização e desserialização JSON

// ─────────────────────────────
// FluentResults / Validation / MediatR
// ─────────────────────────────
global using FluentResults;                    // Result pattern funcional (Success/Failure)
global using FluentValidation;                 // Validação de entrada com FluentValidation
global using MediatR;                          // Biblioteca CQRS para separação de comandos e consultas

// ─────────────────────────────
// MongoDB / MongoFramework
// ─────────────────────────────
global using MongoDB.Driver;                   // Driver principal do MongoDB
global using MongoDB.Driver.Linq;              // Suporte LINQ para MongoDB
global using MongoFramework.Linq;              // MongoFramework LINQ extensions

// ─────────────────────────────
// Infrastructure Layer
// ─────────────────────────────
global using F360Task.Report.Infrastructure;                                          // Namespace base da infraestrutura
global using F360Task.Report.Infrastructure.Exceptions;                              // Exceções da camada de infraestrutura
global using F360Task.Report.Infrastructure.Outbox;                                  // Suporte ao padrão Outbox (mensageria confiável)
global using F360Task.Report.Infrastructure.Infrastructure.Contexts.Idempotent;      // Suporte a comandos idempotentes

// ─────────────────────────────
// Application Layer
// ─────────────────────────────
global using F360Task.Report.Application.Commands;     // Comandos da aplicação (ex: Create, Update, Delete)

// ─────────────────────────────
// Domain Layer
// ─────────────────────────────
global using F360Task.Report.Application.Queries;
global using F360Task.Report.Application.Queries.SchedulerReport;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using F360Task.Report.Application.Validations;
global using F360Task.Report.Application.Extensions;
global using F360Task.Report.Application.Handler;
global using F360Task.Report.Domain.AggregatesModel;
global using F360Task.Report.Domain.Entities.Report;
global using F360Task.Report.Infrastructure;
