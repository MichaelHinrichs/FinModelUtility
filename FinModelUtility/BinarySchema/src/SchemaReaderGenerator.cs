using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using schema;
using schema.text;

namespace schema {
  [Generator(LanguageNames.CSharp)]
  internal class SchemaReaderGenerator : ISourceGenerator {
    private readonly Type schemaAttributeType_ = typeof(SchemaAttribute);
    private readonly SchemaStructureParser parser_ = new();

    private void Generate_(ISchemaStructure structure) {
      var typeSymbol = structure.TypeSymbol;

      var typeName = typeSymbol.Name;
      var typeNamespace = SymbolTypeUtil.MergeContainingNamespaces(typeSymbol);

      var isTypeClass = typeSymbol.TypeKind == TypeKind.Class;
      var isTypeAbstract = typeSymbol.IsAbstract;

      var declaringTypes =
          SchemaReaderGenerator.GetDeclaringTypesDownward_(typeSymbol);
      var symbolType = (isTypeAbstract ? "abstract " : "") +
                       "partial " +
                       (isTypeClass ? "class" : "struct");

      var cbsb = new CurlyBracketStringBuilder();
      cbsb.WriteLine("using System.IO;");

      // TODO: Handle fancier cases here
      cbsb.EnterBlock($"namespace {typeNamespace}");
      foreach (var declaringType in declaringTypes) {
        cbsb.EnterBlock(
            $"{SchemaReaderGenerator.GetSymbolQualifiers_(declaringType)} {typeName}");
      }
      cbsb.EnterBlock(
          $"{SchemaReaderGenerator.GetSymbolQualifiers_(typeSymbol)} {typeName}");

      cbsb.EnterBlock("public void Read(EndianBinaryReader er)");
      foreach (var field in structure.Fields) {
        var indexName = $"this.{field.Name}[i]";
        var eachName = $"{field.Name}Instance";

        var isValuePrimitive =
            field.PrimitiveType != SchemaPrimitiveType.UNDEFINED;

        var valueName = field.IsArray
                            ? !isValuePrimitive ? eachName : indexName
                            : $"this.{field.Name}";

        if (field.IsArray) {
          if (!field.HasConstLength) {
            var fieldTypeName = field.TypeSymbol.Name;
            var fieldTypeNamespace =
                SymbolTypeUtil.MergeContainingNamespaces(field.TypeSymbol);

            var countField = field.LengthField;

            cbsb.EnterBlock($"if (this.{countField.Name} < 0)");
            cbsb.WriteLine(
                $"throw new Exception(\"Expected {countField.Name} to be nonnegative!\");");
            cbsb.ExitBlock();

            cbsb.EnterBlock(
                $"if (this.{field.Name}.Count < this.{countField.Name})");
            cbsb.WriteLine(
                $"this.{field.Name}.Add(new {fieldTypeNamespace}.{fieldTypeName}());");
            cbsb.ExitBlock();

            cbsb.EnterBlock(
                $"while (this.{field.Name}.Count > this.{countField.Name})");
            cbsb.WriteLine($"this.{field.Name}.RemoveAt(0);");
            cbsb.ExitBlock();
          }

          if (!isValuePrimitive) {
            cbsb.EnterBlock($"foreach (var {eachName} in this.{field.Name})");
          } else {
            cbsb.EnterBlock(
                $"for (var i = 0; i < this.{field.Name}.Length; ++i)");
          }
        }

        if (!isValuePrimitive) {
          cbsb.WriteLine($"{valueName}.Read(er);");
        } else {
          var format =
              !field.UseAltFormat
                  ? field.PrimitiveType
                  : SchemaReaderGenerator.ConvertNumberToPrimitive_(
                      field.AltFormat);
          var label =
              SchemaReaderGenerator.GetPrimitiveLabel_(format);

          var cast = "";
          if (field.UseAltFormat) {
            var castTypeName =
                field.PrimitiveType == SchemaPrimitiveType.ENUM
                    ? SchemaReaderGenerator.GetQualifiedName_(field.TypeSymbol)
                    : field.TypeSymbol.Name;
            cast = $"({castTypeName}) ";
          }

          cbsb.WriteLine(!field.IsPrimitiveConst
                             ? $"{valueName} = " + cast + $"er.Read{label}();"
                             : $"er.Assert{label}({valueName});");
        }

        if (field.IsArray) {
          cbsb.ExitBlock();
        }
      }
      cbsb.ExitBlock();

      // TODO: Handle fancier cases here

      // type
      cbsb.ExitBlock();

      // parent types
      foreach (var declaringType in declaringTypes) {
        cbsb.ExitBlock();
      }

      // namespace
      cbsb.ExitBlock();

      var generatedCode = cbsb.ToString();
      this.context_.Value.AddSource(
          SchemaReaderGenerator.GetQualifiedName_(structure.TypeSymbol),
          generatedCode);
    }

