var wallLibs = wallLibs || {}
wallLibs.attachmentHelper = function () {
    var _isYtLink = function (link) {
        var parser = document.createElement('a');
        parser.href = link

        if (parser.hostname.indexOf('youtube.com') != -1 && _getYtId(link) != null)
            return true;

        return false;
    }
    var _getYtId = function (url) {
        try {
            var video_id = url.split('v=')[1];
            var ampersandPosition = video_id.indexOf('&');
            if (ampersandPosition != -1) {
                video_id = video_id.substring(0, ampersandPosition);
            }
            return video_id
        }
        catch (e) {
            return null
        }
    }
    var _isVideoFile = function (link) {
        var parser = document.createElement('a');
        parser.href = link
        var hostName = parser.hostname
        var extension = ""
        if (parser.pathname.split('.').length > 1) {
            var split = parser.pathname.split('.')
            extension = '.' + split[split.length - 1].toLowerCase()
        }

        if (extension == ".mp4" || extension == ".webm" || extension == ".ogg")
            return true;
        return false;
    }
    var _isImage = function (link) {

        var parser = document.createElement('a');
        parser.href = link
        var hostName = parser.hostname
        var extension = ""
        if (parser.pathname.split('.').length > 1) {
            var split = parser.pathname.split('.')
            extension = '.' + split[split.length - 1].toLowerCase()
        }

        if (extension == ".png" || extension == ".gif" || extension == ".bmp" ||
            extension == ".jpg" || extension == ".jpeg" || extension == ".tif" ||
            extension == ".jfif" || extension == ".svg") //it's a picture 
            return true;

        return false;
    }
    return {
        isImage: _isImage,
        isYoutube: _isYtLink,
        getYoutubeVideoId: _getYtId,
        isVideo: _isVideoFile
    }
}