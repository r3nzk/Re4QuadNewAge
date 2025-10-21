using Re4QuadExtremeEditor.Editor.Class.Enums;
using Re4QuadExtremeEditor.Editor.Class.Files;
using Re4QuadExtremeEditor.Editor.Class.TreeNodeObj;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Re4QuadExtremeEditor.Editor.Class
{
    public static class ProjectManager
    {
        public static void LoadProjectData(ProjectFile project)
        {
            SetupESLData(project.FileESL);
            SetupETSData(project.FileETS);
            SetupITAData(project.FileITA);
            SetupAEVData(project.FileAEV);
            SetupDSEData(project.FileDSE);
            SetupFSEData(project.FileFSE);
            SetupSARData(project.FileSAR);
            SetupEARData(project.FileEAR);
            SetupEMIData(project.FileEMI);
            SetupESEData(project.FileESE);
            SetupLITData(project.FileLIT);
            SetupQuadCustomData(project.FileQuadCustom);
            SetupEFFData(project.FileEFF);
        }


        #region Setup Data

        public static void SetupESLData(FileEnemyEslGroup esl)
        {
            if (esl == null) { FileManager.ClearESL(); return; }
            DataBase.FileESL = esl;
            DataBase.NodeESL.Nodes.Clear();
            DataBase.NodeESL.MethodsForGL = DataBase.FileESL.MethodsForGL;
            DataBase.NodeESL.PropertyMethods = DataBase.FileESL.Methods;
            DataBase.NodeESL.DisplayMethods = DataBase.FileESL.DisplayMethods;
            DataBase.NodeESL.MoveMethods = DataBase.FileESL.MoveMethods;
            var nodes = new List<Object3D>();
            for (ushort iN = 0; iN < 256; iN++)
            {
                nodes.Add(Object3D.CreateNewInstance(GroupType.ESL, iN));
            }
            DataBase.NodeESL.Nodes.AddRange(nodes.ToArray());
            DataBase.NodeESL.Expand();
        }

        public static void SetupETSData(FileEtcModelEtsGroup ets)
        {
            if (ets == null) { FileManager.ClearETS(); return; }
            DataBase.FileETS = ets;
            DataBase.NodeETS.Nodes.Clear();
            DataBase.NodeETS.MethodsForGL = DataBase.FileETS.MethodsForGL;
            DataBase.NodeETS.PropertyMethods = DataBase.FileETS.Methods;
            DataBase.NodeETS.DisplayMethods = DataBase.FileETS.DisplayMethods;
            DataBase.NodeETS.MoveMethods = DataBase.FileETS.MoveMethods;
            DataBase.NodeETS.ChangeAmountMethods = DataBase.FileETS.ChangeAmountMethods;
            var nodes = new List<Object3D>();
            for (ushort iN = 0; iN < ets.Lines.Count; iN++)
            {
                Object3D o = Object3D.CreateNewInstance(GroupType.ETS, iN);
                nodes.Add(o);
            }
            DataBase.NodeETS.Nodes.AddRange(nodes.ToArray());
            DataBase.NodeETS.Expand();
        }

        public static void SetupITAData(FileSpecialGroup ita)
        {
            if (ita == null) { FileManager.ClearITA(); return; }
            DataBase.FileITA = ita;
            DataBase.Extras.SetStartRefInteractionTypeContent();
            DataBase.Extras.ClearITAs();
            DataBase.NodeITA.Nodes.Clear();
            DataBase.NodeITA.MethodsForGL = DataBase.FileITA.MethodsForGL;
            DataBase.NodeITA.ExtrasMethodsForGL = DataBase.FileITA.ExtrasMethodsForGL;
            DataBase.NodeITA.PropertyMethods = DataBase.FileITA.Methods;
            DataBase.NodeITA.DisplayMethods = DataBase.FileITA.DisplayMethods;
            DataBase.NodeITA.MoveMethods = DataBase.FileITA.MoveMethods;
            DataBase.NodeITA.ChangeAmountMethods = DataBase.FileITA.ChangeAmountMethods;
            var nodes = new List<Object3D>();
            for (ushort iN = 0; iN < ita.Lines.Count; iN++)
            {
                Object3D o = Object3D.CreateNewInstance(GroupType.ITA, iN);
                nodes.Add(o);
            }
            DataBase.NodeITA.Nodes.AddRange(nodes.ToArray());
            DataBase.NodeITA.Expand();
            DataBase.Extras.AddITAs();
            DataBase.NodeEXTRAS.Expand();
        }

        public static void SetupAEVData(FileSpecialGroup aev)
        {
            if (aev == null) { FileManager.ClearAEV(); return; }
            DataBase.FileAEV = aev;
            DataBase.Extras.SetStartRefInteractionTypeContent();
            DataBase.Extras.ClearAll();
            DataBase.NodeAEV.Nodes.Clear();
            DataBase.NodeAEV.MethodsForGL = DataBase.FileAEV.MethodsForGL;
            DataBase.NodeAEV.ExtrasMethodsForGL = DataBase.FileAEV.ExtrasMethodsForGL;
            DataBase.NodeAEV.PropertyMethods = DataBase.FileAEV.Methods;
            DataBase.NodeAEV.DisplayMethods = DataBase.FileAEV.DisplayMethods;
            DataBase.NodeAEV.MoveMethods = DataBase.FileAEV.MoveMethods;
            DataBase.NodeAEV.ChangeAmountMethods = DataBase.FileAEV.ChangeAmountMethods;
            var nodes = new List<Object3D>();
            for (ushort iN = 0; iN < aev.Lines.Count; iN++)
            {
                Object3D o = Object3D.CreateNewInstance(GroupType.AEV, iN);
                nodes.Add(o);
            }
            DataBase.NodeAEV.Nodes.AddRange(nodes.ToArray());
            DataBase.NodeAEV.Expand();
            DataBase.Extras.AddAll();
            DataBase.NodeEXTRAS.Expand();
        }

        public static void SetupDSEData(File_DSE_Group dse)
        {
            if (dse == null) { FileManager.ClearDSE(); return; }
            DataBase.FileDSE = dse;
            DataBase.NodeDSE.Nodes.Clear();
            DataBase.NodeDSE.PropertyMethods = DataBase.FileDSE.Methods;
            DataBase.NodeDSE.DisplayMethods = DataBase.FileDSE.DisplayMethods;
            DataBase.NodeDSE.MoveMethods = DataBase.FileDSE.MoveMethods;
            DataBase.NodeDSE.ChangeAmountMethods = DataBase.FileDSE.ChangeAmountMethods;
            var nodes = new List<Object3D>();
            for (ushort iN = 0; iN < dse.Lines.Count; iN++)
            {
                Object3D o = Object3D.CreateNewInstance(GroupType.DSE, iN);
                nodes.Add(o);
            }
            DataBase.NodeDSE.Nodes.AddRange(nodes.ToArray());
            DataBase.NodeDSE.Expand();
        }

        public static void SetupFSEData(File_FSE_Group fse)
        {
            if (fse == null) { FileManager.ClearFSE(); return; }
            DataBase.FileFSE = fse;
            DataBase.NodeFSE.Nodes.Clear();
            DataBase.NodeFSE.MethodsForGL = DataBase.FileFSE.MethodsForGL;
            DataBase.NodeFSE.PropertyMethods = DataBase.FileFSE.Methods;
            DataBase.NodeFSE.DisplayMethods = DataBase.FileFSE.DisplayMethods;
            DataBase.NodeFSE.MoveMethods = DataBase.FileFSE.MoveMethods;
            DataBase.NodeFSE.ChangeAmountMethods = DataBase.FileFSE.ChangeAmountMethods;
            var nodes = new List<Object3D>();
            for (ushort iN = 0; iN < fse.Lines.Count; iN++)
            {
                Object3D o = Object3D.CreateNewInstance(GroupType.FSE, iN);
                nodes.Add(o);
            }
            DataBase.NodeFSE.Nodes.AddRange(nodes.ToArray());
            DataBase.NodeFSE.Expand();
        }

        public static void SetupSARData(File_ESAR_Group sar)
        {
            if (sar == null) { FileManager.ClearSAR(); return; }
            DataBase.FileSAR = sar;
            DataBase.NodeSAR.Nodes.Clear();
            DataBase.NodeSAR.MethodsForGL = DataBase.FileSAR.MethodsForGL;
            DataBase.NodeSAR.PropertyMethods = DataBase.FileSAR.Methods;
            DataBase.NodeSAR.DisplayMethods = DataBase.FileSAR.DisplayMethods;
            DataBase.NodeSAR.MoveMethods = DataBase.FileSAR.MoveMethods;
            DataBase.NodeSAR.ChangeAmountMethods = DataBase.FileSAR.ChangeAmountMethods;
            var nodes = new List<Object3D>();
            for (ushort iN = 0; iN < sar.Lines.Count; iN++)
            {
                Object3D o = Object3D.CreateNewInstance(GroupType.SAR, iN);
                nodes.Add(o);
            }
            DataBase.NodeSAR.Nodes.AddRange(nodes.ToArray());
            DataBase.NodeSAR.Expand();
        }

        public static void SetupEARData(File_ESAR_Group ear)
        {
            if (ear == null) { FileManager.ClearEAR(); return; }
            DataBase.FileEAR = ear;
            DataBase.NodeEAR.Nodes.Clear();
            DataBase.NodeEAR.MethodsForGL = DataBase.FileEAR.MethodsForGL;
            DataBase.NodeEAR.PropertyMethods = DataBase.FileEAR.Methods;
            DataBase.NodeEAR.DisplayMethods = DataBase.FileEAR.DisplayMethods;
            DataBase.NodeEAR.MoveMethods = DataBase.FileEAR.MoveMethods;
            DataBase.NodeEAR.ChangeAmountMethods = DataBase.FileEAR.ChangeAmountMethods;
            var nodes = new List<Object3D>();
            for (ushort iN = 0; iN < ear.Lines.Count; iN++)
            {
                Object3D o = Object3D.CreateNewInstance(GroupType.EAR, iN);
                nodes.Add(o);
            }
            DataBase.NodeEAR.Nodes.AddRange(nodes.ToArray());
            DataBase.NodeEAR.Expand();
        }

        public static void SetupEMIData(File_EMI_Group emi)
        {
            if (emi == null) { FileManager.ClearEMI(); return; }
            DataBase.FileEMI = emi;
            DataBase.NodeEMI.Nodes.Clear();
            DataBase.NodeEMI.MethodsForGL = DataBase.FileEMI.MethodsForGL;
            DataBase.NodeEMI.PropertyMethods = DataBase.FileEMI.Methods;
            DataBase.NodeEMI.DisplayMethods = DataBase.FileEMI.DisplayMethods;
            DataBase.NodeEMI.MoveMethods = DataBase.FileEMI.MoveMethods;
            DataBase.NodeEMI.ChangeAmountMethods = DataBase.FileEMI.ChangeAmountMethods;
            var nodes = new List<Object3D>();
            for (ushort iN = 0; iN < emi.Lines.Count; iN++)
            {
                Object3D o = Object3D.CreateNewInstance(GroupType.EMI, iN);
                nodes.Add(o);
            }
            DataBase.NodeEMI.Nodes.AddRange(nodes.ToArray());
            DataBase.NodeEMI.Expand();
        }

        public static void SetupESEData(File_ESE_Group ese)
        {
            if (ese == null) { FileManager.ClearESE(); return; }
            DataBase.FileESE = ese;
            DataBase.NodeESE.Nodes.Clear();
            DataBase.NodeESE.MethodsForGL = DataBase.FileESE.MethodsForGL;
            DataBase.NodeESE.PropertyMethods = DataBase.FileESE.Methods;
            DataBase.NodeESE.DisplayMethods = DataBase.FileESE.DisplayMethods;
            DataBase.NodeESE.MoveMethods = DataBase.FileESE.MoveMethods;
            DataBase.NodeESE.ChangeAmountMethods = DataBase.FileESE.ChangeAmountMethods;
            var nodes = new List<Object3D>();
            for (ushort iN = 0; iN < ese.Lines.Count; iN++)
            {
                Object3D o = Object3D.CreateNewInstance(GroupType.ESE, iN);
                nodes.Add(o);
            }
            DataBase.NodeESE.Nodes.AddRange(nodes.ToArray());
            DataBase.NodeESE.Expand();
        }

        public static void SetupQuadCustomData(FileQuadCustomGroup quad)
        {
            if (quad == null) { FileManager.ClearQuadCustom(); return; }
            DataBase.FileQuadCustom = quad;
            DataBase.NodeQuadCustom.Nodes.Clear();
            DataBase.NodeQuadCustom.MethodsForGL = DataBase.FileQuadCustom.MethodsForGL;
            DataBase.NodeQuadCustom.PropertyMethods = DataBase.FileQuadCustom.Methods;
            DataBase.NodeQuadCustom.DisplayMethods = DataBase.FileQuadCustom.DisplayMethods;
            DataBase.NodeQuadCustom.MoveMethods = DataBase.FileQuadCustom.MoveMethods;
            DataBase.NodeQuadCustom.ChangeAmountMethods = DataBase.FileQuadCustom.ChangeAmountMethods;
            var nodes = new List<Object3D>();
            for (ushort iN = 0; iN < quad.Lines.Count; iN++)
            {
                Object3D o = Object3D.CreateNewInstance(GroupType.QUAD_CUSTOM, iN);
                o.NodeFont = Globals.TreeNodeFontText;
                nodes.Add(o);
            }
            DataBase.NodeQuadCustom.Nodes.AddRange(nodes.ToArray());
            DataBase.NodeQuadCustom.Expand();
        }

        public static void SetupLITData(File_LIT_Group lit)
        {
            if (lit == null) { FileManager.ClearLIT(); return; }
            DataBase.FileLIT = lit;
            DataBase.FileLIT.LightEntrys.ChangeAmountCallbackMethods = DataBase.NodeLIT_Entrys.ChangeAmountCallbackMethods;

            DataBase.NodeLIT_Groups.Nodes.Clear();
            DataBase.NodeLIT_Groups.MethodsForGL = DataBase.FileLIT.LightGroups.MethodsForGL;
            DataBase.NodeLIT_Groups.PropertyMethods = DataBase.FileLIT.LightGroups.Methods;
            DataBase.NodeLIT_Groups.DisplayMethods = DataBase.FileLIT.LightGroups.DisplayMethods;
            DataBase.NodeLIT_Groups.MoveMethods = DataBase.FileLIT.LightGroups.MoveMethods;
            DataBase.NodeLIT_Groups.ChangeAmountMethods = DataBase.FileLIT.LightGroups.ChangeAmountMethods;
            var groupNodes = new List<Object3D>();
            for (ushort iN = 0; iN < lit.LightGroups.Lines.Count; iN++)
            {
                groupNodes.Add(Object3D.CreateNewInstance(GroupType.LIT_GROUPS, iN));
            }
            DataBase.NodeLIT_Groups.Nodes.AddRange(groupNodes.ToArray());
            DataBase.NodeLIT_Groups.Expand();

            DataBase.NodeLIT_Entrys.Nodes.Clear();
            DataBase.NodeLIT_Entrys.MethodsForGL = DataBase.FileLIT.LightEntrys.MethodsForGL;
            DataBase.NodeLIT_Entrys.PropertyMethods = DataBase.FileLIT.LightEntrys.Methods;
            DataBase.NodeLIT_Entrys.DisplayMethods = DataBase.FileLIT.LightEntrys.DisplayMethods;
            DataBase.NodeLIT_Entrys.MoveMethods = DataBase.FileLIT.LightEntrys.MoveMethods;
            DataBase.NodeLIT_Entrys.ChangeAmountMethods = DataBase.FileLIT.LightEntrys.ChangeAmountMethods;
            var entryNodes = new List<Object3D>();
            for (ushort iN = 0; iN < lit.LightEntrys.Lines.Count; iN++)
            {
                entryNodes.Add(Object3D.CreateNewInstance(GroupType.LIT_ENTRYS, iN));
            }
            DataBase.NodeLIT_Entrys.Nodes.AddRange(entryNodes.ToArray());
            DataBase.NodeLIT_Entrys.Expand();
        }

        public static void SetupEFFData(File_EFFBLOB_Group eff)
        {
            if (eff == null) { FileManager.ClearEFFBLOB(); return; }
            DataBase.FileEFF = eff;
            //LoadFileEFFBLOB rest
        }

        #endregion
    }
}
