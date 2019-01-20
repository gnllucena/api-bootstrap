FROM debian:stretch
RUN groupadd -g 999 appuser && \
    useradd -r -u 999 -g appuser appuser
USER appuser
CMD ["cat", "/tmp/secrets.txt"]