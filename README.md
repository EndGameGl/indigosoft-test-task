This solution is a coding assignment for Indigosoft company.

It contains 3 separate projects:
1. `IndigoSoft.QuotationCollector.Server` contains data collectors and parsers for brokers
2. `IndigoSoft.QuotationsMocker.Producer.Api` is a data-mocking API that is used to simulate WebSocket ticker data flow
3. `IndigoSoft.QuotationCollector.Tests` contains some unit tests for testing basic functionality

`IndigoSoft.QuotationCollector.Server` depends on 2 services, that can be launched with docker-compose, located at `Infrastructure/docker-compose.yaml`

Features:
- Automatic WebSocket connection handling per broker
- Configurable data parsing and mapping to unified model
- Data deduplication with Redis cache
- Saving data to a separate Postgres database
- Extensible interfaces for handling new brokers added
- Prometheus metrics for parsed and handled data
- Logging of main event, as well as error logging
