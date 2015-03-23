var wallLibs = wallLibs || {}
wallLibs.editor = function (wall) {
    var editor = {}
    var restApi = wall.api
    editor.add = function () {
        if (!wall.login.canEdit()) return;
        //create create an empty post on the server and then edit
        restApi.post(wall.createApiUrl("addpost"), wall.fillCall({
            header: "",
            content: "",
            attachmentUrl: ""
        }), {},
            function (res) {
                wall.posts.addPost(res)
                editor.selectOrEdit(res.postId)
                setTimeout(function () {
                    //Select the title

                }, 0)
            }, function (err) {
                toastr.error("Something went wrong while trying to add the post.");
            })
    }

    editor.selected = -1

    editor.selectOrEdit = function (postId) {
        editor.selected = postId
        wall.selectedPost = postId
        if (postId == -1) return;
        if (!editor.canEdit(postId)) {
            //ignore the filthy pleb's attempt to edit...
            return;
        }
        //And redirect
        editor.toggleEditing(true);
        editor.createEditorIfNotExists(postId);
    }

    editor.toggleEditing = function (value) {
        if (value != undefined)
            editor.isEditing = value
        else
            editor.isEditing = !editor.isEditing

    }


    editor.isPostEditing = function (postId) {
        return editor.isEditing && (editor.selected == postId)
    }

    editor.delete = function (postId) {
        if (!editor.canEdit(postId)) {
            toastr.error("Sorry, you can't edit that post.");
            return;
        }
        bootbox.confirm("Are you sure you want to delete the post?", function (result) {
            if (!result) return;
            //Delete the post
            restApi.post(wall.createApiUrl("deletepost"), wall.fillCall({
                postId: postId
            }), {},
                function (response) {
                    wall.posts.removePost(postId) //Delete it
                }, function (error) {
                    toastr.error("An error occured while trying to delete the post...");
                })
        })
    }

    //the function to be called when a post's content is changed
    editor.change = function (postId) {
        if (!editor.canEdit(postId)) return; //Ignore changes we don't authorize
        //Otherwise, add to the queue if it isn't in there already
        var post = wall.posts.getPost(postId)
        post.hasUnsynced = true
        //Sanitize new lines
        post.header = post.header.replace(/\r?\n|\r/g, "");
        post.attachment = post.attachment.replace(/\r?\n|\r/g, "");
        //And add to queue if needed
        if (!post.hasActiveQueue) {
            setTimeout(function () {
                editor.uploadEdits(postId)
                post.hasActiveQueue = false
            }, 10000)
            post.hasActiveQueue = true
        }
    }

    editor.forceSyncChanges = function (id) {
        if (!editor.canEdit(id)) return;
        if (id == undefined || id == -1) return;
        setTimeout(function () {
            $('#content-post-' + id).linkify()
            $('#attachment-post-' + id).linkify()
        }, 0) //linkify post
        editor.uploadEdits(id)
    }

    editor.uploadEdits = function (id) {
        var item = wall.posts.getPost(id)
        if (item == undefined) return;
        item.hasUnsynced = false
        restApi.post(wall.createApiUrl("editpost"), wall.fillCall({
            postId: item.postId,
            newHeader: item.header,
            newContent: item.content,
            newAttachment: item.attachment
        }), {}, function (response) {

        }, function (err) {
            toastr.error("Something went wrong while trying to edit the post")
        })
    }

    editor.canEdit = function (postId) {
        return wall.posts.canEdit(postId)
    }

    editor.createEditorIfNotExists = function (postId) {
        var post = wall.posts.getPost(postId)
        if (post._editor == null)
            post._editor = new MediumEditor('#content-post-' + post.postId,
                { 'forcePlainText': false, 'placeholder': 'Content' })
    }

    return editor
}
