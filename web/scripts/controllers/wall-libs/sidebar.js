var wallLibs = wallLibs || {}
wallLibs.sidebar = function (wall) {
    var sidebar = {}
    sidebar.collapsed = true

    sidebar.toggleCollapsed = function () {
        sidebar.collapsed = !sidebar.collapsed
    }

    return sidebar
}