var wallLibs = wallLibs || {}
wallLibs.loader = function (wall, editor) {
    var restApi = wall.api
    var loader = {}
    loader = {}
    loader.loading = false
    loader.hasMore = true
    loader.errored = false
    loader.nextToLoad = 0
    loader.loadCount = 15
    loader.__defineGetter__("busy", function () {
        return loader.loading ||
        loader.errored || wall.login.notAuthenticated
    })

    loader.getNext = function () {
        if (loader.busy) return;
        var info = toastr.info("Loading, please wait.")
        loader.loading = true
        restApi.post(wall.createApiUrl("posts"), wall.fillCall(
            {
                beginning: loader.nextToLoad,
                count: loader.loadCount
            }), {},
            function (response) {
                wall.posts.beginAddCycle()
                response.forEach(wall.posts.addPost);
                if (response.length == 0) loader.hasMore = false
                loader.loading = false
                loader.errored = false
                loader.nextToLoad += loader.loadCount
                toastr.clear(info)
                wall.relink()
            }, function (error) {
                loader.errored = true
                loader.loading = false
                toastr.error("Something went wrong while trying to get the new posts.")
            })
    }

    loader.newPostPoller = function () {
        if (editor.isEditing) return; //Don't update while editing
        if (loader.busy) return;
        var focused = document.activeElement.id
        loader.loading = true
        restApi.post(wall.createApiUrl("posts"), wall.fillCall(
            {
                beginning: 0,
                count: 10
            }), {},
            function (response) {
                wall.posts.beginAddCycle()
                response.forEach(function (post) {
                    wall.posts.addPost(post, true)
                });
                loader.loading = false
                loader.errored = false
                if (wall.hasChanges)
                    wall.relink()
            }, function (error) {
                loader.errored = true
                loader.loading = false
                toastr.error("Something went wrong while trying to update the latest posts")
            })

        setTimeout(loader.newPostPoller, 15000)
    }

    loader.initPostPoller = function (scope) {
        setTimeout(loader.newPostPoller, 15000)
    }

    return loader
}