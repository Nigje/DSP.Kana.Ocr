
using DSP.Khana.Ocr;
using DSP.Khana.Ocr.Engine.v5;
using DSP.Khana.Ocr.PostProcessing;
#if !DESKTOP_WITHOUT_LICENSING
using LicenseModel;
#endif
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bina.Ocr.Wapper
{
    public class BinaOcr
    {
        //*****************************************************************************************************
        //Variables:
        static string AppName = "BinaOcr";
        string Version = "";
        string temporalRootName = "";
        string temporalModelName = "";
        string temporalAppName = "";
        string TemporalDirectoryPath = "";
        string DllDirectoryPath = "";
        string ModelDirectoryPath = "";
        string CurrentDirectory = "";
        string ProjectName = "";
        private static BinaOcr _binaOcr;
        //*****************************************************************************************************
        static BinaOcr()
        {
            _binaOcr = new BinaOcr();
            
        }
        //*****************************************************************************************************
        private BinaOcr()
        {
            InitialVariables();
            InitialDependency();
            
        }
        //*****************************************************************************************************
        public static BinaOcr Instance()
        {
            return _binaOcr;
        }
        //*****************************************************************************************************
#if !DESKTOP_WITHOUT_LICENSING
        private void ChackLicense()
        {
            LicenseManager.CheckLicenseGeneralData(AppName);
        }
#endif
        //*****************************************************************************************************
#if !DESKTOP_WITHOUT_LICENSING
        public static License GetLicense()
        {
            var licenseModel=LicenseManager.GetLicense();
            License license = new License { 
                AppName= licenseModel.AppName,
                ExpireDateTime=licenseModel.ExpireDateTime,
                InstanceCount=licenseModel.InstanceCount,
                Language=licenseModel.Language,
                LicenseType=licenseModel.Type.ToString(),
                UID=licenseModel.UID
            };
            return license;
        }
#endif
        //*****************************************************************************************************
        ~BinaOcr()
        {
            try
            {
                Dispose();
            }
            catch (Exception ex)
            {

            }
        }
        //*****************************************************************************************************
        private void InitialVariables()
        {
            ProjectName = Assembly.GetExecutingAssembly().GetName().Name;
            CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            temporalRootName = ".temporalapp";
            temporalModelName = ".model_" + Version;
            temporalAppName = ".app_" + Version;
            TemporalDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + temporalRootName;
            DllDirectoryPath = TemporalDirectoryPath + "\\" + temporalAppName;
            ModelDirectoryPath = TemporalDirectoryPath + "\\" + temporalModelName;
        }
        //*****************************************************************************************************
        private void InitialDependency()
        {
            CreateTempDirectory(TemporalDirectoryPath);
            CreateTempDirectory(DllDirectoryPath);
            CreateTempDirectory(ModelDirectoryPath,true);

            byte[] binaOcrEngineV5Bytes = null;
            byte[] libleptBytes = null;
            if (Is64bit())
            {
                LoadFileFromDll("BinaOcrEngineV5_64.dll", "BinaOcrEngineV5.dll", DllDirectoryPath);
                LoadFileFromDll("liblept1780_64.dll", "liblept1780.dll", DllDirectoryPath);
                LoadFileFromDll("DSP.Tools64.dll", "DSP.Tools64.dll", CurrentDirectory);
            }
            else
            {
                LoadFileFromDll("BinaOcrEngineV5_86.dll", "BinaOcrEngineV5.dll", DllDirectoryPath);
                LoadFileFromDll("liblept1780_86.dll", "liblept1780.dll", DllDirectoryPath);
                LoadFileFromDll("DSP.Tools32.dll", "DSP.Tools32.dll", CurrentDirectory);
            }
            CreateModelFiles("Data.zip", ModelDirectoryPath);
        }
        //*****************************************************************************************************
        private void CreateModelFiles(string fileName, string modelDirectory)
        {
            CreateTempDirectory(modelDirectory,true);
            string newFileName = fileName;
            if (!File.Exists(modelDirectory + "\\" + newFileName))
                LoadFileFromDll(fileName, newFileName, modelDirectory);
            UnZipFile(modelDirectory + "\\" + newFileName, modelDirectory);
            DeleteTempFile(modelDirectory + "\\" + newFileName);
        }
        //*****************************************************************************************************
        private void UnZipFile(string file,string modelDirectory)
        {
            int counter = 200;
            int delay = 100;
            while (true && counter > 0)
            {
                try
                {
                    ZipFile.ExtractToDirectory(file, modelDirectory);
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("because it is being used by another process"))
                    {
                        Task.Delay(delay);
                        counter--;
                    }
                    else
                        throw ex;
                }
            }
        }
        //*****************************************************************************************************
        private void DeleteTempFile(string fileName)
        {
            int counter = 200;
            int delay = 100;
            while (true && counter > 0)
            {
                try
                {
                    if (File.Exists(fileName))
                        File.Delete(fileName);
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("it is being used by another process"))
                    {
                        Task.Delay(delay);
                        counter--;
                    }
                    else
                        throw ex;
                }
            }
        }
        //*****************************************************************************************************
        private void RetryJob(int counter = 20, int delay = 100)
        {
            while (true && counter > 0)
            {
                try
                {

                    break;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains(""))
                    {
                        Task.Delay(delay);
                        counter--;
                    }
                    else
                        throw ex;
                }
            }
        }
        //*****************************************************************************************************
        private bool Is64bit()
        {
            return IntPtr.Size != sizeof(int);
        }

        //*****************************************************************************************************
        private void CreateTempDirectory(string temporalDirectoryPath, bool forceDelete = false)
        {
            if (forceDelete)
                DeleteTempDirectory(temporalDirectoryPath);
            if (!Directory.Exists(temporalDirectoryPath))
            {
                Directory.CreateDirectory(temporalDirectoryPath);
            }

        }
        //*****************************************************************************************************
        private void DeleteTempDirectory(string directoryPath)
        {
            int count = 1;
            while (count <= 5)
            {
                try
                {
                    if (Directory.Exists(directoryPath))
                        Directory.Delete(directoryPath, true);
                    break;
                }
                catch (Exception ex)
                {
                    if (count == 5)
                        throw ex;
                    count++;
                }
            }
        }

        //*****************************************************************************************************
        private void CheckAndCreateModel()
        {
            if (Directory.Exists(ModelDirectoryPath))
                return;
            CreateModelFiles("Data.zip", ModelDirectoryPath);
        }
        //*****************************************************************************************************
        private string GetStringCore(Bitmap image, PageSegMode pageSegMode, string language, EngineMode engineMode,bool postProcessing=false)
        {
            KhanaPostProcessing khanaPostProcessing = new KhanaPostProcessing();
#if DESKTOP_WITHOUT_LICENSING
            LanguageModel languageModelKhana = null;
#else
            ValidateLicense(language);
            BinaLicense binaLicense = LicenseManager.GetLicense();
            LanguageModel languageModelKhana = MapLicense(binaLicense);
#endif
            CheckAndCreateModel();
            KhanaOcrEngine khanaEngine = new KhanaOcrEngine(languageModelKhana, ModelDirectoryPath, language, engineMode, DllDirectoryPath);
            var page = khanaEngine.Process(image, pageSegMode);
            string result = page.GetText();
            khanaEngine.Dispose();
            result = khanaPostProcessing.FilterExtraEnter(result);
            if(postProcessing && language.ToLower()=="farsi")
                result = khanaPostProcessing.GetString(result);
            return result;
        }
        //*****************************************************************************************************
