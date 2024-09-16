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
open SOME.Login

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
        let maybeUsername = ctx.GetFormValue("username")
        let maybePassword = ctx.GetFormValue("password")

        match maybeUsername, maybePassword with
        | Some username, Some password ->
            match createUser username password with
            | Ok _ ->
                return! text "User created successfully!" next ctx
            | Error errMsg ->
                return! text $"Error: {errMsg}" next ctx
        | _ ->
            return! text "Missing username or password" next ctx
    }

let loginUserHandler (next: HttpFunc) (ctx: HttpContext) = 
    task {

        // Extract username and password from form data
        let username = 
            match ctx.GetFormValue("username") with
            | Some s -> s
            | None -> ""

        let password = 
            match ctx.GetFormValue("password") with
            | Some s -> s
            | None -> ""

        // Check if the username or password is missing
        if username = "" then
            return! text "Username field is empty." next ctx
        elif password = "" then
            return! text "Password field is empty." next ctx
        else
            // Check if user exists
            match SOME.DB.getUserByUsername username with
            | Some user ->
                // Verify password
                if SOME.DB.verifyPassword user.Password password then
                    // Set session values and return success
                    ctx.Session.SetString("userId", string user.Id)
                    ctx.Session.SetString("username", user.Username)
                    return! redirectTo false "/" next ctx 
                else
                    // Incorrect password
                    return! text "Invalid username or password" next ctx
            | None ->
                // User not found
                return! text "User not found." next ctx
    }

let logoutHandler (next: HttpFunc) (ctx: HttpContext) =
    task {
        ctx.Session.Clear()  // Clear the session
        return! redirectTo false "/" next ctx  // Redirect to home page
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
                route "/logout" >=> logoutHandler
            ]
        POST >=> 
            choose [
                route "/createUser" >=> createUserHandler
                route "/loginUser" >=> loginUserHandler
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
        .UseSession()
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore
    services.AddStackExchangeRedisCache(fun options ->
            options.Configuration <- "localhost:6379"  // Configure your Redis connection
            options.InstanceName <- "SampleInstance:"   // Set a name for your Redis instance
        )
    services.AddSession( fun options -> 
        options.IdleTimeout <- TimeSpan.FromMinutes(30.0) // Session timeout of 30 minutes
        options.Cookie.HttpOnly <- true
        options.Cookie.IsEssential <- true
    ) |> ignore

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
