# Study 1

## Source
- Example: file mẫu
- GenApi: api ghi log vào opensearch và gửi nhận message tới RabbitMQ
- docker-compose.yml: file cấu hình docker compose
- opensearch-data: folder chứa dữ liệu store data của opensearch
  - cấu hình đường dẫn này nằm ở trong file docker-compose.yml
  - được tạo sau khi chạy host opensearch
- postgres-data: folder chứa dữ liệu store data của postgreSql
  - cấu hình đường dẫn này nằm ở trong file docker-compose.yml
  - được tạo sau khi chạy host postgres

## Docker
```
docker compose up -d
```
