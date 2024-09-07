module SOME.Login

open System
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.DependencyInjection

// Check if a user is authenticated
let isAuthenticated (ctx: HttpContext) : bool =
    match ctx.Session.GetString("userId") with 
    | s -> true
    | _ -> false

// Protect routes that require authentication
let protectedRouteHandler (next: HttpFunc) (ctx: HttpContext) =
    if isAuthenticated ctx then
        next ctx  // Allow access
    else
        // Redirect to login page if not authenticated
        redirectTo false "/login" next ctx

// Handle user logout
