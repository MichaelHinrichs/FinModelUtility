using System;
using System.Collections.Generic;

using Microsoft.CodeAnalysis;

namespace schema {
  internal interface ISchemaStructureParser {
    ISchemaStructure ParseStructure(INamedTypeSymbol symbol);
  }

  internal interface ISchemaStructure {
    Type Type { get; }
    IReadOnlyList<ISchemaField> Fields { get; }
  }

  internal enum SchemaPrimitiveType {
    SBYTE,
    BYTE,
    INT16,
    UINT16,
    INT32,
    UINT32,
    INT64,
    UINT64,
    SINGLE,
    DOUBLE,
    SN16,
    UN16,
  }

  internal interface ISchemaField {
    string Name { get; }
    Type Type { get; }

    bool IsArray { get; }
    bool HasConstArrayLength { get; }
    ISchemaField ArrayLengthField { get; }

    bool IsPrimitive { get; }
    SchemaPrimitiveType PrimitiveType { get; }
    bool IsPrimitiveConst { get; }
  }

  public class SchemaStructureParser : ISchemaStructureParser {



  }
}
