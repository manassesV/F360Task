// ─────────────────────────────
// System e bibliotecas de terceiros
// ─────────────────────────────
global using RabbitMQ.Client;
global using RabbitMQ.Client.Events;
global using Polly;
global using Polly.Retry;

// ─────────────────────────────
// Microsoft Extensions
// ─────────────────────────────
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;

// ─────────────────────────────
// Projeto: SharedKernel
// ─────────────────────────────
global using F360Task.SharedKernel.ResiliencyStrategy;

// ─────────────────────────────
// Projeto: EventBusRabbitMQ
// ─────────────────────────────
global using F360Task.Report.EventBusRabbitMQ.Consumer;
global using F360Task.Report.EventBusRabbitMQ.BackgroundServices;
global using F360Task.Report.EventBusRabbitMQ.Configs;
global using F360Task.Report.EventBusRabbitMQ.Publisher;
global using RabbitMQ.Client.Exceptions;
global using System.Net.Sockets;
global using F360Task.Report.Infrastructure.Inbox;
global using F360Task.Report.Domain.Seed;
global using MongoDB.Driver;
global using System.Text;
global using Amazon.Runtime;
global using F360Task.Report.Infrastructure.Outbox;
global using F360Task.Report.Infrastructure.Transactions;
global using Microsoft.Extensions.Options;
