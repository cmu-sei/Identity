#!/bin/sh
dst=wwwroot/lib
mkdir -p $dst
curl https://code.jquery.com/jquery-3.5.1.slim.min.js -o $dst/jquery.slim.min.js
curl https://cdn.jsdelivr.net/npm/popper.js@1.16.0/dist/umd/popper.min.js -o $dst/popper.min.js
curl https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/js/bootstrap.min.js -o $dst/bootstrap.min.js
curl https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css -o $dst/bootstrap.min.css
