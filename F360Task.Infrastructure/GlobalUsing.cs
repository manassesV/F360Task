// ========== Framework ==========
global using System;
global using System.Linq;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Hosting;


// ========== MongoDB ==========
global using MongoDB.Driver;
global using MongoFramework;
global using MongoFramework.Linq;

// ========== Domain ==========
global using F360Task.Domain.Seed;
global using F360Task.Domain.AggregatesModel;
global using F360Task.Domain.Entities.Email;
global using F360Task.Domain.Entities.Report;

// ========== Infrastructure ==========
global using F360Task.Infrastructure.Exceptions;
global using F360Task.Infrastructure.Outbox;
global using F360Task.Infrastructure.Repositories;
global using F360Task.Infrastructure.Transactions;
global using F360Task.Infrastructure.Infrastructure.Contexts.Idempotent;
global using F360Task.Infrastructure.Inbox;



// ========== AWS (se aplicável) ==========
global using Amazon.Runtime.Internal.Transform;
