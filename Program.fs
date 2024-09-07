module SOME.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.AspNetCore.Http 
open Microsoft.Extensions.DependencyInjection
open Giraffe
open SOME.Views
open SOME.DB 

// ---------------------------------
// Models
// ---------------------------------

type Message =
    {
        Text : string
    }

// ---------------------------------
// Handlers for Dynamic Data
// ---------------------------------

// Handler to get users and posts and render the front page
let frontPageHandler (next: HttpFunc) (ctx: HttpContext) =
    let users = getUsers()  // Fetch users from DB
    let posts = getPosts()  // Fetch posts from DB
    let view = frontPage (users, posts)  // Pass the data to the frontPage view
    htmlView view next ctx  // Render the view as HTML

// Handler to allow for application to send data to user

let createUserHandler (next: HttpFunc) (ctx: HttpContext) =
    task {
        // Explicitly try to get form values by key
        let maybeUsername = ctx.GetFormValue("username")
        let maybePassword = ctx.GetFormValue("password")

        // Ensure that both username and password are provided
        match maybeUsername, maybePassword with
        | Some username, Some password ->
            // Call your DB function to create the user
            match SOME.DB.createUser username password with
            | Ok _ ->
                return! text "User created successfully!" next ctx
            | Error errMsg ->
                return! text $"Error: {errMsg}" next ctx
        | _ ->
            // Handle case where form fields are missing
            return! text "Missing username or password" next ctx
    }


// ---------------------------------
// Web app
// ---------------------------------

let webApp =
    choose [
        GET >=>
            choose [
                route "/" >=> frontPageHandler  // Use the new handler for dynamic front page
                route "/about" >=> htmlView aboutPage  // Static about page
                route "/login" >=> htmlView loginPage
                route "/signup" >=> htmlView signUpPage
            ]
        POST >=> 
            choose [
                route "/createUser" >=> createUserHandler
            ]
        setStatusCode 404 >=> text "Not Found"
    ]

// ---------------------------------
// Error handler
// ---------------------------------

let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// ---------------------------------
// Config and Main
// ---------------------------------

let configureCors (builder : CorsPolicyBuilder) =
    builder
        .WithOrigins(
            "http://localhost:5000",
            "https://localhost:5001")
       .AllowAnyMethod()
       .AllowAnyHeader()
       |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IWebHostEnvironment>()
    (match env.IsDevelopment() with
    | true  ->
        app
            .UseStaticFiles() 
            .UseDeveloperExceptionPage()
    | false ->
        app .UseGiraffeErrorHandler(errorHandler)
            .UseHttpsRedirection())
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    builder.AddConsole()
           .AddDebug() |> ignore

[<EntryPoint>]
let main args =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .UseContentRoot(contentRoot)
                    .UseWebRoot(webRoot)
                    .Configure(Action<IApplicationBuilder> configureApp)
                    .ConfigureServices(configureServices)
                    .ConfigureLogging(configureLogging)
                    |> ignore)
        .Build()
        .Run()
    0
