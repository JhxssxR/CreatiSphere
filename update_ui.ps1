$targetFiles = Get-ChildItem -Path "c:\IT13_CreatiSphere\Views\Creator\*.xaml"

$sidebarOld = '            <!-- Bottom Items -->
            <VerticalStackLayout Grid.Row="2" Padding="12,24" Spacing="4">
                <Border BackgroundColor="Transparent" StrokeShape="RoundRectangle 8" StrokeThickness="0" Padding="16,12">
                    <HorizontalStackLayout Spacing="14">
                        <Path Data="M19.14,12.94c0.04-0.3,0.06-0.61,0.06-0.94c0-0.32-0.02-0.64-0.06-0.94l2.03-1.58c0.18-0.14,0.23-0.41,0.12-0.61 l-1.92-3.32c-0.12-0.22-0.37-0.29-0.59-0.22l-2.39,0.96c-0.5-0.38-1.03-0.7-1.62-0.94L14.4,2.81c-0.04-0.24-0.24-0.41-0.48-0.41 h-3.84c-0.24,0-0.43,0.17-0.47,0.41L9.25,5.35C8.66,5.59,8.12,5.92,7.63,6.29L5.24,5.33c-0.22-0.08-0.47,0-0.59,0.22L2.73,8.87 C2.62,9.08,2.66,9.34,2.86,9.48l2.03,1.58C4.84,11.36,4.8,11.69,4.8,12s0.02,0.64,0.06,0.94l-2.03,1.58 c-0.18,0.14-0.23,0.41-0.12,0.61l1.92,3.32c0.12,0.22,0.37,0.29,0.59,0.22l2.39-0.96c0.5,0.38,1.03,0.7,1.62,0.94l0.36,2.54 c0.05,0.24,0.24,0.41,0.48,0.41h3.84c0.24,0,0.43-0.17,0.47-0.41l0.36-2.54c0.59-0.24,1.13-0.56,1.62-0.94l2.39,0.96 c0.22,0.08,0.47,0,0.59-0.22l1.92-3.32c0.12-0.22,0.07-0.49-0.12-0.61L19.14,12.94z" Fill="#9CA3AF" Aspect="Uniform" HeightRequest="18" WidthRequest="18" VerticalOptions="Center"/>
                        <Label Text="Settings" TextColor="#9CA3AF" FontSize="14" VerticalOptions="Center"/>
                    </HorizontalStackLayout>
                </Border>
                <Border BackgroundColor="Transparent" StrokeShape="RoundRectangle 8" StrokeThickness="0" Padding="16,12">
                    <Border.GestureRecognizers>
                        <TapGestureRecognizer Tapped="OnSignOutTapped" />
                    </Border.GestureRecognizers>
                    <HorizontalStackLayout Spacing="14">
                        <Path Data="M17,7l-1.41,1.41L18.17,11H8v2h10.17l-2.58,2.58L17,17l5-5L17,7z M4,5h8V3H4C2.9,3,2,3.9,2,5v14c0,1.1,0.9,2,2,2h8v-2H4V5z" Fill="#9CA3AF" Aspect="Uniform" HeightRequest="18" WidthRequest="18" VerticalOptions="Center"/>
                        <Label Text="Sign Out" TextColor="#9CA3AF" FontSize="14" VerticalOptions="Center"/>
                    </HorizontalStackLayout>
                </Border>

                <!-- Creator Profile -->
                <Grid Padding="12,20,12,0" ColumnDefinitions="Auto, *, Auto">
                    <Border HeightRequest="36" WidthRequest="36" StrokeShape="RoundRectangle 18" StrokeThickness="0" BackgroundColor="#0D9488">
                        <Label x:Name="SidebarProfileInitialsLabel" Text="CR" TextColor="White" FontSize="12" FontAttributes="Bold" HorizontalOptions="Center" VerticalOptions="Center" />
                    </Border>
                    <VerticalStackLayout Grid.Column="1" VerticalOptions="Center" Margin="10,0">
                        <Label x:Name="ProfileNameLabel" Text="Creator Account" TextColor="White" FontSize="12" FontAttributes="Bold" />
                        <Label x:Name="ProfileRoleLabel" Text="Creator" TextColor="#6B7280" FontSize="10" />
                    </VerticalStackLayout>
                    <Path Grid.Column="2" Data="M12,8c1.1,0,2-0.9,2-2s-0.9-2-2-2s-2,0.9-2,2S10.9,8,12,8z M12,10c-1.1,0-2,0.9-2,2s0.9,2,2,2s2-0.9,2-2S13.1,10,12,10z M12,16 c-1.1,0-2,0.9-2,2s0.9,2,2,2s2-0.9,2-2S13.1,16,12,16z" Fill="#6B7280" Aspect="Uniform" HeightRequest="16" WidthRequest="16" VerticalOptions="Center" />
                </Grid>
            </VerticalStackLayout>'

