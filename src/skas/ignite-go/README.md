```
# Tests
docker compose -f docker-compose-tests.yml up -d
go test -short ./...

# Docker
docker build . -t skavar_projectName
docker run -p 8080:8080 -p 42079:42079 skavar_projectName

curl --request GET \
  --url http://localhost:8080/example \
  --header 'Content-Type: application/json'