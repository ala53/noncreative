'use strict';
app.controller('wallController', ['$scope', '$http', '$timeout', '$location', '$sce', '$window', '$routeParams', 'restApi', 'authService',
    function ($scope, $http, $timeout, $location, $sce, $window, $routeParams, restApi, authService) {
        $scope.trustUrl = $sce.trustAsResourceUrl
        //Get the wall info
        var wall = new wallLibs.wall($routeParams.page, restApi, authService, $scope)
        $scope.login = wall.login
        $scope.posts = wall.posts
        $scope.edit = new wallLibs.editor(wall)
        $scope.loader = new wallLibs.loader(wall, $scope.edit)
        $scope.attachHelp = new wallLibs.attachmentHelper()
        $scope.sidebar = new wallLibs.sidebar(wall)
        $scope.settings = new wallLibs.wallSettings(wall)
        $scope.wall = wall
        $scope.prettyDate = humaneDate

        $scope.passwordInfo = {
            password: ""
        }
        $scope.checkPassword = function () {
            console.log($scope.passwordInfo.password)
            wall.login.checkPassword($scope.passwordInfo.password)
        }

        $scope.loader.initPostPoller()

        $scope.authentication = authService.authentication
        exportwall = $scope
    }]);

var exportwall