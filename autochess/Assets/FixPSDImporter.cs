public static class FixPSDImporter
    {
        [UnityEditor.InitializeOnLoadMethod]
        public static void ResetPSDImporterFoldout()
        {
            UnityEditor.EditorPrefs.DeleteKey("PSDImporterEditor.m_PlatformSettingsFoldout");
        }
    }