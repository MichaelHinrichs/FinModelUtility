using schema.binary;


namespace fin.schema.data {
  public interface IMagicSection<T> : ISizedSection<T>
      where T : IBinaryConvertible {
    string Magic { get; }
  }
}