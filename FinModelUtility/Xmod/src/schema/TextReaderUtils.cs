using System.Numerics;

using schema.text;

namespace xmod.schema {
  public static class TextReaderUtils {
    public static string ReadKeyValue(ITextReader tr, string prefix) {
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);
      tr.AssertString(prefix);
      tr.AssertChar(':');
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);
      return tr.ReadLine();
    }

    public static TNumber ReadKeyValueNumber<TNumber>(
        ITextReader tr,
        string prefix)
        where TNumber : INumber<TNumber>
      => TNumber.Parse(ReadKeyValue(tr, prefix), null);

    public static T ReadKeyValueInstance<T>(ITextReader tr, string prefix)
        where T : ITextDeserializable, new() {
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);
      tr.AssertString(prefix);
      tr.AssertChar(':');
      tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);
      var instance = tr.ReadNew<T>();
      tr.IgnoreOnceIfPresent(TextReaderConstants.NEWLINE_STRINGS);
      return instance;
    }

    public static T[] ReadInstances<T>(ITextReader tr,
                                       string prefix,
                                       int count)
        where T : ITextDeserializable, new() {
      var values = new T[count];
      for (var i = 0; i < count; ++i) {
        tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);
        tr.AssertString(prefix);
        tr.IgnoreManyIfPresent(TextReaderConstants.WHITESPACE_STRINGS);
        values[i] = tr.ReadNew<T>();
        tr.IgnoreOnceIfPresent(TextReaderConstants.NEWLINE_STRINGS);
      }

      return values;
    }
  }
}