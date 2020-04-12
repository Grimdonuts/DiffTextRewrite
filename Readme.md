A text difference finder.

The following docker commands will build containers for this
docker build -t difftextrewrite .
docker run -p 59134:59134 -e ASPNETCORE_URLS="http://0.0.0.0:59134" difftextrewrite

docker build -t diffsite .
docker run -p 50184:50184 -e ASPNETCORE_URLS="http://0.0.0.0:50184" diffsite
