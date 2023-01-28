using fin.io;

using IronPython.Hosting;
using IronPython.Runtime;

namespace ModelPluginWrappers {
  public static class Program {
    private static void Get_(string url, dynamic param, dynamic headers = null) {
      ;
    }

    private class Property<T> {
      public Property(string name, string description, T @default) {
        ;
      }

      public static Property<T> Create(string name, string description, T @default)
        => new Property<T>(name, description, @default);
    }

    public static void Main() {
      var engine = Python.CreateEngine();

      var blenderDirectory = new FinDirectory("C:\\Users\\Ryan\\Documents\\CSharpWorkspace\\FinModelUtility\\FinModelUtility\\ModelPluginWrappers\\blender");
      var pluginDirectory = blenderDirectory.GetSubdir("io_sketchfab_plugin");

      var scope = engine.CreateScope();
      scope.SetVariable("__path__", new PythonList {
        blenderDirectory.FullName,
      });

      // Hooks up common Python imports
      engine.SetSearchPaths(new[] {
        "C:\\Users\\Ryan\\Documents\\CSharpWorkspace\\FinModelUtility\\FinModelUtility\\ModelPluginWrappers\\lib\\3.4",
        blenderDirectory.FullName,
      });

      // Hooks up missing Python imports
      {
        var requestsModule = engine.CreateModule("requests");
        requestsModule.SetVariable("get", Get_);
      }

      // Hooks up Blender imports
      {
        var bpyModule = engine.CreateModule("bpy");

        var bpyPropsModule = engine.CreateModule("bpy.props");

        {
          bpyPropsModule.SetVariable("StringProperty", Property<string>.Create);
          bpyPropsModule.SetVariable("BoolProperty", Property<bool>.Create);
          bpyPropsModule.SetVariable("IntProperty", Property<int>.Create);
          bpyPropsModule.SetVariable("PointerProperty", Property<nint>.Create);
          bpyPropsModule.SetVariable("EnumProperty", Property<int>.Create);
        }

        var bpyUtilsModule = engine.CreateModule("bpy.utils");
        var bpyUtilsPreviewsModule = engine.CreateModule("bpy.utils.previews");
      }

      /*var customModule = engine.CreateModule("custom");
      customModule.SetVariable("foo", () => {
        ;
      });

      engine.Execute(@"
import custom

custom.foo()
        ");*/

      //var initPy = pluginDirectory.GetExistingFile("__init__.py");
      //engine.ExecuteFile(initPy.FullName, scope);

      engine.Execute($@"
import {pluginDirectory.Name}

{pluginDirectory.Name}.register()
", scope);
    }
  }
}