#if !DESKTOP_WITHOUT_LICENSING
        private LanguageModel MapLicense(BinaLicense binaLicense)
        {
            LanguageModel languageModelKhana = new LanguageModel();
            languageModelKhana.Language = binaLicense.Language;
            languageModelKhana.UID = binaLicense.UID;
            languageModelKhana.IsValid = binaLicense.IsValid;
            languageModelKhana.AppKey = FingerPrint.FingerPrint.Value();
            languageModelKhana.AppName = AppName;
            languageModelKhana.LicenseAppName = binaLicense.AppName;
            languageModelKhana.ExpireDateTime = binaLicense.ExpireDateTime;
            return languageModelKhana;
        }
#endif

        //*****************************************************************************************************
#if !DESKTOP_WITHOUT_LICENSING
        private void ValidateLicense(string language)
        {
            ChackLicense();
            if (!LicenseManager.SelectedLanguageIsValid(language))
            {
                throw new ArgumentException("The input language is not supported by this license.");
            }
            LicenseManager.SelectedLanguageIsValid(language);
        }
#endif

        //*****************************************************************************************************
        public string GetString(Bitmap image, PageSegmentationModeEnum pageSegmentationMode, LanguageEnum language, EngineModeEnum engineMode, bool postProcessing=false)
        {

            return GetStringCore(image, MapPageSegMode(pageSegmentationMode), MapLanguage(language), MapEngineMode(engineMode), postProcessing);
        }
        //*****************************************************************************************************
        public async Task<string> GetStringAsync(Bitmap image, PageSegmentationModeEnum pageSegmentationMode, LanguageEnum language, EngineModeEnum engineMode,bool postProcessing=false)
        {
            return await Task.Run<string>(() =>
            {
                return GetString(image, pageSegmentationMode, language, engineMode,postProcessing);
            });
        }
        //*****************************************************************************************************

        public string GetParts(Bitmap image, PageSegmentationModeEnum pageSegmentationMode, LanguageEnum language, EngineModeEnum engineMode)
        {

            return GetPartsCore(image, MapPageSegMode(pageSegmentationMode), MapLanguage(language), MapEngineMode(engineMode));
        }
        //*****************************************************************************************************
        private string GetPartsCore(Bitmap image, PageSegMode pageSegMode, string language, EngineMode engineMode)
        {
#if DESKTOP_WITHOUT_LICENSING
            LanguageModel languageModelKhana = null;
#else
            ValidateLicense(language);
            BinaLicense binaLicense = LicenseManager.GetLicense();
            LanguageModel languageModelKhana = MapLicense(binaLicense);
#endif
            CheckAndCreateModel();
            KhanaOcrEngine khanaEngine = new KhanaOcrEngine(languageModelKhana, ModelDirectoryPath, language, engineMode, DllDirectoryPath);
            var page = khanaEngine.Process(image, pageSegMode);
            var iter = page.GetIterator();
            iter.Begin();
            string result = "";
            iter.Next(PageIteratorLevel.TextLine);
            do
            {
                var confidende = iter.GetConfidence(PageIteratorLevel.TextLine);
                var t = iter.GetText(PageIteratorLevel.TextLine);
                result += confidende.ToString() + "\t\t\t" + t + "\r\n";
            }
            while (iter.Next(PageIteratorLevel.TextLine));

            khanaEngine.Dispose();
            return result;
        }
        //*****************************************************************************************************
        #region MapModels
        private PageSegMode MapPageSegMode(PageSegmentationModeEnum pageSegmentationMode)
        {
            return (PageSegMode)Enum.Parse(typeof(PageSegMode), pageSegmentationMode.ToString());

        }
        //*****************************************************************************************************
        private EngineMode MapEngineMode(EngineModeEnum engineModeEnum)
        {
            switch (engineModeEnum)
            {
                case EngineModeEnum.KhanaStructuralOnly:
                    return EngineMode.KhanaOcrEngineOnly;
                case EngineModeEnum.KhanaDeepOnly:
                    return EngineMode.LstmOnly;
                case EngineModeEnum.KhanaStructuralAndKhanaDeep:
                    return EngineMode.KhanaOcrEngineAndLstm;
                default:
                    return EngineMode.Default;
            }
        }

        //*****************************************************************************************************
        private string MapLanguage(LanguageEnum language)
        {
            return language.ToString();
        }

        #endregion
        //*****************************************************************************************************
        private void LoadFileFromDll(string sourceName, string destinationName, string path)
        {
            string filePath = path + "\\" + destinationName;
            if (File.Exists(filePath))
                return;

            using (Stream stm = Assembly.GetExecutingAssembly().GetManifestResourceStream(
                ProjectName + ".Properties." + sourceName))
            {
                // Copy the assembly to the temporary file
                try
                {
                    using (Stream outFile = File.Create(path + "\\" + destinationName))
                    {
                        const int sz = 4096;
                        byte[] buf = new byte[sz];
                        while (true)
                        {
                            int nRead = stm.Read(buf, 0, sz);
                            if (nRead < 1)
                                break;
                            outFile.Write(buf, 0, nRead);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                    // This may happen if another process has already created and loaded the file.
                    // Since the directory includes the version number of this assembly we can
                    // assume that it's the same bits, so we just ignore the excecption here and
                    // load the DLL.
                }
            }
        }
        //*****************************************************************************************************
        public void Dispose()
        {
            DeleteTempDirectory(ModelDirectoryPath);
        }
        //*****************************************************************************************************
#if !DESKTOP_WITHOUT_LICENSING
        public static string GetFingerPrint()
        {
            return FingerPrint.FingerPrint.Value();
        }
#endif
        //*****************************************************************************************************
        public static string GetApplicationName()
        {
            return AppName;
        }
        //****************************************************************************************************
#if !DESKTOP_WITHOUT_LICENSING
        public static bool RegisterApplication(string licenseText)
        {
            return LicenseManager.Register(licenseText);
        }
#endif
        //****************************************************************************************************


    }
}
