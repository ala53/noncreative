'use strict';
app.controller('creatorController', ['$scope', '$location', '$timeout', 'restApi', 'authService', function ($scope, $location, $timeout, restApi, authService) {
    $scope.createWorked = true;
    $scope.createMessage = ''
    $scope.permissions = 'ViewEdit'
    $scope.url = (function () {
        var text = "";
        var possible = "abcdefghijklmnopqrstuvwxyz0123456789";

        for (var i = 0; i < 20; i++)
            text += possible.charAt(Math.floor(Math.random() * possible.length));

        return text;
    })()
    $scope.title = "My wall"
    $scope.subtitle = "Filled with magic!"
    $scope.password = ''

    $scope.tryCreate = function () {
        restApi.post(config.webApiBaseUrl + "api/walls/create", {
            title: $scope.title,
            subtitle: $scope.subtitle,
            requestedUrl: $scope.url,
            wantedPassword: $scope.password,
            unauthorizedUserPermissions: $scope.permissions
        }, {},
            function (response) {
                $location.path('/walls/' + $scope.url)
            }, function (error) {
                $scope.createWorked = false
                $scope.createMessage = "Something is wrong with the application";
            })
    }
}])