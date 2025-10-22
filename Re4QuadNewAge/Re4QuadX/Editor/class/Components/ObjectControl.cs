using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using NsCamera;
using OpenTK;
using Re4QuadX.Editor.Class.Enums;
using Re4QuadX.Editor.Class.TreeNodeObj;

namespace Re4QuadX.Editor.Class
{
    public enum MoveControlType
    {
        Null,
        Square,
        Vertical,
        Horizontal1,
        Horizontal2,
        Horizontal3
    }

    public enum ManipulationTargetType
    {
        None,
        Compound,
        Object,
        TriggerZone,
        TriggerPoint0,
        TriggerPoint1,
        TriggerPoint2,
        TriggerPoint3,
        TriggerWall01,
        TriggerWall12,
        TriggerWall23,
        TriggerWall30,
        AshleyZone
    }
    public class ManipulationTarget{
        public string DisplayName { get; set; }
        public ManipulationTargetType Type { get; set; }

        public override string ToString(){
            return DisplayName;
        }
    }

    public class ObjectControl
    {
        private readonly Camera camera;
        private readonly CustomDelegates.ActivateMethod UpdateGL;
        private readonly CustomDelegates.ActivateMethod UpdateCameraMatrix;
        private readonly CustomDelegates.ActivateMethod UpdatePropertyGrid;
        private readonly CustomDelegates.ActivateMethod UpdateTreeViewObjs;

        public MoveObjType MoveObjTypeSelected { get; private set; } = MoveObjType.Null;
        public ManipulationTarget SelectedTarget { get; private set; }
        public bool EnableSquare { get; private set; }
        public bool EnableVertical { get; private set; }
        public bool EnableHorizontal1 { get; private set; }
        public bool EnableHorizontal2 { get; private set; }
        public bool EnableHorizontal3 { get; private set; }

        private bool move_Invert;
        private Point moveObj_lastMouseXY;
        private Dictionary<MoveObj.ObjKey, Vector3[]> SavedObjectData;
        private Dictionary<MoveObj.ObjKey, Vector3[]> SavedObjectRotationData;

        private Vector3 totalGizmoTranslation;
        private Vector3 totalGizmoRotationAngles;
        private Quaternion totalGizmoRotationQuat;

        public ObjectControl(ref Camera camera,
            CustomDelegates.ActivateMethod updateGL,
            CustomDelegates.ActivateMethod updateCameraMatrix,
            CustomDelegates.ActivateMethod updatePropertyGrid,
            CustomDelegates.ActivateMethod updateTreeViewObjs)
        {
            this.camera = camera;
            this.UpdateGL = updateGL;
            this.UpdateCameraMatrix = updateCameraMatrix;
            this.UpdatePropertyGrid = updatePropertyGrid;
            this.UpdateTreeViewObjs = updateTreeViewObjs;
        }

