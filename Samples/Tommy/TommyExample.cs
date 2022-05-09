// Reference the Tommy namespace at the start of the file
using Tommy;
using System;
using System.IO;

namespace Samples.Tommy
{
    public static class TommyExample
    {
        private static readonly string _exampleTOMLFileDir = "Assets/toml-for-unity/Samples/Tommy";
        public static void ParseExample()
        {
            // Parse{ into a node
            using (StreamReader reader = File.OpenText(Path.Combine(_exampleTOMLFileDir, "configuration.toml")))
            {
                // Parse the table
                TomlTable table = TOML.Parse(reader);

                // Console.WriteLine(table["title"]); // Prints "TOML Example"
                UnityEngine.Debug.Log(table["title"]);

                // You can check the type of the node via a property and access the exact type via As*-property
                // Console.WriteLine(table["owner"]["dob"].IsDateTime); // Prints "True"
                UnityEngine.Debug.Log(table["owner"]["dob"].IsDateTime);

                // You can also do both with C# 7 syntax
                if (table["owner"]["dob"] is TomlDateTime date)
                    // Console.WriteLine(date.OnlyDate); // Some types contain additional properties related to formatting
                    UnityEngine.Debug.Log(date);

                // You can also iterate through all nodes inside an array or a table
                foreach (TomlNode node in table["database"]["ports"])
                    // Console.WriteLine(node);
                    UnityEngine.Debug.Log(node);
            }
        }

        public static void ExceptionHandleExample()
        {
            TomlTable table;
            // Parse{ into a node
            using (StreamReader reader = File.OpenText(Path.Combine(_exampleTOMLFileDir, "broken.toml")))
            {
                try
                {
                    // Read the TOML file normally.
                    table = TOML.Parse(reader);
                }
                catch (TomlParseException ex)
                {
                    // Get access to the table that was parsed with best-effort.
                    table = ex.ParsedTable;

                    // Handle syntax error in whatever fashion you prefer
                    foreach (TomlSyntaxException syntaxEx in ex.SyntaxErrors)
                        // Console.WriteLine($"Error on {syntaxEx.Column}:{syntaxEx.Line}: {syntaxEx.Message}");
                        UnityEngine.Debug.Log($"Error on {syntaxEx.Column}:{syntaxEx.Line}: {syntaxEx.Message}");
                }
            }
            // If you do not wish to handle exceptions, you can instead use 
            // TommyExtensions.TryParse().
        }

        public static void GenerateTOMLFileExample()
        {
            string TOMLFilePath = Path.Combine(_exampleTOMLFileDir, "generatedTOMLFile.toml");
            // Generate a TOML file programmatically
            TomlTable toml = new TomlTable
            {
                ["title"] = "TOML Example",
                // You can also insert comments before a node with a special property
                ["value-with-comment"] = new TomlString
                {
                    Value = "Some value",
                    Comment = "This is just some value with a comment"
                },
                // You don't need to specify a type for tables or arrays -- Tommy will figure that out for you
                ["owner"] =
                {
                    ["name"] = "Tom Preston-Werner",
                    ["dob"] = DateTime.Now
                },
                ["array-table"] = new TomlArray
                {
                    // This is marks the array as a TOML array table
                    IsTableArray = true,
                    [0] =
                    {
                        ["value"] = 10
                    },
                    [1] =
                    {
                        ["value"] = 20
                    }
                },
                ["inline-table"] = new TomlTable
                {
                    IsInline = true,
                    ["foo"] = "bar",
                    ["bar"] = "baz",
                    // Implicit cast from TomlNode[] to TomlArray
                    ["array"] = new TomlNode[] {1, 2, 3}
                }
            };


            // You can also define the toml file (or edit the loaded file directly):
            toml["other-value"] = 10;
            toml["value with spaces"] = new TomlString
            {
                IsMultiline = true,
                Value = "This is a\nmultiline string\n你好"
            };

            // Write to a file (or any TextWriter)
            // You can forcefully escape ALL Unicode characters by uncommenting the following line:
            // TOML.ForceASCII = true;
            using (StreamWriter writer = File.CreateText(TOMLFilePath))
            {
                toml.WriteTo(writer);
                // Remember to flush the data if needed!
                writer.Flush();
            }
        }

        public static void CollapsedValuesExample()
        {
            TomlTable table = new TomlTable {
                ["foo"] = new TomlTable {
                    ["bar"] = new TomlTable {
                        ["baz"] = new TomlString {
                            Value = "Hello, world!"
                        }
                    }
                }
            };
            using (StreamWriter writer = File.CreateText(Path.Combine(_exampleTOMLFileDir, "normal.toml")))
            {
                table.WriteTo(writer);
                // Remember to flush the data if needed!
                writer.Flush();
            }
            table = new TomlTable {
                ["foo"] = new TomlTable {
                    ["bar"] = new TomlTable {
                        ["baz"] = new TomlString {
                            CollapseLevel = 1, // Here we collapse the foo.bar.baz by one level
                            Value = "Hello, world!"
                        }
                    }
                }
            };
            using (StreamWriter writer = File.CreateText(Path.Combine(_exampleTOMLFileDir, "collapsed.toml")))
            {
                table.WriteTo(writer);
                // Remember to flush the data if needed!
                writer.Flush();
            }
        }
    }
}