﻿using CloudStreamForms.Core;
using Esprima.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace CloudStreamForms.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public struct SettingsHolder
        {
            public string header;
            public SettingsItem[] settings;
        }

        public class SettingsButton : SettingsItem
        {
            public SettingsButton(string _img, string title, string minTitle, Func<Task> _onChange)
            {
                img = _img;
                isSwitch = false;
                isButton = true;
                OnChange = _onChange;
                mainTxt = title;
                descriptTxt = minTitle;
            }
        }

        public class SettingsCondButton : SettingsItem
        {
            public SettingsCondButton(string _img, string title, string minTitle, Func<Task> _onClick, Func<string> _onResult, Func<bool> _canAppear)
            {
                img = _img;
                isSwitch = false;
                isCondButton = true;
                CanAppear = _canAppear;
                mainTxt = title;
                descriptTxt = minTitle;
                OnChange = _onClick;
                OnResult = _onResult;
            }
        }
        public class SettingsList : SettingsItem
        {
            public SettingsList(string _img, string title, string minTitle, Func<string> _onResult, Func<Task> _onChange)
            {
                img = _img;
                isSwitch = false;
                isList = true;
                OnChange = _onChange;
                mainTxt = title;
                descriptTxt = minTitle;
                OnResult = _onResult;
            }
        }

        public class SettingsItem
        {
            public string img;
            public string mainTxt;
            public string descriptTxt;
            public bool isButton = false;
            public bool isSwitch = true;
            public bool isList = false;
            public bool isCondButton = false;
            private string _varName;
            public EventHandler onAppear;
            public string VarName {
                set {
                    _varName = value;
                    variable = new VarRef<bool>(() => { return (bool)typeof(Settings).GetProperty(_varName).GetValue(null); }, (t) => { typeof(Settings).GetProperty(_varName).SetValue(null, t); });
                }
            }
            public VarRef<bool> variable;
            public Func<string> OnResult;
            public bool isFromAppear;
            public Func<Task> OnChange;
            public Func<Task> OnHumanInput;
            public Func<bool> CanAppear;
            public Button btt;
        }


        public class VarRef<T>
        {
            private Func<T> _get;
            private Action<T> _set;

            public VarRef(Func<T> @get, Action<T> @set)
            {
                _get = @get;
                _set = @set;
            }

            public T Value {
                get { return _get(); }
                set { _set(value); }
            }
        }

        public static readonly string[] SkipTimes = {
            "5s","10s","15s","20s","30s","40s","50s","60s"
        };

        public static readonly string[] SmallSkipTimes = {
            "1s","2s","3s","4s","5s","7.5s","10s","15s","20s"
        };

        public static readonly int[] SmallSkipTimesMs = {
            1000,2000,3000,4000,5000,7500,10000,15000,20000
        };

        public static SettingsHolder GeneralSettings = new SettingsHolder() {
            header = "General",
            settings = new SettingsItem[] {
                new SettingsItem() { img= "MainSearchIcon.png",mainTxt="Quick Search",descriptTxt="Search every character" ,VarName = nameof( Settings.SearchEveryCharEnabled) },
                new SettingsItem() { img= "outline_subtitles_white_48dp.png",mainTxt="Subtitles",descriptTxt="Auto download subtitles" ,VarName = nameof( Settings.SubtitlesEnabled) },
                new SettingsItem() { img= "outline_cached_white_48dp.png",mainTxt="Cache data",descriptTxt="Speed up loading time" ,VarName = nameof( Settings.CacheData) },
                new SettingsItem() { img= "outline_cached_white_48dp.png",mainTxt="Use AniList",descriptTxt="Prefer AniList over MAL for faster load" ,VarName = nameof( Settings.UseAniList) },
                new SettingsItem() { img= "outline_record_voice_over_white_48dp.png",mainTxt="Default dub",descriptTxt="Autoset to dub/sub when it can" ,VarName = nameof( Settings.DefaultDub) },
                new SettingsItem() { img= "outline_history_white_48dp.png",mainTxt="Pause history",descriptTxt="Will pause all viewing history" ,VarName = nameof( Settings.PauseHistory) },
                new SettingsItem() { img= "baseline_ondemand_video_white_48dp.png",mainTxt="Autoload next episode",descriptTxt="Autoload the next episode in the background while in the app videoplayer" ,VarName = nameof( Settings.LazyLoadNextLink) },
                new SettingsItem() { img= "baseline_fast_forward_white_48dp.png",mainTxt="Show skip",descriptTxt="Skip opening/credits in videoplayer" ,VarName = nameof(Settings.VideoPlayerShowSkip) },
               
                new SettingsItem() { img= "sponsorblock.png",mainTxt="YouTube sponsorblock",descriptTxt="Skip ads, intro, outro" ,VarName = nameof(Settings.VideoPlayerSponsorblock) },
                new SettingsItem() { img= "sponsorblock.png",mainTxt="Autoskip YouTube ads",descriptTxt="" ,VarName = nameof(Settings.VideoPlayerSponsorblockAutoSkipAds) },

                new SettingsItem() { img= "baseline_ondemand_video_white_48dp.png",mainTxt="Use in app videoplayer",descriptTxt="" ,VarName = nameof( Settings.UseVideoPlayer)},
                new SettingsList("baseline_ondemand_video_white_48dp.png","Current Videoplayer","External videoplayer",() => { return App.GetVideoPlayerName((App.VideoPlayer)Settings.PreferedVideoPlayer); }, async () => {
                    List<string> videoOptions = new List<string>() { App.GetVideoPlayerName(App.VideoPlayer.None) };
                    List<App.VideoPlayer> avalibePlayers = new List<App.VideoPlayer>() { App.VideoPlayer.None };
                    foreach (var player in App.GetEnumList<App.VideoPlayer>()) {
                        if (App.GetVideoPlayerInstalled(player)) {
                            avalibePlayers.Add(player);
                            videoOptions.Add(App.GetVideoPlayerName(player));
                        }
                    }

                    string action = await ActionPopup.DisplayActionSheet("External Videoplayer",avalibePlayers.IndexOf((App.VideoPlayer)Settings.PreferedVideoPlayer),videoOptions.ToArray());

                    for (int i = 0; i < videoOptions.Count; i++)
                    {
                        if(videoOptions[i] == action) {
                            Settings.PreferedVideoPlayer =(int)avalibePlayers[i];
                            break;
                        }
                    }
                }),
            },
        };

        public static SettingsHolder TimeSettings = new SettingsHolder() {
            header = "Time",
            settings = new SettingsItem[] {
                 new SettingsList("baseline_fast_forward_white_48dp.png","App Videoplayer fast forward","",() => { return $"{Settings.VideoPlayerSkipTime}s"; }, async () => {
                    string skip = await ActionPopup.DisplayActionSheet("Videoplayer skip",SkipTimes.IndexOf(Settings.VideoPlayerSkipTime + "s"),SkipTimes);
                    if(skip == "Cancel" || skip == "") return;

                    if(int.TryParse(skip.Replace("s",""),out int res)) {
                        Settings.VideoPlayerSkipTime = res;
                    }
                }),
                new SettingsList("baseline_fast_forward_white_48dp.png","Chromecast fast forward","",() => { return $"{Settings.ChromecastSkipTime}s"; }, async () => {
                    string skip = await ActionPopup.DisplayActionSheet("Chromecast skip",SkipTimes.IndexOf(Settings.ChromecastSkipTime + "s"),SkipTimes);
                    if(skip == "Cancel" || skip == "") return;

                    if(int.TryParse(skip.Replace("s",""),out int res)) {
                        Settings.ChromecastSkipTime = res;
                    }
                }),
                new SettingsList("baseline_access_time_white_48dp.png","Loading time","",() => { return $"{ (Settings.LoadingMiliSec/100)/10.0  }s"; }, async () => {
                    string skip = await ActionPopup.DisplayActionSheet("Set loading time",SmallSkipTimesMs.IndexOf(Settings.LoadingMiliSec),SmallSkipTimes);
                    if(skip == "Cancel" || skip == "") return;

                    if(decimal.TryParse(skip.Replace("s",""),out decimal res)) {
                        Settings.LoadingMiliSec = (int)(res*1000);
                    }
                }),
            },
        };


        public static SettingsHolder UISettings = new SettingsHolder() {
            header = "UI",
            settings = new SettingsItem[] {
                new SettingsList("outline_color_lens_white_48dp.png","Theme","Set app theme",() => {return Settings.BlackBgNames[Settings.BlackBgType]; },async () => {
                    string action = await ActionPopup.DisplayActionSheet("Select Theme",Settings.BlackBgType,Settings.BlackBgNames);
                    int index;
                    if((index = Settings.BlackBgNames.IndexOf(action)) != -1) {
                        Settings.BlackBgType = index;
                    }
                    Appear();
                    App.UpdateBackground();
                }),
                new SettingsItem() { img= "outline_aspect_ratio_white_48dp.png",mainTxt="Show statusbar",descriptTxt="This will not affect app videoplayer" ,VarName = nameof( Settings.HasStatusBar),OnChange = async () => { App.UpdateStatusBar();} },
                new SettingsItem() { img= "outline_reorder_white_48dp.png",mainTxt="Top 100",descriptTxt="" ,VarName = nameof( Settings.Top100Enabled)},
                new SettingsItem() { img= "outline_description_white_48dp.png",mainTxt="Episode description",descriptTxt="To remove spoilers or shorten episode list" ,VarName = nameof( Settings.EpDecEnabled)},
                new SettingsItem() { img= "animation.png",mainTxt="List animation",descriptTxt="To remove the popup animation for top 100" ,VarName = nameof( Settings.ListViewPopupAnimation)},
            },
        };

        public static SettingsHolder ProviderActive = new SettingsHolder() {
            header = "Providers",
            settings = new SettingsItem[] {
                new SettingsList("baseline_theaters_white_48dp.png","Movie Providers","", () => { return CloudStreamCore.mainCore.movieProviders.Select(t => t.Name).Where(t => Settings.IsProviderActive(t)).Count() + "/" + CloudStreamCore.mainCore.movieProviders.Length + " Active"; }, async () => {
                    List<string> names = CloudStreamCore.mainCore.movieProviders.Select(t => t.Name).ToList();
                    List<bool> res = await ActionPopup.DisplaySwitchList(names,names.Select(t => Settings.IsProviderActive(t)).ToList(),"Movie providers");
                    for (int i = 0; i < res.Count; i++)
                    {
                        App.SetKey("ProviderActive",names[i],res[i]);
                    }
                }),
                new SettingsList("baseline_theaters_white_48dp.png","Anime Providers","", () => { return CloudStreamCore.mainCore.animeProviders.Select(t => t.Name).Where(t => Settings.IsProviderActive(t)).Count() + "/" + CloudStreamCore.mainCore.animeProviders.Length + " Active"; },async () => {
                    List<string> names = CloudStreamCore.mainCore.animeProviders.Select(t => t.Name).ToList();
                    List<bool> res = await ActionPopup.DisplaySwitchList(names,names.Select(t => Settings.IsProviderActive(t)).ToList(),"Anime providers");
                    for (int i = 0; i < res.Count; i++)
                    {
                        App.SetKey("ProviderActive",names[i],res[i]);
                    }
                }),
            },

        };

        public static SettingsHolder ClearSettigns = new SettingsHolder() {
            header = "Clear data",
            settings = new SettingsItem[] {
                new SettingsButton("outline_delete_white_48dp.png","Clear history","Will clear all watch history",async () => {
                    int items = App.GetKeyCount(App.VIEW_HISTORY) + App.GetKeyCount(App.VIEW_TIME_POS) +App.GetKeyCount(App.VIEW_TIME_DUR);
                    string o = await ActionPopup.DisplayActionSheet($"Clear watch history ({items} items)","Yes, clear all watch history","No, dont clear watch history");
                    if(o.StartsWith("Yes")) {
                        App.RemoveFolder(App.VIEW_HISTORY);
                        App.RemoveFolder(App.VIEW_TIME_POS);
                        App.RemoveFolder(App.VIEW_TIME_DUR);
                    }
                }),
                new SettingsButton("outline_delete_white_48dp.png","Clear cached data","Will clear all cached data",async () => {
                    int items = App.GetKeyCount("CacheMAL") + App.GetKeyCount("CacheImdb");
                    string o = await ActionPopup.DisplayActionSheet($"Clear cached data ({items} items)", "Yes, clear all cached data","No, dont clear cached data");
                    if(o.StartsWith("Yes")) {
                        App.RemoveFolder(App.VIEW_HISTORY);
                        App.RemoveFolder(App.VIEW_TIME_POS);
                        App.RemoveFolder(App.VIEW_TIME_DUR);
                    }
                }),
                new SettingsButton("outline_delete_white_48dp.png","Clear bookmarks","Will remove all bookmarks",async () => {
                    int items = App.GetKeyCount(App.BOOKMARK_DATA);
                    string o = await ActionPopup.DisplayActionSheet($"Remove all bookmarks ({items} items)","Yes, remove all bookmarks","No, dont remove bookmarks");
                    if(o.StartsWith("Yes")) {
                        App.RemoveFolder(App.BOOKMARK_DATA);
                    }
                }),
                new SettingsButton("baseline_refresh_white_48dp.png","Reset settings","Will reset all settings to default",async () => {
                    string o = await ActionPopup.DisplayActionSheet($"Reset all settings","Yes, reset to default","No, dont reset settings");
                    if(o.StartsWith("Yes")) {
                        App.RemoveFolder("Settings");
                        Appear();
                    }
                }),
            },
        };

        public static SettingsHolder SubtitleSettings = new SettingsHolder() {
            header = "Subtitles",
            settings = new SettingsItem[] {
                new SettingsList("outline_subtitles_white_48dp.png","Subtitle Font","For in app videoplayer and chromecast",() => {return Settings.GlobalSubtitleFont; },async () => {
                    string action = await ActionPopup.DisplayActionSheet("Subtitle Font",Settings.GlobalFonts.IndexOf(Settings.GlobalSubtitleFont),Settings.GlobalFonts.ToArray());
                    if(action != "" && action != "Cancel") {
                        Settings.GlobalSubtitleFont = action;
                    }
                }),
                new SettingsList("outline_subtitles_white_48dp.png","Subtitle Outline","For in app videoplayer",() => {return Settings.SubtitleTypeNames[Settings.SubtitleType]; },async () => {
                    string action = await ActionPopup.DisplayActionSheet("Subtitle Outline",Settings.SubtitleType,Settings.SubtitleTypeNames);
                    int index;
                    if((index = Settings.SubtitleTypeNames.IndexOf(action)) != -1) {
                        Settings.SubtitleType = index;
                    }
                }),
                new SettingsList("outline_subtitles_white_48dp.png","Subtitle Language","Default subtitle language",() => {return  CloudStreamCore.subtitleNames[Settings.NativeSubtitles]; },async () => {
                    string action = await ActionPopup.DisplayActionSheet("Subtitle Language",Settings.NativeSubtitles,CloudStreamCore.subtitleNames);
                    int index;
                    if((index = CloudStreamCore.subtitleNames.IndexOf(action)) != -1) {
                        Settings.NativeSubtitles = index;
                    }
                }),
            },
        };

        public static SettingsHolder AdvancedSettings = new SettingsHolder() {
            header = "Advanced Settings",
            settings = new SettingsItem[] {
                 new SettingsList("baseline_report_problem_white_48dp.png","Tapjack protection (Requires restart)","Disable this if you cant press certain things", () => { return $"{(Settings.TapjackProtectionButton ? 1 : 0) + (Settings.TapjackProtectionPicker ? 1 : 0) + (Settings.TapjackProtectionSearch ? 1 : 0)}/3 Active"; },async () => {
                     List<bool> res = await ActionPopup.DisplaySwitchList(new List<string>() { "Search","Buttons","Picker" }, new List<bool>() { Settings.TapjackProtectionSearch,Settings.TapjackProtectionButton,Settings.TapjackProtectionPicker},"Tapjack protection");
                     Settings.TapjackProtectionSearch = res[0];
                     Settings.TapjackProtectionButton = res[1];
                     Settings.TapjackProtectionPicker = res[2];
                 }),
                 new SettingsItem() { img = "baseline_report_problem_white_48dp.png", mainTxt = "Ignore SSL Certificate", descriptTxt = "If you disable this, some sites cant be reached", VarName = nameof(Settings.IgnoreSSLCert) },
             },

        };

        public static SettingsHolder BuildSettings = new SettingsHolder() {
            header = "Build v" + App.GetBuildNumber(),
            settings = new SettingsItem[] {
                 new SettingsItem() {
                    mainTxt = "Show app updates",
                    img = "baseline_notifications_active_white_48dp.png",//"outline_build_white_48dp.png",
                    VarName = nameof(Settings.ShowAppUpdate),
                    OnHumanInput = async () => {
                        if(MainPage.NewGithubUpdate && Settings.ShowAppUpdate) {
                            await MainPage.ShowUpdate();
                        }
                    }
                },
                new SettingsCondButton( "outline_get_app_white_48dp.png","No update found","",async
                     () => { await MainPage.ShowUpdate(true); },
                     () => { return $"Update from v{App.GetBuildNumber()} to {MainPage.githubUpdateTag}"; },
                     () => { return MainPage.NewGithubUpdate; }),
                new SettingsButton("GitHub-Mark-Light-120px-plus.png","Open Github","https://github.com/LagradOst/CloudStream-2",async () => {
                    await App.OpenBrowser("https://github.com/LagradOst/CloudStream-2");
                }),
                new SettingsButton("Discord-Logo-White.png","Join Discord","https://discord.gg/5Hus6fM",async () => {
                    await App.OpenBrowser("https://discord.gg/5Hus6fM");
                }),

                new SettingsButton("baseline_feedback_white_48dp.png","Leave feedback","",async () => {
                    await thisPage.Navigation.PushModalAsync(new Feedback());
                }),
                new SettingsList("outline_settings_white_48dp.png","Manage Account","", () => { return (Settings.HasAccountLogin ? $"Logged in as {Settings.AccountUsername}" : ""); },async ()  => {
                   await Settings.ManageAccountClicked(() => Appear());
                }),
            },
        };

        public static List<SettingsHolder> settings = new List<SettingsHolder>() {
            GeneralSettings,
            UISettings,
            SubtitleSettings,
            ProviderActive,
            TimeSettings,
            AdvancedSettings,
            ClearSettigns,
            BuildSettings,
        };

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Appear();
        }

        static void Appear()
        {
            thisPage.BackgroundColor = Settings.BlackRBGColor;
            double _color = Math.Min(Settings.BlackColor + 3, 255) / 255.0;
            Color c = new Color(_color);

            foreach (var set in settings) {
                foreach (var subSet in set.settings) {
                    if (subSet.btt != null) {
                        subSet.onAppear?.Invoke(null, EventArgs.Empty);
                        subSet.btt.BackgroundColor = c;
                    }
                }
            }
        }

        static SettingsPage thisPage;

        public SettingsPage()
        {
            InitializeComponent();
            BindingContext = this;

            Device.InvokeOnMainThreadAsync(async () => {

                Settings.OnInit();
                Settings.OnInitAfter();

                thisPage = this;

                await Task.Delay(1000);
                int counter = 0;

                void AddChild(View v)
                {
                    AddTextGrid.Children.Add(v);
                    Grid.SetRow(v, counter);
                    counter++;
                }

                foreach (var set in settings) {
                    AddChild(new Label() { Text = set.header, FontSize = 17, FontAttributes = FontAttributes.Bold, TranslationX = 10, Margin = new Thickness(10, 20, 10, 10), TextColor = Color.White });

                    foreach (var subSet in set.settings) {

                        var mainLabel = new Label() { Text = subSet.mainTxt, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex("#e6e6e6") };
                        var sublabel = new Label() { Text = subSet.descriptTxt, VerticalOptions = LayoutOptions.Center, FontSize = 12, TextColor = Color.FromHex("#AAA") };
                        var _img = new FFImageLoading.Forms.CachedImage() {
                            HorizontalOptions = LayoutOptions.Start,
                            WidthRequest = 25,
                            HeightRequest = 25,
                            Margin = new Thickness(5),
                            TranslationX = 5,
                            VerticalOptions = LayoutOptions.Center,
                            Source = App.GetImageSource(subSet.img),
                        };

                        var subGrid = new Grid() {
                            HeightRequest = 50,
                            RowSpacing = -10,
                            Padding = new Thickness(0, 0, 50, 0),
                            VerticalOptions = LayoutOptions.Center,
                        };
                        subGrid.Children.Add(mainLabel);
                        if (subSet.descriptTxt.IsClean()) {
                            subGrid.Children.Add(sublabel);
                        }

                        var bgBtn = new Button() {
                            BackgroundColor = Color.FromHex("#141414")
                        };
                        subSet.btt = bgBtn;
                        var _grid = new Grid() {
                            HeightRequest = 70,
                        };

                        List<View> mainChilds = new List<View>() {
                       bgBtn,

                        new Grid() {
                            InputTransparent = true,
                            ColumnSpacing=10,

                            ColumnDefinitions = new ColumnDefinitionCollection() {
                                new ColumnDefinition() {
                                    Width = GridLength.Auto
                                },
                                new ColumnDefinition() {
                                    Width = GridLength.Star
                                },
                            },
                            Children = {
                               _img,
                               subGrid,
                            }
                        }
                    };
                        if (subSet.isSwitch) {
                            var _switch = new Switch() {
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.End,
                            };
                            mainChilds.Insert(1, _switch);

                            bgBtn.Clicked += (o, e) => {
                                subSet.isFromAppear = false;
                                _switch.IsToggled = !_switch.IsToggled;
                                subSet.variable.Value = _switch.IsToggled;
                            };
                            _switch.Toggled += (o, e) => {
                                subSet.variable.Value = e.Value;
                                subSet.OnChange?.Invoke();
                                if (!subSet.isFromAppear) {
                                    subSet.OnHumanInput?.Invoke();
                                }
                            };
                            subSet.onAppear += (o, e) => {
                                subSet.isFromAppear = true;
                                _switch.IsToggled = subSet.variable.Value;
                            };
                        };
                        if (subSet.isButton) {
                            bgBtn.Clicked += (o, e) => {
                                subSet.OnChange?.Invoke();
                            };
                        }
                        if (subSet.isList) {
                            var _tex = new Label() {
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.End,
                                Padding = new Thickness(10),
                                InputTransparent = true,
                            };
                            mainChilds.Insert(1, _tex);

                            bgBtn.Clicked += async (o, e) => {
                                await subSet.OnChange();
                                _tex.Text = subSet.OnResult();
                            };

                            subSet.onAppear += (o, e) => {
                                _tex.Text = subSet.OnResult();
                            };
                        }
                        if (subSet.isCondButton) {
                            string defTxt = subSet.mainTxt;
                            subSet.onAppear += (o, e) => {
                                bool enabled = subSet.CanAppear();
                                mainLabel.Text = enabled ? subSet.OnResult() : defTxt;
                                _grid.Opacity = enabled ? 1 : 0.5;
                            };
                            bgBtn.Clicked += async (o, e) => {
                                if (subSet.CanAppear()) {
                                    await subSet.OnChange();
                                    subSet.onAppear?.Invoke(null, EventArgs.Empty);
                                }
                            };
                        }

                        foreach (var _child in mainChilds) {
                            _grid.Children.Add(_child);
                        }
                        AddChild(_grid);
                        if (subSet.descriptTxt.IsClean()) {
                            Grid.SetRow(sublabel, 1);
                        }
                        Grid.SetColumn(subGrid, 1);
                    }
                }
                Appear();
            });

        }
    }
}