# Cutting QR API latency from 30s to 5s under load

A QR generation API that looks fine with Postman can collapse under concurrent POS traffic. JMeter (or k6) is where the truth shows up.

## What to measure first

- **p50 / p95 / p99** — averages lie
- Thread pool starvation and lock contention
- Downstream I/O (DB, remote image/ steganography, HTTP calls)

## Fixes that usually move the needle

1. Remove accidental sync-over-async.
2. Cache stable inputs (merchant config, logo assets).
3. Bound parallelism — unlimited `Task.WhenAll` can make things worse.
4. Stream responses instead of buffering giant payloads.

## Takeaway

Treat load tests as part of the feature, not a pre-release ceremony. The ~30s → ~5s win on a payment QR endpoint came from measuring contention, not from rewriting the whole service.