        #region Manipulation target
        public void SetSelectedManipulationTarget(ManipulationTarget target)
        {
            SelectedTarget = target ?? new ManipulationTarget { Type = ManipulationTargetType.None, DisplayName = "" };
            Globals.CurrentManipulationTarget = SelectedTarget;
            UpdateGL();
        }
        public List<ManipulationTarget> GetManipulationTargets()
        {
            List<TreeNode> selectedNodes = DataBase.SelectedNodes.Values.ToList();
            MoveObjCombos combos = MoveObjCombos.Null;
            string objectTypeName = "Object";
            if (selectedNodes.Count > 0)
            {
                for (int i = 0; i < selectedNodes.Count; i++)
                {
                    if (selectedNodes[i] is Object3D obj)
                    {
                        var parent = obj.Parent;
                        if (parent is EnemyNodeGroup)
                        {
                            combos |= MoveObjCombos.Enemy;
                            objectTypeName = "Enemy";
                        }
                        else if (parent is EtcModelNodeGroup)
                        {
                            combos |= MoveObjCombos.Etcmodel;
                            objectTypeName = "EtcModel";
                        }
                        else if (parent is SpecialNodeGroup Special)
                        {
                            TriggerZoneCategory triggerZoneCategory = Special.PropertyMethods.GetTriggerZoneCategory(obj.ObjLineRef);
                            if (triggerZoneCategory == TriggerZoneCategory.Category01 || triggerZoneCategory == TriggerZoneCategory.Category02)
                            {
                                combos |= MoveObjCombos.TriggerZone;
                            }

                            if (Special.PropertyMethods.GetSpecialType(obj.ObjLineRef) == SpecialType.T03_Items)
                            {
                                combos |= MoveObjCombos.Item;
                                objectTypeName = "Item";
                            }
                        }
                        else if (parent is ExtraNodeGroup Extra)
                        {
                            var Association = DataBase.Extras.AssociationList[obj.ObjLineRef];
                            if (Association.FileFormat == SpecialFileFormat.AEV)
                            {
                                var SpecialType = DataBase.FileAEV.Methods.GetSpecialType(Association.LineID);
                                if (SpecialType == SpecialType.T12_AshleyHideCommand)
                                {
                                    combos |= MoveObjCombos.ExtraSpecialAshley;
                                }
                                else
                                {
                                    combos |= MoveObjCombos.ExtraSpecialWarpLadderGrappleGun;
                                }
                            }
                            else if (Association.FileFormat == SpecialFileFormat.ITA)
                            {
                                var SpecialType = DataBase.FileITA.Methods.GetSpecialType(Association.LineID);
                                if (SpecialType == SpecialType.T12_AshleyHideCommand)
                                {
                                    combos |= MoveObjCombos.ExtraSpecialAshley;
                                }
                                else
                                {
                                    combos |= MoveObjCombos.ExtraSpecialWarpLadderGrappleGun;
                                }
                            }
                        }
                        else if (parent is NewAge_DSE_NodeGroup)
                        {
                            combos |= MoveObjCombos.DisableMoveObject;
                        }
                        else if (parent is NewAge_LIT_Groups_NodeGroup)
                        {
                            combos |= MoveObjCombos.DisableMoveObject;
                        }
                        else if (parent is NewAge_LIT_Entrys_NodeGroup)
                        {
                            combos |= MoveObjCombos.LitEntry;
                            objectTypeName = "LIT Point";
                        }
                        else if (parent is NewAge_FSE_NodeGroup fse)
                        {
                            TriggerZoneCategory triggerZoneCategory = fse.PropertyMethods.GetTriggerZoneCategory(obj.ObjLineRef);
                            if (triggerZoneCategory == TriggerZoneCategory.Category01 || triggerZoneCategory == TriggerZoneCategory.Category02)
                            {
                                combos |= MoveObjCombos.TriggerZone;
                            }
                        }
                        else if (parent is NewAge_ESAR_NodeGroup esar)
                        {
                            TriggerZoneCategory triggerZoneCategory = esar.PropertyMethods.GetTriggerZoneCategory(obj.ObjLineRef);
                            if (triggerZoneCategory == TriggerZoneCategory.Category01 || triggerZoneCategory == TriggerZoneCategory.Category02)
                            {
                                combos |= MoveObjCombos.TriggerZone;
                            }
                        }
                        else if (parent is NewAge_ESE_NodeGroup)
                        {
                            combos |= MoveObjCombos.EseEntry;
                            objectTypeName = "ESE Point";
                        }
                        else if (parent is NewAge_EMI_NodeGroup)
                        {
                            combos |= MoveObjCombos.EmiEntry;
                            objectTypeName = "EMI Point";
                        }
                        else if (parent is QuadCustomNodeGroup quad)
                        {
                            TriggerZoneCategory triggerZoneCategory = quad.PropertyMethods.GetTriggerZoneCategory(obj.ObjLineRef);
                            if (triggerZoneCategory == TriggerZoneCategory.Category01 || triggerZoneCategory == TriggerZoneCategory.Category02)
                            {
                                combos |= MoveObjCombos.TriggerZone;
                            }

                            QuadCustomPointStatus status = quad.PropertyMethods.GetQuadCustomPointStatus(obj.ObjLineRef);
                            if (status == QuadCustomPointStatus.ArrowPoint01 || status == QuadCustomPointStatus.CustomModel02)
                            {
                                combos |= MoveObjCombos.QuadCustom;
                                objectTypeName = "QuadCustom Point";
                            }
                        }
                        else if (parent is NewAge_EFF_NodeGroup)
                        {
                            combos |= MoveObjCombos.DisableMoveObject;
                        }
                        else if (parent is NewAge_EFF_Table9Entry_NodeGroup)
                        {
                            combos |= MoveObjCombos.EffTable9;
                        }
                        else if (parent is NewAge_EFF_EffectGroup_NodeGroup)
                        {
                            combos |= MoveObjCombos.EffEntry;
                        }
                        else if (parent is NewAge_EFF_EffectEntry_NodeGroup)
                        {
                            combos |= MoveObjCombos.EffEntry;
                        }
                    }
                }
                combos -= MoveObjCombos.Null;
                if (combos.HasFlag(MoveObjCombos.DisableMoveObject))
                {
                    combos = MoveObjCombos.DisableMoveObject;
                }
            }

            var targets = new List<ManipulationTarget>();
            if (combos == MoveObjCombos.Null || combos == MoveObjCombos.DisableMoveObject)
            {
                targets.Add(new ManipulationTarget { Type = ManipulationTargetType.None, DisplayName = "No object selected." });
                return targets;
            }

            bool isCompound = combos.HasFlag(MoveObjCombos.TriggerZone) &&
                      (combos.HasFlag(MoveObjCombos.Item) || combos.HasFlag(MoveObjCombos.QuadCustom));
            if (isCompound){
                targets.Add(new ManipulationTarget { Type = ManipulationTargetType.Compound, DisplayName = "All" });
            }

            //standard objects (enemy, item, etcModel, etc)
            if (combos.HasFlag(MoveObjCombos.Enemy) || combos.HasFlag(MoveObjCombos.Etcmodel) || combos.HasFlag(MoveObjCombos.Item) || combos.HasFlag(MoveObjCombos.QuadCustom))
            {
                targets.Add(new ManipulationTarget { Type = ManipulationTargetType.Object, DisplayName = objectTypeName });
            }

            //trigger zones
            if (combos.HasFlag(MoveObjCombos.TriggerZone))
            {
                targets.Add(new ManipulationTarget { Type = ManipulationTargetType.TriggerZone, DisplayName = "Trigger Zone" });
                targets.Add(new ManipulationTarget { Type = ManipulationTargetType.TriggerPoint0, DisplayName = "Trigger Point 0" });
                targets.Add(new ManipulationTarget { Type = ManipulationTargetType.TriggerPoint1, DisplayName = "Trigger Point 1" });
                targets.Add(new ManipulationTarget { Type = ManipulationTargetType.TriggerPoint2, DisplayName = "Trigger Point 2" });
                targets.Add(new ManipulationTarget { Type = ManipulationTargetType.TriggerPoint3, DisplayName = "Trigger Point 3" });
            }

            if (combos.HasFlag(MoveObjCombos.ExtraSpecialAshley))
            {
                targets.Add(new ManipulationTarget { Type = ManipulationTargetType.Object, DisplayName = "Ashley Position" });
                targets.Add(new ManipulationTarget { Type = ManipulationTargetType.AshleyZone, DisplayName = "Hide Zone" });
            }


            if (targets.Count == 0)
            {
                targets.Add(new ManipulationTarget { Type = ManipulationTargetType.Object, DisplayName = "Object Position" });
            }

            return targets;
        }

        #endregion

