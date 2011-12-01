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

namespace jspkgCompiler {
    class Program {
        static void Main(string[] args) {
            if (args.Length != 1) {
                Console.WriteLine("Usage: jspgkCompiler.exe <path/to/package/root>");
                Environment.Exit(1);
            }

            var root = args[0];
            var jspkg = Path.Combine(root, "jspkg");

            if(Directory.Exists(jspkg)) {
                Directory.Delete(jspkg, true);
            }
            
            Directory.CreateDirectory(jspkg);

            var packager = new JSPackager.JSPackager(root);
            foreach (var pkg in packager.Packages) {
                var path = Path.Combine(jspkg, pkg + ".js");
                using (var f = File.Create(path)) {
                    packager.WritePackage(pkg, f);
                }
            }
        }
    }
}
