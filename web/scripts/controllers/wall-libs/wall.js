var wallLibs = wallLibs || {}
wallLibs.wall = function (page, restApi, authService, scope) {
    var wall = {}
    wall.api = restApi
    wall.page = page
    wall.authService = authService
    wall.scope = scope

    //Sorting defer code
    var hasSortTriggered = false;
    var doSort = function () {
        wall.posts.sort(function (a, b) {
            return b.postId - a.postId
        })
        hasSortTriggered = false;
    }

    var triggerSort = function () {
        if (!wall.hasChanges) return;
        if (!hasSortTriggered)
            setTimeout(doSort, 0)
        hasSortTriggered = true;
    }
    //POST ARRAYS
    wall.posts = []
    wall.postDict = {}
    wall.selectedPost = -1
    wall.hasChanges = false
    wall.posts.beginAddCycle = function () {
        wall.hasChanges = false
    }
    wall.posts.addPost = function (post, ignoreEditing) {
        var index = -1;
        if (wall.postDict[post.postId] != undefined)
            index = wall.postDict[post.postId].__index

        if (index == -1) {
            wall.posts.push(post)
            wall.postDict[post.postId] = post
            wall.postDict[post.postId].__index = wall.posts.length - 1
            triggerSort()
            wall.hasChanges = true
        }
        else {
            if ((wall.selectedPost == post.postId || wall.posts[index].hasUnsynced) && ignoreEditing) {
                return;
            }
            if (wall.posts[index].header != post.header) {
                wall.posts[index].header = post.header
                wall.hasChanges = true
            }
            if (wall.posts[index].attachment != post.attachment) {
                wall.posts[index].attachment = post.attachment
                wall.hasChanges = true
            }
            if (wall.posts[index].content != post.content) {
                wall.posts[index].content = post.content
                wall.hasChanges = true
            }
            if (wall.posts[index].updateTime != post.updateTime) {
                wall.posts[index].updateTime = post.updateTime
                wall.hasChanges = true
            }
            if (wall.posts[index].keyPublic != post.keyPublic) {
                wall.posts[index].keyPublic = post.keyPublic
                wall.hasChanges = true
            }
        }
    }


    wall.posts.canEdit = function (postId) {
        if (!wall.login.canEdit()) return false;
        if (wall.login.isModerator()) return true;
        var pKey = restApi.getKeys().public;
        var other = wall.posts.getPost(postId);
        if (other == undefined) return false;
        return pKey == other.keyPublic ||
            pKey == wall.login.wallInfo.ownerPublic;
    }


    //Helper to find a post
    wall.posts.getPost = function (postId) {
        return wall.postDict[postId]
    }
    //And helper to remove a post
    wall.posts.removePost = function (postId) {
        var post = wall.posts.getPost(postId)
        var index = wall.posts.indexOf(post)
        if (index > -1) {
            wall.posts.splice(index, 1);
        }
    }

    var _passCache = localStorage.getItem(page + "-password");
    //Get or set the password
    wall.password = function (value) {
        if (value != undefined) {//it's a set call 
            localStorage.setItem(page + "-password", value)
            _passCache = value
        }

        return _passCache
    }

    wall.apiPath = config.webApiBaseUrl + "api/walls/"

    wall.createApiUrl = function (appCall) {
        return wall.apiPath + appCall + "/" + page
    }

    //Fills the password, etc. post data for a call
    wall.fillCall = function (obj) {
        obj.password = wall.password()
        return obj
    }

    //LOGIN ENGINE 
    wall.login = {}
    wall.login.notAuthenticated = true;
    wall.login.needsPassword = false;
    wall.login.isWorking = false
    wall.login.isPrivate = false
    wall.login.hasRetrievedInfo = false
    wall.login.error = ""
    //Check passwords
    wall.login.checkPassword = function (password) {

        wall.login.isWorking = true
        wall.password(password)
        //Do a json call
        restApi.post(wall.createApiUrl("checkpassword"), {
            password: password
        }, {}, function (response) {
            if (response.authenticated) {
                wall.login.notAuthenticated = false
                wall.login.needsPassword = false
                wall.login.error = ""
            } else {
                wall.login.notAuthenticated = true
                wall.login.needsPassword = true
                wall.login.error = "Password incorrect."
            }
            wall.login.isWorking = false
        }, function (error) {
            wall.login.error = "Something went wrong :("
            wall.login.isWorking = false
        })
    }

    wall.login.initialize = function () {
        wall.login.isWorking = true
        //Do request to get info
        restApi.get(wall.createApiUrl("info"), {}, function (response) {
            wall.login.isWorking = false
            wall.login.wallInfo = response
            wall.login.hasRetrievedInfo = true
            wall.login.isPrivate = wall.login.wallInfo.isPrivate
            wall.login.needsPassword = wall.login.wallInfo.hasPassword; //it needs a password

            if (!response.hasPassword && !wall.login.isPrivate) {
                //No password, let them in
                wall.login.notAuthenticated = false
                return;
            } else {
                if (wall.password() == undefined ||
                    wall.password() == null)
                    return;
                wall.login.checkPassword(wall.password())
            }
        }, function (error) {
            bootbox.alert("An unknown error occurred while trying to get information.")
            wall.isWorking = false
        })
    }

    wall.login.isModerator = function () {
        if (wall.login.notAuthenticated) return false;
        if (wall.login.wallInfo.ownerPublic == getKeys().public) return true;

        var lowerPermission = false;
        var allowed = false
        wall.login.wallInfo.authorizedUsers.forEach(function (usr) {
            if (usr.keyPublic == getKeys().public && (
                usr.permissions == wallLibs.wall.permissionLevel.View ||
                usr.permissions == wallLibs.wall.permissionLevel.ViewEdit ||
                usr.permissions == wallLibs.wall.permissionLevel.None))
                lowerPermission = true;
            if (usr.keyPublic == getKeys().public &&
                usr.permissions == wallLibs.wall.permissionLevel.ViewEditModerate)
                allowed = true;
        })

        if (lowerPermission)
            return false; //Double check that they don't have a lower permission level

        if (wall.login.wallInfo.unauthorizedUsersPermission ==
            wallLibs.wall.permissionLevel.ViewEditModerate) return true;

        return allowed
    }
    wall.login.canView = function () {
        if (wall.login.notAuthenticated) return false;
        if (wall.login.wallInfo.ownerPublic == getKeys().public) return true;

        var banned = false;
        var allowed = false
        wall.login.wallInfo.authorizedUsers.forEach(function (usr) {
            if (usr.keyPublic == getKeys().public &&
                usr.permissions == wallLibs.wall.permissionLevel.None)
                banned = true;
            if (usr.keyPublic == getKeys().public && (
                usr.permissions == wallLibs.wall.permissionLevel.View ||
                usr.permissions == wallLibs.wall.permissionLevel.ViewEdit ||
                usr.permissions == wallLibs.wall.permissionLevel.ViewEditModerate))
                allowed = true;
        })
        if (banned)
            return false; //Double check to make sure they aren't banned

        if (wall.login.wallInfo.unauthorizedUsersPermission ==
            wallLibs.wall.permissionLevel.View) return true;
        if (wall.login.wallInfo.unauthorizedUsersPermission ==
            wallLibs.wall.permissionLevel.ViewEdit) return true;
        if (wall.login.wallInfo.unauthorizedUsersPermission ==
            wallLibs.wall.permissionLevel.ViewEditModerate) return true;

        return allowed
    }
    wall.login.canEdit = function () {
        if (wall.login.notAuthenticated) return false;
        if (wall.login.wallInfo.ownerPublic == getKeys().public) return true;

        var loweredPermissions = false;
        var allowed = false
        wall.login.wallInfo.authorizedUsers.forEach(function (usr) {
            if (usr.keyPublic == getKeys().public &&
                usr.permissions == wallLibs.wall.permissionLevel.None)
                loweredPermissions = true;
            if (usr.keyPublic == getKeys().public && (
                usr.permissions == wallLibs.wall.permissionLevel.ViewEdit ||
                usr.permissions == wallLibs.wall.permissionLevel.ViewEditModerate))
                allowed = true;
        })
        if (loweredPermissions)
            return false;

        if (wall.login.wallInfo.unauthorizedUsersPermission ==
            wallLibs.wall.permissionLevel.ViewEdit) return true;
        if (wall.login.wallInfo.unauthorizedUsersPermission ==
            wallLibs.wall.permissionLevel.ViewEditModerate) return true;

        return allowed
    }

    wall.relink = function () {
        setTimeout(function () {
            $('.wall-post-attachment').linkify()
        }, 0) //linkify post attachments
    }

    //Initialize
    wall.login.initialize()

    return wall
}


wallLibs.wall.permissionLevel = {
    View: "View",
    ViewEdit: "ViewEdit",
    ViewEditModerate: "ViewEditModerate",
    None: "None"
}
wallLibs.wall.wallMode = {
    Grid: "Grid",
    Freeform: "Freeform",
    Stream: "Stream"
}