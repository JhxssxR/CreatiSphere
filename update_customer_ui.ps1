$targetFiles = Get-ChildItem -Path "c:\IT13_CreatiSphere\Views\Customer\*.xaml"

$sidebarOld = '<Path Grid.Column="2" Data="M12,22c1.1,0,2-0.9,2-2h-4C10,21.1,10.9,22,12,22z M18,16v-5c0-3.1-1.6-5.6-4.5-6.3V4c0-0.8-0.7-1.5-1.5-1.5S10.5,3.2,10.5,4v0.7C7.6,5.4,6,7.9,6,11v5l-2,2v1h16v-1L18,16z"
                          Fill="#64748B" Aspect="Uniform" HeightRequest="18" WidthRequest="18" VerticalOptions="Center"/>'
$sidebarNew = '<Grid Grid.Column="2" VerticalOptions="Center">
                        <Grid.GestureRecognizers>
                            <TapGestureRecognizer Tapped="OnNotificationsTapped" />
                        </Grid.GestureRecognizers>
                        <Path Data="M12,22c1.1,0,2-0.9,2-2h-4C10,21.1,10.9,22,12,22z M18,16v-5c0-3.1-1.6-5.6-4.5-6.3V4c0-0.8-0.7-1.5-1.5-1.5S10.5,3.2,10.5,4v0.7C7.6,5.4,6,7.9,6,11v5l-2,2v1h16v-1L18,16z"
                              Fill="#64748B" Aspect="Uniform" HeightRequest="18" WidthRequest="18" />
                    </Grid>'

$topnavOld = '<!-- Bell Icon -->
                    <Grid WidthRequest="24" HeightRequest="24">
                        <Path Data="M12 22c1.1 0 2-.9 2-2h-4c0 1.1.89 2 2 2zm6-6v-5c0-3.07-1.64-5.64-4.5-6.32V4c0-.83-.67-1.5-1.5-1.5s-1.5.67-1.5 1.5v.68C7.63 5.36 6 7.92 6 11v5l-2 2v1h16v-1l-2-2z" 
                              Fill="#64748B" Aspect="Uniform" HeightRequest="22" WidthRequest="22" VerticalOptions="Center" HorizontalOptions="Center" />
                        <Ellipse Fill="#EF4444" WidthRequest="8" HeightRequest="8" HorizontalOptions="End" VerticalOptions="Start" Margin="0,0,0,0"/>
                    </Grid>'
$topnavNew = '<!-- Bell Icon -->
                    <Grid WidthRequest="24" HeightRequest="24">
                        <Grid.GestureRecognizers>
                            <TapGestureRecognizer Tapped="OnNotificationsTapped" />
                        </Grid.GestureRecognizers>
                        <Path Data="M12 22c1.1 0 2-.9 2-2h-4c0 1.1.89 2 2 2zm6-6v-5c0-3.07-1.64-5.64-4.5-6.32V4c0-.83-.67-1.5-1.5-1.5s-1.5.67-1.5 1.5v.68C7.63 5.36 6 7.92 6 11v5l-2 2v1h16v-1l-2-2z" 
                              Fill="#64748B" Aspect="Uniform" HeightRequest="22" WidthRequest="22" VerticalOptions="Center" HorizontalOptions="Center" />
                        <Ellipse Fill="#EF4444" WidthRequest="8" HeightRequest="8" HorizontalOptions="End" VerticalOptions="Start" Margin="0,0,0,0"/>
                    </Grid>'

