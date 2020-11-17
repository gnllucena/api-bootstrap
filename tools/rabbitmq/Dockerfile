FROM rabbitmq:3.6-management-alpine

ADD rabbitmq.config /etc/rabbitmq/
ADD definitions.json /etc/rabbitmq/

CMD ["rabbitmq-server"]