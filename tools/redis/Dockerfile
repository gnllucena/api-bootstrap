FROM redis:6.0.9
COPY redis.conf /usr/local/etc/redis/
COPY seed.sh /usr/local/etc/redis/
COPY start.sh /usr/local/etc/start/
CMD ["sh", "/usr/local/etc/start/start.sh"]