    private static INamedTypeSymbol[] GetDeclaringTypesDownward_(
        INamedTypeSymbol type) {
      var declaringTypes = new List<INamedTypeSymbol>();

      var declaringType = type.ContainingType;
      while (declaringType != null) {
        declaringTypes.Add(declaringType);
        declaringType = declaringType.ContainingType;
      }
      declaringTypes.Reverse();

      return declaringTypes.ToArray();
    }

    private static string GetSymbolQualifiers_(INamedTypeSymbol typeSymbol)
      => "public " +
         (typeSymbol.IsAbstract ? "abstract " : "") +
         "partial " +
         (typeSymbol.TypeKind == TypeKind.Class ? "class" : "struct");

    private static string GetQualifiedName_(ITypeSymbol typeSymbol) {
      var mergedNamespace =
          SymbolTypeUtil.MergeContainingNamespaces(typeSymbol);

      return mergedNamespace == null
                 ? typeSymbol.Name
                 : $"{mergedNamespace}.{typeSymbol.Name}";
    }

    private static string GetPrimitiveLabel_(SchemaPrimitiveType type)
      => type switch {
          SchemaPrimitiveType.SBYTE => "Sbyte",
          SchemaPrimitiveType.BYTE => "Byte",
          SchemaPrimitiveType.INT16 => "Int16",
          SchemaPrimitiveType.UINT16 => "UInt16",
          SchemaPrimitiveType.INT32 => "Int32",
          SchemaPrimitiveType.UINT32 => "UInt32",
          SchemaPrimitiveType.INT64 => "Int64",
          SchemaPrimitiveType.UINT64 => "UInt64",
          SchemaPrimitiveType.SINGLE => "Single",
          SchemaPrimitiveType.DOUBLE => "Double",
          SchemaPrimitiveType.SN16 => "Sn16",
          SchemaPrimitiveType.UN16 => "Un16",
          _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };

    private static SchemaPrimitiveType ConvertNumberToPrimitive_(
        SchemaNumberType type)
      => type switch {
          SchemaNumberType.SBYTE => SchemaPrimitiveType.SBYTE,
          SchemaNumberType.BYTE => SchemaPrimitiveType.BYTE,
          SchemaNumberType.INT16 => SchemaPrimitiveType.INT16,
          SchemaNumberType.UINT16 => SchemaPrimitiveType.UINT16,
          SchemaNumberType.INT32 => SchemaPrimitiveType.INT32,
          SchemaNumberType.UINT32 => SchemaPrimitiveType.UINT32,
          SchemaNumberType.INT64 => SchemaPrimitiveType.INT64,
          SchemaNumberType.UINT64 => SchemaPrimitiveType.UINT64,
          SchemaNumberType.SINGLE => SchemaPrimitiveType.SINGLE,
          SchemaNumberType.DOUBLE => SchemaPrimitiveType.DOUBLE,
          SchemaNumberType.SN16 => SchemaPrimitiveType.SN16,
          SchemaNumberType.UN16 => SchemaPrimitiveType.UN16,
          _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
      };

    public void Initialize(GeneratorInitializationContext context) {
      context.RegisterForSyntaxNotifications(() => new CustomReceiver(this));
    }

    private class CustomReceiver : ISyntaxContextReceiver {
      private readonly SchemaReaderGenerator g_;

      public CustomReceiver(SchemaReaderGenerator g) {
        this.g_ = g;
      }

      public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
        TypeDeclarationSyntax syntax;
        ISymbol symbol;
        if (context.Node is ClassDeclarationSyntax classDeclarationSyntax) {
          syntax = classDeclarationSyntax;
        } else if (context.Node is StructDeclarationSyntax
                   structDeclarationSyntax) {
          syntax = structDeclarationSyntax;
        } else {
          return;
        }

        symbol = context.SemanticModel.GetDeclaredSymbol(syntax);
        if (symbol is not INamedTypeSymbol namedTypeSymbol) {
          return;
        }

        this.g_.CheckType(context, syntax, namedTypeSymbol);
      }
    }

    public void CheckType(
        GeneratorSyntaxContext context,
        TypeDeclarationSyntax syntax,
        INamedTypeSymbol symbol) {
      if (!SymbolTypeUtil.HasAttribute(symbol, this.schemaAttributeType_)) {
        return;
      }

      if (!SymbolTypeUtil.IsPartial(syntax)) {
        return;
      }

      var structure = this.parser_.ParseStructure(null, symbol);
      if (structure.Error) {
        return;
      }

      this.Enqueue(structure);
    }

    public void Execute(GeneratorExecutionContext context) {
      this.context_ = context;
      foreach (var structure in this.queue_) {
        this.Generate_(structure);
      }
      this.queue_.Clear();
    }

    private GeneratorExecutionContext? context_;
    private List<ISchemaStructure> queue_ = new();

    public void Enqueue(ISchemaStructure structure) {
      if (this.context_ == null) {
        this.queue_.Add(structure);
      } else {
        this.Generate_(structure);
      }
    }
  }
}