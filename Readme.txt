docker build --no-cache -f ./HlifeApi/Dockerfile ./HlifeApi

docker compose -f ./HlifeApi/docker-compose.yml build