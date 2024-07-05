// src/Views.fs
module SOME.Views

open Giraffe.ViewEngine

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

let frontPage = 
    [
        main [] [
            h1 [ _class "pnp-h1" ] [ rawText "Push and Pull (P&P)" ]
            h2 [ _class "pnp-h2" ] [ rawText "Small social network for peeps"]
            div [ _class "pnp-content-box "] [
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
            span [] [ 
                p [ _class "pnp-text"] [rawText "The idea is simple: to create a simple social media web app for peeps"]
                ]
        ]
    ] |> masterView "Some.info"