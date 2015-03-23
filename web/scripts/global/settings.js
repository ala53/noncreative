//This is the client settings file
//In here are all of the settings for the client side config
//Server side config is stored in web.config and settings.xml
//Server side data is stored in App_Data/storage/

var config = {
    //The base directory for the client side application, relative to the server's root path
    appBase:"/web/",
    //Allow or disallow anonymous user registration
    //You will not be able to create users if this is false
    //Make sure you also set the server side setting as false so an attacker cannot
    //craft a post to the server and 'hack' a registration.
    //Users can still be added by registered users...
    allowRegistration: true,
    //The name of the app to show in the titlebar and such.
    appDisplayName: "NonCreative",
    //The internal name of the app. Do not change.
    appName: "NonCreative-APP.01B",
    //Enable this to enable pretty urls
    //E.g. /app#/login is /app/login
    //This requires server side rewrites
    //redirect every request in /ClientApplication to /ClientApplication/index.html
    urlRewriteEnabled: false,
    //The url where the Core API is stored for authentication and actual data processing.
    webApiBaseUrl: "/",
    };