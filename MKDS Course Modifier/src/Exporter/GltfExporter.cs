using System.Collections.Generic;

using MKDS_Course_Modifier._3D_Formats;
using MKDS_Course_Modifier.GCN;

namespace mkds.exporter {
  public static class GltfExporter {
    public static void Export(BMD bmd, IEnumerable<BCA> bca) {
      var joints = bmd.GetJoints();
      
      //ScaleDialog scaleDialog = new ScaleDialog();
      //int num = (int) scaleDialog.ShowDialog();
      float scale = 1; //scaleDialog.scale;
      List<MA.AnimatedNode> animatedNodeList = new List<MA.AnimatedNode>();
      /*foreach (MA.Node node in joints) {
        int index1 = bmd.JNT1.StringTable[node.Name];
        node.Trans.X *= scale;
        node.Trans.Y *= scale;
        node.Trans.Z *= scale;
        List<float> floatList1 = new List<float>();
        List<float> floatList2 = new List<float>();
        List<float> floatList3 = new List<float>();
        List<float> floatList4 = new List<float>();
        List<float> floatList5 = new List<float>();
        List<float> floatList6 = new List<float>();
        List<float> floatList7 = new List<float>();
        List<float> floatList8 = new List<float>();
        List<float> floatList9 = new List<float>();
        for (int index2 = 0; index2 < (int)this.AnimLength; ++index2) {
          floatList1.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.translationsX, (float)index2) * scale);
          floatList2.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.translationsY, (float)index2) * scale);
          floatList3.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.translationsZ, (float)index2) * scale);
          floatList4.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.rotationsX, (float)index2));
          floatList5.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.rotationsY, (float)index2));
          floatList6.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.rotationsZ, (float)index2));
          floatList7.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.scalesX, (float)index2));
          floatList8.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.scalesY, (float)index2));
          floatList9.Add(this.Joints[index1].GetAnimValue(this.Joints[index1].Values.scalesZ, (float)index2));
        }
        animatedNodeList.Add(new MA.AnimatedNode((int)this.AnimLength, floatList1.ToArray(), floatList2.ToArray(), floatList3.ToArray(), floatList4.ToArray(), floatList5.ToArray(), floatList6.ToArray(), floatList7.ToArray(), floatList8.ToArray(), floatList9.ToArray()));
      }
      return MA.WriteAnimation(joints, animatedNodeList.ToArray());*/
    }

  }
}
