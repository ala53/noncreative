//Wonderful code from http://bitoftech.net/2014/06/01/token-based-authentication-asp-net-web-api-2-owin-asp-net-identity/
'use strict';
app.controller('loginController', ['$scope', '$location', '$http', '$timeout', 'authService', function ($scope, $location, $http, $timeout, authService) {

    $scope.appDisplayName = config.appDisplayName;
    $scope.registrationEnabled = config.allowRegistration;

    $scope.loginData = {
        userName: "",
        password: "",
        useRefreshTokens: false
    }; 

    $scope.loginMessage = '';



    $scope.authentication = authService.authentication;

    $scope.logOut = function () {
        authService.logOut();
        clearKeys();
        $location.path('/login');
    }

    $scope.login = function () {
        //Don't use refresh tokens
        $scope.loginData.useRefreshTokens = false;
        authService.login($scope.loginData).then(function (response) {

            $location.path('/');

        },
         function (err) {
             $scope.loginMessage = err.error_description;
         });
    };

    $scope.authCompletedCB = function (fragment) {

        $scope.$apply(function () {

            if (fragment.haslocalaccount == 'False') {

                authService.logOut();

                authService.externalAuthData = {
                    provider: fragment.provider,
                    userName: fragment.external_user_name,
                    externalAccessToken: fragment.external_access_token
                };

                $location.path('/');

            }
            else {
                //Obtain access token and redirect to orders
                var externalData = { provider: fragment.provider, externalAccessToken: fragment.external_access_token };
                authService.obtainAccessToken(externalData).then(function (response) {

                    $location.path('/');

                },
             function (err) {
                 $scope.loginMessage = err.error_description;
             });
            }

        });


    }

    $scope.changeNameWorked = false;
    $scope.chNameMessage = "";

    $scope.saveDispName = function () {
        debugger;
        authService.updateDisplayName($scope.displayName).then(function (response) {

            $scope.changeNameWorked = true;
            $scope.chNameMessage = "Ok, you're all good.";
        },
         function (response) {
             var errors = [];
             for (var key in response.data.modelState) {
                 for (var i = 0; i < response.data.modelState[key].length; i++) {
                     errors.push(response.data.modelState[key][i]);
                 }
             }
             $scope.changeNameWorked = false;
             $scope.chNameMessage = "Sorry, we couldn't save: " + errors.join(' ');
         });
    }

    $scope.changePasswordWorked = false;
    $scope.chPassMessage = "";
    $scope.saveChangedPassword = function () {

        authService.changePassword(currentPassword, newPassword, confirmNewPassword).
            then(function (response) {
                $scope.changePasswordWorked = true;
                $scope.chPassMessage = "Ok, you're all good. You're password was changed.";

        },
         function (response) {
             var errors = [];
             for (var key in response.data.modelState) {
                 for (var i = 0; i < response.data.modelState[key].length; i++) {
                     errors.push(response.data.modelState[key][i]);
                 }
             }
             $scope.changePasswordWorked = false;
             $scope.chPassMessage = "Trying to change your password failed: " + errors.join(' ');
         });
    }


    $scope.savedSuccessfully = false;
    $scope.registrationMessage = "";

    $scope.registration = {
        userName: "",
        password: "",
        confirmPassword: ""
    };

    $scope.register = function () {

        authService.saveRegistration($scope.registration).then(function (response) {

            $scope.savedSuccessfully = true;
            $scope.registrationMessage = "You've sucessfully registered. Just log in when you're ready.";
            startTimer();

        },
         function (response) {
             var errors = [];
             for (var key in response.data.modelState) {
                 for (var i = 0; i < response.data.modelState[key].length; i++) {
                     errors.push(response.data.modelState[key][i]);
                 }
             }
             $scope.savedSuccessfully = false;
             $scope.registrationMessage = "Uh oh! Registration failed: " + errors.join(' ');
         });
    };

    var startTimer = function () {
        var timer = $timeout(function () {
            $timeout.cancel(timer);
            $location.path('/login');
        }, 2000);
    }
}]);