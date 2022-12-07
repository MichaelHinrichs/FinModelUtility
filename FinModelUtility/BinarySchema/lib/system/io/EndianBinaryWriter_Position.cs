using schema.io;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;


namespace System.IO {
  public sealed partial class EndianBinaryWriter {
    private readonly Stack<string> scopes_ = new();
    private readonly OutOfOrderDictionary<string, long> startPositions_ = new();
    private readonly OutOfOrderDictionary<string, long> endPositions_ = new();

    public Task<long> GetSizeOfMemberRelativeToScope(
        string memberPath) {
      var fullPath = this.GetCurrentScope_() + memberPath;
      var startTask = this.startPositions_.Get(fullPath);
      var endTask = this.startPositions_.Get(fullPath);
      return Task.WhenAll(startTask, endTask)
                 .ContinueWith(_ => endTask.Result - startTask.Result);
    }

    public void MarkStartOfMember(string memberName) {
      this.scopes_.Push(memberName);
      this.startPositions_.Set(this.GetCurrentScope_(),
                               this.GetAbsolutePosition());
    }

    public void MarkEndOfMember() {
      this.endPositions_.Set(this.GetCurrentScope_(),
                             this.GetAbsolutePosition());
      this.scopes_.Pop();

      if (this.scopes_.Count == 0) {
        this.startPositions_.Clear();
        this.endPositions_.Clear();
      }
    }

    private string GetCurrentScope_() {
      var totalString = new StringBuilder();
      foreach (var scope in scopes_) {
        if (totalString.Length > 0) {
          totalString.Append(".");
        }
        totalString.Append(scope);
      }
      return totalString.ToString();
    }
  }
}