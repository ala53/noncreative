//Enable strict mode
"use strict";


//This is just the app bootstrap. All it does is load the application's JS files.
//This has the advantage of only referencing one script in the header
//which means that we can pack all the js into one file for release



//Credit http://trevweb.me.uk/javascript-include/

// Essentially 'new XMLHttpRequest()' but safer.
function newXmlHttpRequestObject() {
    try {
        if (window.XMLHttpRequest) {
            return new XMLHttpRequest();
        }
            // Ancient version of IE (5 or 6)?
        else if (window.ActiveXObject) {
            return new ActiveXObject("Microsoft.XMLHTTP");
        }

        throw new Error("XMLHttpRequest or equivalent not available");
    }
    catch (e) {
        throw e;
    }
}

// Synchronous file read. Should be avoided for remote URLs.
function getUrlContentsSynch(url) {
    try {
        var xmlHttpReq = newXmlHttpRequestObject();
        xmlHttpReq.open("GET", url, false); // 'false': synchronous.
        xmlHttpReq.send(null);

        if (xmlHttpReq.status == 200) {
            return xmlHttpReq.responseText;
        }

        throw new Error("Failed to get URL contents");
    }
    catch (e) {
        throw e;
    }
}

function include(filePath) {
    var headElement = document.getElementsByTagName("head")[0];
    var newScriptElement = document.createElement("script");

    newScriptElement.type = "text/javascript";
    newScriptElement.text = getUrlContentsSynch(filePath);
    headElement.appendChild(newScriptElement);
}

function addTag(name, attributes, sync) {
    var headEl = document.getElementsByTagName("head")[0];
    var el = document.createElement(name),
        attrName;

    for (attrName in attributes) {
        el.setAttribute(attrName, attributes[attrName]);
    }

    sync ? document.write(outerHTML(el)) : headEl.appendChild(el);
}


function bindEvent(element, type, handler) {
    if (element.addEventListener) {
        element.addEventListener(type, handler, false);
    } else {
        element.attachEvent('on' + type, handler);
    }
}


//Show an error on ie8 or less
if (!document.addEventListener) {
    alert("Internet Explorer 9 or greater is required to view this website.");
}

//And on FF3 or less (Gecko Engine 1.9.2 or less)
if (navigator.userAgent.toLowerCase())
    if (parseFloat(navigator.appVersion.substr(0, 3)) <= 1.9)
        alert("Firefox 4 or above is required to view this website.");


bindEvent(window, 'load', loadScripts);

function loadScripts() {
}