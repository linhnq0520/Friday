# gRPC vs RabbitMQ in core banking flows

In teller and OTP-style systems you almost always need **both** synchronous RPC and asynchronous messaging. The mistake is using one tool for every hop.

## Prefer gRPC when

- The caller needs an immediate success/failure (balance check, OTP verify).
- Latency budget is tight and payloads are typed.
- Services share a trusted network and strong contracts.

## Prefer RabbitMQ when

- Work can finish after the HTTP/gRPC response (notifications, post-commit side effects).
- You need fan-out, retries, or buffering under spike load.
- Downstream systems may be temporarily unavailable.

## A practical split

| Flow | Style |
|------|--------|
| OTP validate | gRPC / HTTP sync |
| Posting + notify channels | RabbitMQ async |
| Cross-service orchestration | Mix — gateway sync edge, queue for durable steps |

YARP (or another gateway) keeps the public edge boring while internal services evolve behind it.
