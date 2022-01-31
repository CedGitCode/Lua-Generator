using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using System;
using System.Reflection;

namespace lua_generator
{
    public partial class MainWindow : Window
    {

        bool wantMaterials, wantModels, wantResources, wantEntities, wantWeapons = false;
        
        string getOutputFolderPath;
        string getAddonName;

        bool hasGoodPath, hasGoodName = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        void luaGenerator_Activated(object sender, EventArgs e)
        {
            this.Topmost = true;
        }

        void luaGenerator_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = false;
        }
 
        private void Border_TopMenu_MouseDown(object sender, MouseEventArgs e)
        { 
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
            
        }

        private void closeMainFrame(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void minimizeMainFrame(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized; 
        }

        private void validateDirectory()
        {
            string outputFolderPath = TextBox_OutputFolderPath.Text;
           
            if (outputFolderPath.Length != 0)
            {
                if (outputFolderPath.Substring(outputFolderPath.Length - 1) != "/")
                    outputFolderPath = outputFolderPath + "/";
            }

            if (string.IsNullOrEmpty(outputFolderPath) || !Directory.Exists(outputFolderPath))
            {
                Label_AdvertOutputFolder.Content = "Chemin incorrecte !";
                Label_AdvertOutputFolder.Foreground = System.Windows.Media.Brushes.Red;
                hasGoodPath = false;
            }
            else
            {
                Label_AdvertOutputFolder.Content = "Chemin validé !";
                Label_AdvertOutputFolder.Foreground = System.Windows.Media.Brushes.Green;
                Keyboard.ClearFocus();

                getOutputFolderPath = outputFolderPath;
                hasGoodPath = true;
            }

            Label_ValidateAddon.Content = "";
        }
        private void TextBox_OutputFolder_KeyDown(object sender, KeyEventArgs e)
        { 
            
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                validateDirectory();
            }
        }

        private void TextBox_OutputFolder_LostFocus(object sender, RoutedEventArgs e)
        {
            validateDirectory();
        }

