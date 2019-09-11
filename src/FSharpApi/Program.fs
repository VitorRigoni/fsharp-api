module FSharpApi.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Repository
open Giraffe
open Giraffe.HttpStatusCodeHandlers.RequestErrors

// ---------------------------------
// Models
// ---------------------------------

type Message = { Text : string }
type PersonNotFound = { statusCode: int; error: string}

// ---------------------------------
// Views
// ---------------------------------

module Views =
    open GiraffeViewEngine

    let layout (content: XmlNode list) =
        html [] [
            head [] [
                title []  [ encodedText "FSharpApi" ]
                link [ _rel  "stylesheet"
                       _type "text/css"
                       _href "/main.css" ]
            ]
            body [] content
        ]

    let partial () =
        h1 [] [ encodedText "FSharpApi" ]

    let index (model : Message) =
        [
            partial()
            p [] [ encodedText model.Text ]
        ] |> layout

// ---------------------------------
// Web app
// ---------------------------------

let setText txt =
    { Text = txt }

let indexHandler name =
    name
    |> sprintf "Hello %s, from Giraffe!"
    |> setText
    |> Views.index
    |> htmlView

let jsonIndexHandler name =
    name
    |> sprintf "Hello %s, from Giraffe!"
    |> setText
    |> json 

let personNotFound msg =
    { statusCode = 404; error = msg }
    |> json
    |> notFound

let matchPersonQueryResult person =
    match person with
    | Ok x -> json x
    | Error msg -> personNotFound msg

let getPersonByFirstName firstName =
    firstName
    |> getPersonWithFirstName
    |> matchPersonQueryResult

let webApp =
    choose [
        GET >=>
            choose ([
                route "/" >=> jsonIndexHandler "world"
                routef "/hello/%s" indexHandler
                route "/people" >=> json listPeople
                routef "/people/%s" getPersonByFirstName
            ])
        setStatusCode 404 >=> text "Not Found"
        setStatusCode 405 >=> text "Invalid method" ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    (ex, "An unhandled exception has occurred while executing the request.")
    |> logger.LogError
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8080")
           .AllowAnyMethod()
           .AllowAnyHeader()
           |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IHostingEnvironment>()
    (match env.IsDevelopment() with
    | true  -> app.UseDeveloperExceptionPage()
    | false -> app.UseGiraffeErrorHandler errorHandler)
        .UseHttpsRedirection()
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    builder.AddFilter(fun l -> l.Equals LogLevel.Error)
           .AddConsole()
           .AddDebug() |> ignore

[<EntryPoint>]
let main _ =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(contentRoot)
        .UseIISIntegration()
        .UseWebRoot(webRoot)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)
        .Build()
        .Run()
    0