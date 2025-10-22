using Newtonsoft.Json;
using Re4QuadX.Editor.Class.Files;
using System.Collections.Generic;

namespace Re4QuadX.Editor.Class
{
    public class ProjectFile
    {
        public string Version { get; set; } = "1.0";

        //file
        public FileEnemyEslGroup FileESL { get; set; }
        public FileEtcModelEtsGroup FileETS { get; set; }
        public FileSpecialGroup FileITA { get; set; }
        public FileSpecialGroup FileAEV { get; set; }
        public File_DSE_Group FileDSE { get; set; }
        public File_EMI_Group FileEMI { get; set; }
        public File_ESAR_Group FileSAR { get; set; }
        public File_ESAR_Group FileEAR { get; set; }
        public File_ESE_Group FileESE { get; set; }
        public File_FSE_Group FileFSE { get; set; }
        public File_LIT_Group FileLIT { get; set; }
        public FileQuadCustomGroup FileQuadCustom { get; set; }
        public File_EFFBLOB_Group FileEFF { get; set; }

        public ushort? SelectedRoomID { get; set; }
    }
}
