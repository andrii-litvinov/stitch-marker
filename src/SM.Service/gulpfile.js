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

gulp.task("bower", () => gulp.src(bowerFiles(), { base: bowerrc.directory }).pipe(gulp.dest(`${hosting.webroot}/lib`)));

gulp.task("default", ["app", "watch"]);

gulp.task("watch",
    () => {
        gulp.watch("./index.html", ["app"]);
        gulp.watch("./app/*.html", ["app"]);
    });

// ReSharper restore PossiblyUnassignedProperty