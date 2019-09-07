# FSharpApi

A [Giraffe](https://github.com/giraffe-fsharp/Giraffe) web application, which has been created via the `dotnet new giraffe` command.

## Build and test the application

### Windows

Run the `build.bat` script in order to restore, build and test (if you've selected to include tests) the application:

```
> ./build.bat
```

### Linux/macOS

Run the `build.sh` script in order to restore, build and test (if you've selected to include tests) the application:

```
$ ./build.sh
```

## Run the application

After a successful build you can start the web application by executing the following command in your terminal:

```
dotnet run src/FSharpApi
```

After the application has started visit [http://localhost:5000](http://localhost:5000) in your preferred browser.

## Docker

Run the docker-compose file.
`docker-compose up`

This will create the application and the db with a restore of `AdventureWorks2017` database. But theres a catch!

Wait until the database is fully up and restored before retrieving any endpoint. Because of how F# works, if you try to retrieve the /people endpoint before the database is fully up it'll throw an exception and won't recover from it.
If that happens, restart the app container after the db is fully up.

On docker, the app will start at port 5001: [http://localhost:5001](http://localhost:5001)