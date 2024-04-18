# EventSourcing.EventStreaming.Microservice.Example

## Summary
This repository serves as a demonstration of implementing the outbox pattern with a SQL database and Object-Relational Mapping (ORM). The primary objective is to showcase the integration of this pattern to efficiently issue events into an Event Bus. By doing so, it enables and leverages the principles of Domain Driven Design (DDD) and Event Sourcing (ES).

## What is the Outbox Pattern?
The outbox pattern is a design pattern used in distributed systems to ensure reliable event delivery. It involves the use of an outbox table in a database to store events that need to be published to an external system, such as an Event Bus. This pattern ensures that events are reliably published, even in the event of failures during the publishing process.

## Key Features
- Outbox Table: A dedicated table in the SQL database to store events awaiting publication.
- ORM Integration: Seamless integration with Object-Relational Mapping for efficient data manipulation.
- Event Bus Integration: Facilitation of event publication to an Event Bus for further processing using technologies such as Apache Kafka.
- Domain Driven Design (DDD) Principles: Enabling the development of software that reflects real-world business domains to help assist with decoupling domain's.
- Event Sourcing (ES) Benefits: Leveraging event-driven architectures to maintain a reliable audit trail and help support complex business logic.
