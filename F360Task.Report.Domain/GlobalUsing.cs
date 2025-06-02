// ─────────────────────────────
// MediatR
// ─────────────────────────────
global using MediatR;                          // Biblioteca CQRS (Command/Query Handler abstraction)

// ─────────────────────────────
// Domain Layer
// ─────────────────────────────
global using F360Task.Report.Domain.Seed;             // Interfaces e contratos base do domínio
global using F360Task.Report.Domain.Entities.Report;  // Entidades relacionadas a relatórios
