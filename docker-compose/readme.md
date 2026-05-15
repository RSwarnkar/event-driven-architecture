# latest RabbitMQ 4.x



docker run -it --rm --name rabbitmq-channel -p 5672:5672 -p 15672:15672 rabbitmq:4-management



docker exec -it rabbitmq rabbitmqctl status
docker exec -it rabbitmq rabbitmqctl list_queues


docker exec -it telemetry-producer 