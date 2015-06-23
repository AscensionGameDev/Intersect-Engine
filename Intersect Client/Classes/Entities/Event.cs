using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intersect_Client.Classes
{
    public class Event : Entity
    {
        public string Desc = "";
        public int Layer;
        public int GraphicType;
        public string Graphic = "";
        public string FaceGraphic = "";
        public int Graphicx;
        public int Graphicy;
        public int DisablePreview;

        public void Load(ByteBuffer bf)
        {
            base.Load(bf);
            HideName = bf.ReadInteger();
            DisablePreview = bf.ReadInteger();
            Desc = bf.ReadString();
        }
    }


}
