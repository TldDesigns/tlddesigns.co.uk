/// <binding AfterBuild='prepareCSP' />
/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
//var critical = require('critical');
var critical = require('critical').stream;
//var rev = require('gulp-rev-hash');
var cacheBuster = require('gulp-cache-bust');
var gulpRename = require('gulp-rename');
var htmlMin = require('gulp-htmlmin');
var plumber = require('gulp-plumber');
//var processHtml = require('gulp-dom');
//var domProcessor = require('gulp-dom-processor');
var cheerio = require('gulp-cheerio');

//var htmlPath = ['/**/*.html', '!/fonts/**/*.html', '!/bin/**/*.html', '!/My Project/**/*.html', '!/node_modules/**.*.html', '!/obj/**/*.html', '!/Properties/**/*.html'];
var htmlPath = ['./*.html', '!./*.min.html', './*/*.html', '!./*/*.min.html'];
var htmlMinPath = ['./*.min.html', './*/*.min.html'];

gulp.task('default', function () {
    // place code for your default task here
    console.log('Build started');
});

gulp.task('cacheBuster', ['minHtml'], function () {
    return gulp.src(htmlMinPath)
    .pipe(cacheBuster())
    //.pipe(gulpRename('index2.html'))
    .pipe(gulp.dest('.'));
});

gulp.task('critical', ['cacheBuster'], function (cb) {
    //gulp.task('critical', function (cb) {
    //critical.generate({
    //    inline: true,
    //    base: '.',
    //    src: 'index.min.html',
    //    dest: './index.min.html',
    //    minify: true//,
    //    //width: 320,
    //    //height: 480
    //});

    //return gulp.src(htmlPath)
    //.pipe(critical.generate({
    //    inline: true,
    //    minify: true//,
    //    //width: 320,
    //    //height: 480
    //}))
    //.pipe(gulp.dest('.'));

    return gulp.src(htmlMinPath)
    //return gulp.src('./*/*.min.html')
    .pipe(plumber())
    .pipe(critical({
        base: '.',
        inline: true,
        minify: true//,
        //width: 320,
        //height: 480
    }))
    .pipe(gulp.dest('.'));
});


gulp.task('minHtml', ['default'], function () {
    //return gulp.src(['./**/*.html', '!./fonts/**/*.html', '!./bin/**/*.html', '!./My Project/**/*.html', '!./node_modules/**.*.html', '!./obj/**/*.html', '!./Properties/**/*.html'])
    //return gulp.src(['./*/*.html', '!./*/*.min.html'])
    //return gulp.src(['./*.html', '!./*.min.html', './*/*.html', '!./*/*.min.html'])
    return gulp.src(htmlPath)
    .pipe(htmlMin({ collapseWhitespace: true, removeComments: true }))
    //.pipe(gulpRename('index.min.html'))
        .pipe(gulpRename(function (path) {
            path.basename += '.min'
            return path;
        }))
    .pipe(gulp.dest('.'));
})

//gulp.task('rev', function () {
//    gulp.src('./index.html')
//    .pipe(rev({ assetsDir: 'css/' }))
//    .pipe(gulp.dest('./index.new.html'));
//});

//gulp.task('prepareCSP', ['critical'], function () {
//gulp.task('prepareCSP', function () {
//    return gulp.src(htmlMinPath)
//    .pipe(processHtml(function () {
//        var onloadNodes = this.querySelectorAll("[onload]");

//        onloadNodes.forEach(function (values) {
//            values.onload = null;
//        })

//        return this;
//    }))
//});

////gulp.task('prepareCSP', function () {
//gulp.task('prepareCSP', ['critical'], function () {

//    var nonceSecret = "secret1";

//    var processorConfig = {
//        load: function (filePath) {
//            return [{
//                selector: '[onload]',
//                replace: function ($element) {
//                    $element.attr('onload', null);
//                    return $element;
//                }
//            },
//             {
//                selector: "link[rel='stylesheet']",
//                replace: function ($element) {
//                    $element.attr('data-nonceSecret', nonceSecret);
//                    return $element;
//                }
//            },
//            {
//                selector: "script:not([src])",
//                replace: function ($element) {
//                    $element.attr('data-nonceSecret', nonceSecret);
//                    return $element;
//                }
//            },
//            {
//                selector: "script[async]",
//                replace: function ($element) {
//                    $element.attr('data-nonceSecret', nonceSecret);
//                    return $element;
//                }
//            },
//            {
//                selector: "link[as='style']",
//                replace: function ($element) {
//                    $element.attr('data-nonceSecret', nonceSecret);
//                    return $element;
//                }
//            },
//            {
//                selector: "style",
//                replace: function ($element) {
//                    $element.attr('data-nonceSecret', nonceSecret);
//                    return $element;
//                }
//            }
//            ,
//            //{
//            //    selector: "script:not([async])",
//            //    replace: function ($element) {
//            //        $element.attr('data-nonceSecret', nonceSecret);
//            //        return $element;
//            //    }
//            //}
//            //,
//            {
//                selector: "script[src]:not([async])",
//                replace: function ($element) {
//                    $element.attr('data-nonceSecret', nonceSecret);
//                    return $element;
//                }
//            }
//            ]
//        }
//    }


//    return gulp.src(htmlMinPath)
//        .pipe(plumber())
//        .pipe(domProcessor(processorConfig))
//        .pipe(gulp.dest('.'))
//    ;
//})

//gulp.task('prepareCSP', function () {
gulp.task('prepareCSP', ['critical'], function () {
    //var nonceSecret = "secret1"; 

    var nonceSecret = process.env.nonce_secret; //This secret should be kept secure, treat like a password, DO NOT publish to source control
        
    return gulp.src(htmlMinPath)
            .pipe(plumber())
            .pipe(cheerio(function ($, file) {

                //Remove inline event handlers

                $('[onload]').each(function () {
                    var onload = $(this);
                    onload.attr('onload', null);
                })

                //Add nonce secret for server-side processing

                $("style, link[rel='stylesheet'], link[as='style'], script").each(function () {
                    var element = $(this);

                    element.attr('data-nonceSecret', nonceSecret);
                })

            }))
            .pipe(gulp.dest('.'));
})