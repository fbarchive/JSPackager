# JSPackager

> JSPackager is a tool for combining multiple JavaScript files together to reduce the number of requests needed to fetch all of the JavaScript for a website.

## Defining Packages

Packages are defined in a config file named **jspkg.json** which should contain a list of objects that have `name` and `files` attributes with an optional `dependencies` attribute. For example:

```json
[
  {
    "name": "Core",
    "files": ["jquery-1.7.1.js", "jquery-ui-1.8.15.js"]
  },
  {
    "name": "Home",
    "dependencies": ["Core"],
    "files": ["home.js"]
  },
  {
    "name": "LogIn",
    "dependencies": ["Core"],
    "files": ["login.js"]
  }
]
```

## Dynamic Packaging

JSPackager include an HttpHandler that dynamically generates packages to facilitate development.

You can configure it in the **system.webServer/handler**s section of your **web.config** like so:

```xml
<add name="JSPackagerHttpHandler" 
     type="JSPackager.HttpHandler,JSPackager" 
     preCondition="integratedMode" 
     path="*.jspkg" verb="*" 
/>
```

Once configured, you can reference your JS packages with a **.jspkg** extension and they will be automatically handled by JSPackager:

```html
<script src="Login.jspgk"></script>
```

The HttpHandler assumes that **jspkg.json** is in the root of your **Website/** directory and that all of the paths specified in `files` attributes are relative to **Website/**.


## Compile-time Packaging

JSPackager also includes a utility called **jspkgCompiler.exe** that you can use to build packages without invoking ASP.NET:

```powershell
jspkgCompiler.exe your\project\Website
```

... which will generate one JavaScript file per package and store them in jspkg. Assuming the previous configuration this would produce:

```
jspkg/
  Core.js
  Home.js
  LogIn.js
```

... which you can include in your HTML pages normally.


## Contributing

The source is available at: https://developers.kilnhg.com/Repo/Miscellaneous/Group/JSPackager

You're welcome to make a clone and start hacking ;-)
