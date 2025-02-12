#docker build -f ".\Dockerfile.entrypoint" -t demo:entrypoint --progress=plain .
#docker build -f ".\Dockerfile.dcmd" -t demo:dcmd --progress=plain .
#docker build -f ".\Dockerfile.combo" -t demo:combo --progress=plain .

#docker run -p 5000:80 demo:entrypoint "does not replace argument"
#docker run -p 5000:80 demo:dcmd "./HelloWorld" "with fully replaced command"
#docker run -p 5000:80 demo:combo "with replaced argument"
