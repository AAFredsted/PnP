// src/Views.fs
module SOME.Views

open Giraffe.ViewEngine
open SOME.DB
let cssPath = "main.css"

let masterView (pageTitle : string) (content : XmlNode list) = 
    html [] [
        head [] [
            meta [ _charset "utf-8" ]
            meta [ _name "viewport"; _content "width=device-width, initial-scale=1.0" ]
            meta [ _name "description"; _content "Basic SOME website for learning about asp-net in the functional context" ]
            meta [ _name "author"; _content "Andreas Andrä-Fredsted" ]

            link [ attr "href" cssPath; attr "rel" "stylesheet" ]

            title [] [ encodedText pageTitle ]
        ]
        header [] [
            nav [] [
                li [] [ a [ _href "/"] [rawText "Home"]]
                li [] [ a [ _href "/about" ] [ rawText "About" ] ]
            ]
        ]
        body [] content 
    ]

let frontPage (users: User list, posts: Post list) = 
    [
        main [] [
            h1 [ _class "pnp-h1" ] [ rawText "Push and Pull (P&P)" ]
            h2 [ _class "pnp-h2" ] [ rawText "Small social network for peeps"]
            div [ _class "push"] [
                a [ _href "/login"] [
                    button [ _class "login" ] [ rawText "Login"]
                ]
                a [ _href "/signup"] [
                    button [ _class "signUp"] [ rawText "Sign Up"]
                ]
                
            ]
            div [ _class "pnp-content-box "] [
                h3 [] [ rawText "Users" ]
                ul [] (
                    users |> List.map (fun user -> 
                        li [] [ rawText user.Username]
                    )
                )
                h3 [] [ rawText "Posts" ]
                ul [] (
                    posts |> List.map (fun post -> 
                        li [] [ 
                            rawText $"User ID: {post.UserId}"
                            rawText $" | Content: {post.Content}"
                            rawText $" | Posted at: {post.CreatedAt}"
                        ]

                    )
                )
                    

                ul [] [
                    li [] [ a [ _href "https://github.com/AAFredsted/"] [ rawText "Github" ] ]
                ]
            ]
        ]
    ] |> masterView "SOME.info"

let aboutPage = 
    [
        main [] [
            h1 [ _class "pnp-h1" ] [ rawText "About P&P"]
            div [ _class "push"] [
                button [ _class "logIn" ] [ rawText "Login"]
                button [ _class "signUp"] [ rawText "Sign Up"]
            ]
            span [] [ 
                p [ _class "pnp-text"] [rawText "The idea is simple: to create a simple social media web app for peeps"]
                ]
        ]
    ] |> masterView "Some.info"


let loginPage =
    [
        main [] [

        ]
    ] |> masterView "Some.login"


let signUpPage =    
    [
        main [] [

        ]
    ] |> masterView "Some.signup"
