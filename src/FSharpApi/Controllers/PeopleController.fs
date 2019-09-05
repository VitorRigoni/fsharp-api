module PeopleController

open PeopleRepository
open Giraffe.HttpStatusCodeHandlers.RequestErrors
open Giraffe

type PersonNotFound = { statusCode: int; error: string}

let personNotFound msg =
    notFound (json { statusCode = 404; error = msg})

let matchPersonQueryResult person =
    match person with
    | Ok x -> json x
    | Error msg -> personNotFound msg

let getPersonByFirstName (firstName: string) =
    getPersonWithFirstName firstName
    |> matchPersonQueryResult

let addPerson =
    json []

let getRoutes : (List<HttpHandler>) = [
        GET >=>
            choose [
                route "/people" >=> json listPeople
                routef "/people/%s" getPersonByFirstName
            ]
    ]

let postRoutes : List<HttpHandler> = [
    POST >=>
        choose [
            route "/people/add" >=> addPerson
        ]
    ]
