wallLibs = wallLibs || {}

wallLibs.wallSettings = function (wall) {
    var settings = {}
    settings.hasError = false
    settings.error = ''
    settings.newPassword = ''

    settings.apiPath = config.webApiBaseUrl + "users/authorizedUser/"

    settings.createAuthorizedUserApiUrl = function (appCall) {
        return wall.apiPath + appCall + "/" + page
    }


    settings.update = function () {
        wall.api.post(wall.createApiUrl("update"), wall.fillCall({
            title: wall.login.wallInfo.title,
            subtitle: wall.login.wallInfo.subtitle,
            requestedUrl: wall.login.wallInfo.url,
            backgroundUrl: wall.login.wallInfo.backgroundUrl,
            tileBackground: wall.login.wallInfo.tileBackground,
            unauthorizedUserPermissions: wall.login.wallInfo.unauthorizedUsersPermission
        }), {}, function (response) {
            toastr.info("Wall changes saved!");
            wall.login.wallInfo = response;
        }, function (err) {
            toastr.error("Something went wrong while trying to save. Try refreshing?");
        })
    }

    settings.addUser = { name: '', permissionLevel: 'ViewEdit' }
    settings.revokeUser = function (user) {
        wall.api.post(settings.createAuthorizedUserApiUrl("remove"), wall.fillCall({
            username: user.name,
        }), {},
            function (response) {
                toastr.info("User removed")
                wall.login.wallInfo = response;
            }, function (err) {
                toastr.error("Something went wrong.")
            })
    }

    settings.addAuthorized = function () {
        wall.api.post(settings.createAuthorizedUserApiUrl("add"), wall.fillCall({
            username: settings.addUser.name,
            permissionLevel: settings.addUser.permissionLevel
        }), {},
            function (response) {
                toastr.info("User added")
                wall.login.wallInfo = response;
            }, function (err) {
                toastr.error("Something went wrong.")
            })
    }

    settings.updateAuthorized = function (user) {
        wall.api.post(settings.createAuthorizedUserApiUrl("update"), wall.fillCall({
            username: user.name,
            permissionLevel: user.permissionLevel
        }), {},
            function (response) {
                toastr.info("User updated")
                wall.login.wallInfo = response;
            }, function (err) {
                toastr.error("Something went wrong.")
            })
    }

    settings.changePassword = function () {
        wall.api.post(wall.createApiUrl("update"), wall.fillCall({
            title: wall.login.wallInfo.title,
            subtitle: wall.login.wallInfo.subtitle,
            requestedUrl: wall.login.wallInfo.url,
            backgroundUrl: wall.login.wallInfo.backgroundUrl,
            tileBackground: wall.login.wallInfo.tileBackground,
            unauthorizedUserPermissions: wall.login.wallInfo.unauthorizedUsersPermission,
            wantedPassword: settings.newPassword
        }), {}, function (response) {
            toastr.info("Password and wall changes saved!");
            wall.password(settings.newPassword)
            wall.login.wallInfo = response;
        }, function (err) {
            toastr.error("Something went wrong while trying to save the new password. Try refreshing?");
        })
    }

    return settings;
}