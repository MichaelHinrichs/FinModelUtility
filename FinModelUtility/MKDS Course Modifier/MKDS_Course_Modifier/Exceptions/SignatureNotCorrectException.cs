// Decompiled with JetBrains decompiler
// Type: MKDS_Course_Modifier.Exceptions.SignatureNotCorrectException
// Assembly: MKDS Course Modifier, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DAEF8B62-698B-42D0-BEDD-3770EB8C9FE8
// Assembly location: R:\Documents\CSharpWorkspace\Pikmin2Utility\MKDS Course Modifier\MKDS Course Modifier.exe

using System;
using System.Runtime.Serialization;

namespace MKDS_Course_Modifier.Exceptions
{
  [Serializable]
  public class SignatureNotCorrectException : Exception
  {
    public string BadSignature { get; private set; }

    public string CorrectSignature { get; private set; }

    public long Offset { get; private set; }

    public SignatureNotCorrectException(string BadSignature, string CorrectSignature, long Offset)
    {
      this.BadSignature = BadSignature;
      this.CorrectSignature = CorrectSignature;
      this.Offset = Offset;
    }

    public SignatureNotCorrectException(string message)
      : base(message)
    {
    }

    public SignatureNotCorrectException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected SignatureNotCorrectException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public override string ToString()
    {
      return "Signature '" + this.BadSignature + "' at 0x" + this.Offset.ToString("X8") + " does not match '" + this.CorrectSignature + "'.";
    }
  }
}
