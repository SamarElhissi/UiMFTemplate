/* eslint-disable no-console */

/*
Available commands:
1. `gulp watch`
2. `gulp watch --minify`
3. `gulp build`
*/

// Compilation process:
// 1. build svelte components and copy them to "./build" (the build is done with typescript compilation)
// 2. compile compile-app-ts and move it to "../wwwroot/js/app.js"
// 3. compile all ["scr/**/*.scss", "src/**/*.css"] files and move them to "../wwwroot/css/main.css"
// 4. move all files from "./wwwroot" to "../wwwroot".

const gulp = require('gulp');
const rollup = require('rollup');
const del = require('del');
const terser = require("rollup-plugin-terser").terser;
const { argv } = require('yargs');
const inject = require('gulp-inject');
const svelte = require("rollup-plugin-svelte");
const sveltePreprocessor = require("svelte-preprocess");
const postcss = require("rollup-plugin-postcss");
const resolve = require("@rollup/plugin-node-resolve");
const commonjs = require("@rollup/plugin-commonjs");
const typescript = require("@rollup/plugin-typescript");
const builtins = require("rollup-plugin-node-builtins");
const globals = require("rollup-plugin-node-globals");
const json = require("@rollup/plugin-json");
const sass = require("gulp-sass");
const concat = require("gulp-concat");
const hash = require("gulp-hash-filename");
const cleanCSS = require("gulp-clean-css");
const sourcemaps = require("gulp-sourcemaps");
const merge2 = require("merge2");

distDir = "../wwwroot",
svelteComponentsDir = "build/svelte";
const prod = !!argv.prod;
const minify = !!argv.minify;

process.on("unhandledRejection", r => console.log(r)); // eslint-disable-line no-console

gulp.task("cleanJs", () => del(`${distDir}/js/**`, { force: true }));
gulp.task("cleanCss", () => del(`${distDir}/css/**`, { force: true }));

gulp.task("clean", () => del("build", { force: true }));

gulp.task(
	"compile-app-ts",
	gulp.series("cleanJs", async function compileTypsceipt () {
	  const plugins = [
		svelte({
		  dev: !prod,
		  extensions: [".svelte"],
		  preprocess: sveltePreprocessor(),
		  emitCss: true,
		  onwarn: function (warning) {
			return;
		  }
		}),
		postcss({
		  extract: true,
		}),
		resolve({
		  jsnext: true,
		  main: true,
		  browser: true,
		  preferBuiltins: true,
		}),
		commonjs({ include: "node_modules/**", extensions: [".js", ".ts"] }),
		typescript({
		  tsconfig: "tsconfig.json",
		}),
		globals(),
		builtins(),
		json()
	  ];
  
	  if (minify) {
		plugins.push(terser());
	  }
  
	  if (prod) {
		plugins.push(terser({ sourcemap: true }));
	  }
  
	  const bundle = await rollup.rollup({
			  input: "src/App.ts",
			  plugins,
			  onwarn: function (warning) {
				  return;
			  }
	  });
	  
	  bundle.warn
  
	  return bundle.write({
			  sourcemap: true,
			  format: "iife",
			  dir: `${distDir}/js/`,
			  entryFileNames: prod ? "bundle.[hash].js" : "bundle.js",
		  });
	})
  );

gulp.task("copy-assets", () => {
	const copyAssets = gulp
		.src("wwwroot/assets/**")
		.pipe(gulp.dest(`${distDir}/assets`));
	const copyLibs = gulp
		.src("wwwroot/bootstrap/**")
		.pipe(gulp.dest(`${distDir}/css/bootstrap`));
	const copyFonts = gulp
		.src("wwwroot/fonts/**")
		.pipe(gulp.dest(`${distDir}/css/fonts`));

	return merge2([copyAssets, copyLibs, copyFonts]);
});

gulp.task(
	"sass",
	gulp.series("cleanCss", () => gulp
		.src(["src/**/*.scss", "src/**/*.css"])
		.pipe(sass().on("error", sass.logError))
		.pipe(concat("main.css"))
		.pipe(hash({
			format: "{name}.{hash}.{size}{ext}"
		}))
		.pipe(cleanCSS({ compatibility: "*" }))
		.pipe(sourcemaps.write())
		.pipe(gulp.dest(`${distDir}/css/`)))
);


gulp.task(
	"inject-js",
	gulp.series("compile-app-ts", function injectJs() {
		const target = gulp.src(`${distDir}/index.html`);
		// It's not necessary to read the files (will speed up things), we're only after their paths:
		const sources = gulp.src([`./${distDir}/js/*.js`], { read: false });
		return target
			.pipe(
				inject(sources, {
					addRootSlash: false,
					ignorePath: "../wwwroot/"
				})
			)
			.pipe(gulp.dest(`${distDir}`));
	})
);

gulp.task(
	"inject-css",
	gulp.series("sass", "copy-assets", () => {
		const target = gulp.src(`${distDir}/index.html`);
		// It's not necessary to read the files (will speed up things), we're only after their paths:
		const sources = gulp.src([`./${distDir}/css/*.css`], { read: false });
		return target
			.pipe(inject(sources, { addRootSlash: false, ignorePath: "../wwwroot/" }))
			.pipe(gulp.dest(`${distDir}`));
	})
);

function watching() {
	gulp.watch(["src/**/*.scss", "src/**/*.css"], gulp.parallel("inject-css"));
	gulp.watch("wwwroot/**", gulp.series("inject-js", "inject-css"));
	gulp.watch(["src/**", "gulpfile.js"], gulp.parallel("compile-app-ts"));
}

watching.description = "watching";

gulp.task(
	"watch",
	gulp.series("inject-js", "inject-css", "copy-assets", watching)
);

function building() {
	gulp.series("inject-js", "inject-css", "copy-assets");
}

building.description = "building";

gulp.task("build", gulp.series(building));