        private void TextBox_OutputFolder_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace((string)Label_AdvertOutputFolder.Content))
            {
                Label_AdvertOutputFolder.Content = "";
                hasGoodPath = false;
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {

            string getToggleName = sender.ToString();

            if (getToggleName.Contains("materials"))
                wantMaterials = !wantMaterials;
            else if (getToggleName.Contains("models"))
                wantModels = !wantModels;
            else if (getToggleName.Contains("resources"))
                wantResources = !wantResources;
            else if (getToggleName.Contains("entities"))
                wantEntities = !wantEntities;
            else if (getToggleName.Contains("weapons"))
                wantWeapons = !wantWeapons;
        }

        private void validateAddonName()
        {
            string addonName = TextBox_AddonName.Text;
            char[] notAllowedCharTable = { '&', 'é', '~', '{', '}', '+', '-', '*', '/', '@', '^', '!', '?', '§' };
            int notAllowedChar = addonName.IndexOfAny(notAllowedCharTable);

            if (string.IsNullOrWhiteSpace(addonName) || notAllowedChar > -1)
            {
                Label_AdvertAddonName.Content = "Nom d'addon incorrect !";
                Label_AdvertAddonName.Foreground = System.Windows.Media.Brushes.Red;
                hasGoodName = false;
            }
            else
            {
                Label_AdvertAddonName.Content = "Nom d'addon validé !";
                Label_AdvertAddonName.Foreground = System.Windows.Media.Brushes.Green;
                Keyboard.ClearFocus();

                getAddonName = addonName;
                hasGoodName = true;
            }

            Label_ValidateAddon.Content = "";
        }
        private void TextBox_AddonName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                validateAddonName();
            }
        }

        private void TextBox_AddonName_LostFocus(object sender, RoutedEventArgs e)
        {
            validateAddonName();
        }

        private void TextBox_AddonName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace((string)Label_AdvertAddonName.Content))
            {
                Label_AdvertAddonName.Content = "";
                hasGoodName = false;
            }
        }

        private void GenerateWeaponsFolder(string rootPath)
        {
            // rootPath = nomAddon/weapons
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            string weaponFolder = rootPath + getAddonName + "_weapons/";

            if (!Directory.Exists(weaponFolder))
                Directory.CreateDirectory(weaponFolder);

            if (!File.Exists(weaponFolder + "shared.lua"))
            {
                File.WriteAllText(weaponFolder + "shared.lua", Properties.Resources.weapon_source);
            }
        }

        private void GenerateModelsFolder(string rootPath)
        {
            if (!Directory.Exists(rootPath + "/models"))
                Directory.CreateDirectory(rootPath + "/models");
        }

        private void GenerateMaterialsFolder(string rootPath)
        {
            if (!Directory.Exists(rootPath + "/materials"))
                Directory.CreateDirectory(rootPath + "/materials");
        }

        private void GenerateResourceFolder(string rootPath)
        {
            if (!Directory.Exists(rootPath + "/resource"))
                Directory.CreateDirectory(rootPath + "/resources");

            if (!Directory.Exists(rootPath + "/resource/fonts"))
                Directory.CreateDirectory(rootPath + "/resource/fonts");
        }

        private void GenerateEntitiesFolder(string rootPath)
        {
            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);

            if (!Directory.Exists(rootPath + getAddonName + "_entity/"))
                Directory.CreateDirectory(rootPath + getAddonName + "_entity/");
           
            if (!File.Exists(rootPath + getAddonName + "_entity/" + "cl_init.lua"))
            {
                File.WriteAllText(rootPath + getAddonName + "_entity/" + "cl_init.lua", Properties.Resources.cl_init);
            }

            if (!File.Exists(rootPath + getAddonName + "_entity/" + "init.lua"))
            {
                File.WriteAllText(rootPath + getAddonName + "_entity/" + "init.lua", Properties.Resources.init);
            }

            if (!File.Exists(rootPath + getAddonName + "_entity/" + "shared.lua"))
            {
                File.WriteAllText(rootPath + getAddonName + "_entity/" + "shared.lua", Properties.Resources.shared);
            }
        }

        private void GenerateLoaderFolder(string rootPath)
        {

            string filesDirectory = getAddonName + "_files/";
            string clientFolder = getAddonName + "_client/";
            string sharedFolder = getAddonName + "_shared/";
            string serverFolder = getAddonName + "_server/";

            if (!Directory.Exists(rootPath))
                Directory.CreateDirectory(rootPath);


            if (!File.Exists(rootPath + "/loader_" + getAddonName + ".lua"))
                using (StreamWriter loaderFile = File.CreateText(rootPath + "/loader_" + getAddonName + ".lua"))
                {
                    loaderFile.WriteLine("if SERVER then");

                    loaderFile.WriteLine("\tinclude(\"" + filesDirectory + sharedFolder + "sh_init.lua\")");
                    loaderFile.WriteLine("\tAddCSLuaFile(\"" + filesDirectory + sharedFolder + "sh_init.lua\")\n");

                    loaderFile.WriteLine("\tAddCSLuaFile(\"" + filesDirectory + clientFolder + "cl_init.lua\")\n");

                    loaderFile.WriteLine("\tinclude(\"" + filesDirectory + serverFolder + "sv_init.lua\")");
                    loaderFile.WriteLine("end\n");

                    loaderFile.WriteLine("if CLIENT then");
                    loaderFile.WriteLine("\tinclude(\"" + filesDirectory + sharedFolder + "sh_init.lua\")\n");
                    loaderFile.WriteLine("\tinclude(\"" + filesDirectory + clientFolder + "cl_init.lua\")");
                    loaderFile.WriteLine("end");
                }
        }

        private void GenerateSharedFile(string p_rootPath)
        {
            Directory.CreateDirectory(p_rootPath);

            if (!File.Exists(p_rootPath + "sh_init.lua"))
                using (StreamWriter sharedFile = File.CreateText(p_rootPath + "sh_init.lua"))
                {
                    sharedFile.WriteLine("// Generate");
                }
        }

        private void GenerateServerFile(string p_rootPath)
        {
            Directory.CreateDirectory(p_rootPath);

            if (!File.Exists(p_rootPath + "sv_init.lua"))
                using (StreamWriter serverFile = File.CreateText(p_rootPath + "sv_init.lua"))
                {
                    serverFile.WriteLine("// Generate");
                }
        }

        private void GenerateClientFile(string p_rootPath)
        {
            Directory.CreateDirectory(p_rootPath);

            if (!File.Exists(p_rootPath + "cl_init.lua"))
                using (StreamWriter clientFile = File.CreateText(p_rootPath + "cl_init.lua"))
                {
                    clientFile.WriteLine("// Generate");
                }
        }

        private void GenerateAddonFiles(string p_rootPath)
        {

            if (!Directory.Exists(p_rootPath + getAddonName + "_files"))
                Directory.CreateDirectory(p_rootPath + getAddonName + "_files");

            if (!Directory.Exists(p_rootPath + getAddonName + "_files/" + getAddonName + "_client/"))
                GenerateClientFile(p_rootPath + getAddonName + "_files/" + getAddonName + "_client/");

            if (!Directory.Exists(p_rootPath + getAddonName + "_files/" + getAddonName + "_server/"))
                GenerateServerFile(p_rootPath + getAddonName + "_files/" + getAddonName + "_server/");

            if (!Directory.Exists(p_rootPath + getAddonName + "_files/" + getAddonName + "_shared/"))
                GenerateSharedFile(p_rootPath + getAddonName + "_files/" + getAddonName + "_shared/");
        }

        private void Button_GenerateAddon_Click(object sender, RoutedEventArgs e)
        {
            string rootPath;

            if (hasGoodName && hasGoodPath)
            {
                rootPath = getOutputFolderPath + getAddonName;

                if (!Directory.Exists(rootPath))
                    Directory.CreateDirectory(rootPath);

                GenerateLoaderFolder(rootPath + "/lua/autorun");

                GenerateAddonFiles(rootPath + "/lua/");

                if (wantMaterials)
                    GenerateMaterialsFolder(rootPath);
                if (wantEntities)
                    GenerateEntitiesFolder(rootPath + "/lua/entities/");
                if (wantModels)
                    GenerateModelsFolder(rootPath);
                if (wantResources)
                    GenerateResourceFolder(rootPath);
                if (wantWeapons)
                    GenerateWeaponsFolder(rootPath + "/lua/weapons/");

                Label_ValidateAddon.Content = "Addon crée avec succès !";
                Label_ValidateAddon.Foreground = System.Windows.Media.Brushes.Green;
            } else
            {
                Label_ValidateAddon.Content = "Impossible de créer l'addon";
                Label_ValidateAddon.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
    }
}
