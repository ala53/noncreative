//and setup the controller
var app = angular.module('app', ['ngRoute', 'vs-repeat', 'ngAnimate', 'ngSanitize', 'LocalStorageModule', 'infinite-scroll', 'angular-loading-bar']);
//In this file, we store our angularJS code.

//set the doc title to the title and app title
document.title = config.appDisplayName;
$("#navbar-brand-link").text(config.appDisplayName);
$("#navbar-brand-link").prop('title', config.appDisplayName);
document.write("<base href='" + config.appBase + "' />");

//Set up toastr
toastr.options.preventDuplicates = true;
toastr.options.progressBar = true;
toastr.options.timeOut = 15;
toastr.options.extendedTimeOut = 30;

//Set up routes
app.config(function ($routeProvider, $locationProvider, $sceProvider) {
    $routeProvider

        //Home page provider
    .when('/',
    {
        templateUrl: "views/home.html",
        controller: "homeController"
    })
        //Login provider
    .when('/login',
    {
        templateUrl: "views/login.html",
        controller: "loginController"
    })
        //Walls
        .when('/walls/:page',
    {
        templateUrl: "views/wall.html",
        controller: "wallController"
    })
        //Wall items
        .when('/walls/:page/:id',
    {
        templateUrl: "views/wall.html",
        controller: "wallController"
    })
        //Wall creator
        .when('/creator',
    {
        templateUrl: "views/creator.html",
        controller: "creatorController"
    })
        //Failsafe
    .otherwise({ redirectTo: '/' });

    //Enable HTML5 mode - if it's set up
    if (config.urlRewriteEnabled)
        $locationProvider.html5Mode(true);
});

app.config(function ($sceProvider) {
    $sceProvider.enabled(false);
})


app.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.ngEnter);
                });

                event.preventDefault();
            }
        });
    };
});


//all the controllers are in their own files
