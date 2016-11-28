var gulp = require("gulp");

gulp.task("default", function () {
    gulp
        .src("./html/*.html")
        .pipe(gulp.dest("./wwwroot"));
});