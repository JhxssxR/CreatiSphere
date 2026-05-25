using Microsoft.Maui.Controls;
using CreatiSphere.Services;
using System.Threading.Tasks;
using Shapes = Microsoft.Maui.Controls.Shapes;

namespace CreatiSphere.Views.Customer
{
    public partial class CustomOrdersPage : ContentPage
    {
        private readonly DatabaseService _databaseService;
        private const int CurrentUserId = 1002;

        // ── Step 2 State ──────────────────────────────────────────────────
        private string _selectedStyle   = "Minimalist";
        private string _selectedPalette = "Corporate Teal";
        private decimal _totalPrice = 2500.00m;

        // Style → (bgImage, subtitle, chip1, chip2, chip3)
        private static readonly Dictionary<string, (string Img, string Sub, string C1, string C2, string C3)> StyleData = new()
        {
            ["Minimalist"]  = ("redesign_bg.png", "Clean lines · Industrial reliability",  "Minimalism",  "Industrial Modern", "Brutalist Influence"),
            ["Watercolor"]  = ("login_bg.png",    "Expressive fluidity · Artisanal touch",  "Watercolor",  "Organic Flow",      "Soft Textures"),
            ["Vector Art"]  = ("artist1.png",     "Modern precision · Scalable design",     "Vector Art",  "Geometric Bold",    "Digital Native"),
            ["Conceptual"]  = ("artist2.png",     "Experimental · Abstract narratives",     "Conceptual",  "Surrealist",        "Mixed Media"),
        };

        // Palette → (name, pct, progress, color1..5, progressHex)
        private static readonly Dictionary<string, (string Name, string Pct, double Prog, string C1, string C2, string C3, string C4, string C5, string ProgHex)> PaletteData = new()
        {
            ["Corporate Teal"] = ("Teal Emphasis",  "40%", 0.40, "#065F46", "#CBD5E1", "#1E293B", "#475569", "#94A3B8", "#065F46"),
            ["Modern Slate"]   = ("Slate Dominance","55%", 0.55, "#1E293B", "#475569", "#94A3B8", "#CBD5E1", "#F1F5F9", "#1E293B"),
            ["Organic Earth"]  = ("Earth Warmth",   "50%", 0.50, "#92400E", "#D97706", "#4D7C0F", "#78350F", "#F59E0B", "#92400E"),
        };

        public CustomOrdersPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUserInfo();
        }

        private async Task LoadUserInfo()
        {
            try
            {
                ProfileNameLabel.Text = "Alex Morgan";
                ProfileInitialsLabel.Text = "AM";
                ProfileRoleLabel.Text = "Standard Partner";
                await Task.CompletedTask;
            }
            catch { }
        }

