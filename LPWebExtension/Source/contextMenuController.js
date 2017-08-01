const menuItemId = "send-selection-to-cloud";

browser.contextMenus.create({
    id: menuItemId,
    title: "Send '%s' to LPCloud",
    contexts: ["selection"],
});


browser.contextMenus.onClicked.addListener((info, tab) => {
    console.error("Sent it!");

    if (info.menuItemId === menuItemId) {
        // Examples: text and HTML to be copied.
        const text = info.selectionText;
        // Always HTML-escape external input to avoid XSS.
        // const safeUrl = escapeHTML(info.linkUrl);
        // const html = `This is HTML: <a href="${safeUrl}">${safeUrl}</a>`;

        // The example will show how data can be copied, but since background
        // pages cannot directly write to the clipboard, we will run a content
        // script that copies the actual content.


        var newItem = "{'Text': 'ExtensionSentItem'}";

        var xhttp = new XMLHttpRequest();
        xhttp.open("POST", "http://localhost:19206/api/TextItem", true);
        xhttp.setRequestHeader("Content-type", "application/json");

        xhttp.onreadystatechange = function () {
            if (xhttp.readyState == 4 && xhttp.status == 200) {
                console.log(xhttp.responseText);
            }
        }

        xhttp.onerror = function (e) {
            console.error(xhr.statusText);
        };
        xhttp.send(newItem);









        var xhr = new XMLHttpRequest();
        xhr.open("GET", "http://localhost:19206/api/TextItem/2", true);
        xhr.onload = function (e) {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    console.log(xhr.responseText);
                    var response = JSON.parse(xhr.responseText);
                } else {
                    console.error(xhr.statusText);
                }
            }
        };
        xhr.onerror = function (e) {
            console.error(xhr.statusText);
        };
        xhr.send(null);





















        // clipboard-helper.js defines function copyToClipboard.
        //const code = "copyToClipboard(" +
        //    JSON.stringify(text) + "," +
        //    JSON.stringify(html) + ");";



        const code = "copyToClipboard(" +
            JSON.stringify(text) + "," +
            "" + ");";






















        browser.tabs.executeScript({
            code: "typeof copyToClipboard === 'function';",
        }).then((results) => {
            // The content script's last expression will be true if the function
            // has been defined. If this is not the case, then we need to run
            // clipboard-helper.js to define function copyToClipboard.
            if (!results || results[0] !== true) {
                return browser.tabs.executeScript(tab.id, {
                    file: "clipboard-helper.js",
                });
            }
        }).then(() => {
            return browser.tabs.executeScript(tab.id, {
                code,
            });
        }).catch((error) => {
            // This could happen if the extension is not allowed to run code in
            // the page, for example if the tab is a privileged page.
            console.error("Failed to copy text: " + error);
        });
    }
});

// https://gist.github.com/Rob--W/ec23b9d6db9e56b7e4563f1544e0d546
function escapeHTML(str) {
    // Note: string cast using String; may throw if `str` is non-serializable, e.g. a Symbol.
    // Most often this is not the case though.
    return String(str)
        .replace(/&/g, "&amp;")
        .replace(/"/g, "&quot;").replace(/'/g, "&#39;")
        .replace(/</g, "&lt;").replace(/>/g, "&gt;");
}
