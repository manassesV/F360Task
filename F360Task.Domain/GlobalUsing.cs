// ─────────────────────────────
// MediatR
// ─────────────────────────────
global using MediatR;                          // Biblioteca CQRS (Command/Query Handler abstraction)

// ─────────────────────────────
// Domain Layer
// ─────────────────────────────
global using F360Task.Domain.Seed;             // Interfaces e contratos base do domínio
global using F360Task.Domain.Events;           // Eventos de domínio (Domain Events)
global using F360Task.Domain.Entities.Email;   // Entidades relacionadas a e-mails
