'use strict';
app.controller('navbarController', ['$scope', '$location', 'authService', function ($scope, $location, authService) {

    $scope.logOut = function () {
        authService.logOut();
        $location.path('/login');
    }

    $scope.authentication = authService.authentication;
}]);
