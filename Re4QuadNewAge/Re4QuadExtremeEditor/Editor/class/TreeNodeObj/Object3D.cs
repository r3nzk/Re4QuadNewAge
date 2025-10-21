using OpenTK;
using Re4QuadExtremeEditor.Editor.Class.Enums;
using Re4QuadExtremeEditor.Editor.Class.Files;
using Re4QuadExtremeEditor.Editor.Class.MyProperty;
using Re4QuadExtremeEditor.Editor.Class.MyProperty._EFF_Property;
using Re4QuadExtremeEditor.Editor.Class.ObjMethods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Re4QuadExtremeEditor.Editor.Class.TreeNodeObj
{
    /// <summary>
    /// <para>classe que representa os objetos, é usado no treeView;</para>
    /// <para>no Quad64 é usado no PropertyGrid, porem aqui dou outra finalidade;</para>
    /// </summary>
    public class Object3D : TreeNode, NsMultiselectTreeView.IAltNode, NsMultiselectTreeView.IFastNode, IEquatable<Object3D>, NsCamera.IObject3D
    {
        public static Object3D CreateNewInstance(GroupType group, ushort objLineRef) 
        {
            Object3D o = new Object3D();
            o.Name = objLineRef.ToString();
            o.Text = "";
            o.Group = group;
            o.ObjLineRef = objLineRef;
            return o;
        }

        private UpdateMethods updateMethods = Globals.updateMethods;
        protected Object3D() : base() {}
        protected Object3D(string text) : base(text){ }
        protected Object3D(string text, TreeNode[] children) : base(text, children) { }

        public GroupType Group { get; protected set; }

        public ushort ObjLineRef { get; protected set; }


        private GenericProperty _property = null;
        public GenericProperty Property
        {
            get
            {
                if (_property == null)
                {
                    _property = CreatePropertyInstance();
                }
                return _property;
            }
        }
        //assign the property to the object
        private GenericProperty CreatePropertyInstance()
        {
            if (Parent == null) return null;

            try
            {
                switch (Group)
                {
                    case GroupType.ESL:
                        return new EnemyProperty(ObjLineRef, updateMethods, ((EnemyNodeGroup)Parent).PropertyMethods);
                    case GroupType.ETS:
                        return new EtcModelProperty(ObjLineRef, updateMethods, ((EtcModelNodeGroup)Parent).PropertyMethods);
                    case GroupType.ITA:
                    case GroupType.AEV:
                        return new SpecialProperty(ObjLineRef, updateMethods, ((SpecialNodeGroup)Parent).PropertyMethods);
                    case GroupType.EXTRAS:
                        var r = DataBase.Extras.AssociationList[this.ObjLineRef];
                        if (r.FileFormat == SpecialFileFormat.AEV) { return new SpecialProperty(r.LineID, updateMethods, DataBase.NodeAEV.PropertyMethods, true); }
                        else if (r.FileFormat == SpecialFileFormat.ITA) { return new SpecialProperty(r.LineID, updateMethods, DataBase.NodeITA.PropertyMethods, true); }
                        return null;
                    case GroupType.DSE:
                        return new NewAge_DSE_Property(ObjLineRef, updateMethods, ((NewAge_DSE_NodeGroup)Parent).PropertyMethods);
                    case GroupType.FSE:
                        return new NewAge_FSE_Property(ObjLineRef, updateMethods, ((NewAge_FSE_NodeGroup)Parent).PropertyMethods);
                    case GroupType.SAR:
                    case GroupType.EAR:
                        return new NewAge_ESAR_Property(ObjLineRef, updateMethods, ((NewAge_ESAR_NodeGroup)Parent).PropertyMethods);
                    case GroupType.ESE:
                        return new NewAge_ESE_Property(ObjLineRef, updateMethods, ((NewAge_ESE_NodeGroup)Parent).PropertyMethods);
                    case GroupType.EMI:
                        return new NewAge_EMI_Property(ObjLineRef, updateMethods, ((NewAge_EMI_NodeGroup)Parent).PropertyMethods);
                    case GroupType.LIT_ENTRYS:
                        return new NewAge_LIT_Entry_Property(ObjLineRef, updateMethods, ((NewAge_LIT_Entrys_NodeGroup)Parent).PropertyMethods);
                    case GroupType.LIT_GROUPS:
                        return new NewAge_LIT_Group_Property(ObjLineRef, updateMethods, ((NewAge_LIT_Groups_NodeGroup)Parent).PropertyMethods);
                    case GroupType.QUAD_CUSTOM:
                        return new QuadCustomProperty(ObjLineRef, updateMethods, ((QuadCustomNodeGroup)Parent).PropertyMethods);
                    case GroupType.EFF_EffectEntry:
                        return new EFF_TableEffectEntry_Property(ObjLineRef, updateMethods, ((NewAge_EFF_EffectEntry_NodeGroup)Parent).PropertyMethods);
                    case GroupType.EFF_Table0:
                        return new EFF_Table0_Property(ObjLineRef, updateMethods, ((NewAge_EFF_NodeGroup)Parent).PropertyMethods);
                    case GroupType.EFF_Table1:
                        return new EFF_Table1_Property(ObjLineRef, updateMethods, ((NewAge_EFF_NodeGroup)Parent).PropertyMethods);
                    case GroupType.EFF_Table2:
                        return new EFF_Table2_Property(ObjLineRef, updateMethods, ((NewAge_EFF_NodeGroup)Parent).PropertyMethods);
                    case GroupType.EFF_Table3:
                        return new EFF_Table3_Property(ObjLineRef, updateMethods, ((NewAge_EFF_NodeGroup)Parent).PropertyMethods);
                    case GroupType.EFF_Table4:
                        return new EFF_Table4_Property(ObjLineRef, updateMethods, ((NewAge_EFF_NodeGroup)Parent).PropertyMethods);
                    case GroupType.EFF_Table6:
                        return new EFF_Table6_Property(ObjLineRef, updateMethods, ((NewAge_EFF_NodeGroup)Parent).PropertyMethods);
                    case GroupType.EFF_Table7_Effect_0:
                    case GroupType.EFF_Table8_Effect_1:
                        return new EFF_TableEffectGroup_Property(ObjLineRef, updateMethods, ((NewAge_EFF_EffectGroup_NodeGroup)Parent).PropertyMethods);
                    case GroupType.EFF_Table9:
                        return new EFF_Table9_Property(ObjLineRef, updateMethods, ((NewAge_EFF_Table9Entry_NodeGroup)Parent).PropertyMethods);
                }
            }
            catch (Exception ex)
            {
                Editor.Console.Error($"Error creating property for {Text}: {ex.Message}");
                return null;
            }

            return null;
        }

        public Matrix4 GetRotationMatrix()
        {
            if (this.Property == null){
                return Matrix4.Identity;
            }

            var targetType = Globals.CurrentManipulationTarget.Type;

            //if we are manipulating triggerzones, it is converted to world space (cause most triggerzones are just points)
            switch (targetType)
            {
                case ManipulationTargetType.TriggerPoint0:
                case ManipulationTargetType.TriggerPoint1:
                case ManipulationTargetType.TriggerPoint2:
                case ManipulationTargetType.TriggerPoint3:
                case ManipulationTargetType.AshleyZone:
                    return Matrix4.Identity;
            }

            //other objects
            Matrix4 rotationMatrix = Matrix4.Identity;

            Func<short, float> shortToRad = (val) => (val / 32767.0f) * MathHelper.Pi;
            Func<float, float> degToRad = MathHelper.DegreesToRadians;

            switch (Group)
            {
                case GroupType.ESL:
                    rotationMatrix = ((EnemyNodeGroup)Parent).MethodsForGL.GetRotation(ObjLineRef);
                    break;
                case GroupType.ETS:
                    rotationMatrix = ((EtcModelNodeGroup)Parent).MethodsForGL.GetAngle(ObjLineRef);
                    break;
                case GroupType.AEV:
                case GroupType.ITA:
                    //check if triggerzone to decide if rotation will be limited
                    if (Globals.CurrentMoveObjType.HasFlag(MoveObjType._SquareMoveTriggerZone))
                    {
                        //return only Y rotation cause triggers dont support other angles
                        rotationMatrix = GetRotationFromTriggerCorners((BaseTriggerZoneProperty)Property);
                    }
                    else //return complete rotation
                    {
                        var specialProp = (SpecialProperty)Property;
                        if (specialProp.GetSpecialType() == SpecialType.T03_Items){
                            rotationMatrix = ((SpecialNodeGroup)Parent).MethodsForGL.GetItemRotation(ObjLineRef);
                        }else{
                            rotationMatrix = GetRotationFromTriggerCorners(specialProp);
                        }
                    }
                    break;

                case GroupType.EXTRAS:
                    var r = DataBase.Extras.GetAssociationObj(this.ObjLineRef);
                    ExtrasMethodsForGL methods;
                    if (r.FileFormat == SpecialFileFormat.AEV) { methods = DataBase.NodeAEV.ExtrasMethodsForGL; }
                    else if (r.FileFormat == SpecialFileFormat.ITA) { methods = DataBase.NodeITA.ExtrasMethodsForGL; }
                    else { break; }

                    var specialType = methods.GetSpecialType(r.LineID);
                    switch (specialType)
                    {
                        case SpecialType.T01_WarpDoor:
                            rotationMatrix = methods.GetWarpRotation(r.LineID);
                            break;
                        case SpecialType.T13_LocalTeleportation:
                        case SpecialType.T10_FixedLadderClimbUp:
                            rotationMatrix = methods.GetLocationAndLadderRotation(r.LineID);
                            break;
                        case SpecialType.T15_AdaGrappleGun:
                            rotationMatrix = methods.GetGrappleGunFacingAngleRotation(r.LineID);
                            break;
                    }
                    break;
                case GroupType.QUAD_CUSTOM:
                    //check if triggerzone to decide if rotation will be limited
                    if (Globals.CurrentMoveObjType.HasFlag(MoveObjType._SquareMoveTriggerZone))
                    {
                        //return only Y rotation cause triggers dont support other angles
                        rotationMatrix = GetRotationFromTriggerCorners((BaseTriggerZoneProperty)Property);
                    }
                    else //return complete rotation
                    {
                        rotationMatrix = ((QuadCustomNodeGroup)Parent).MethodsForGL.GetAngle(ObjLineRef);
                    }
                    break;
                case GroupType.EMI:
                    rotationMatrix = ((NewAge_EMI_NodeGroup)Parent).MethodsForGL.GetAngle(ObjLineRef);
                    break;
                case GroupType.FSE:
                case GroupType.SAR:
                case GroupType.EAR:
                    rotationMatrix = GetRotationFromTriggerCorners((BaseTriggerZoneProperty)Property);
                    break;
                case GroupType.EFF_Table7_Effect_0:
                case GroupType.EFF_Table8_Effect_1:
                    rotationMatrix = ((NewAge_EFF_EffectGroup_NodeGroup)Parent).MethodsForGL.GetAngle(ObjLineRef);
                    break;
                case GroupType.EFF_EffectEntry:
                    rotationMatrix = ((NewAge_EFF_EffectEntry_NodeGroup)Parent).MethodsForGL.GetAngle(ObjLineRef);
                    break;
            }

            return rotationMatrix;
        }

        private Matrix4 GetRotationFromTriggerCorners(BaseTriggerZoneProperty triggerProp)
        {
            //here we'll try to aproximate the trigger corners to try to get
            //a valid orientation and show the triggerzone editor gizmo;
            //this is still a little bit rough, but better then triggers being always worldspace

            var p0 = new Vector2(triggerProp.TriggerZoneCorner0_X, triggerProp.TriggerZoneCorner0_Z);
            var p3 = new Vector2(triggerProp.TriggerZoneCorner3_X, triggerProp.TriggerZoneCorner3_Z);
            var direction = p3 - p0;

            if (direction.LengthSquared > 0.001f)
            {
                float angleY = (float)Math.Atan2(direction.X, direction.Y);
                return Matrix4.CreateRotationY(angleY);
            }
            return Matrix4.Identity;
        }


        public Vector3 GetGizmoPosition()
        {
            //for simple objects, gizmo position is always the center
            Vector3 defaultPosition = (Parent as dynamic).MoveMethods.GetObjPostion_ToCamera(this.ObjLineRef);

            var targetType = Globals.CurrentManipulationTarget.Type;

            //for complex/compound objects, gizmo position must be calculated
            switch (targetType)
            {
                //center on the main object's origin as def
                case ManipulationTargetType.None:
                case ManipulationTargetType.Object:
                case ManipulationTargetType.Compound:
                    return defaultPosition;

                //center on the whole triggerzone
                case ManipulationTargetType.TriggerZone:
                    {
                        var triggerProp = (BaseTriggerZoneProperty)this.Property;
                        if (triggerProp != null)
                        {
                            Vector3 center = Vector3.Zero;
                            var category = (TriggerZoneCategory)triggerProp.Category;
                            switch (category)
                            {
                                case TriggerZoneCategory.Category01: //square
                                    center.X = (triggerProp.TriggerZoneCorner0_X + triggerProp.TriggerZoneCorner1_X + triggerProp.TriggerZoneCorner2_X + triggerProp.TriggerZoneCorner3_X) / 4.0f;
                                    center.Z = (triggerProp.TriggerZoneCorner0_Z + triggerProp.TriggerZoneCorner1_Z + triggerProp.TriggerZoneCorner2_Z + triggerProp.TriggerZoneCorner3_Z) / 4.0f;
                                    break;
                                case TriggerZoneCategory.Category02: //cylinder
                                    center.X = triggerProp.TriggerZoneCorner0_X;
                                    center.Z = triggerProp.TriggerZoneCorner0_Z;
                                    break;
                                default:
                                    return defaultPosition;
                            }
                            center.Y = triggerProp.TriggerZoneTrueY + (triggerProp.TriggerZoneMoreHeight / 2.0f);
                            return center / 100.0f;
                        }
                        return defaultPosition;
                    }

                //center on the ashley hide zone
                case ManipulationTargetType.AshleyZone:
                    {
                        var posArray = (Parent as dynamic).MoveMethods.GetObjPostion_ToMove_General(this.ObjLineRef);
                        if (posArray != null && posArray.Length >= 7)
                        {
                            return posArray[6] / 100.0f;
                        }
                        return defaultPosition;
                    }

                //center on a specific triggerzone point
                case ManipulationTargetType.TriggerPoint0:
                case ManipulationTargetType.TriggerPoint1:
                case ManipulationTargetType.TriggerPoint2:
                case ManipulationTargetType.TriggerPoint3:
                    {
                        var posArray = (Parent as dynamic).MoveMethods.GetObjPostion_ToMove_General(this.ObjLineRef);
                        if (posArray == null || posArray.Length < 6) { return defaultPosition; }

                        float halfHeight = posArray[5].Z / 2.0f;
                        Vector3 pointPosition = Vector3.Zero;
                        int pointIndex = 0;

                        if (targetType == ManipulationTargetType.TriggerPoint0) pointIndex = 1;
                        else if (targetType == ManipulationTargetType.TriggerPoint1) pointIndex = 2;
                        else if (targetType == ManipulationTargetType.TriggerPoint2) pointIndex = 3;
                        else if (targetType == ManipulationTargetType.TriggerPoint3) pointIndex = 4;

                        if (posArray.Length > pointIndex)
                        {
                            float baseY = posArray[5].Y;
                            pointPosition = posArray[pointIndex];
                            pointPosition.Y = baseY + halfHeight;
                            return pointPosition / 100.0f;
                        }

                        return defaultPosition;
                    }
                default:
                    return defaultPosition;
            }
        }


        /// <summary>
        /// Retorna o texto do node;
        /// </summary>
        public string AltText { get {
                if (Parent is TreeNodeGroup parent)
                {
                    return parent.DisplayMethods.GetNodeText(ObjLineRef);
                }
                return "Error Text"; }}

        /// <summary>
        /// atualiza a cor para o node;
        /// </summary>
        public Color AltForeColor { get {
                if (Parent is TreeNodeGroup parent)
                {
                    return parent.DisplayMethods.GetNodeColor(ObjLineRef);
                }
                return ForeColor;} }


        /// <summary>
        /// retorna o posição do objeto para ser usada na camera orbit
        /// </summary>
        public Vector3 GetObjPosition_ToCamera() 
        {
            if (Parent is TreeNodeGroup parent)
            {
                return parent.MoveMethods.GetObjPostion_ToCamera(ObjLineRef);
            }
            return Vector3.Zero;
        }

        /// <summary>
        /// retorna o angulo do objeto para ser usada na camera orbit
        /// </summary>
        public float GetObjAngleY_ToCamera()
        {
            if (Parent is TreeNodeGroup parent)
            {
                return parent.MoveMethods.GetObjAngleY_ToCamera(ObjLineRef);
            }
            return 0;
        }

        /// <summary>
        /// retorna as coordenadas de posição na escala real do jogo pra poder fazer a ação de mover
        /// <para> para enimigos, etcmodel, item, warp, ladder, ashey point, GrappleGun [0]</para>
        /// <para> triggerZone point0 [1]</para>
        /// <para> triggerZone point1 [2]</para>
        /// <para> triggerZone point2 [3]</para>
        /// <para> triggerZone point3 [4]</para>
        /// <para> ReturnTriggerZoneCircleRadius(ID), ReturnTriggerZoneTrueY(ID), ReturnTriggerZoneMoreHeight(ID) [5]</para>
        /// <para> TriggerZone Center [6]</para>
        /// </summary>
        public Vector3[] GetObjPostion_ToMove_General() 
        {
            if (Parent is TreeNodeGroup parent)
            {
                return parent.MoveMethods.GetObjPostion_ToMove_General(ObjLineRef);
            }
            return null;
        }

        /// <summary>
        /// leia GetObjPostion_ToMove_General
        /// <para> Ordem:</para>
        /// <para> para enimigos, etcmodel, item, warp, ladder, ashey point, GrappleGun [0]</para>
        /// <para> triggerZone point0 [1]</para>
        /// <para> triggerZone point1 [2]</para>
        /// <para> triggerZone point2 [3]</para>
        /// <para> triggerZone point3 [4]</para>
        /// <para> ReturnTriggerZoneCircleRadius(ID), ReturnTriggerZoneTrueY(ID), ReturnTriggerZoneMoreHeight(ID) [5]</para>
        /// <para> TriggerZone Center [6]</para>
        /// </summary>
        /// <param name="value"></param>
        public void SetObjPostion_ToMove_General(Vector3[] value) 
        {
            if (Parent is TreeNodeGroup parent)
            {
                parent.MoveMethods.SetObjPostion_ToMove_General(ObjLineRef, value);
            }
        }

        public Vector3[] GetObjRotarionAngles_ToMove() 
        {
            if (Parent is TreeNodeGroup parent)
            {
                return parent.MoveMethods.GetObjRotationAngles_ToMove(ObjLineRef);
            }
            return null;

        }

        public void SetObjRotarionAngles_ToMove(Vector3[] value)
        {
            if (Parent is TreeNodeGroup parent)
            {
                parent.MoveMethods.SetObjRotationAngles_ToMove(ObjLineRef, value);
            }
        }

        public Vector3[] GetObjScale_ToMove()
        {
            if (Parent is TreeNodeGroup parent)
            {
                return parent.MoveMethods.GetObjScale_ToMove(ObjLineRef);
            }
            return null;

        }

        public void SetObjScale_ToMove(Vector3[] value)
        {
            if (Parent is TreeNodeGroup parent)
            {
                parent.MoveMethods.SetObjScale_ToMove(ObjLineRef, value);
            }
        }

        public TriggerZoneCategory GetTriggerZoneCategory() 
        {
            if (Parent is TreeNodeGroup parent)
            {
                return parent.MoveMethods.GetTriggerZoneCategory(ObjLineRef);
            }
            return TriggerZoneCategory.Disable;
        }

        public int HashCodeID { get { return (int)(((uint)Group * 0x10000) + ObjLineRef); } } 

        public override int GetHashCode()
        {
            return HashCodeID;
        }

        public override bool Equals(object obj)
        {
            return (obj is NsMultiselectTreeView.IFastNode fast && fast.HashCodeID == HashCodeID);
        }

        public bool Equals(Object3D other)
        {
            return other.HashCodeID == HashCodeID;
        }
    }
}
