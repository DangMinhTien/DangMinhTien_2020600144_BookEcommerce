var gulp = require('gulp'),
    cssmin = require("gulp-cssmin")
rename = require("gulp-rename");
const sass = require('gulp-sass')(require('sass'));

gulp.task('default-css', function () {
    return gulp.src('wwwroot/scss/site.scss')
        .pipe(sass().on('error', sass.logError))
        .pipe(cssmin())
        .pipe(rename({
           suffix: ".min"
        }))
        .pipe(gulp.dest('wwwroot/css/'));
});
gulp.task('watch-css', function () {
    gulp.watch('wwwroot/scss/*.scss', gulp.series('default-css'))
})

// Task để theo dõi sự thay đổi của các tệp JavaScript
gulp.task('default-js', function () {
    return watch('wwwroot/js/*.js')
        .on('change', function () {
            // Thực hiện các tác vụ cần thiết khi có sự thay đổi trong các tệp JavaScript
        });
});

// Task mặc định để chạy task default-js
gulp.task('watch-js', function () {
    gulp.watch('wwwroot/js/*.js', gulp.series('default-js'))
})
gulp.task('default-css-admin', function () {
    return gulp.src('wwwroot/scss/site_admin.scss')
        .pipe(sass().on('error', sass.logError))
        .pipe(cssmin())
        .pipe(rename({
            suffix: ".min"
        }))
        .pipe(gulp.dest('wwwroot/admin/css/'));
});
gulp.task('watch-css-admin', function () {
    gulp.watch('wwwroot/scss/*.scss', gulp.series('default-css-admin'))
})

gulp.task('default-js-admin', function () {
    return watch('wwwroot/admin/js/*.js')
        .on('change', function () {
            // Thực hiện các tác vụ cần thiết khi có sự thay đổi trong các tệp JavaScript
        });
});

// Task mặc định để chạy task default-js
gulp.task('watch-js-admin', function () {
    gulp.watch('wwwroot/admin/js/*.js', gulp.series('default-js-admin'))
})