        public List<MoveObjTypeObjForListBox> GetAvailableMoveModes()
        {
            List<TreeNode> selectedNodes = DataBase.SelectedNodes.Values.ToList();
            MoveObjCombos combos = MoveObjCombos.Null;

            if (selectedNodes.Count > 0)
            {
                for (int i = 0; i < selectedNodes.Count; i++)
                {
                    if (selectedNodes[i] is Object3D obj)
                    {
                        var parent = obj.Parent;
                        if (parent is EnemyNodeGroup)
                        {
                            combos |= MoveObjCombos.Enemy;
                        }
                        else if (parent is EtcModelNodeGroup)
                        {
                            combos |= MoveObjCombos.Etcmodel;
                        }
                        else if (parent is SpecialNodeGroup Special)
                        {
                            TriggerZoneCategory triggerZoneCategory = Special.PropertyMethods.GetTriggerZoneCategory(obj.ObjLineRef);
                            if (triggerZoneCategory == TriggerZoneCategory.Category01 || triggerZoneCategory == TriggerZoneCategory.Category02)
                            {
                                combos |= MoveObjCombos.TriggerZone;
                            }

                            if (Special.PropertyMethods.GetSpecialType(obj.ObjLineRef) == SpecialType.T03_Items)
                            {
                                combos |= MoveObjCombos.Item;
                            }
                        }
                        else if (parent is ExtraNodeGroup Extra)
                        {
                            var Association = DataBase.Extras.AssociationList[obj.ObjLineRef];
                            if (Association.FileFormat == SpecialFileFormat.AEV)
                            {
                                var SpecialType = DataBase.FileAEV.Methods.GetSpecialType(Association.LineID);
                                if (SpecialType == SpecialType.T12_AshleyHideCommand)
                                {
                                    combos |= MoveObjCombos.ExtraSpecialAshley;
                                }
                                else
                                {
                                    combos |= MoveObjCombos.ExtraSpecialWarpLadderGrappleGun;
                                }
                            }
                            else if (Association.FileFormat == SpecialFileFormat.ITA)
                            {
                                var SpecialType = DataBase.FileITA.Methods.GetSpecialType(Association.LineID);
                                if (SpecialType == SpecialType.T12_AshleyHideCommand)
                                {
                                    combos |= MoveObjCombos.ExtraSpecialAshley;
                                }
                                else
                                {
                                    combos |= MoveObjCombos.ExtraSpecialWarpLadderGrappleGun;
                                }
                            }
                        }
                        else if (parent is NewAge_DSE_NodeGroup)
                        {
                            combos |= MoveObjCombos.DisableMoveObject;
                        }
                        else if (parent is NewAge_LIT_Groups_NodeGroup)
                        {
                            combos |= MoveObjCombos.DisableMoveObject;
                        }
                        else if (parent is NewAge_LIT_Entrys_NodeGroup)
                        {
                            combos |= MoveObjCombos.LitEntry;
                        }
                        else if (parent is NewAge_FSE_NodeGroup fse)
                        {
                            TriggerZoneCategory triggerZoneCategory = fse.PropertyMethods.GetTriggerZoneCategory(obj.ObjLineRef);
                            if (triggerZoneCategory == TriggerZoneCategory.Category01 || triggerZoneCategory == TriggerZoneCategory.Category02)
                            {
                                combos |= MoveObjCombos.TriggerZone;
                            }
                        }
                        else if (parent is NewAge_ESAR_NodeGroup esar)
                        {
                            TriggerZoneCategory triggerZoneCategory = esar.PropertyMethods.GetTriggerZoneCategory(obj.ObjLineRef);
                            if (triggerZoneCategory == TriggerZoneCategory.Category01 || triggerZoneCategory == TriggerZoneCategory.Category02)
                            {
                                combos |= MoveObjCombos.TriggerZone;
                            }
                        }
                        else if (parent is NewAge_ESE_NodeGroup)
                        {
                            combos |= MoveObjCombos.EseEntry;
                        }
                        else if (parent is NewAge_EMI_NodeGroup)
                        {
                            combos |= MoveObjCombos.EmiEntry;
                        }
                        else if (parent is QuadCustomNodeGroup quad)
                        {
                            TriggerZoneCategory triggerZoneCategory = quad.PropertyMethods.GetTriggerZoneCategory(obj.ObjLineRef);
                            if (triggerZoneCategory == TriggerZoneCategory.Category01 || triggerZoneCategory == TriggerZoneCategory.Category02)
                            {
                                combos |= MoveObjCombos.TriggerZone;
                            }

                            QuadCustomPointStatus status = quad.PropertyMethods.GetQuadCustomPointStatus(obj.ObjLineRef);
                            if (status == QuadCustomPointStatus.ArrowPoint01 || status == QuadCustomPointStatus.CustomModel02)
                            {
                                combos |= MoveObjCombos.QuadCustom;
                            }
                        }
                        else if (parent is NewAge_EFF_NodeGroup)
                        {
                            combos |= MoveObjCombos.DisableMoveObject;
                        }
                        else if (parent is NewAge_EFF_Table9Entry_NodeGroup)
                        {
                            combos |= MoveObjCombos.EffTable9;
                        }
                        else if (parent is NewAge_EFF_EffectGroup_NodeGroup)
                        {
                            combos |= MoveObjCombos.EffEntry;
                        }
                        else if (parent is NewAge_EFF_EffectEntry_NodeGroup)
                        {
                            combos |= MoveObjCombos.EffEntry;
                        }
                    }
                }
                combos -= MoveObjCombos.Null;
                if (combos.HasFlag(MoveObjCombos.DisableMoveObject))
                {
                    combos = MoveObjCombos.DisableMoveObject;
                }
            }

            var items = new List<MoveObjTypeObjForListBox>();
            if (combos == MoveObjCombos.Null || combos == MoveObjCombos.DisableMoveObject)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.Null, ""));
            }
            else if (combos == MoveObjCombos.Enemy)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal123RotationObjXYZ, Lang.GetText(eLang.MoveMode_Enemy_PositionAndRotationAll)));
            }
            else if (combos == MoveObjCombos.Etcmodel)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal123RotationObjXYZ, Lang.GetText(eLang.MoveMode_EtcModel_PositionAndRotationAll)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareNone_VerticalScaleObjAll_Horizontal123ScaleObjXYZ, Lang.GetText(eLang.MoveMode_EtcModel_Scale)));
            }
            else if (combos == MoveObjCombos.EffEntry)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal123RotationObjXYZ, Lang.GetText(eLang.MoveMode_EffEntry_PositionAndRotationAll)));
            }
            else if (combos == MoveObjCombos.EffTable9)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal123None, Lang.GetText(eLang.MoveMode_EffEffTable9_PositionPoint)));
            }
            else if (combos == MoveObjCombos.EseEntry)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal123None, Lang.GetText(eLang.MoveMode_EseEntry_PositionPoint)));
            }
            else if (combos == MoveObjCombos.LitEntry)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal123None, Lang.GetText(eLang.MoveMode_LitEntry_PositionPoint)));
            }
            else if (combos == MoveObjCombos.EmiEntry)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal1None_Horizontal2RotationObjY_Horizontal3None, Lang.GetText(eLang.MoveMode_EmiEntry_PositionAndAnglePoint)));
            }
            else if (combos == MoveObjCombos.ExtraSpecialWarpLadderGrappleGun)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal1None_Horizontal2RotationObjY_Horizontal3None, Lang.GetText(eLang.MoveMode_Obj_PositionAndRotationY)));
            }
            else if (combos == MoveObjCombos.ExtraSpecialAshley)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal123None, Lang.GetText(eLang.MoveMode_Ashley_Position)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveAshleyAllPointsXZ_VerticalNone_Horizontal1None_Horizontal2RotationZoneY_Horizontal3ScaleAll, Lang.GetText(eLang.MoveMode_AshleyZone_MoveAll)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveAshleyPoint0XZ_VerticalNone_Horizontal123None, Lang.GetText(eLang.MoveMode_AshleyZone_Point0)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveAshleyPoint1XZ_VerticalNone_Horizontal123None, Lang.GetText(eLang.MoveMode_AshleyZone_Point1)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveAshleyPoint2XZ_VerticalNone_Horizontal123None, Lang.GetText(eLang.MoveMode_AshleyZone_Point2)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveAshleyPoint3XZ_VerticalNone_Horizontal123None, Lang.GetText(eLang.MoveMode_AshleyZone_Point3)));
            }
            else if (combos == MoveObjCombos.Item)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal123RotationObjXYZ, Lang.GetText(eLang.MoveMode_Item_PositionAndRotationAll)));
            }
            else if (combos == MoveObjCombos.QuadCustom)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal123RotationObjXYZ, Lang.GetText(eLang.MoveMode_QuadCustomPoint_PositionAndRotationAll)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareNone_VerticalScaleObjAll_Horizontal123ScaleObjXYZ, Lang.GetText(eLang.MoveMode_QuadCustomPoint_Scale)));
            }
            else if (combos == MoveObjCombos.TriggerZone)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneAllPointsXZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal2RotationZoneY_Horizontal3ScaleAll, Lang.GetText(eLang.MoveMode_TriggerZone_MoveAll)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint0XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point0)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint1XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point1)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint2XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point2)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint3XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point3)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallPoint01and12XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall01)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallPoint12and23XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall12)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallpoint23and30XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall23)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallPoint30and01XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall30)));
            }
            else if (combos == MoveObjCombos.Combo_Item_TriggerZone)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal123RotationObjXYZ, Lang.GetText(eLang.MoveMode_Item_PositionAndRotationAll)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneAllPointsXZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal2RotationZoneY_Horizontal3ScaleAll, Lang.GetText(eLang.MoveMode_TriggerZone_MoveAll)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint0XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point0)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint1XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point1)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint2XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point2)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint3XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point3)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallPoint01and12XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall01)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallPoint12and23XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall12)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallpoint23and30XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall23)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallPoint30and01XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall30)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.AllMoveXYZ_Horizontal123None, Lang.GetText(eLang.MoveMode_TriggerZone_MoveAll_Obj_Position)));
            }
            else if (combos == MoveObjCombos.Combo_QuadCustom_TriggerZone)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal123RotationObjXYZ, Lang.GetText(eLang.MoveMode_QuadCustomPoint_PositionAndRotationAll)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareNone_VerticalScaleObjAll_Horizontal123ScaleObjXYZ, Lang.GetText(eLang.MoveMode_QuadCustomPoint_Scale)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneAllPointsXZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal2RotationZoneY_Horizontal3ScaleAll, Lang.GetText(eLang.MoveMode_TriggerZone_MoveAll)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint0XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point0)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint1XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point1)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint2XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point2)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint3XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point3)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallPoint01and12XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall01)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallPoint12and23XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall12)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallpoint23and30XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall23)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallPoint30and01XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall30)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.AllMoveXYZ_Horizontal123None, Lang.GetText(eLang.MoveMode_TriggerZone_MoveAll_Obj_Position)));
            }
            else if (combos == MoveObjCombos.Combo_Item_QuadCustom_TriggerZone)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal123RotationObjXYZ, Lang.GetText(eLang.MoveMode_Obj_PositionAndRotationAll)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneAllPointsXZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal2RotationZoneY_Horizontal3ScaleAll, Lang.GetText(eLang.MoveMode_TriggerZone_MoveAll)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint0XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point0)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint1XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point1)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint2XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point2)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZonePoint3XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Point3)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallPoint01and12XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall01)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallPoint12and23XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall12)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallpoint23and30XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall23)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveTriggerZoneWallPoint30and01XZ_VerticalMoveTriggerZoneY_Horizontal1ChangeTriggerZoneHeight_Horizontal23None, Lang.GetText(eLang.MoveMode_TriggerZone_Wall30)));
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.AllMoveXYZ_Horizontal123None, Lang.GetText(eLang.MoveMode_TriggerZone_MoveAll_Obj_Position)));
            }
            else if (combos == MoveObjCombos.Combo_Item_QuadCustom)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal123RotationObjXYZ, Lang.GetText(eLang.MoveMode_Obj_PositionAndRotationAll)));
            }
            else if ((combos.HasFlag(MoveObjCombos.Enemy)
                || combos.HasFlag(MoveObjCombos.Etcmodel)
                || combos.HasFlag(MoveObjCombos.QuadCustom)
                || combos.HasFlag(MoveObjCombos.Item)
                || combos.HasFlag(MoveObjCombos.EffEntry)
                ) && !(
                combos.HasFlag(MoveObjCombos.TriggerZone)
                || combos.HasFlag(MoveObjCombos.EffTable9)
                || combos.HasFlag(MoveObjCombos.EmiEntry)
                || combos.HasFlag(MoveObjCombos.EseEntry)
                || combos.HasFlag(MoveObjCombos.LitEntry)
                || combos.HasFlag(MoveObjCombos.ExtraSpecialAshley)
                || combos.HasFlag(MoveObjCombos.ExtraSpecialWarpLadderGrappleGun)
                ))
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal123RotationObjXYZ, Lang.GetText(eLang.MoveMode_Obj_PositionAndRotationAll)));
            }
            else if ((
              combos.HasFlag(MoveObjCombos.Enemy)
              || combos.HasFlag(MoveObjCombos.Etcmodel)
              || combos.HasFlag(MoveObjCombos.QuadCustom)
              || combos.HasFlag(MoveObjCombos.Item)
              || combos.HasFlag(MoveObjCombos.EmiEntry)
              || combos.HasFlag(MoveObjCombos.ExtraSpecialWarpLadderGrappleGun)
              || combos.HasFlag(MoveObjCombos.EffEntry)
              ) && !(
              combos.HasFlag(MoveObjCombos.TriggerZone)
              || combos.HasFlag(MoveObjCombos.EffTable9)
              || combos.HasFlag(MoveObjCombos.EseEntry)
              || combos.HasFlag(MoveObjCombos.LitEntry)
              || combos.HasFlag(MoveObjCombos.ExtraSpecialAshley)
              ))
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.SquareMoveObjXZ_VerticalMoveObjY_Horizontal1None_Horizontal2RotationObjY_Horizontal3None, Lang.GetText(eLang.MoveMode_Obj_PositionAndRotationY)));
            }
            else if (combos != MoveObjCombos.Null)
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.AllMoveXYZ_Horizontal123None, Lang.GetText(eLang.MoveMode_Obj_Position)));
            }
            else
            {
                items.Add(new MoveObjTypeObjForListBox(MoveObjType.Null, ""));
            }
            return items;
        }

        public void SetSelectedMoveMode(MoveObjType type)
        {
            if (type != MoveObjType.Null)
            {
                MoveObjTypeSelected = type;
                Globals.CurrentMoveObjType = MoveObjTypeSelected;
                EnableSquare = (type.HasFlag(MoveObjType._SquareMoveObjXZ) || type.HasFlag(MoveObjType._SquareMoveTriggerZone) || type.HasFlag(MoveObjType._SquareMoveAshleyZone) || type.HasFlag(MoveObjType._AllMoveXYZ));
                EnableVertical = (type.HasFlag(MoveObjType._VerticalMoveObjY) || type.HasFlag(MoveObjType._VerticalScaleObjAll) || type.HasFlag(MoveObjType._VerticalMoveTriggerZoneY) || type.HasFlag(MoveObjType._AllMoveXYZ));
                EnableHorizontal1 = (type.HasFlag(MoveObjType._Horizontal1RotationObjX) || type.HasFlag(MoveObjType._Horizontal1ScaleObjX) || type.HasFlag(MoveObjType._Horizontal1ChangeTriggerZoneHeight));
                EnableHorizontal2 = (type.HasFlag(MoveObjType._Horizontal2RotationObjY) || type.HasFlag(MoveObjType._Horizontal2ScaleObjY) || type.HasFlag(MoveObjType._Horizontal2RotationZoneY));
                EnableHorizontal3 = (type.HasFlag(MoveObjType._Horizontal3RotationObjZ) || type.HasFlag(MoveObjType._Horizontal3ScaleObjZ) || type.HasFlag(MoveObjType._Horizontal3TriggerZoneScaleAll) || type.HasFlag(MoveObjType._Horizontal3AshleyZoneScaleAll));
            }
            else
            {
                MoveObjTypeSelected = MoveObjType.Null;
                Globals.CurrentMoveObjType = MoveObjTypeSelected;
                EnableSquare = false;
                EnableVertical = false;
                EnableHorizontal1 = false;
                EnableHorizontal2 = false;
                EnableHorizontal3 = false;
            }
            UpdateGL();
        }

        public void DropToGround()
        {
            if (DataBase.SelectedRoom != null)
            {
                foreach (TreeNode item in DataBase.SelectedNodes.Values)
                {
                    if (item.Parent != null && item is Object3D obj)
                    {
                        var PosArr = obj.GetObjPostion_ToMove_General();
                        if (PosArr.Length >= 1)
                        {
                            if (MoveObjTypeSelected.HasFlag(MoveObjType._AllMoveXYZ)
                                || MoveObjTypeSelected.HasFlag(MoveObjType._SquareMoveObjXZ))
                            {
                                PosArr[0].Y = DataBase.SelectedRoom.DropToGround(PosArr[0]);
                            }
                        }

                        if (PosArr.Length >= 7)
                        {
                            if (MoveObjTypeSelected.HasFlag(MoveObjType._AllMoveXYZ))
                            {
                                PosArr[5].Y = DataBase.SelectedRoom.DropToGround(new Vector3(PosArr[6].X, PosArr[5].Y, PosArr[6].Z));
                            }
                            else if (MoveObjTypeSelected.HasFlag(MoveObjType._SquareMoveTriggerZone))
                            {
                                TriggerZoneCategory category = obj.GetTriggerZoneCategory();
                                if (((MoveObjTypeSelected.HasFlag(MoveObjType.__AllPointsXZ) && category == TriggerZoneCategory.Category01)
                                    || (category == TriggerZoneCategory.Category02)))
                                {
                                    PosArr[5].Y = DataBase.SelectedRoom.DropToGround(new Vector3(PosArr[6].X, PosArr[5].Y, PosArr[6].Z));
                                }
                            }
                        }
                        obj.SetObjPostion_ToMove_General(PosArr);
                    }
                }
                if (camera.isOrbitCamera())
                {
                    camera.UpdateCameraOrbitOnChangeValue();
                    UpdateCameraMatrix();
                }
                UpdateGL();
                UpdatePropertyGrid();
                if (Globals.TreeNodeRenderHexValues)
                {
                    UpdateTreeViewObjs();
                }
            }
        }

        public string SetMoveSpeed(int trackBarValue)
        {
            float newValue;
            if (trackBarValue > 50)
            {
                newValue = 100.0f + ((trackBarValue - 50) * 8f);
            }
            else
            {
                newValue = (trackBarValue / 50.0f) * 100f;
            }
            if (newValue < 1f)
            {
                newValue = 1f;
            }
            else if (newValue > 96f && newValue < 114f)
            {
                newValue = 100f;
            }

            MoveObj.objSpeedMultiplier = newValue / 100.0f;
            return Lang.GetText(eLang.labelObjSpeed) + " " + ((int)newValue).ToString().PadLeft(3) + "%";
        }

        public void SetObjKeepOnGround(bool value) => MoveObj.KeepOnGround = value;
        public void SetTriggerZoneKeepOnGround(bool value) => MoveObj.TriggerZoneKeepOnGround = value;
        public void SetMoveRelativeCamera(bool value) => MoveObj.MoveRelativeCamera = value;
        public void SetLockMoveSquare(bool horizontal, bool vertical)
        {
            MoveObj.LockMoveSquareHorisontal = horizontal;
            MoveObj.LockMoveSquareVertical = vertical;
        }

        public void StartMove(MoveControlType control, Point location, MouseButtons button)
        {
            moveObj_lastMouseXY = location;
            move_Invert = (button == MouseButtons.Right);

            totalGizmoTranslation = Vector3.Zero;
            totalGizmoRotationAngles = Vector3.Zero;
            totalGizmoRotationQuat = Quaternion.Identity;

            switch (control)
            {
                case MoveControlType.Vertical:
                    SavedObjectData = MoveObjTypeSelected.HasFlag(MoveObjType._VerticalScaleObjAll) ? MoveObj.GetSavedScales() : MoveObj.GetSavedPosition();
                    break;
                case MoveControlType.Horizontal1:
                    if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal1RotationObjX)) SavedObjectData = MoveObj.GetSavedRotationAngles();
                    else if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal1ScaleObjX)) SavedObjectData = MoveObj.GetSavedScales();
                    else SavedObjectData = MoveObj.GetSavedPosition();
                    break;
                case MoveControlType.Horizontal2:
                    if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal2RotationObjY)) SavedObjectData = MoveObj.GetSavedRotationAngles();
                    else if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal2ScaleObjY)) SavedObjectData = MoveObj.GetSavedScales();
                    else SavedObjectData = MoveObj.GetSavedPosition();
                    break;
                case MoveControlType.Horizontal3:
                    if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal3RotationObjZ)) SavedObjectData = MoveObj.GetSavedRotationAngles();
                    else if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal3ScaleObjZ)) SavedObjectData = MoveObj.GetSavedScales();
                    else SavedObjectData = MoveObj.GetSavedPosition();
                    break;
                default:
                    SavedObjectData = MoveObj.GetSavedPosition();
                    break;
            }

            SavedObjectRotationData = MoveObj.GetSavedRotationAngles();
        }
        public void EndMove()
        {
            SavedObjectData = null;
            SavedObjectRotationData = null;
            totalGizmoTranslation = Vector3.Zero;
            totalGizmoRotationAngles = Vector3.Zero;
            totalGizmoRotationQuat = Quaternion.Identity;
        }

        public void PerformMove(MoveControlType control, MouseEventArgs e)
        {
            foreach (TreeNode item in DataBase.SelectedNodes.Values)
            {
                if (item is Object3D obj && item.Parent is TreeNodeGroup)
                {
                    Vector3[] oldPos = null;
                    var key = new MoveObj.ObjKey(obj.ObjLineRef, obj.Group);
                    if (SavedObjectData.ContainsKey(key))
                    {
                        oldPos = SavedObjectData[key];
                    }

                    switch (control)
                    {
                        case MoveControlType.Square:
                            PerformSquareMove(obj, e, oldPos);
                            break;
                        case MoveControlType.Vertical:
                            PerformVerticalMove(obj, e, oldPos);
                            break;
                        case MoveControlType.Horizontal1:
                            PerformHorizontal1Move(obj, e, oldPos);
                            break;
                        case MoveControlType.Horizontal2:
                            PerformHorizontal2Move(obj, e, oldPos);
                            break;
                        case MoveControlType.Horizontal3:
                            PerformHorizontal3Move(obj, e, oldPos);
                            break;
                    }
                }
            }

            if (camera.isOrbitCamera())
            {
                camera.UpdateCameraOrbitOnChangeValue();
                UpdateCameraMatrix();
            }
            UpdateGL();
            UpdatePropertyGrid();
            if (Globals.TreeNodeRenderHexValues)
            {
                UpdateTreeViewObjs();
            }
        }

        private void PerformSquareMove(Object3D obj, MouseEventArgs e, Vector3[] oldPos)
        {
            MoveObj.MoveDirection dir = (MoveObj.LockMoveSquareHorisontal) ? MoveObj.MoveDirection.Z :
                                      (MoveObj.LockMoveSquareVertical) ? MoveObj.MoveDirection.X :
                                      MoveObj.MoveDirection.X | MoveObj.MoveDirection.Z;

            if (MoveObjTypeSelected.HasFlag(MoveObjType._AllMoveXYZ))
            {
                MoveObj.MoveTriggerZonePlusObjPositionXYZ(obj, e, moveObj_lastMouseXY, oldPos, camera, dir, move_Invert);
            }
            else if (MoveObjTypeSelected.HasFlag(MoveObjType._SquareMoveObjXZ))
            {
                MoveObj.MoveObjPositionXYZ(obj, e, moveObj_lastMouseXY, oldPos, camera, dir, move_Invert);
            }
            else if (MoveObjTypeSelected.HasFlag(MoveObjType._SquareMoveTriggerZone))
            {
                MoveObj.MoveTriggerZonePositionXZ(obj, e, moveObj_lastMouseXY, oldPos, camera, dir, move_Invert, MoveObjTypeSelected, obj.GetTriggerZoneCategory());
            }
            else if (MoveObjTypeSelected.HasFlag(MoveObjType._SquareMoveAshleyZone))
            {
                MoveObj.MoveTriggerZonePositionXZ(obj, e, moveObj_lastMouseXY, oldPos, camera, dir, move_Invert, MoveObjTypeSelected, TriggerZoneCategory.Category01);
            }
        }

        private void PerformVerticalMove(Object3D obj, MouseEventArgs e, Vector3[] oldPos)
        {
            if (MoveObjTypeSelected.HasFlag(MoveObjType._AllMoveXYZ))
            {
                MoveObj.MoveTriggerZonePlusObjPositionXYZ(obj, e, moveObj_lastMouseXY, oldPos, camera, MoveObj.MoveDirection.Y, move_Invert);
            }
            else if (MoveObjTypeSelected.HasFlag(MoveObjType._VerticalMoveObjY))
            {
                MoveObj.MoveObjPositionXYZ(obj, e, moveObj_lastMouseXY, oldPos, camera, MoveObj.MoveDirection.Y, move_Invert);
            }
            else if (MoveObjTypeSelected.HasFlag(MoveObjType._VerticalMoveTriggerZoneY))
            {
                MoveObj.MoveTriggerZonePositionY(obj, e, moveObj_lastMouseXY, oldPos, move_Invert);
            }
            else if (MoveObjTypeSelected.HasFlag(MoveObjType._VerticalScaleObjAll))
            {
                MoveObj.MoveObjScaleXYZ(obj, e, moveObj_lastMouseXY, oldPos, MoveObj.MoveDirection.X | MoveObj.MoveDirection.Y | MoveObj.MoveDirection.Z, move_Invert);
            }
        }

        private void PerformHorizontal1Move(Object3D obj, MouseEventArgs e, Vector3[] oldPos)
        {
            if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal1RotationObjX))
            {
                MoveObj.MoveObjRotationAnglesXYZ(obj, e, moveObj_lastMouseXY, oldPos, MoveObj.MoveDirection.X, move_Invert);
            }
            else if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal1ScaleObjX))
            {
                MoveObj.MoveObjScaleXYZ(obj, e, moveObj_lastMouseXY, oldPos, MoveObj.MoveDirection.X, move_Invert);
            }
            else if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal1ChangeTriggerZoneHeight))
            {
                MoveObj.MoveTriggerZoneHeight(obj, e, moveObj_lastMouseXY, oldPos, move_Invert);
            }
        }

        private void PerformHorizontal2Move(Object3D obj, MouseEventArgs e, Vector3[] oldPos)
        {
            if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal2RotationObjY))
            {
                MoveObj.MoveObjRotationAnglesXYZ(obj, e, moveObj_lastMouseXY, oldPos, MoveObj.MoveDirection.Y, move_Invert);
            }
            else if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal2ScaleObjY))
            {
                MoveObj.MoveObjScaleXYZ(obj, e, moveObj_lastMouseXY, oldPos, MoveObj.MoveDirection.Y, move_Invert);
            }
            else if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal2RotationZoneY))
            {
                MoveObj.MoveZoneRotate(obj, e, moveObj_lastMouseXY, oldPos, move_Invert);
            }
        }

        private void PerformHorizontal3Move(Object3D obj, MouseEventArgs e, Vector3[] oldPos)
        {
            if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal3RotationObjZ))
            {
                MoveObj.MoveObjRotationAnglesXYZ(obj, e, moveObj_lastMouseXY, oldPos, MoveObj.MoveDirection.Z, move_Invert);
            }
            else if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal3ScaleObjZ))
            {
                MoveObj.MoveObjScaleXYZ(obj, e, moveObj_lastMouseXY, oldPos, MoveObj.MoveDirection.Z, move_Invert);
            }
            else if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal3TriggerZoneScaleAll))
            {
                MoveObj.MoveTriggerZoneScale(obj, e, moveObj_lastMouseXY, oldPos, move_Invert, obj.GetTriggerZoneCategory());
            }
            else if (MoveObjTypeSelected.HasFlag(MoveObjType._Horizontal3AshleyZoneScaleAll))
            {
                MoveObj.MoveAshleyZoneScale(obj, e, moveObj_lastMouseXY, oldPos, move_Invert);
            }
        }

        public void ApplyGizmoTranslation(Vector3 frameTranslationDelta)
        {
            if (SavedObjectData == null || SelectedTarget == null || SelectedTarget.Type == ManipulationTargetType.None) return;

            totalGizmoTranslation += frameTranslationDelta;
            Vector3 totalGameUnitTranslation = totalGizmoTranslation * 100f;

            foreach (var entry in SavedObjectData)
            {
                Object3D obj = MoveObj.GetObjectFromKey(entry.Key);
                if (obj == null) continue;

                Vector3[] initialData = entry.Value;
                Vector3[] newData = (Vector3[])initialData.Clone();

                switch (SelectedTarget.Type)
                {
                    case ManipulationTargetType.Compound:
                        for (int i = 0; i < newData.Length; i++)
                        {
                            if (i == 5){
                                newData[5].Y = initialData[5].Y + totalGameUnitTranslation.Y;
                            }else{
                                newData[i] = initialData[i] + totalGameUnitTranslation;
                            }
                        }
                        break;

                    case ManipulationTargetType.Object:
                        if (newData.Length > 0) newData[0] = initialData[0] + totalGameUnitTranslation;
                        break;

                    case ManipulationTargetType.TriggerZone:
                    case ManipulationTargetType.AshleyZone:
                        var horizontalTranslation = new Vector3(totalGameUnitTranslation.X, 0, totalGameUnitTranslation.Z);
                        var verticalTranslation = new Vector3(0, totalGameUnitTranslation.Y, 0);
                        //move all corners horizontally
                        for (int i = 1; i <= 4; i++){
                            if (newData.Length > i) newData[i] = initialData[i] + horizontalTranslation;
                        }
                        //move center horizontally
                        if (newData.Length > 6) newData[6] = initialData[6] + horizontalTranslation;
                        //move entire zone vertically
                        if (newData.Length > 5) newData[5].Y = initialData[5].Y + verticalTranslation.Y;
                        break;

                    case ManipulationTargetType.TriggerPoint0:
                        if (newData.Length > 1) newData[1] = initialData[1] + totalGameUnitTranslation;
                        break;
                    case ManipulationTargetType.TriggerPoint1:
                        if (newData.Length > 2) newData[2] = initialData[2] + totalGameUnitTranslation;
                        break;
                    case ManipulationTargetType.TriggerPoint2:
                        if (newData.Length > 3) newData[3] = initialData[3] + totalGameUnitTranslation;
                        break;
                    case ManipulationTargetType.TriggerPoint3:
                        if (newData.Length > 4) newData[4] = initialData[4] + totalGameUnitTranslation;
                        break;
                }

                obj.SetObjPostion_ToMove_General(newData);
            }

            if (camera.isOrbitCamera())
            {
                camera.UpdateCameraOrbitOnChangeValue();
                UpdateCameraMatrix();
            }
            UpdateGL();
            UpdatePropertyGrid();
        }


        public void ApplyGizmoRotation(float angleDeltaDeg, Gizmo.GizmoAxis axis)
        {
            if (SavedObjectData == null || SavedObjectRotationData == null) return;

            //gizmo rotation for now is a bit rough for compound axis rotation,
            //but if we keep simple rotations it mostly works for now, I plan to revist this later when I stop being a dumbass

            switch (axis)
            {
                case Gizmo.GizmoAxis.X: totalGizmoRotationAngles.X += angleDeltaDeg; break;
                case Gizmo.GizmoAxis.Y: totalGizmoRotationAngles.Y += angleDeltaDeg; break;
                case Gizmo.GizmoAxis.Z: totalGizmoRotationAngles.Z += angleDeltaDeg; break;
            }

            Vector3 rotationCenterGameUnits = Vector3.Zero;
            if (DataBase.LastSelectNode is Object3D o)
            {
                rotationCenterGameUnits = o.GetGizmoPosition() * 100f;
            }

            foreach (var entry in SavedObjectData)
            {
                Object3D obj = MoveObj.GetObjectFromKey(entry.Key);
                if (obj == null) continue;

                switch (SelectedTarget.Type)
                {
                    case ManipulationTargetType.Compound:{
                        //main object
                        if (SavedObjectRotationData.TryGetValue(entry.Key, out Vector3[] initialRot) && initialRot != null)
                        {
                            Vector3[] newRot = (Vector3[])initialRot.Clone();
                            newRot[0] = initialRot[0] + totalGizmoRotationAngles;
                            obj.SetObjRotarionAngles_ToMove(newRot);
                        }

                        //triggerzones y
                        if (axis == Gizmo.GizmoAxis.Y)
                        {
                            Vector3[] initialPos = entry.Value;
                            Vector3[] newPos = (Vector3[])initialPos.Clone();

                            float totalAngleRad = MathHelper.DegreesToRadians(totalGizmoRotationAngles.Y);
                            Matrix4 totalRotationMatrix = Matrix4.CreateRotationY(totalAngleRad);

                            for (int i = 1; i <= 4; i++)
                            {
                                if (newPos.Length > i)
                                {
                                    Vector3 centeredPoint = initialPos[i] - rotationCenterGameUnits;
                                    Vector3 rotatedPoint = Vector3.TransformPosition(centeredPoint, totalRotationMatrix);
                                    newPos[i] = rotatedPoint + rotationCenterGameUnits;
                                }
                            }
                            obj.SetObjPostion_ToMove_General(newPos);
                        }
                        break;}

                    case ManipulationTargetType.Object:{
                        //this block rotates the main object if it has rotation data
                        if (SavedObjectRotationData.TryGetValue(entry.Key, out Vector3[] initialRot) && initialRot != null)
                        {
                            Vector3[] newRot = (Vector3[])initialRot.Clone();
                            newRot[0] = initialRot[0] + totalGizmoRotationAngles;
                            obj.SetObjRotarionAngles_ToMove(newRot);
                        }
                        break;
                    }

                    case ManipulationTargetType.TriggerZone:
                    case ManipulationTargetType.AshleyZone:
                        //this block rotates the trigger zone's points around its center on the yaxis
                        if (axis == Gizmo.GizmoAxis.Y)
                        {
                            Vector3[] initialPos = entry.Value;
                            Vector3[] newPos = (Vector3[])initialPos.Clone();

                            float totalAngleRad = MathHelper.DegreesToRadians(totalGizmoRotationAngles.Y);
                            Matrix4 totalRotationMatrix = Matrix4.CreateRotationY(totalAngleRad);

                            //rotate each corner point
                            for (int i = 1; i <= 4; i++)
                            {
                                if (newPos.Length > i)
                                {
                                    Vector3 centeredPoint = initialPos[i] - rotationCenterGameUnits;
                                    Vector3 rotatedPoint = Vector3.TransformPosition(centeredPoint, totalRotationMatrix);
                                    newPos[i] = rotatedPoint + rotationCenterGameUnits;
                                }
                            }
                            obj.SetObjPostion_ToMove_General(newPos);
                        }
                        break;
                }
            }

            if (camera.isOrbitCamera())
            {
                camera.UpdateCameraOrbitOnChangeValue();
                UpdateCameraMatrix();
            }
            UpdateGL();
            UpdatePropertyGrid();
        }

        public void ToggleGrid()
        {
            Globals.CamGridEnable = !Globals.CamGridEnable;
            UpdateGL();
        }

        public string SetGridSize(string text)
        {
            if (int.TryParse(text, out int value))
            {
                Globals.CamGridvalue = value;
                UpdateGL();
                return value.ToString();
            }
            else
            {
                Globals.CamGridvalue = 100;
                UpdateGL();
                return "100";
            }
        }
    }
}