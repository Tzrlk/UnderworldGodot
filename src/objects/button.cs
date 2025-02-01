using System.Diagnostics;
using Godot;

namespace Underworld
{
    /// <summary>
    /// For rendering switches (except for the rotary switch)
    /// </summary>
    public class button : model3D
    {
        Node3D modelNode;
        public static button CreateInstance(Node3D parent, uwObject obj, string name)
        {
            var b = new button(obj);
            if (obj.invis == 0)
            {
                b.modelNode = b.Generate3DModel(parent, name);
                SetModelRotation(parent, b);
                //DisplayModelPoints(b, modelNode);
                if (obj.xpos == 0)
                {
                    parent.Position += new Vector3(+0.05f, 0f, 0f);
                }
                if (obj.ypos == 0)
                {
                    parent.Position += new Vector3(0f, 0f, -0.05f);
                }
                if (obj.xpos == 7)
                {
                    parent.Position += new Vector3(-0.05f, 0f, 0f);
                }
                if (obj.ypos == 7)
                {
                    parent.Position += new Vector3(0f, 0f, +0.05f);
                }
            }
            else
            {
                Debug.Print($"{obj.a_name} I:{obj.index} L:{obj.link}  X:{obj.tileX} Y:{obj.tileY} is an invisible button");
            }
            return b;
        }

        public button(uwObject _uwobject)
        {
            uwobject = _uwobject;
            uwobject.instance = this;
        }

        public static bool Use(uwObject obj)
        {
            TryAndUse(obj,3);
            // ToggleItemID(obj);
            // RefreshButtonSprite(obj);
            return true;
        }


        
        /// <summary>
        /// Controls toggling of switches.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="toggleMode">0, do nothing, 1,2 try and go to a target state, 3 toggle</param>
        /// <returns></returns>
        public static bool TryAndUse(uwObject obj, int toggleMode)
        {
            if (toggleMode==0)
            {
                return false;
            }
            else
            {
                if (toggleMode != 3)
                {
                    int dx = 0;
                    if (IsOn(obj))
                    {
                        dx = 1;
                    }

                    int ax = 0;
                    if (toggleMode>2)
                    {
                        ax = 1;
                    }
                    if (ax != dx)
                    {
                        return false;
                    }
                }
                //Debug.Print($"toggle button {obj.a_name} {obj.index}");
                ToggleItemID(obj);
                RefreshButtonSprite(obj);
                return true;
            }
        }

        private static void ToggleItemID(uwObject obj)
        {
            if (IsOn(obj))
            {//on-->off
                obj.item_id -= 8;
            }
            else
            {//off->on
                obj.item_id += 8;
            }
        }


        private static void RefreshButtonSprite(uwObject obj)
        {
            if (obj.instance != null)
            {
                var newmaterial = GetTmFlat.GetMaterial(obj.item_id - 368);
                var _button = (button)obj.instance;
                var mdl = (MeshInstance3D)(_button.modelNode);
                mdl.Mesh.SurfaceSetMaterial(0, newmaterial);
            }
        }



        /// <summary>
        /// Checks if button is in the on state
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        static bool IsOn(uwObject obj)
        {
            return obj.classindex > 7;
            // if (obj.classindex <= 7)
            // {
            //     return false; //
            // }
            // else
            // {
            //     return true;
            // }
        }

        public override Vector3[] ModelVertices()
        {
            var v = new Vector3[4];
            v[0] = new Vector3(-0.0625f, 0f, 0.0625f);
            v[1] = new Vector3(0.1875f, 0f, 0.0625f);
            v[2] = new Vector3(0.1875f, 0.25f, 0.0625f);
            v[3] = new Vector3(-0.0625f, 0.25f, 0.0625f);
            return v;
        }

        public override int[] ModelTriangles(int meshNo)
        {
            var tris = new int[6];
            tris[0] = 0;
            tris[1] = 3;
            tris[2] = 2;
            tris[3] = 2;
            tris[4] = 1;
            tris[5] = 0;
            return tris;
        }

        public override Vector2[] ModelUVs(Vector3[] verts)
        {
            var uv = new Vector2[4];
            uv[0] = new Vector2(0, 1);
            uv[1] = new Vector2(1, 1);
            uv[2] = new Vector2(1, 0);
            uv[3] = new Vector2(0, 0);
            return uv;
        }

        public override ShaderMaterial GetMaterial(int textureno, int surface)
        {
            return GetTmFlat.GetMaterial(uwobject.item_id - 368);
        }
    }
}