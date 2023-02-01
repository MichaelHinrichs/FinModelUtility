using fin.io;

using IronPython.Hosting;
using IronPython.Runtime;

namespace ModelPluginWrappers {
  public static class BlenderProgram {
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
      var pluginDirectory = blenderDirectory.GetSubdir("io_scene_gltf2");

      var scope = engine.CreateScope();

      // Hooks up common Python imports
      engine.SetSearchPaths(new[] {
        "C:\\Users\\Ryan\\Documents\\CSharpWorkspace\\FinModelUtility\\FinModelUtility\\ModelPluginWrappers\\lib\\3.4",
        blenderDirectory.FullName,
      }.Concat(blenderDirectory.GetExistingSubdirs().Select(subdir => subdir.FullName)).ToArray());

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
          bpyPropsModule.SetVariable("CollectionProperty", Property<Array>.Create);
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