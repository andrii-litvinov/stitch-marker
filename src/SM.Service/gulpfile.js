var gulp = require("gulp");

gulp.task("app",
    () => {
        gulp
            .src("./index.html")
            .pipe(gulp.dest("./wwwroot"));

        gulp
            .src("./app/*.html")
            .pipe(gulp.dest("./wwwroot/app"));
    });

gulp.task("polymer",
    () => {
        gulp
            .src("./bower_components/polymer/polymer.html")
            .pipe(gulp.dest("./wwwroot/lib/polymer"));
        gulp
            .src("./bower_components/polymer/polymer-legacy.html")
            .pipe(gulp.dest("./wwwroot/lib/polymer"));
        gulp
            .src("./bower_components/polymer/polymer-element.html")
            .pipe(gulp.dest("./wwwroot/lib/polymer"));

        gulp
            .src("./bower_components/polymer/src/**/*.html")
            .pipe(gulp.dest("./wwwroot/lib/polymer/src"));
    });

gulp.task("default", ["app", "watch"]);

gulp.task("watch",
    () => {
        gulp.watch("./index.html", ["app"]);
        gulp.watch("./app/*.html", ["app"]);
    });