$notificationsModal = '        <!-- Notifications Modal -->
        <Grid x:Name="NotificationsOverlay" Grid.ColumnSpan="2" BackgroundColor="#20000000" IsVisible="False">
            <BoxView BackgroundColor="Transparent">
                <BoxView.GestureRecognizers>
                    <TapGestureRecognizer Tapped="OnCloseNotificationsTapped" />
                </BoxView.GestureRecognizers>
            </BoxView>
            <Border BackgroundColor="White" StrokeShape="RoundRectangle 16" StrokeThickness="1" Stroke="#E2E8F0"
                    HorizontalOptions="End" VerticalOptions="Start" WidthRequest="380" Margin="0,70,30,0" Padding="0">
                <VerticalStackLayout>
                    <Grid Padding="20,16" BackgroundColor="#F8FAFC">
                        <Label Text="Notifications" TextColor="#0F172A" FontSize="16" FontAttributes="Bold" VerticalOptions="Center" />
                        <Label Text="✕" TextColor="#64748B" FontSize="16" HorizontalOptions="End" VerticalOptions="Center">
                            <Label.GestureRecognizers>
                                <TapGestureRecognizer Tapped="OnCloseNotificationsTapped" />
                            </Label.GestureRecognizers>
                        </Label>
                    </Grid>
                    <BoxView HeightRequest="1" Color="#E2E8F0" />
                    <ScrollView MaximumHeightRequest="320">
                        <VerticalStackLayout x:Name="NotificationsContainer" Spacing="0">
                            <!-- Sample Notification Item -->
                            <Grid Padding="20,16" ColumnDefinitions="Auto, *, Auto">
                                <Border HeightRequest="8" WidthRequest="8" StrokeShape="RoundRectangle 4" BackgroundColor="#3B82F6" VerticalOptions="Start" Margin="0,6,12,0" StrokeThickness="0" />
                                <VerticalStackLayout Grid.Column="1" Spacing="4">
                                    <Label Text="Order Update" TextColor="#0F172A" FontSize="14" FontAttributes="Bold" />
                                    <Label Text="Your custom asset is ready for review." TextColor="#475569" FontSize="13" />
                                    <Label Text="5 mins ago" TextColor="#94A3B8" FontSize="11" />
                                </VerticalStackLayout>
                                <HorizontalStackLayout Grid.Column="2" Spacing="8" VerticalOptions="Start">
                                    <Border BackgroundColor="#F1F5F9" StrokeShape="RoundRectangle 6" StrokeThickness="0" Padding="8,4">
                                        <Border.GestureRecognizers><TapGestureRecognizer Tapped="OnMarkAsReadTapped" /></Border.GestureRecognizers>
                                        <Label Text="Read" TextColor="#64748B" FontSize="11" FontAttributes="Bold" />
                                    </Border>
                                    <Border BackgroundColor="#FEE2E2" StrokeShape="RoundRectangle 6" StrokeThickness="0" Padding="8,4">
                                        <Border.GestureRecognizers><TapGestureRecognizer Tapped="OnDeleteNotificationTapped" /></Border.GestureRecognizers>
                                        <Label Text="Del" TextColor="#EF4444" FontSize="11" FontAttributes="Bold" />
                                    </Border>
                                </HorizontalStackLayout>
                            </Grid>
                            <BoxView HeightRequest="1" Color="#F1F5F9" />
                        </VerticalStackLayout>
                    </ScrollView>
                    <BoxView HeightRequest="1" Color="#E2E8F0" />
                    <Button Text="Clear All" BackgroundColor="Transparent" TextColor="#EF4444" FontSize="13" FontAttributes="Bold" HeightRequest="44" CornerRadius="0" Clicked="OnClearAllNotificationsClicked" />
                </VerticalStackLayout>
            </Border>
        </Grid>
    </Grid>
</ContentPage>'

foreach ($file in $targetFiles) {
    $content = Get-Content $file.FullName -Raw
    
    $content = $content.Replace($sidebarOld, $sidebarNew)
    $content = $content.Replace($topnavOld, $topnavNew)
    
    # Using Regex to correctly insert the modal before the closing tags
    $content = $content -replace "(?s)\s*</Grid>\s*</ContentPage>\s*`$", "`n$notificationsModal`n"
    
    Set-Content $file.FullName $content -NoNewline
}

$csNew = '
        private void OnNotificationsTapped(object? sender, EventArgs e)
        {
            NotificationsOverlay.IsVisible = true;
        }

        private void OnCloseNotificationsTapped(object? sender, EventArgs e)
        {
            NotificationsOverlay.IsVisible = false;
        }

        private void OnMarkAsReadTapped(object? sender, EventArgs e)
        {
            if (sender is Border border && border.Parent is HorizontalStackLayout hsl && hsl.Parent is Grid grid)
            {
                if (grid.Children[0] is Border dot)
                {
                    dot.IsVisible = false;
                }
            }
        }

        private void OnDeleteNotificationTapped(object? sender, EventArgs e)
        {
            if (sender is Border border && border.Parent is HorizontalStackLayout hsl && hsl.Parent is Grid grid)
            {
                NotificationsContainer.Children.Remove(grid);
            }
        }

        private void OnClearAllNotificationsClicked(object? sender, EventArgs e)
        {
            NotificationsContainer.Children.Clear();
            NotificationsOverlay.IsVisible = false;
        }
    }
}'

$csFiles = Get-ChildItem -Path "c:\IT13_CreatiSphere\Views\Customer\*.xaml.cs"
foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw
    
    if (-not $content.Contains("OnNotificationsTapped")) {
        $content = $content -replace "(?s)\s*}\s*}\s*`$", $csNew
        Set-Content $file.FullName $content -NoNewline
    }
}
