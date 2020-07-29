# pullcdn.ps1
$dst = ".\wwwroot\lib"
mkdir -p $dst
Invoke-WebRequest -Uri https://code.jquery.com/jquery-3.5.1.slim.min.js -OutFile $dst/jquery.slim.min.js
Invoke-WebRequest -Uri 'https://cdn.jsdelivr.net/npm/popper.js@1.16.0/dist/umd/popper.min.js' -OutFile $dst/popper.min.js
Invoke-WebRequest -Uri https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/js/bootstrap.min.js -OutFile $dst/bootstrap.min.js
Invoke-WebRequest -Uri https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css -OutFile $dst/bootstrap.min.css
