# REPR Design Pattern

![.Net](https://img.shields.io/badge/-.NET%208.0-blueviolet?logo=dotnet) 
![Openapi](https://img.shields.io/badge/Docs-OpenAPI%208.0-success?style=flat-square)
![Swagger](https://img.shields.io/badge/-Swagger-%23Clojure?logo=swagger&logoColor=white)
![Rest](https://img.shields.io/badge/rest-40AEF0?logo=rest&logoColor=white)
![HTTP3](https://img.shields.io/badge/HTTP%203-v3.0-brightgreen)
![HTTP2](https://img.shields.io/badge/HTTP%202-v2.0-blue)
![HTTP1](https://img.shields.io/badge/HTTP%201-v1.1-orange)

The REPR (Request-Endpoint-Response) pattern is a design approach for organizing APIs in .NET Core applications. Unlike traditional MVC or REST-based patterns, REPR focuses on defining APIs around endpoints instead of controllers, offering improved modularity, maintainability, and scalability.

This repository demonstrates the **REPR (Request-Endpoint-Response)** pattern using **.NET 8**. It also features **OpenAPI documentation** for seamless exploration and understanding of the API. **HTTP/3/2/1** fallback code supports **Brotli** compression and falls back to **Gzip** for **response compression**.

![cqrs_pattern](./Screenshots/REPR_Pattern.jpg)

# Project structure / technology
* .NET 8: Technology

## Hosting projects: REPRPatternApi
The executing code runs from these projects.

## Libraries:
- **OpenAPI Documentation**: Automatically generated API documentation using OpenAPI for better understanding and testing of the 
API.
- **Scalar**: Replaces Swagger for calling and testing APIs.
