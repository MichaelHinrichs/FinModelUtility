using schema.binary;


namespace fin.schema.data {
  public interface ISizedSection<T> : IBinaryConvertible
      where T : IBinaryConvertible {
    T Data { get; }
  }
}