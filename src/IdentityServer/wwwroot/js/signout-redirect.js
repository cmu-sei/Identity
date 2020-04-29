// Copyright 2020 Carnegie Mellon University.
// Released under a MIT (SEI) license. See LICENSE.md in the project root.

window.addEventListener("load", function () {
    var a = document.querySelector("a.PostLogoutRedirectUri");
    if (a) {
        window.location = a.href;
    }
});
