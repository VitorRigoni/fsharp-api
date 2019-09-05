module PeopleController

open PeopleRepository
open FSharp.Control.Tasks.V2.ContextInsensitive
open Giraffe.HttpStatusCodeHandlers.RequestErrors
open Giraffe

let getPeople =
    json listPeople

let getPerson (firstName: string) =
    match getPersonWithFirstName firstName with
    | Some x -> json x
    | None -> setStatusCode 404

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
