A text difference finder.

This has its limits... and isn't ideal for files even though its set to do files.

The following docker commands will build containers for this


docker build -t difftextrewrite . <br>
docker run -p 59134:59134 -e ASPNETCORE_URLS="http://0.0.0.0:59134" difftextrewrite

docker build -t diffsite . <br>
docker run -p 50184:50184 -e ASPNETCORE_URLS="http://0.0.0.0:50184" diffsite
