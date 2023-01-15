using OpenTK;
using fin.math;

namespace level5 {
  public enum InterpolationType {
    Constant,
    Linear,
    Hermite,
    Step
  }

  public class GenericAnimKey<T> {
    public float Frame { get; set; } // todo: this needs to be read only or something
    public float InTan { get; set; }
    public float OutTan { get; set; }
    public T Value { get; set; }
    public InterpolationType InterpolationType { get; set; }
  }

  public class GenericKeyGroup<T> {
    /// <summary>
    /// A read-only view of the keys
    /// </summary>
    public IList<GenericAnimKey<T>> Keys {
      get {
        return this.keys_.Values;
      }
    }

    public float MaxFrame {
      get {
        if (this.keys_.Count == 0)
          return 0;

        return this.keys_.Keys.Max();
      }
    }

    private SortedList<float, GenericAnimKey<T>> keys_ = new SortedList<float, GenericAnimKey<T>>();

    public void AddKey(float frame, T value, InterpolationType type = InterpolationType.Linear, float TanIn = 0, float TanOut = float.MaxValue) {
      if (this.keys_.ContainsKey(frame)) {
        return;
      }

      GenericAnimKey<T> key = new GenericAnimKey<T>();
      key.Frame = frame;
      key.Value = value;
      key.InTan = TanIn;
      key.OutTan = TanOut == float.MaxValue ? TanIn : TanOut;
      key.InterpolationType = type;
      this.keys_.Add(frame, key);
    }

    /// <summary>
    /// Removes key frame at
    /// </summary>
    /// <param name="frame"></param>
    public void RemoveKey(float frame) {
      if (this.keys_.ContainsKey(frame))
        this.keys_.Remove(frame);
    }

    public GenericAnimKey<T> GetKey(float frame) {
      int left = this.BinarySearchKeys_(frame);

      return this.keys_.Values[left];
    }

    private int BinarySearchKeys_(float frame) {
      int lower = 0;
      int upper = this.keys_.Count - 1;
      int middle = 0;

      while (lower <= upper) {
        middle = (upper + lower) / 2;
        if (frame == this.keys_.Values[middle].Frame)
          return middle;
        else
        if (frame < this.keys_.Values[middle].Frame)
          upper = middle - 1;
        else
          lower = middle + 1;
      }


      return Math.Max(lower < upper ? lower : upper, 0);
    }

    /// <summary>
    /// Gets the interpolated value at given frame
    /// </summary>
    /// <param name="Frame"></param>
    /// <returns></returns>
    public T GetValue(float Frame) {
      if (this.keys_.Count == 1)
        return this.keys_.Values[0].Value;

      int left = this.BinarySearchKeys_(Frame);
      int right = left + 1;

      if (right >= this.keys_.Count)
        return this.keys_.Values[left].Value;

      if (this.keys_.Values[left].Value is float) {
        if (this.keys_.Values[left].InterpolationType == InterpolationType.Step
            || this.keys_.Values[left].InterpolationType == InterpolationType.Constant) {
          return (T)(object)this.keys_.Values[left].Value!;
        }
        if (this.keys_.Values[left].InterpolationType == InterpolationType.Linear) {
          float leftValue = (float)(object)this.keys_.Values[left].Value;
          float rightValue = (float)(object)this.keys_.Values[right].Value;
          float leftFrame = this.keys_.Keys[left];
          float rightFrame = this.keys_.Keys[right];

          float value = Interpolation.Lerp(leftValue, rightValue, leftFrame, rightFrame, Frame);

          if (float.IsNaN(value))
            value = 0;

          return (T)(object)value;
        }
        if (this.keys_.Values[left].InterpolationType == InterpolationType.Hermite) {
          float leftValue = (float)(object)this.keys_.Values[left].Value;
          float rightValue = (float)(object)this.keys_.Values[right].Value;
          float leftTan = this.keys_.Values[left].OutTan;
          float rightTan = this.keys_.Values[right].InTan;
          float leftFrame = this.keys_.Keys[left];
          float rightFrame = this.keys_.Keys[right];

          float value = Interpolation.Hermite(Frame, leftFrame, rightFrame, leftTan, rightTan, leftValue, rightValue);

          if (float.IsNaN(value))
            value = 0;

          return (T)(object)value;
        }
      }
      if (this.keys_.Values[left].Value is Vector4) {
        if (this.keys_.Values[left].InterpolationType == InterpolationType.Linear) {
          var leftValue = (Vector4)(object)this.keys_.Values[left].Value;
          var rightValue = (Vector4)(object)this.keys_.Values[right].Value;
          float leftFrame = this.keys_.Keys[left];
          float rightFrame = this.keys_.Keys[right];

          float t = (Frame - leftFrame) / (rightFrame - leftFrame);

          var value = Vector4.Lerp(leftValue, rightValue, t);

          return (T)(object)value;
        }
      }

      return this.keys_.Values[left].Value;
    }

    public void Optimize() {
      if (this.keys_.Count == 0)
        return;

      var temp_keys = new SortedList<float, GenericAnimKey<T>>();
      var values = this.keys_.Values.ToList();

      temp_keys.Add(values[0].Frame, values[0]);
      var prevValue = values[0].Value;
      for (int i = 1; i < values.Count; i++) {
        if (values[i].InterpolationType != InterpolationType.Linear
            || values[i].InterpolationType != InterpolationType.Step) {
          return;
        }

        if (!values[i].Value.Equals(prevValue)) {
          temp_keys.Add(values[i].Frame, values[i]);
        }
        prevValue = values[i].Value;
      }

      this.keys_ = temp_keys;
    }
  }

  public class Interpolation {
    public static float Hermite(float Frame, float FrameLeft, float FrameRight, float LS, float RS, float LHS, float RHS) {
      float Result;

      float FrameDiff = Frame - FrameLeft;
      float Weight = FrameDiff / (FrameRight - FrameLeft);

      Result = LHS + (LHS - RHS) * (2 * Weight - 3) * Weight * Weight;
      Result += (FrameDiff * (Weight - 1)) * (LS * (Weight - 1) + RS * Weight);

      return Result;
    }

    public static float Lerp(float av, float bv, float v0, float v1, float t) {
      if (v0 == v1) return av;

      if (t == v0) return av;
      if (t == v1) return bv;


      float mu = (t - v0) / (v1 - v0);
      return ((av * (1 - mu)) + (bv * mu));
    }

    public static Quaternion Slerp(Vector4 v0, Vector4 v1, float t) {
      v0.Normalize();
      v1.Normalize();

      var dot = Vector4.Dot(v0, v1);

      const double DOT_THRESHOLD = 0.9995;
      if (Math.Abs(dot) > DOT_THRESHOLD) {
        Vector4 result = v0 + new Vector4((float)t) * (v1 - v0);
        result.Normalize();
        return new Quaternion(result.Xyz, result.W);
      }
      if (dot < 0.0f) {
        v1 = -v1;
        dot = -dot;
      }

      if (dot < -1) dot = -1;
      if (dot > 1) dot = 1;
      var theta_0 = FinTrig.Acos(dot);  // theta_0 = angle between input vectors
      var theta = theta_0 * t;    // theta = angle between v0 and result 

      Vector4 v2 = v1 - v0 * new Vector4((float)dot);
      v2.Normalize();              // { v0, v2 } is now an orthonormal basis

      Vector4 res = v0 * new Vector4((float)FinTrig.Cos(theta)) + v2 * new Vector4((float)Math.Sign(theta));
      return new Quaternion(res.Xyz, res.W);
    }

  }
}