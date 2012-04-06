/*
 * JSPackager v0.1
 *
 * Copyright 2011, Fog Creek Software
 * Licensed under the MIT license.
 * See LICENSE.txt
 */ 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

using JSPackager.Extensions;

namespace JSPackager {
    public class HttpHandler : IHttpHandler {
        public bool IsReusable {
            get { return false; }
        }

        private IEnumerable<string> SearchPath(string path) {
            var bits = path.Split('\\');

            for (int i = 0; i < bits.Length; i++) {
                yield return string.Join(
                    Path.DirectorySeparatorChar.ToString(),
                    bits.Skip(bits.Length - i).ToArray()
                );
            }
        }

        private string GetRoot(HttpContext context) {
            var root = context.Server.MapPath("~");
            foreach(var guess in SearchPath(Path.GetDirectoryName(context.Request.Path))) {
                var path = Path.Combine(root, guess);
                if(File.Exists(Path.Combine(path, "jspkg.json"))) {
                    return path;
                }
            }

            throw new FileNotFoundException("Could not find package spec.", "jspkg.json");
        }

        public void ProcessRequest(HttpContext context) {
            var packager = new JSPackager(GetRoot(context));
            var package = Path.GetFileNameWithoutExtension(context.Request.Path);

            context.Response.ContentType = "text/javascript";
            
            try {
                packager.WritePackage(package, context.Response.OutputStream);
            } catch (PackageDoesNotExistException ex) {
                context.Response.StatusCode = 404;
                context.AddError(ex);
            }
        }
    }

    internal class JSPackage {
        public string Name { get; set; }
        public List<string> Dependencies { get; set; }
        public List<string> Files { get; set; }

        public List<string> ResolveDependencies(IDictionary<string, JSPackage> pkgMap) {
            var result = new List<string>();
            ResolveDependencies(pkgMap, result);

            return result;
        }

        private void ResolveDependencies(IDictionary<string, JSPackage> pkgMap, List<string> result) {
            foreach (var dependency in Dependencies) {
                pkgMap[dependency].ResolveDependencies(pkgMap, result);
            }

            foreach (var file in Files) {
                if (!result.Contains(file)) {
                    result.Add(file);
                }
            }
        }
    }

    public class PackageDoesNotExistException : Exception {
        public PackageDoesNotExistException(string msg) : base("Package '{0}' does not exist.".Fmt(msg)) { }
    }

    public class JSPackager {
        private readonly string root;

        private readonly List<JSPackage> packages;
        private readonly Dictionary<string, JSPackage> pkgMap;

        public IEnumerable<string> Packages {
            get { return packages.Select(pkg => pkg.Name); }
        }

        public JSPackager(string root) {
            this.root = root;

            var parser = new JavaScriptSerializer();
            var json = File.ReadAllText(Path.Combine(root, "jspkg.json"));

            packages = parser.Deserialize<List<Dictionary<string, object>>>(json).Select(dict => new JSPackage {
                Name = (string)dict["name"],
                Files = dict.GetList<string>("files"),
                Dependencies = dict.GetList<string>("dependencies"),
            }).ToList();
            pkgMap = packages.ToDictionary(pkg => pkg.Name, pkg => pkg);
        }

        public void WritePackage(string name, Stream result) {
            var package = pkgMap.Get(name);
            if (package == null) {
                throw new PackageDoesNotExistException(name);
            }

            WritePackage(package, result);
        }

        private void WritePackage(JSPackage pkg, Stream result) {
            var files = pkg.ResolveDependencies(pkgMap).ToArray();
            var header = Encoding.ASCII.GetBytes("/* files: {0} */\r\n".Fmt(String.Join(", ", files)));

            result.Write(header, 0, header.Length);
            foreach (var file in files) {
                var src = File.ReadAllBytes(Path.Combine(root, file));
                result.Write(src, 0, src.Length);
            }
        }

        public IEnumerable<string> Files(string name) {
            var package = pkgMap[name];
            return package.ResolveDependencies(pkgMap);
        }
    }
}

