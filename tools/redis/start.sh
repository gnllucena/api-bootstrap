redis-server --daemonize yes && sleep 1;
redis-cli < /usr/local/etc/redis/seed.sh;
redis-cli save;
redis-cli shutdown;
redis-server --include /usr/local/etc/redis/redis.conf;