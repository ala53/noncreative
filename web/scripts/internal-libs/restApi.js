'use strict';
app.factory('restApi', ['$http', '$q', '$location', 'authService', function ($http, $q, $location, authService) {

    var restApi = {};

    var post = function (url, data, headers, ok, error) {
        url = url + "?t=" + Math.random() //Kill caching
        if (headers == null || headers == undefined)
            headers = {}
        if (data == null || data == undefined)
            data = {}

        headers['Content-Type'] = 'application/json';
        headers['Accept'] = 'application/json';
        if (authService.authentication.isAuth) {
            headers['Authorization'] = 'Bearer ' + authService.authentication.accessToken;
            //Set the keys in the data
            data.keyPrivate = getKeys(authService.authentication.accessToken).private
            data.keyPublic = getKeys(authService.authentication.accessToken).public
        }
        else {
            //Set the keys in the data
            data.keyPrivate = getKeys(undefined).private
            data.keyPublic = getKeys(undefined).public
        }

        var req = {
            method: "POST",
            url: url,
            headers: headers,
            data: data
        }
        $http(req)
            .success(
            function (response) {
                ok(response)
            })
            .error(
            function (err, status) {
                error(err, status)
            });
    }

    var get = function (url, headers, ok, error) {
        url = url + "?t=" + Math.random() //Kill caching
        if (headers == null || headers == undefined)
            headers = {}

        headers['Content-Type'] = 'application/json';
        headers['Accept'] = 'application/json';
        if (authService.authentication.isAuth) {
            headers['Authorization'] = 'Bearer ' + authService.authentication.accessToken;
        }
        var req = {
            method: "GET",
            url: url,
            headers: headers
        }
        $http(req)
            .success(
            function (response) {
                ok(response)
            })
            .error(
            function (err, status) {
                error(status)
            });
    }

    restApi.post = post;
    restApi.get = get;
    restApi.getKeys = function () {
        return getKeys(authService.accessToken)
    }

    restApi.clearKeys = function () {
        clearKeys()
    }

    Object.freeze(restApi);


    return restApi;
}]
);

var _keyCache = JSON.parse(localStorage.getItem("authKeys"))
function getKeys(token) {
    var keys;

    if (_keyCache != null && _keyCache != undefined && (token == undefined || token == _keyCache.token))
        return _keyCache;

    try {
        var unparsed = localStorage.getItem("authKeys");
        //If no keys or the keys are not for this user
        if (unparsed == null || unparsed == undefined) {
            //POST to server
            jQuery.ajax({
                type: 'GET',
                beforeSend: function (request) {
                    request.setRequestHeader("Authorization", "Bearer " + token);
                },
                url: config.webApiBaseUrl + "api/key",
                success: function (result) {
                    keys = {
                        'public': result.keyPublic,
                        'private': result.keyPrivate,
                        token: token
                    }
                    _keyCache = keys;
                    localStorage.setItem("authKeys", JSON.stringify(keys))
                },
                failure: function (result) {
                    bootbox.dialog("A severe error occurred (getting authentication failed).")
                },
                async: false
            });
        }
        else {
            keys = JSON.parse(localStorage.getItem("authKeys"))
        }
        //And return
        return keys
    }
    catch (e) {
        //Clear the cache and call again
        clearKeys()
        return getKeys(token)
    }
}

function clearKeys() {
    localStorage.removeItem("authKeys")
}