module PeopleRepository

open System
open Dapper
open System.Data.SqlClient
open FSharp.Control.Tasks.V2.ContextInsensitive

type PersonType =
    | IN
    | EM
    | SP
    | SC
    | VC
    | GC

let mapStringToPersonType = function
    | "IN" -> IN
    | "EM" -> EM
    | "SP" -> SP
    | "SC" -> SC
    | "VC" -> VC
    | "GC" -> GC
    | _ -> EM

[<CLIMutable>]
type Person =
    {
        BusinessEntityId: int
        PersonType: string
        Title: string
        FirstName: string
        MiddleName: string
        LastName: string
        ModifiedDate: DateTime
    }

type PersonDto =
    {
        BusinessEntityId: int
        PersonType: PersonType
        Title: string
        FirstName: string
        MiddleName: string
        LastName: string
        ModifiedDate: DateTime
    }

let mapToPersonDto (person: Person) : PersonDto =
    {
        BusinessEntityId = person.BusinessEntityId
        PersonType = mapStringToPersonType person.PersonType
        Title = person.Title
        FirstName = person.FirstName
        MiddleName = person.MiddleName
        LastName = person.LastName
        ModifiedDate = person.ModifiedDate
    }
    

let mapToPersonDtoList (person: Person list) : PersonDto list =
    person
    |> List.map mapToPersonDto
    

let connectionString = "Server=localhost,1433;Database=AdventureWorks2017;User Id=sa;Password=yourStrong(!)Password;"
let connection = new SqlConnection(connectionString)

let listPeople =
    task {
        let sql = "SELECT TOP 100 * FROM Person.Person"
        let! result = connection.QueryAsync<Person>(sql)
        return result
        |> List.ofSeq
        |> mapToPersonDtoList
    }

type PersonWithFirstNameParam = { firstName: string }

let getPersonWithFirstName (firstName: string) =
    task {
        let sql = "SELECT * FROM Person.Person WHERE FirstName = @firstName"
        try 
            let! result = connection.QueryFirstAsync<Person>(sql, { firstName = firstName })
            return Some(result)
        with
        | :? InvalidOperationException -> return None
    }
    

