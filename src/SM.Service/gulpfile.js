// ReSharper disable PossiblyUnassignedProperty

var gulp = require("gulp");
var fs = require("fs");
var bowerFiles = require("main-bower-files");
var bowerrc = JSON.parse(fs.readFileSync("./.bowerrc"));
var hosting = JSON.parse(fs.readFileSync("./hosting.json"));

gulp.task("app",
    () => {
        gulp.src("./index.html").pipe(gulp.dest(hosting.webroot));
        gulp.src("./app/*.html").pipe(gulp.dest(`${hosting.webroot}/app`));
    });

gulp.task("bower",
    () => {
        const destination = `${hosting.webroot}/lib`;
        gulp.src(bowerFiles(), { base: bowerrc.directory }).pipe(gulp.dest(destination));
        
        // Copy polymer dependecies as they are not included into bower.js main.
        gulp.src(`${bowerrc.directory}/polymer/polymer-*.html`, { base: bowerrc.directory }).pipe(gulp.dest(destination));
        gulp.src(`${bowerrc.directory}/polymer/src/**/*.html`, { base: bowerrc.directory }).pipe(gulp.dest(destination));
    });

gulp.task("default", ["app", "watch"]);

gulp.task("watch",
    () => {
        gulp.watch("./index.html", ["app"]);
        gulp.watch("./app/*.html", ["app"]);
    });

// ReSharper restore PossiblyUnassignedProperty