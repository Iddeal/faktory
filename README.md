# faktory
Insanely simple build tool

# Usage

1. Create a new .NET Framework 4.8.1 Console app
  - We recommend creating a folder called "BuildTool" in the source folder for you project and using this as the `Location` for your new console app. Select `Place solution and project in the same directory`. This keeps the build tool out of your main solution and it can be loaded and edited independently.    
2. Add `Faktory.Core` nuget package
3. Modify `Program.cs` to call into Faktory.
  - `static void Main(string[] args) { Faktory.Core.FaktoryProgram.Run(args); }`
4. Create your faktory class
```
namespace BuildTool
{
    public class MyFaktory : Faktory.Core.Faktory
    {
        
    }
}
```
5. The bare minimum required to get a working faktory is to override the `RunBuild` method.

Be sure to call .Execute() at the end of your chain.

```
protected override void RunBuild()
{            
    .Run(Clean)
    .Then(Build)
    .Execute();
}

void Clean() { // Clean }
void Build() { // Build }
```

# Options

Your Faktory can accept arguments from the command line. Arguments must be in a `key=value` form. Some examples:
```
✅ BuildTool.exe version=1.2.3.4 size=255
❌ BuildTool.exe version 1.2.3.4 255
```

All command line arguments are passed to your Faktory and made available as a KeyValuePair accessible via `Options`. All values are mapped to a `string`.
```
var version = Options['version'];
var size = Convert.ToInt32(Options['size']);
```

### Required Options
You can make some options required using the `Requires()` method. The following example marks "version" and "size" as required parameters and will result in an error if either is not provided during execution.
```
protected override void RunBuild()
{
    .Requires("version", "size")            
    .Run(Clean)
    .Then(Build)
    .Execute();
}
```

# Configuring your Faktory

If you need to perform any kind of configuration or option validation, you can do so by overriding the `Configure()` method. This is useful to set up environment for example to determine if you're running under CI or not.

This example validates the passed options and uses the `Fail()` method to instantly fail the build. Take care to use case-insensitive string comparisons if needed.

This example also sets the MSBuildPath (as of version 1 it must be set manually) using the `Config()` method. 

```
protected override void Configure()
{
    // Validate options
    if (new[] { "DEBUG", "RELEASE", }.Contains(Options["type"]) == false)
    {
        Fail("Option 'type' must be one of ['DEBUG', 'RELEASE']");
    }

    // Set the MSBuildPath
    Config.Set("MSBuildPath", @"C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\amd64\MSBuild.exe");
}
```

# More Info

Check out the https://github.com/Iddeal/faktory/wiki for more information.
