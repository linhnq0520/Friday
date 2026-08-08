# Migrating WCF payment services to ASP.NET Core

Legacy WCF endpoints are common in payment and POS ecosystems. Clients already speak SOAP or custom bindings; ripping them out overnight is rarely an option.

## Start with a strangler boundary

1. Inventory contracts, bindings, and authentication modes.
2. Stand up ASP.NET Core controllers (or Minimal APIs) that mirror the same business operations.
3. Put a gateway or IIS rewrite in front so traffic can shift gradually.

## What usually hurts

- **Shared DTOs** that leak WCF attributes into the new stack — redefine request/response models for HTTP.
- **Ambient transactions** assumptions — replace with explicit unit-of-work boundaries.
- **IIS hosting quirks** — confirm hosting model (in-process / out-of-process) early.

## Cutover checklist

- Contract parity tests (same inputs → same money movement outcomes)
- Auth migration (Windows/NTLM/custom → JWT or API keys where appropriate)
- Observability: request IDs, latency histograms, and failure rates before flipping 100% traffic

This site itself is a smaller modernization lab: keep shipping thin vertical slices, measure, then expand.
