# UniversalEducationService

### Introduction
The universal education service (hereinafter referred as UES) project is a education system providing education services. The core service of the system is the exam subsystem, emcompassed by three other main subsystems, login, notification, and course.
The API of this project are divided into several layers, from the basic data layer, followed by module (methods) layer to the uppermost service layer. Users can invoke functions of each layer to satisfy various needs.

### Techniques involved

- Programming language: C# (8 Preview, including a method which uses the new feature in C# 8, switch expression)
- ORM (Object-Relation Mapping): Entity Framework (6.2.0)
- Database: SQLServer (not finally decided)
- Service protocol: Rest (I wish to use SOAP actually)
- Unit Test: NUnit 3.11.0

### Solution Structure

- Project UES: contains classes definitions and module methods
- Test: contains a web server providing Rest-style services
- NUnitTestProject1: (stupid name) NUnit project intended to test the correctness of the methods