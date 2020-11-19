# API Bootstrap
The philosophy behind this repository is pretty simple: every time a new API has to be created, the same set of functionalities must be written, and I got tired of writing it over and over again. You see, good programmers are lazy, and I am very lazy.

## How to use it
If you have [Make](https://www.gnu.org/software/make/) available on your OS, you can run dependecies services using it (On Makefile folder):

	make run

If you don't and have [Docker Compose](https://docs.docker.com/compose/) (On docker-compose.yaml folder):

	docker-compose -f docker-compose.yml up

If you don't have Docker Compose:

	Install it :)

You can run the application through Visual Studio or from command line (On the API folder):

	dotnet run

## Some philosophies on this software
This is by no means the right way to code, it's just a way that I found easier to code on my daily bases:

### Logs
Log everywhere and everything you can, mind your log level (you don't want to crash your log server). If you're writing software that does many differents thing accordingly of the input, be clear in your logs when your software does and doesn't do something. Always put a identifier on every log so you can trace to the affected entity you're working on.

### Cache
Cache is cool, use it, but don't reinvent the wheel. You don't want to solve a distributed cache problem, when Redis, for example, already solved.

### Publishing on message queues
If you're using RabbitMQ, don't publish on a queue, that's what exchanges are for (mostly).

### Interfaces
Hey, ever use [Inversion of Control](https://en.wikipedia.org/wiki/Inversion_of_control)? I didn't, I just use my interfaces for unit testing, if you won't use IoC, keep your interfaces and classes together, it's easier to maintain and expand.

### Exceptions
In this application there is a [Exception Handling Middleware](https://github.com/gnllucena/api-bootstrap/blob/master/src/api/Middlewares/ExceptionHandlingMiddleware.cs), so every thrown exception will be catched on it and the corresponding status code will be sent to the client. Don't like to use exceptions? Well, there is 57 different exception classes on dotnet, maybe you should use them more often.

### Controllers
They serialize requests and send responses, that's all.

### Services
Get everything your entity needs, validate it through FluentValidation and ValidationService, send it to it's repository (or cache, or message broker)

Input, processing, output.

### Validations 
Assyncronous validations should use [ValidationService.cs](https://github.com/gnllucena/api-bootstrap/blob/master/src/Common/Services/ValidationService.cs), syncronous validation should use fluentvalidation 

### Repositories
They run queries, what's more for repositories?

### Many Class Libraries
Just don't.

### Logs, yeah, again.
Really, log everything, when your application crashes, they'll be your best friend.

## Some cool things I think you should know
* Every HTTP response has a [X-Request-ID](https://devcenter.heroku.com/articles/http-request-id) header and every log (to Console or AWS CloudWatch) has this value embedded so the developer team can trace the request lifecycle.
	* Response Header - [LoggingMiddleware.cs](https://github.com/gnllucena/api-bootstrap/blob/master/src/api/Middlewares/LoggingMiddleware.cs) - line 21
	* Embedded value on logs - [LoggingMiddleware.cs](https://github.com/gnllucena/api-bootstrap/blob/master/src/api/Middlewares/LoggingMiddleware.cs) - line 23

* When a RabbitMQ connection is opened, the client tries to connect to, or create, a new set of [exchanges and queues](https://www.rabbitmq.com/tutorials/amqp-concepts.html). Every queue will be created with a deadletter, so when a message cannot be fully proccessed it will be sent to a different queue to be processed afterwards, and in case this behavior happens a few times, the message goes to an error queue (with original queue name and exception value), where the developer team should analyze it and see what is going on, all this behavior is based on appsettings values.
	* Exchanges and queues configuration - [MessagingFactory.cs](https://github.com/gnllucena/api-bootstrap/blob/master/src/Common/Factories/MessagingFactory.cs) - lines 58 to 62

	* Deadletter on exception - [MessagingService.cs](https://github.com/gnllucena/api-bootstrap/blob/master/src/Common/Services/MessagingService.cs) - line 80

	* Error on too many deadletters - [MessagingService.cs](https://github.com/gnllucena/api-bootstrap/blob/master/src/Common/Services/MessagingService.cs) - line 94

* Cache is present with Redis, and it's very simples to use, just open, and don't forget to close it, the connection with the server, there is a service written to facilitate your cache needs.
	* Opening and closing connection - [UserService.cs](https://github.com/gnllucena/api-bootstrap/blob/master/src/Common/Services/UserService.cs) - lines 174 and 196
	* Cache operations - [CacheService.cs](https://github.com/gnllucena/api-bootstrap/blob/master/src/Common/Services/CacheService.cs) - lines 13 to 18

* Infraestructure as code with docker is able to start all services this application uses: Redis, MySQL and RabbitMQ. Some configurations are overwritten through Dockerfiles, like MySQL user table and RabbitMQ user, virtual host and permissions configurations.
	* Redis configuration - [docker-compose.yml](https://github.com/gnllucena/api-bootstrap/blob/master/docker-compose.yml) - lines 20 to 24
	* MySQL configuration - [docker-compose.yml](https://github.com/gnllucena/api-bootstrap/blob/master/docker-compose.yml) - line 3
	* MySQL user table - [Dockerfile](https://github.com/gnllucena/api-bootstrap/blob/master/tools/mysql/Dockerfile) - lines 20 and 24
	* RabbitMQ configuration - [docker-compose.yml](https://github.com/gnllucena/api-bootstrap/blob/master/docker-compose.yml) - lines 3 to 8
	* RabbitMQ user, virtual host and permissions configurations - [Dockerfile](https://github.com/gnllucena/api-bootstrap/blob/master/tools/rabbitmq/Dockerfile) - lines 3 and 4