        // ── Sidebar Navigation ────────────────────────────────────────────
        private async void OnDashboardTapped(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//CustomerDashboard");

        private async void OnExploreTapped(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//BrowseMarketplacePage");

        private async void OnTrackOrdersTapped(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//TrackOrdersPage");

        private async void OnMessagesTapped(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//MessagesPage");

        private async void OnFeedbackTapped(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//FeedbackPage");

        private async void OnRewardsTapped(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//RewardsPage");

        private async void OnSignOutTapped(object? sender, EventArgs e)
            => await Shell.Current.GoToAsync("//MainPage");

        // ── Step 1 → Step 2 ──────────────────────────────────────────────
        private void OnContinueToAestheticsClicked(object? sender, EventArgs e)
        {
            Step1Content.IsVisible = false;
            Step2Content.IsVisible = true;
            Step3Content.IsVisible = false;
            Step4Content.IsVisible = false;
            NavBreadcrumb1.Text = "Order Commissioning";
            NavBreadcrumb2.Text = "Step 2: Aesthetic Direction";
            NavBreadcrumbPill.BackgroundColor = Color.FromArgb("#F1F5F9");
        }

        // ── Step 2: Visual Style Selection ───────────────────────────────
        private void SelectStyle(string name, Border selected, Border[] others, VisualElement checkOn, VisualElement[] checksOff)
        {
            _selectedStyle = name;
            selected.Stroke = Color.FromArgb("#0D9488");
            selected.StrokeThickness = 2;
            checkOn.IsVisible = true;
            foreach (var b in others) { b.Stroke = Color.FromArgb("#E2E8F0"); b.StrokeThickness = 1; }
            foreach (var c in checksOff) c.IsVisible = false;
        }

        private void OnStyleMinimalistTapped(object? sender, TappedEventArgs e)
            => SelectStyle("Minimalist", StyleMinimalist, new[] { StyleWatercolor, StyleVectorArt, StyleConceptual },
                           CheckMinimalist, new VisualElement[] { CheckWatercolor, CheckVectorArt, CheckConceptual });

        private void OnStyleWatercolorTapped(object? sender, TappedEventArgs e)
            => SelectStyle("Watercolor", StyleWatercolor, new[] { StyleMinimalist, StyleVectorArt, StyleConceptual },
                           CheckWatercolor, new VisualElement[] { CheckMinimalist, CheckVectorArt, CheckConceptual });

        private void OnStyleVectorArtTapped(object? sender, TappedEventArgs e)
            => SelectStyle("Vector Art", StyleVectorArt, new[] { StyleMinimalist, StyleWatercolor, StyleConceptual },
                           CheckVectorArt, new VisualElement[] { CheckMinimalist, CheckWatercolor, CheckConceptual });

        private void OnStyleConceptualTapped(object? sender, TappedEventArgs e)
            => SelectStyle("Conceptual", StyleConceptual, new[] { StyleMinimalist, StyleWatercolor, StyleVectorArt },
                           CheckConceptual, new VisualElement[] { CheckMinimalist, CheckWatercolor, CheckVectorArt });

        // ── Step 2: Color Palette Selection ──────────────────────────────
        private void SelectPalette(string name, Border selected, Border[] others, VisualElement checkOn, VisualElement[] checksOff)
        {
            _selectedPalette = name;
            selected.Stroke = Color.FromArgb("#0D9488");
            selected.StrokeThickness = 2;
            checkOn.IsVisible = true;
            foreach (var b in others) { b.Stroke = Color.FromArgb("#E2E8F0"); b.StrokeThickness = 1; }
            foreach (var c in checksOff) c.IsVisible = false;
        }

        private void OnPaletteTealTapped(object? sender, TappedEventArgs e)
            => SelectPalette("Corporate Teal", PaletteTeal, new[] { PaletteSlate, PaletteEarth },
                             CheckPaletteTeal, new VisualElement[] { CheckPaletteSlate, CheckPaletteEarth });

        private void OnPaletteSlateTapped(object? sender, TappedEventArgs e)
            => SelectPalette("Modern Slate", PaletteSlate, new[] { PaletteTeal, PaletteEarth },
                             CheckPaletteSlate, new VisualElement[] { CheckPaletteTeal, CheckPaletteEarth });

        private void OnPaletteEarthTapped(object? sender, TappedEventArgs e)
            => SelectPalette("Organic Earth", PaletteEarth, new[] { PaletteTeal, PaletteSlate },
                             CheckPaletteEarth, new VisualElement[] { CheckPaletteTeal, CheckPaletteSlate });

        // ── Step 2 → Step 1 ──────────────────────────────────────────────
        private void OnBackToBriefClicked(object? sender, EventArgs e)
        {
            Step2Content.IsVisible = false;
            Step1Content.IsVisible = true;
            Step3Content.IsVisible = false;
            Step4Content.IsVisible = false;
            NavBreadcrumb1.Text = "Custom Orders";
            NavBreadcrumb2.Text = "New Commission";
            NavBreadcrumbPill.BackgroundColor = Colors.Transparent;
        }

        // ── Step 2 → Step 3 ──────────────────────────────────────────────
        private void OnContinueToReviewClicked(object? sender, EventArgs e)
        {
            Step1Content.IsVisible = false;
            Step2Content.IsVisible = false;
            Step3Content.IsVisible = true;
            Step4Content.IsVisible = false;
            
            NavBreadcrumb1.Text = "New Commission";
            NavBreadcrumb2.Text = "Step 3: Review Details";
            
            PopulateReviewAesthetics();
            PopulateReviewBrief();
        }

        private void PopulateReviewBrief()
        {
            // -- Fetch from Step 1 Inputs --
            ReviewBriefTitle.Text = string.IsNullOrWhiteSpace(BriefTitleEntry.Text) 
                ? "Untitled Commission" 
                : BriefTitleEntry.Text;

            ReviewBriefIntent.Text = string.IsNullOrWhiteSpace(BriefIntentEditor.Text)
                ? "No specific intent provided."
                : BriefIntentEditor.Text;

            string width = string.IsNullOrWhiteSpace(BriefWidthEntry.Text) ? "0" : BriefWidthEntry.Text;
            string height = string.IsNullOrWhiteSpace(BriefHeightEntry.Text) ? "0" : BriefHeightEntry.Text;
            ReviewBriefDimensions.Text = $"{width}cm x {height}cm";
            
            // -- Budget Range --
            string min = string.IsNullOrWhiteSpace(BudgetMinEntry.Text) ? "0" : BudgetMinEntry.Text;
            string max = string.IsNullOrWhiteSpace(BudgetMaxEntry.Text) ? "0" : BudgetMaxEntry.Text;
            
            if (min == "0" && max == "0")
                ReviewBriefBudget.Text = "Not Specified";
            else if (max == "0")
                ReviewBriefBudget.Text = $"${min}+";
            else
                ReviewBriefBudget.Text = $"${min} - ${max}";
        }

        private void PopulateReviewAesthetics()
        {
            // -- Visual Style --
            if (StyleData.TryGetValue(_selectedStyle, out var style))
            {
                ReviewStyleBgImage.Source   = ImageSource.FromFile(style.Img);
                ReviewStyleNameLabel.Text   = _selectedStyle;
                ReviewStyleSubtitleLabel.Text = style.Sub;
                ReviewChip1Label.Text = style.C1;
                ReviewChip2Label.Text = style.C2;
                ReviewChip3Label.Text = style.C3;
                ReviewChip3Row.IsVisible  = !string.IsNullOrEmpty(style.C3);
            }

            // -- Color Palette --
            if (PaletteData.TryGetValue(_selectedPalette, out var pal))
            {
                ReviewPColor1.BackgroundColor = Color.FromArgb(pal.C1);
                ReviewPColor2.BackgroundColor = Color.FromArgb(pal.C2);
                ReviewPColor3.BackgroundColor = Color.FromArgb(pal.C3);
                ReviewPColor4.BackgroundColor = Color.FromArgb(pal.C4);
                ReviewPColor5.BackgroundColor = Color.FromArgb(pal.C5);
                ReviewPaletteNameLabel.Text    = pal.Name;
                ReviewPalettePercentLabel.Text = pal.Pct;
                ReviewPaletteProgress.Progress = pal.Prog;
                ReviewPaletteProgress.ProgressColor = Color.FromArgb(pal.ProgHex);
            }

            // -- Update Summary Images --
            if (Step1PreviewContainer.IsVisible && Step1ReferenceImagePreview.Source != null)
            {
                ReviewRefImage1.Source = Step1ReferenceImagePreview.Source;
            }
            else if (PreviewContainer.IsVisible && ReferenceImagePreview.Source != null)
            {
                ReviewRefImage1.Source = ReferenceImagePreview.Source;
            }
            else if (StyleData.TryGetValue(_selectedStyle, out var s))
            {
                ReviewRefImage1.Source = ImageSource.FromFile(s.Img);
                // We can use fallback images for the others if we don't have a full set per style
                ReviewRefImage2.Source = ImageSource.FromFile("industrial_patina.png");
                ReviewRefImage3.Source = ImageSource.FromFile("architectural_shadows.png");
            }
        }

        // ── Step 3 → Step 2 ──────────────────────────────────────────────
        private void OnBackToAestheticsClicked(object? sender, EventArgs e)
        {
            Step1Content.IsVisible = false;
            Step2Content.IsVisible = true;
            Step3Content.IsVisible = false;
            Step4Content.IsVisible = false;
        }

        // ── Step 3 → Step 4 ──────────────────────────────────────────────
        private async void OnFinalizeAndSecureClicked(object? sender, EventArgs e)
        {
            if (!AgreementCheckbox.IsChecked)
            {
                await this.DisplayAlertAsync("Agreement Required", "Please agree to the Terms of Service & Commission Agreement to proceed.", "OK");
                return;
            }

            PopulatePaymentSummary();

            Step1Content.IsVisible = false;
            Step2Content.IsVisible = false;
            Step3Content.IsVisible = false;
            Step4Content.IsVisible = true;

            NavBreadcrumb1.Text = "Checkout";
            NavBreadcrumb2.Text = "Step 4: Payment Details";
            NavBreadcrumbPill.BackgroundColor = Color.FromArgb("#F0FDFA");
        }

        // ── Step 4 → Step 3 ──────────────────────────────────────────────
        private void OnBackToReviewClicked(object? sender, EventArgs e)
        {
            Step1Content.IsVisible = false;
            Step2Content.IsVisible = false;
            Step3Content.IsVisible = true;
            Step4Content.IsVisible = false;

            NavBreadcrumb1.Text = "New Commission";
            NavBreadcrumb2.Text = "Step 3: Review Details";
        }

        private void PopulatePaymentSummary()
        {
            PaymentSummaryTitleLabel.Text = ReviewBriefTitle.Text;
            PaymentSummaryImage.Source = ReviewRefImage1.Source;

            // -- Dynamic Price Calculation --
            decimal commissionFee = 2250.00m;
            if (decimal.TryParse(BudgetMaxEntry.Text, out decimal max) && max > 0)
                commissionFee = max;
            else if (decimal.TryParse(BudgetMinEntry.Text, out decimal min) && min > 0)
                commissionFee = min;

            decimal serviceFee = commissionFee * 0.05m; // 5% platform fee
            decimal escrowFee = 25.00m;
            _totalPrice = commissionFee + serviceFee + escrowFee;
            decimal dueNow = _totalPrice * 0.5m;

            SummaryCommissionFeeLabel.Text = $"${commissionFee:N2}";
            SummaryServiceFeeLabel.Text = $"${serviceFee:N2}";
            SummaryEscrowFeeLabel.Text = $"${escrowFee:N2}";
            SummaryTotalAmountLabel.Text = $"${_totalPrice:N2}";
            SummaryDueNowLabel.Text = $"${dueNow:N2}";
            SummaryRemainingBalanceLabel.Text = $"Remaining Balance: ${_totalPrice - dueNow:N2}";
        }

        // ── Payment Tab Switching ─────────────────────────────────────────
        private void OnCreditCardTabTapped(object? sender, TappedEventArgs e)
        {
            CreditCardPanel.IsVisible = true;
            DigitalWalletPanel.IsVisible = false;

            CreditCardTab.Stroke = Color.FromArgb("#0D9488");
            CreditCardTab.BackgroundColor = Color.FromArgb("#F0FDFA");
            DigitalWalletTab.Stroke = Color.FromArgb("#E2E8F0");
            DigitalWalletTab.BackgroundColor = Colors.White;

            UpdateTabLabelColor(CreditCardTab, "#0D9488");
            UpdateTabLabelColor(DigitalWalletTab, "#64748B");
        }

        private void OnDigitalWalletTabTapped(object? sender, TappedEventArgs e)
        {
            CreditCardPanel.IsVisible = false;
            DigitalWalletPanel.IsVisible = true;

            DigitalWalletTab.Stroke = Color.FromArgb("#0D9488");
            DigitalWalletTab.BackgroundColor = Color.FromArgb("#F0FDFA");
            CreditCardTab.Stroke = Color.FromArgb("#E2E8F0");
            CreditCardTab.BackgroundColor = Colors.White;

            UpdateTabLabelColor(DigitalWalletTab, "#0D9488");
            UpdateTabLabelColor(CreditCardTab, "#64748B");
        }

        private static void UpdateTabLabelColor(Border tab, string hexColor)
        {
            if (tab.Content is VerticalStackLayout layout)
                foreach (var child in layout.Children)
                    if (child is Label lbl)
                        lbl.TextColor = Color.FromArgb(hexColor);
        }

        // ── Digital Wallet Provider Selection ────────────────────────────
        private void OnGCashSelected(object? sender, TappedEventArgs e)
        {
            GCashOption.Stroke = Color.FromArgb("#0D9488");
            GCashOption.BackgroundColor = Color.FromArgb("#F0FDFA");
            PayMayaOption.Stroke = Color.FromArgb("#E2E8F0");
            PayMayaOption.BackgroundColor = Colors.White;
        }

        private void OnPayMayaSelected(object? sender, TappedEventArgs e)
        {
            PayMayaOption.Stroke = Color.FromArgb("#0D9488");
            PayMayaOption.BackgroundColor = Color.FromArgb("#F0FDFA");
            GCashOption.Stroke = Color.FromArgb("#E2E8F0");
            GCashOption.BackgroundColor = Colors.White;
        }

        // ── Pay & Submit ──────────────────────────────────────────────────
        private async void OnPayAndSubmitClicked(object? sender, EventArgs e)
        {
            decimal dueNow = _totalPrice * 0.5m;
            bool confirmed = await this.DisplayAlertAsync(
                "Confirm Downpayment",
                $"You are about to pay ${dueNow:N2} USD (50% Downpayment) for your commission.\n\n" +
                "Proceed with secure payment?",
                $"Yes, Pay ${dueNow:N2}",
                "Cancel");

            if (!confirmed) return;

            // -- Save to Database --
            var newOrder = new CustomOrder
            {
                OrderID = "ORD-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
                Title = string.IsNullOrWhiteSpace(BriefTitleEntry.Text) ? "Untitled Commission" : BriefTitleEntry.Text,
                Description = BriefIntentEditor.Text,
                ClientName = ProfileNameLabel.Text,
                Status = "Briefing",
                Priority = "Normal",
                Category = "Custom Art",
                Resolution = "300 DPI",
                ColorProfile = "sRGB",
                Deliverables = "Digital Source Files",
                Price = _totalPrice
            };

            var result = await _databaseService.AddCustomOrderAsync(newOrder);

            if (result.success)
            {
                await this.DisplayAlertAsync(
                    "🎉 Commission Submitted!",
                    "Your commission has been placed successfully and saved to your history.\n\n" +
                    "The artist will be notified immediately.",
                    "View My Orders");

                await Shell.Current.GoToAsync("//TrackOrdersPage");
            }
            else
            {
                await this.DisplayAlertAsync("Error", $"Your order was processed but we had trouble saving it to your history.\n\nDetails: {result.error}", "OK");
            }
        }

        // ── FAQ & References ─────────────────────────────────────────────
        private async void OnBulkUploadTapped(object? sender, EventArgs e)
        {
            try
            {
                var results = await FilePicker.Default.PickMultipleAsync(new PickOptions
                {
                    PickerTitle = "Select Inspiration Assets",
                    FileTypes = FilePickerFileType.Images
                });

                if (results != null && results.Any())
                {
                    // For now, we'll show the first one in the main previews
                    var first = results.First();
                    var source = ImageSource.FromFile(first!.FullPath);

                    // Update Step 1
                    Step1ReferenceImagePreview.Source = source;
                    Step1PreviewContainer.IsVisible = true;
                    Step1UploadZone.IsVisible = false;

                    // Update Step 2
                    ReferenceImagePreview.Source = source;
                    PreviewContainer.IsVisible = true;
                    DropZonePrompt.IsVisible = false;

                    int count = results.Count();
                    if (count > 1)
                    {
                        await this.DisplayAlertAsync("Bulk Upload", $"{count} assets selected. They will be included in your brief.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await this.DisplayAlertAsync("Upload Error", "Unable to process bulk upload: " + ex.Message, "OK");
            }
        }

        private async void OnAddReferencesTapped(object? sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Reference Images",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    // Update preview UI
                    var source = ImageSource.FromFile(result.FullPath);

                    // Update Step 1
                    Step1ReferenceImagePreview.Source = source;
                    Step1PreviewContainer.IsVisible = true;
                    Step1UploadZone.IsVisible = false;

                    // Update Step 2
                    ReferenceImagePreview.Source = source;
                    PreviewContainer.IsVisible = true;
                    DropZonePrompt.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                await this.DisplayAlertAsync("Picker Error", "Unable to open file picker: " + ex.Message, "OK");
            }
        }

        private void OnRemoveReferenceClicked(object? sender, EventArgs e)
        {
            Step1ReferenceImagePreview.Source = null;
            Step1PreviewContainer.IsVisible = false;
            Step1UploadZone.IsVisible = true;

            ReferenceImagePreview.Source = null;
            PreviewContainer.IsVisible = false;
            DropZonePrompt.IsVisible = true;
        }

        private void OnFaqTapped(object? sender, EventArgs e)
        {
            if (sender is Grid grid && grid.GestureRecognizers.Count > 0 && grid.GestureRecognizers[0] is TapGestureRecognizer tap && tap.CommandParameter is string param)
            {
                Label? answer = param switch
                {
                    "1" => Faq1Answer,
                    "2" => Faq2Answer,
                    "3" => Faq3Answer,
                    _ => null
                };

                Shapes.Path? arrowPath = param switch
                {
                    "1" => Faq1Arrow,
                    "2" => Faq2Arrow,
                    "3" => Faq3Arrow,
                    _ => null
                };

                if (answer != null)
                {
                    answer.IsVisible = !answer.IsVisible;
                    if (arrowPath != null)
                    {
                        arrowPath.Rotation = answer.IsVisible ? 180 : 0;
                    }
                }
            }
        }

        private void OnTypefaceChanged(object? sender, EventArgs e)
        {
            if (TypefacePicker.SelectedIndex != -1)
            {
                string? selected = TypefacePicker.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selected)) return;

                // Update preview text style based on selection
                if (selected.Contains("Serif"))
                {
                    HeadingPreviewLabel.Text = "The\nClassic\nCanvas";
                    BodyPreviewLabel.Text = "Timeless elegance for sophisticated storytelling and artisanal branding.";
                }
                else if (selected.Contains("Geometric"))
                {
                    HeadingPreviewLabel.Text = "THE\nMODERN\nGRID";
                    BodyPreviewLabel.Text = "Bold, clean, and perfectly balanced for high-impact visual identities.";
                }
                else
                {
                    HeadingPreviewLabel.Text = "The\nCurated\nCanvas";
                    BodyPreviewLabel.Text = "Refined text for clear communication and modern professional aesthetics.";
                }
            }
        }
    }
}
