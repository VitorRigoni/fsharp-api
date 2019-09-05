module PeopleController

open PeopleRepository
open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe.HttpStatusCodeHandlers.RequestErrors
open Giraffe

type Person =
    {
        FirstName : string;
        LastName : string
    }

let getPeople =
    fun next ctx ->
        task {
            let! result = listPeople
            return! json result next ctx
        }

let getPerson (firstName: string) =
    fun next ctx ->
        task {
            let! result = (getPersonWithFirstName firstName)
            match result with
            | Some x -> return! json x next ctx
            | None -> return! setStatusCode 404 next ctx
        }

let addPerson =
    json []

let getRoutes : (List<HttpHandler>) = [
        GET >=>
            choose [
                route "/people" >=> getPeople
                routef "/people/%s" getPerson
            ]
    ]

let postRoutes : List<HttpHandler> = [
    POST >=>
        choose [
            route "/people/add" >=> addPerson
        ]
    ]
