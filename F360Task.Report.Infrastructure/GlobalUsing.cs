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
global using MongoDB.Driver.Linq;
global using MongoFramework.Infrastructure.Diagnostics;


// ========== Domain ==========
global using F360Task.Report.Domain.Seed;
global using F360Task.Report.Domain.AggregatesModel;
global using F360Task.Report.Domain.Entities.Report;

// ========== Infrastructure ==========
global using F360Task.Report.Infrastructure.Exceptions;
global using F360Task.Report.Infrastructure.Outbox;
global using F360Task.Report.Infrastructure.Repositories;
global using F360Task.Report.Infrastructure.Transactions;
global using F360Task.Report.Infrastructure.Infrastructure.Contexts.Idempotent;
global using F360Task.Report.Infrastructure.Inbox;



// ========== AWS (se aplicável) ==========
global using Amazon.Runtime.Internal.Transform;