$sidebarNew = '            <!-- Bottom Items -->
            <VerticalStackLayout Grid.Row="2" Padding="12,24" Spacing="4">
                <!-- Creator Profile -->
                <Grid Padding="12,0,12,20" ColumnDefinitions="Auto, *, Auto">
                    <Border HeightRequest="36" WidthRequest="36" StrokeShape="RoundRectangle 18" StrokeThickness="0" BackgroundColor="#0D9488">
                        <Label x:Name="SidebarProfileInitialsLabel" Text="CR" TextColor="White" FontSize="12" FontAttributes="Bold" HorizontalOptions="Center" VerticalOptions="Center" />
                    </Border>
                    <VerticalStackLayout Grid.Column="1" VerticalOptions="Center" Margin="10,0">
                        <Label x:Name="ProfileNameLabel" Text="Creator Account" TextColor="White" FontSize="12" FontAttributes="Bold" />
                        <Label x:Name="ProfileRoleLabel" Text="Creator" TextColor="#6B7280" FontSize="10" />
                    </VerticalStackLayout>
                    <Path Grid.Column="2" Data="M12,8c1.1,0,2-0.9,2-2s-0.9-2-2-2s-2,0.9-2,2S10.9,8,12,8z M12,10c-1.1,0-2,0.9-2,2s0.9,2,2,2s2-0.9,2-2S13.1,10,12,10z M12,16 c-1.1,0-2,0.9-2,2s0.9,2,2,2s2-0.9,2-2S13.1,16,12,16z" Fill="#6B7280" Aspect="Uniform" HeightRequest="16" WidthRequest="16" VerticalOptions="Center" />
                </Grid>

                <Border BackgroundColor="Transparent" StrokeShape="RoundRectangle 8" StrokeThickness="0" Padding="16,12">
                    <Border.GestureRecognizers>
                        <TapGestureRecognizer Tapped="OnSignOutTapped" />
                    </Border.GestureRecognizers>
                    <HorizontalStackLayout Spacing="14">
                        <Path Data="M17,7l-1.41,1.41L18.17,11H8v2h10.17l-2.58,2.58L17,17l5-5L17,7z M4,5h8V3H4C2.9,3,2,3.9,2,5v14c0,1.1,0.9,2,2,2h8v-2H4V5z" Fill="#9CA3AF" Aspect="Uniform" HeightRequest="18" WidthRequest="18" VerticalOptions="Center"/>
                        <Label Text="Sign Out" TextColor="#9CA3AF" FontSize="14" VerticalOptions="Center"/>
                    </HorizontalStackLayout>
                </Border>
            </VerticalStackLayout>'

$bellOld = '<Path Data="M12,22c1.1,0,2-0.9,2-2h-4C10,21.1,10.9,22,12,22z M18,16v-5c0-3.07-1.63-5.64-4.5-6.32V4c0-0.83-0.67-1.5-1.5-1.5 s-1.5,0.67-1.5,1.5v0.68C7.64,5.36,6,7.92,6,11v5l-2,2v1h16v-1L18,16z" Fill="#64748B" Aspect="Uniform" HeightRequest="20" WidthRequest="20" />'
$bellNew = '<Grid>
                            <Grid.GestureRecognizers>
                                <TapGestureRecognizer Tapped="OnNotificationsTapped" />
                            </Grid.GestureRecognizers>
                            <Path Data="M12,22c1.1,0,2-0.9,2-2h-4C10,21.1,10.9,22,12,22z M18,16v-5c0-3.07-1.63-5.64-4.5-6.32V4c0-0.83-0.67-1.5-1.5-1.5 s-1.5,0.67-1.5,1.5v0.68C7.64,5.36,6,7.92,6,11v5l-2,2v1h16v-1L18,16z" Fill="#64748B" Aspect="Uniform" HeightRequest="20" WidthRequest="20" />
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
                                    <Label Text="New Commission Request" TextColor="#0F172A" FontSize="14" FontAttributes="Bold" />
                                    <Label Text="Alex Morgan requested a custom asset." TextColor="#475569" FontSize="13" />
                                    <Label Text="2 mins ago" TextColor="#94A3B8" FontSize="11" />
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
    $content = $content.Replace($bellOld, $bellNew)
    
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

$csFiles = Get-ChildItem -Path "c:\IT13_CreatiSphere\Views\Creator\*.xaml.cs"
foreach ($file in $csFiles) {
    $content = Get-Content $file.FullName -Raw
    $content = $content -replace "(?s)\s*}\s*}\s*`$", $csNew
    Set-Content $file.FullName $content -NoNewline
}
