module SOME.DB

open System
open Npgsql.FSharp
open BCrypt.Net

// Define the 'User' type
type User = {
    Id: int
    Username: string
    Password: string
    CreatedAt: DateTime
}

// Define the 'Post' type
type Post = {
    Id: int
    UserId: int
    Content: string
    CreatedAt: DateTime
}

// Database connection string
let connectionString = "Host=localhost;Username=postgres;Password=2406;Database=pnp"

// Fetch all users from the 'users' table
let getUsers () : User list =
    Sql.connect connectionString
    |> Sql.query "SELECT id, username, password, created_at FROM users"
    |> Sql.execute (fun read ->
        {
            Id = read.int "id"
            Username = read.text "username"
            Password = read.text "password"
            CreatedAt = read.dateTime "created_at"
        })

// Fetch all posts from the 'posts' table
let getPosts () : Post list =
    Sql.connect connectionString
    |> Sql.query "SELECT id, user_id, content, created_at FROM posts"
    |> Sql.execute (fun read ->
        {
            Id = read.int "id"
            UserId = read.int "user_id"
            Content = read.text "content"
            CreatedAt = read.dateTime "created_at"
        })

let createUser username password = 
    try 
        let hashedPassword = BCrypt.Net.BCrypt.HashPassword(password)  // Hash the password

        // Insert the user into the database
        Sql.connect connectionString 
        |> Sql.query "INSERT INTO users (username, password) VALUES (@username, @password)"
        |> Sql.parameters [ "@username", SqlValue.String username
                            "@password", SqlValue.String hashedPassword ]
        |> Sql.executeNonQuery
        |> ignore
        Ok ()
        
    with
    | ex -> Error ex.Message
