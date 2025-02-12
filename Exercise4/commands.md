docker build -t exercise4 .
docker run --name ex4 -p 8080:8080 -it --rm exercise4
docker run --name ex4 -p 8080:8080 -it --rm -e MyVariable=Demo exercise4

docker build -t exercise4:debug --build-arg BUILD_CONFIGURATION=Debug .
docker run --name ex4 -p 8080:8080 -it --rm exercise4:debug
docker run --name ex4 -p 8080:8080 -it --rm -e ASPNETCORE_ENVIRONMENT=Development exercise4:debug
