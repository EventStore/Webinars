# Event Store Webinars

Code repository for Event Store [webinars](https://www.eventstore.com/webinars).

## Pre-requisites

Ensure you have the following tools:

- Your favorite IDE or code editor
- [Docker](https://docs.docker.com/install/)
- [Docker Compose](https://docs.docker.com/compose/install/) (usually included in Docker for Desktop)

You will also need the `docker-compose.yml` file included in this repository.

If you want to use anything else than MongoDB for projections, feel free to change the compose file.

## Structure

Find the repository structure below.

### Event Sourcing basics

The code is located in `01-eventsourcing`. 

It's divided in two parts:
- Step 1: using events, but still persisting the state without events: `mongo-persistence`.
- Step 2: fully event-sourced solution: `esdb-persistence`.
